using System;
using System.Security.Principal;
using System.Web.Security;
using AVD.Common.Auth;

namespace AVD.Core.Auth
{
    // Based off https://codeutil.wordpress.com/2013/05/14/forms-authentication-in-asp-net-mvc-4/
    [Serializable]
    public class AVDPrincipal : IPrincipal
    {
        #region Implementation of IPrincipal

        /// <summary>
        /// Determines whether the current principal belongs to the specified role.
        /// </summary>
        /// <returns>
        /// true if the current principal is a member of the specified role; otherwise, false.
        /// </returns>
        /// <param name="role">The name of the role for which to check membership. </param>
        public bool IsInRole(string role)
        {
            return Roles.IsUserInRole(this.Identity.Name, role);
        }

        /// <summary>
        /// Gets the identity of the current principal.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Security.Principal.IIdentity"/> object associated with the current principal.
        /// </returns>
        public IIdentity Identity { get; private set; }

        #endregion

        public AVDIdentityBase AVDIdentity { get { return (AVDIdentityBase)Identity; } set { Identity = value; } }

        public AVDPrincipal(AVDIdentityBase identity)
        {
            Identity = identity;
        }
    }
}