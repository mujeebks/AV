using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Security;
using AutoMapper;
using AVD.Common.Caching;
using AVD.Common.Exceptions;
using AVD.Common.Logging;
using AVD.Core.Auth.Dtos;
using AVD.Core.Exceptions;
using AVD.DataAccessLayer;
using AVD.DataAccessLayer.Models;
using AVD.DataAccessLayer.Repositories;
using AVD.Core.Auth.Exceptions;

namespace AVD.Core.Auth
{
    /// <summary>
    /// This is the default provider we use for membership. This class bascially is a pass through to the AD provider with
    /// some logic placed on top for AVD specific needs (so we can return a MembershipUser contexualised as an Agent or basic User
    /// as appropriate).
    /// </summary>
    public class AVDMembershipProvider : MembershipProvider
    {
        private Func<IRepository<User>> UserRepositoryFactory { get; set; }

        /// <summary>
        /// The name of the Membership Provider which will be used to query for users (e.g. ActiveDirectory)
        /// </summary>
        public string AuthMembershipProviderName { get; private set; }

        public AVDMembershipProvider()
        {
            UserRepositoryFactory = (() => RepositoryFactory.Get<User>());
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);

            AuthMembershipProviderName = config["AuthMembershipProviderName"];
        }

        /// <summary>
        /// The provider to use to actually validate the user & get basic info
        /// </summary>
        private MembershipProvider GetAuthMembershipProvider()
        {
            // Note - this can not go in the constructor as it will cause a stack overflow exception
            return Membership.Providers[AuthMembershipProviderName];
        }

        /// <summary>
        /// This will return a AVD Membership User (Agent or User)
        /// </summary>
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            Logger.Instance.LogFunctionEntry(this.GetType().Name, "GetUser");

            
            var adUser = GetAuthMembershipProvider().GetUser(username, userIsOnline);

            if (adUser == null)
                return null;

            UserDto userDto;


            // FirstOrDefault is okay here as the Username column has a unique constraint
            var user = UserRepositoryFactory().FirstOrDefault(t => t.Username == username);

            // If we are asking for a user by username and it doesn't exist return NULL
            if (user == null)
                return null;

            int userId = user.UserId;

           
            userDto = new UserDto(Name, adUser, userId,adUser.UserName.Split('.').First(), adUser.UserName.Split('.').Last());
           
            Logger.Instance.LogFunctionExit(this.GetType().Name, "GetUser");

            return userDto;
        }           


        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            if (email == null)
                throw new ArgumentNullException("email");

            return GetAuthMembershipProvider().GetUserNameByEmail(email);
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { throw new NotImplementedException(); }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            get { throw new NotImplementedException(); }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This will validate the username
        /// </summary>
        public override bool ValidateUser(string username, string password)
        {
            Logger.Instance.LogFunctionEntry(this.GetType().Name, "ValidateUser");
            var isValid = GetAuthMembershipProvider().ValidateUser(username, password);

            Logger.Instance.Info(typeof(AVDMembershipProvider).Name, "ValidateUser",
                        "ValidateUser(" + username + ", [password])=" + isValid);

            // Ensure that they are in our local table
            var user = GetOrAddUser(username);

            Logger.Instance.LogFunctionExit(this.GetType().Name, "ValidateUser");

            return isValid;
        }


        /// <summary>
        /// This will add to our local User table the email address and return a new User object, or return the existing
        /// one if it has already been added.
        /// </summary>
        private User GetOrAddUser(String localUsername)
        {
            Logger.Instance.LogFunctionEntry(this.GetType().Name, "GetOrAddUser");

            var repo = UserRepositoryFactory();

            // Search for the user (if they have logged in before)
            var user = repo.SingleOrDefault(t => t.Username == localUsername);

            // Create the user if its a new one
            if (user == null)
            {
                user = new User { Username = localUsername };
                repo.InsertAndSave(user); // perf fix
                repo.SaveChanges();
            }

            Logger.Instance.LogFunctionExit(this.GetType().Name, "GetOrAddUser");
            return user;
        }
    }
}