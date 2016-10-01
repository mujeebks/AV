using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Security;
using AutoMapper;
using AVD.Common.Auth;
using AVD.Common.Caching;
using AVD.Common.Exceptions;
using AVD.Common.Logging;
using AVD.Core.Auth.Dtos;
using AVD.DataAccessLayer;
using AVD.DataAccessLayer.Enums;
using AVD.DataAccessLayer.Models;
using AVD.DataAccessLayer.Repositories;
using AVD.Core.Auth.Exceptions;
using AVD.Core.Users;

namespace AVD.Core.Auth
{
    /// <summary>
    /// This class validates and logs in a user. If the user is an agent, it will
    /// also sync the agent profile with TRAMS.
    /// </summary>
    public class AuthManager
    {
        private UserManager UserManager { get; set; }

        private static ICachingManager CachingManager { get; set; }

        private Lazy<IRepository<User>> UserRepository { get; set; }

        static AuthManager()
        {
            CachingManager = ServiceLocator.CachingManager;
        }

        public AuthManager(IRepository<User> userRepository = null)
        {
            if (userRepository != null)
                UserRepository = new Lazy<IRepository<User>>(() => userRepository);
            else
                UserRepository = new Lazy<IRepository<User>>(() => RepositoryFactory.Get<User>());

            UserManager = new UserManager();
        }

        /// <summary>
        /// This will validate the user and return a session token which is used
        /// to identify the user for the duration of their session. Once
        /// the user logs out or the session times out the token will no longer be valid.
        /// </summary>
        /// <param name="email">The email address of the user</param>
        /// <param name="password">The password of the user</param>
        /// <param name="syncedProfileWithProvider">If this is not null, it will return whether the agent's profile was successfully synced with the provider (TRAMS) or not.</param>
        /// <returns>An authenticated User Dto which identifies the users session (via the AuthGuid) plus human relevant identifying information (first/last names, id)</returns>
        /// <remarks>This assumes the user is an agent. When we have more user types, this method should be generic</remarks>
        public UserDto ValidateAndSyncUser(String email, String password, out bool? syncedProfileWithProvider, bool operatingAsSuperUser = false)
        {
             Logger.Instance.LogFunctionEntry(GetType().Name, "ValidateAndSyncUser");

            // This is only set if there is a provider to sync the users profile with (i.e. TRAMS)
            syncedProfileWithProvider = null;

            if(email == null)
                throw new ArgumentNullException("email");

            var username = Membership.GetUserNameByEmail(email);
            
            if (username == null)
            {
                throw new BusinessException(AuthBusinessExceptionTypes.Login_UserNameNotFound, String.Format("Email {0} was not matched to a username", email));
            }

            // Clear the cache entry for this user
            ClearCacheKeyForLoggedOnUser(username);

            if (operatingAsSuperUser)
            {
#if PROD
                throw new ApplicationException("THIS SHOULD NOT EXIST IN A PROD BUILD");
#endif

                if (password != null)
                    throw new ArgumentException(@"When using super powers the password must be null", "password");

                // Let's be REALLY sure that this is expected.
                if (!HttpContext.Current.User.IsInRole(RoleTypes.SuperUser.ToString()))
                {
                    Logger.Instance.Error(GetType().Name, "ValidateAndSyncUser", String.Format("Attempt to use super-user functionality to gain access as user:{0},{1} when not a super user!", email, username));
                    throw new UnauthorizedAccessException("This level of access is not available to the logged in user.");
                }
                
                Logger.Instance.Warn(GetType().Name, "ValidateAndSyncUser", String.Format("Validation step skipped for user:{0},{1}", email, username));
            }
            else
            {
                // Validate their credentials. This will also add them to our local User table if they
                // have not been added before (only if its valid).

                if (!Membership.ValidateUser(username, password))
                    throw new BusinessException(AuthBusinessExceptionTypes.Login_MembershipAuthenticationFailed, username);
            }

            // Credentials validated - now ensure they belong to at least one role.
            if (!Roles.IsUserInRole(username, RoleTypes.WVS.ToString()))
            {
                throw new BusinessException(AuthBusinessExceptionTypes.Login_AccountSetupInvalid, null, String.Format("User {0} is not in role {1}", username, RoleTypes.WVS));
            }
                
            // If they are an agent, log them in as such and and ensure their account is synced with TRAMS
            // Get the user - they do exist as the ValidateUser() method will add them if they
            // have not been added before.
            var user = UserRepository.Value.Get(t => t.Username == username).SingleOrDefault();

            if (user == null)
                throw new ApplicationException(String.Format("User {0}/{1} is not in the local DB", username, email));

            // Do one last check - ensure the account is in the approved state.
            // NB: This has to happen AFAVDR the account has been synced with TRAMS (TryGetAndSyncAgent) otherwise
            // it may not allow updates to the local DB to occur which may cause an exception to be thrown (e.g. agent not
            // mapped to office)
            MembershipUser memUser = Membership.GetUser(username);
            if (memUser != null)
            {
                if (!memUser.IsApproved) //is approved determines if his/her account is disabled.
                {
                    throw new BusinessException(AuthBusinessExceptionTypes.Login_AccountDisabled, null, String.Format("Account {0} is currently disabled.", username));
                }
            }

            // Log at warn level that authentication succeeded for 
            Logger.Instance.LogFunctionExit(GetType().Name, "ValidateAndSyncUser");
                
            return (UserDto)Membership.GetUser(username);
        }

        /// <summary>
        /// This will take the current identity and replace it with the AVD identity/principal which
        /// provides access to AVD specific user info
        /// </summary>
        public static void SetCurrentPrincipalWithAVDIdentity(IIdentity identity)
        {
            Logger.InstanceVerbose.LogFunctionEntry(typeof(AuthManager).Name, "SetCurrentPrincipalWithAVDIdentity");

            if(identity == null)
                throw new ArgumentNullException("identity",@"Can not set the principal with a NULL identity");

            Logger.InstanceVerbose.Info(typeof(AuthManager).Name, "SetCurrentPrincipalWithAVDIdentity", "Getting the user for " + identity.Name);

            AVDIdentityBase teIdentity;

            var key = GetCacheKeyForAVDIdentity(identity.Name);
            teIdentity = CachingManager.GetOrAddToCache(key,
                () => GetAVDIdentity(identity.Name, identity.IsAuthenticated, identity.AuthenticationType));


            teIdentity.IsAuthenticated = identity.IsAuthenticated;

            var principal = new AVDPrincipal(teIdentity);

            // Could be NULL if not a web project
            if (HttpContext.Current != null)
                HttpContext.Current.User = principal;

            System.Threading.Thread.CurrentPrincipal = principal;

            Logger.InstanceVerbose.Info(typeof(AuthManager).Name, "SetCurrentPrincipalWithAVDIdentity", "Set the principal - " + principal);

            Logger.InstanceVerbose.LogFunctionExit(typeof(AuthManager).Name, "SetCurrentPrincipalWithAVDIdentity");
        }

        public static string GetCacheKeyForAVDIdentity(String userName)
        {
            var key = CachingManager.GetHashCodeKey("GetCacheKeyForAVDIdentity", userName, "AVDIdentity");
            return key;
        }
        public static string GetCacheKeyForRoles(String userName)
        {
            var key = CachingManager.GetHashCodeKey("GetCacheKeyForRoles", userName, "Roles");
            return key;
        }

        public static void ClearCacheKeyForLoggedOnUser(String userName)
        {
            Logger.Instance.LogFunctionEntry(typeof(AuthManager).Name, "ClearCacheKeyForLoggedOnUser", userName);
            CachingManager.Remove<string[]>(GetCacheKeyForAVDIdentity(userName));
            CachingManager.Remove<string[]>(GetCacheKeyForRoles(userName));
            Logger.Instance.LogFunctionExit(typeof(AuthManager).Name, "ClearCacheKeyForLoggedOnUser");
        }

        private static AVDIdentityBase GetAVDIdentity(String userName, bool isAuthenticated, String authenticationType)
        {
            Logger.Instance.LogFunctionEntry(typeof(AuthManager).Name, "GetAVDIdentity");

            AVDIdentityBase teIdentity;

            // Else it's a user that we store in our user tables. This will be either an Agent or Non-Agent.
            var membershipUser = Membership.GetUser(userName);

            if (membershipUser == null)
                throw new ApplicationException("User can not be found '" + userName);

            var userDto = membershipUser as UserDto;

            if (userDto == null)
                throw new ApplicationException("Can cast the MembershipUser into " + typeof (UserDto).Name);

            if (userDto.UserId <= 0)
                throw new ApplicationException("Invalid userDto - " + userDto);
          
            // Non-Agent User
            teIdentity = new AVDUserIdentity(userName, isAuthenticated, authenticationType, userDto.FirstName, userDto.LastName, userDto.Email, userDto.UserId,
                null, null, null, null, null);
            
            Logger.Instance.LogFunctionExit(typeof(AuthManager).Name, "GetAVDIdentity");

            return teIdentity;
        }
        
        public AuthenticatedUserDto GetAuthenticatedUserSession()
        {
            var i = System.Threading.Thread.CurrentPrincipal.Identity;

            if (!i.IsAuthenticated)
                throw new AuthenticationException("The user is not authenticated");

            var userDto = (UserDto) Membership.GetUser(System.Threading.Thread.CurrentPrincipal.Identity.Name);

            return new AuthenticatedUserDto
            {
                UserDto = userDto
            };

            //GetOrRecreateAuthenticatedUser(i.Name);
        }

        public static bool IsCurrentUserAuthenticated()
        {
            return System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated;
        }

        public static int GetCurrentUserId()
        {
            var i = System.Threading.Thread.CurrentPrincipal.Identity;

            if (!i.IsAuthenticated)
                throw new AuthenticationException("The user is not authenticated");

            if (i is AVDIdentityBase)
            {
                return (i as AVDIdentityBase).UserId;
            }
            else
            {
                throw new AuthenticationException("Expected the current principal to be of type AVDIdentityBase");
            }
        }


        public bool TryGetLoggedInAgent(out AgentUserDto agent)
        {
            var authenticatedUserDto = GetAuthenticatedUserSession();

            if (authenticatedUserDto.UserDto is AgentUserDto)
                agent = authenticatedUserDto.UserDto as AgentUserDto;
            else
                agent = null;

            return agent != null;
        }

        public AgentUserDto GetLoggedInAgent()
        {
            AgentUserDto agent;
            
            if(!TryGetLoggedInAgent(out agent))
                // TODO: Return Forbidden http status here?
                throw new ApplicationException(String.Format("User Dto is not of the expected type {0}, Actual:{1}",
                    typeof(AgentUserDto).Name, GetAuthenticatedUserSession().UserDto.GetType().Name));

            return agent;
        }


        public static bool IsAuthorized(string path, IPrincipal principal)
        {
            if (principal == null)
            {
                return false;
            }

            var aclUtility = CachingManager.GetOrAddToCache(
                CachingManager.GetHashCodeKey("AclUtility(GetAclList())"), () => new AclUtility(GetAclList())
            );

            return aclUtility.AreRolesValidForPath(path, principal.IsInRole);
        }

        public enum AclTypes
        {
            Api,
            Page
        }

        public static IEnumerable<UriAclEntryDto> GetAclList(AclTypes? aclType = null)
        {
            var apiAclEntryRepository = RepositoryFactory.Get<ApiAclEntry>();

            IEnumerable<UriAclEntryDto> entries = apiAclEntryRepository.GetAll().Select(
                    entry => new UriAclEntryDto
                    {
                        Path = entry.Path,
                        RoleNames = new[] { entry.Role.Name }
                    });

            // Add filtering based on type. Should refactor to a table or something if we add more types.
            if (aclType != null)
                if (aclType == AclTypes.Api)
                    entries = entries.Where(t => t.Path.StartsWith("/api/", StringComparison.InvariantCultureIgnoreCase));
                else if(aclType == AclTypes.Page)
                    entries = entries.Where(t => !t.Path.StartsWith("/api/", StringComparison.InvariantCultureIgnoreCase));
                else
                    throw new NotSupportedException(AclTypes.Page.ToString());

            return entries;
        }

        public UserDto GetUserInformation(int agentId)
        {
            var agentRepo = new Repository<User>();
            var agent = agentRepo.GetByID(agentId);

            if (agent == null)
                throw new NotFoundException(typeof(User), agentId);

            return PopulateAgentInfoDto(agent);   
        }

        // Gets the agent info dto from an agent model object
        public static UserDto PopulateAgentInfoDto(User user)
        {
            try
            {
                UserDto dto = Mapper.Map<User, UserDto>(user);

                return dto;
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("PopulateAgentInfoDto", "PopulateAgentInfoDto", ex, "could not get the agent info");
                throw ex;
            }
        }

        /// <summary>
        /// Check if logged-in user is Admin
        /// </summary>
        /// <returns></returns>
        public static bool IsAdmin()
        {
            var roles = Roles.GetRolesForUser(System.Threading.Thread.CurrentPrincipal.Identity.Name);
            if (roles.Contains(RoleTypes.Administrator.ToString()))
                return true;
            else
                return false;
        }
    }
}
