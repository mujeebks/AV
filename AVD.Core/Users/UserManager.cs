using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Security;
using AVD.Common;
using AVD.Common.Configuration;
using AVD.Common.Exceptions;
using AVD.Common.Logging;
using AVD.Core.Auth.Dtos;
using AVD.Core.Auth.Exceptions;
using AVD.Core.Communication;
using AVD.Core.Communication.Dtos;
using AVD.Core.Exceptions;
using AVD.Core.Properties;
using AVD.Core.Providers.ActiveDirectory;
using AVD.Core.Shared;
using AVD.Core.Users.Exceptions;
using AVD.DataAccessLayer;
using AVD.DataAccessLayer.Enums;
using AVD.DataAccessLayer.Models;
using AVD.DataAccessLayer.Repositories;

namespace AVD.Core.Users
{
    public class UserManager
    { 
        public const string PUBLIC_TRIPVIEW_ACCOUNT = "PUBLIC.TRIPVIEW";
        public const string PUBLIC_AIRSEARCHVIEW_ACCOUNT = "PUBLIC.AIRSEARCHVIEW";
        private Lazy<IRepository<User>> UserRepository { get; set; }
        private IPersistentCacheManager DbCacheManager { get; set; }

        public UserManager()
        {
            UserRepository = new Lazy<IRepository<User>>(() => RepositoryFactory.Get<User>());
            DbCacheManager = ServiceLocator.PersistentCache;
        }

        /// <summary>
        /// Creates a password reset token and emails a link to the user to change their password.
        /// </summary>
        /// <param name="email">Email address of the user.</param>
        public void SendPasswordReset(string email)
        {
            if (String.IsNullOrWhiteSpace(email))
                throw new ArgumentOutOfRangeException("email");

            var username = Membership.GetUserNameByEmail(email);

            if (string.IsNullOrEmpty(username))
            {
                throw new BusinessException(AuthBusinessExceptionTypes.Login_UserNameNotFound, "User does not exist in our local DB");
            }

            User user = UserRepository.Value.SingleOrDefault(t => t.Username == username);

            if (user == null)
            {
                throw new BusinessException(
                    AuthBusinessExceptionTypes.Login_UserNameNotFound,
                    "User does not exist in our local DB");
            }

            var token = Guid.NewGuid().ToString();
            var resetToken = new ResetUserPasswordToken { ResetTokenDateTime = DateTime.Now, UserId = user.UserId };
            DbCacheManager.Add(token, resetToken);

            var tokenLink = string.Format("{0}{1}?token={2}", EMRMCoreConfigSettings.Instance().AdxUrl, UserConfigSettings.Instance().ForgotPasswordResetAdxRelPath, token);
            var body = string.Format(UserMessages.Email_ResetPassword_ResetTokenMessage_Body, email, tokenLink);

            var emailMsg = new EmailRequestDto
            {
                                   To = new List<EmailAddressDto>
                                           {
                                               new EmailAddressDto
                                                   {
                                                       Address = email
                                                   }
                                           },
                                   Subject = UserMessages.Email_ResetPassword_ResetTokenMessage_Subject,
                                   Body = body,
                                   EmailType = EmailTypes.SystemEmail,
                                   IsBodyHtml = false
                               };

            var resp = new EmailWorker().SendSystemEmail(emailMsg);

            if (!resp.IsSuccessful)
            {
                if (resp.ValidationMessages.Any())
                {
                    // Any user generated validation errors should have been caught 
                    // earlier, treat these as system exceptions
                    Logger.Instance.Error(this.GetType().Name, "SendPasswordReset", resp.ValidationMessages);
                    throw new ApplicationException("Unexpected validation errors");
                }

                if (resp.FailureMessages.Any())
                {
                    // We can't gracefully handle any of these failure messages so just
                    // log them and throw a business exception to try again later.
                    Logger.Instance.Error(this.GetType().Name, "SendPasswordReset", resp.FailureMessages);
                    throw new BusinessException(
                        AuthBusinessExceptionTypes.Reset_CouldNotSendEmail,
                        "Failure sending message, see FailureMessage collection.",
                        resp.FailureMessages);
                }

                Logger.Instance.Error(this.GetType().Name, "SendPasswordReset", "Email could not be sent, no validation or failure messages returned.");
                throw new ApplicationException("Email could not be sent, no validation or failure messages returned.");
            }
        }

        public void SendWelcomeAdx(string email)
        {
            if (String.IsNullOrWhiteSpace(email))
                throw new ArgumentOutOfRangeException("email");

            var username = Membership.GetUserNameByEmail(email);

            if (string.IsNullOrEmpty(username))
            {
                throw new BusinessException(AuthBusinessExceptionTypes.Login_UserNameNotFound, "User does not exist in our local DB");
            }

            User user = UserRepository.Value.SingleOrDefault(t => t.Username == username);

            if (user == null)
            {
                throw new BusinessException(
                    AuthBusinessExceptionTypes.Login_UserNameNotFound,
                    "User does not exist in our local DB");
            }

            var token = Guid.NewGuid().ToString();
            var resetToken = new ResetUserPasswordToken { ResetTokenDateTime = DateTime.Now, UserId = user.UserId };
            DbCacheManager.Add(token, resetToken);

            var tokenLink = string.Format("{0}{1}?token={2}&email={3}", EMRMCoreConfigSettings.Instance().AdxUrl, UserConfigSettings.Instance().WelcomeAdxRelPath, token, email);
            var body = string.Format(UserMessages.Welcome_Adx_Email_Body, tokenLink);

            var emailMsg = new EmailRequestDto
            {
                To = new List<EmailAddressDto>
                                           {
                                               new EmailAddressDto
                                                   {
                                                       Address = email
                                                   }
                                           },

                Subject = UserMessages.Welcome_Adx_Email_Subject,
                Body = body,
                EmailType = EmailTypes.SystemEmail,
                IsBodyHtml = false
            };

            var resp = new EmailWorker().SendSystemEmail(emailMsg);

            if (!resp.IsSuccessful)
            {
                if (resp.ValidationMessages.Any())
                {
                    // Any user generated validation errors should have been caught 
                    // earlier, treat these as system exceptions
                    Logger.Instance.Error(this.GetType().Name, "SendWelcomeAdx", resp.ValidationMessages);
                    throw new ApplicationException("Unexpected validation errors");
                }

                if (resp.FailureMessages.Any())
                {
                    // We can't gracefully handle any of these failure messages so just
                    // log them and throw a business exception to try again later.
                    Logger.Instance.Error(this.GetType().Name, "SendWelcomeAdx", resp.FailureMessages);
                    throw new BusinessException(
                        AuthBusinessExceptionTypes.Reset_CouldNotSendEmail,
                        "Failure sending message, see FailureMessage collection.",
                        resp.FailureMessages);
                }

                Logger.Instance.Error(this.GetType().Name, "SendWelcomeAdx", "Email could not be sent, no validation or failure messages returned.");
                throw new ApplicationException("Email could not be sent, no validation or failure messages returned.");
            }
        }

        /// <summary>
        /// Given a password reset token, set the new password for the user.
        /// </summary>
        /// <param name="resetToken">Password reset token.</param>
        /// <param name="newPassword">New password.</param>
        /// <returns>Username of the user whose's password was set.</returns>
        public string SetPassword(string resetToken, string newPassword)
        {
            if (string.IsNullOrEmpty(resetToken))
                throw new ArgumentOutOfRangeException("resetToken");

            if (string.IsNullOrEmpty(newPassword))
                throw  new ArgumentOutOfRangeException("newPassword");


            var userId = GetUserIdFromResetToken(resetToken);

            var user = UserRepository.Value.GetByID(userId);

            if (user == null)
                throw new BusinessException(AuthBusinessExceptionTypes.Reset_TokenExpired, "Token either doesn't exist or has expired.");

            var adManager = this.GetActiveDirectoryManager();
            adManager.SetPassword(user.Username, newPassword);

            // Clear the token
            DbCacheManager.Remove<ResetUserPasswordToken>(resetToken);
            
            return user.Username;
        }

        public int GetUserIdFromResetToken(string resetToken)
        {
            var resetExpiry = DateTime.Now.Subtract(new TimeSpan(0, UserConfigSettings.Instance().ForgotPasswordTokenExpiryInHours, 0, 0));

            // Fetch the user record for the provided token, ensure the token hasn't expired
            var token = DbCacheManager.Get<ResetUserPasswordToken>(resetToken);

            if (token == null || token.ResetTokenDateTime < resetExpiry)
            {
                throw new BusinessException(UsersBusinessExceptionTypes.TokenExpired);
            }

            return token.UserId;
        }

        public bool IsResetPasswordTokenValid(string resetToken)
        {
            var resetExpiry = DateTime.Now.Subtract(new TimeSpan(0, UserConfigSettings.Instance().ForgotPasswordTokenExpiryInHours, 0, 0));

            // Fetch the user record for the provided token, ensure the token hasn't expired
            var token = DbCacheManager.Get<ResetUserPasswordToken>(resetToken);

            if (token == null || token.ResetTokenDateTime < resetExpiry)
            {
                return false;
            }

            return true;
        }

        public void ChangePassword(int userId, String newPassword, String oldPassword = null)
        {
            throw new NotImplementedException("This is only partly done - not to be used yet");
            /*
            Logger.Instance.LogFunctionEntry(typeof(UserManager).Name, "ChangePassword");
            if(userId <= 0)
                throw new ArgumentOutOfRangeException("userId");

            if (oldPassword == null)
            {
                // auth check - only allow if admin or logged in user
            }
            else if (oldPassword == String.Empty)
            {
                throw new ArgumentException("oldPassword", "should be null or non-empty");
            }

            if (String.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentException("newPassword", "is null or empty");
            
            User user = UserRepository.GetByID(userId);

            if(user == null)
                throw new ApplicationException("User ID does not exist in our local DB");                
            
            // TODO: Move to UserMessages
            if(!Roles.IsUserInRole(user.Username, RoleTypes.Agent.ToString()))
                throw new BusinessException("Can not reset the password for a non-agent");

            var membershipUser = Membership.GetUser(user.Username);

            if (membershipUser == null)
                throw new ApplicationException("User doesn't map to a membership user UserId=" + userId);

            if (oldPassword == null)
            {
                Logger.Instance.Info(typeof(UserManager).Name, "ChangePassword", "Resetting the password for " + user + " so it can be changed.");

                // Reset the password and then change it.
                oldPassword = membershipUser.ResetPassword();
            }

            // Reset the password
            if (!membershipUser.ChangePassword(oldPassword, newPassword))
            {
                // TODO: Move to user messages
                throw new BusinessException("Could not change password - most likely the old password doesn't match the existing password");
            }

            Logger.Instance.Info(typeof(UserManager).Name, "ChangePassword", "Password successfuly changed for user " + user);

            Logger.Instance.LogFunctionExit(typeof(UserManager).Name, "ChangePassword");
             */
        }
   
        /// <summary>
        /// Returns true if the User with userId has accepted the T&C.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>true if TC previously accepted, else false</returns>
        public bool CheckTcAccepted(int userId)
        {
            if (userId <= 0)
                throw new ArgumentOutOfRangeException("userId");

            User user = UserRepository.Value.GetByID(userId);

            if (user == null)
                throw new ApplicationException("User ID does not exist in our local DB");


            return true;

        }

        /// <summary>
        /// Sets the TCAccepted column for User with userId to dateAccepted.
        /// dateAccepted is passed in from the controller.
        /// </summary>
        public void SetTcAccepted(string username)
        {
            if (String.IsNullOrWhiteSpace(username))
                throw new ArgumentOutOfRangeException("username");

            User user = UserRepository.Value.SingleOrDefault(t => t.Username == username);

            if (user == null)
                throw new ApplicationException("User ID does not exist in our local DB");

            DateTime dateAccepted = DateTime.Now;

            UserRepository.Value.Update(user);
            UserRepository.Value.SaveChanges();
        }

        public ActiveDirectoryManager GetActiveDirectoryManager()
        {
            Logger.Instance.LogFunctionEntry(this.GetType().Name, "GetActiveDirectoryManager");

            var roleConfig = ActiveDirectoryConfigSettings.Instance().RoleConfigurationCollection;
            var workerType = roleConfig["ADWorkerType"];

            Type type;

            if (workerType == null)
                type = typeof(ActiveDirectoryWorker);
            else
            {
                type = Type.GetType(workerType);

                if (type == null)
                    throw new ApplicationException("Can not find type " + workerType);
            }

            var activeDirectoryMembershipProviderName = roleConfig["activeDirectoryMembershipProviderName"];

            object instance;

            try
            {
                if (activeDirectoryMembershipProviderName != null)
                    instance = Activator.CreateInstance(type, roleConfig);
                else
                    instance = Activator.CreateInstance(type);
            }
            catch (Exception ex)
            {
                // See if we can guess what the problem is
                if (activeDirectoryMembershipProviderName == null && type == typeof(ActiveDirectoryWorker))
                    throw new ApplicationException("Missing config param \"activeDirectoryMembershipProviderName\" missing from the TERoleProvider. Add this in and set it to the value of the AD Membership provider so the worker can be initialised with the correct values (e.g. LDAP Uri, user/pass)", ex);

                // Must be something different
                throw;
            }

            var activeDirectoryWorker = instance as IActiveDirectoryWorker;
            ActiveDirectoryManager adManager;
            if (activeDirectoryWorker == null)
                throw new ApplicationException("The type " + instance.GetType().Name + " does not implement interface" + typeof(IActiveDirectoryWorker).Name);

            Logger.Instance.LogFunctionExit(this.GetType().Name, "GetActiveDirectoryManager");

            return new ActiveDirectoryManager(activeDirectoryWorker);
        }

        public static IEnumerable<RoleTypes> GetAssignableRoles()
        {
            // Standard roles admins can add
            yield return RoleTypes.ADXCashPayment;
            yield return RoleTypes.Air;            
            yield return RoleTypes.Sabre;
            yield return RoleTypes.SabreCryptic;
            yield return RoleTypes.Hotel;
            yield return RoleTypes.Insurance;
            yield return RoleTypes.ExternalServices;

            // For now, restrict who can add these to the SuperUser
            // account for security.
            if (Roles.IsUserInRole(RoleTypes.SuperUser.ToString()))
            {
                yield return RoleTypes.Administrator;
                yield return RoleTypes.SabreCrypticNoWhitelist;
                yield return RoleTypes.SuperAgent;
            }
        }

        public bool SetUserRoles(string username, IEnumerable<RoleTypes> assignableRoles, IEnumerable<RoleTypes> selectedRoles)
        {
            Logger.Instance.LogFunctionEntry(this.GetType().Name, "SetUserRoles");

            var currentRoles = System.Web.Security.Roles.GetRolesForUser(username);
            var selectedRolesStr = selectedRoles.Select(t => t.ToString()).ToList();
            var assignableRolesStr = assignableRoles.Select(t => t.ToString());

            var addingRoles = selectedRolesStr.Except(currentRoles).ToList();
            var removingRoles = assignableRolesStr.Except(selectedRolesStr).ToList();

            if(addingRoles.Any())
                Logger.Instance.Warn(GetType().Name, "SetUserRoles - Adding Roles: ", String.Join(",", addingRoles));

            if (removingRoles.Any())
                Logger.Instance.Warn(GetType().Name, "SetUserRoles - Removing Roles: ", String.Join(",", removingRoles));

            // If there is no change, return
            if (!addingRoles.Any() && !removingRoles.Any())
                return true;

            foreach (var role in addingRoles)
            {
                if (!currentRoles.Contains(role))
                {
                    System.Web.Security.Roles.AddUserToRole(username, role);
                }
            }

            foreach (var role in removingRoles)
            {
                if (currentRoles.Contains(role))
                {
                    System.Web.Security.Roles.RemoveUserFromRole(username, role);
                }
            }

            // Validate
            var updatedRoles = System.Web.Security.Roles.GetRolesForUser(username).Select(t => t.ToString()).ToList();
            if (updatedRoles.Intersect(assignableRolesStr).Union(selectedRolesStr).Count() != selectedRoles.Count())
            {
                Logger.Instance.Error(GetType().Name, "SetUserRoles", 
                    String.Format("Updated roles incorrect Before{0}, After{1}, Assignable{2}, Selected{3}",
                    String.Join(",", currentRoles),
                    String.Join(",", updatedRoles),
                    String.Join(",", assignableRoles),
                    String.Join(",", selectedRoles)));

                Logger.Instance.LogFunctionExit(this.GetType().Name, "SetUserRoles");

                return false;
            }

            Logger.Instance.LogFunctionExit(this.GetType().Name, "SetUserRoles");

            return true;
        }
    }
}
