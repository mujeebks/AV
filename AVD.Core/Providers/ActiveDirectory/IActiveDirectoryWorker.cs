using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVD.Core.Providers.ActiveDirectory
{
    public interface IActiveDirectoryWorker
    {
        /// <summary>
        /// Given the username (as specified in the AD membership provider via "AttributeMapUsername") this will
        /// return the groups the user belongs too.
        /// </summary>
        List<String> GetGroups(String username);

        /// <summary>
        /// Creates a new User in Active Directory.
        /// </summary>
        /// <param name="firstname"></param>
        /// <param name="lastname"></param>
        /// <param name="email"></param>
        /// <param name="groups"></param>
        /// <param name="company"></param>
        /// <param name="password">Optional password. Set to a random GUID if not specified.</param>
        /// <returns></returns>
        bool CreateUser(string firstname, string lastname, string email, IEnumerable<string> groups, string company, out string username, out string setPassword, string password = null);

        /// <summary>
        /// Deletes a user with the given username belonging to the given company.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="company"></param>
        /// <returns></returns>
        bool DeleteUser(string username, string company);

        /// <summary>
        /// Sets the password for the specified user.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="newPassword">New password.</param>
        void SetPassword(string username, string newPassword);

        /// <summary>
        /// This will set the user in AD in the given groups, removing and adding the user as necessary.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="groups"></param>
        void SetGroups(string username, IEnumerable<string> groups);
    }

}
