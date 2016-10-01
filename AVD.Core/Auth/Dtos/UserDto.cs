using System;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace AVD.Core.Auth.Dtos
{
    /// <summary>
    /// Represents a user in the user data store (in our case, active directory)
    /// </summary>
    public class UserDto  : MembershipUser
    {
        public UserDto(String providerName, MembershipUser user, int userId, string firstname, string lastname, DateTime? lastLoginDate = null, DateTime? lastActivityDate = null, DateTime? lastLockoutDate = null)
            : base(
                providerName, user.UserName, user.ProviderUserKey, user.Email, user.PasswordQuestion, user.Comment,
                user.IsApproved, user.IsLockedOut, user.CreationDate, lastLoginDate ?? DateTime.MinValue, lastActivityDate ?? DateTime.MinValue,
                user.LastPasswordChangedDate, lastLockoutDate ?? DateTime.MinValue)
        {
            this.UserId = userId;
            this.FirstName = firstname;
            this.LastName = lastname;
        }

        public int UserId { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }

        public override string ToString()
        {
            return String.Format("UserId: {0}, FirstName:{1}, LastName:{2}", UserId, FirstName, LastName);
        }
    }
}
