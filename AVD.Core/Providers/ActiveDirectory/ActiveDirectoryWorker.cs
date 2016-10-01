using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using AVD.Common.Exceptions;
using AVD.Common.Helpers;
using AVD.Core.Exceptions;
using AVD.Core.Properties;
using AVD.Core.Auth.Exceptions;
using AVD.Core.Providers.ActiveDirectory;

namespace AVD.Core.Providers.ActiveDirectory
{
    public class ActiveDirectoryWorker : IActiveDirectoryWorker
    {
        private String ADUser;
        private String ADPassword;
        private String ADConnection;
        private String AttributeMapUsername;

        public ActiveDirectoryWorker(NameValueCollection config)
        { 
            var connectionStringName = config["connectionStringName"];

            if (connectionStringName == null)
                throw new ConfigurationErrorsException("Missing config value connectionStringName");

            ADUser = config["connectionUsername"];
            ADPassword = config["connectionPassword"];
            ADConnection = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            AttributeMapUsername = config["attributeMapUsername"];

            if(ADUser == null)
                throw new ConfigurationErrorsException("Missing config value ADUser");

            if (ADPassword == null)
                throw new ConfigurationErrorsException("Missing config value ADPassword");

            if (AttributeMapUsername == null)
                throw new ConfigurationErrorsException("Missing config value AttributeMapUsername");
        }

        /// <summary>
        /// Given the username (as specified in the AD membership provider via "AttributeMapUsername") this will
        /// return the groups the user belongs too.
        /// </summary>
        public List<String> GetGroups(String username)
        {
            // From http://stackoverflow.com/questions/4460558/how-to-get-all-the-ad-groups-for-a-particular-user
            var de = new DirectoryEntry(ADConnection, ADUser, ADPassword);
            var searcher = new DirectorySearcher(de);
            //searcher.Filter = "(&(ObjectClass=group))";
            searcher.Filter = "(" + AttributeMapUsername + "=" + username + ")";
            searcher.PropertiesToLoad.Add("distinguishedName");
            searcher.PropertiesToLoad.Add("sAMAccountName");
            searcher.PropertiesToLoad.Add("name");
            searcher.PropertiesToLoad.Add("objectSid");
            
            searcher.PropertiesToLoad.Add("memberOf"); 
             
            var groupNames = new List<String>();

                SearchResult result = searcher.FindOne();

            if (result == null)
                return null;

                int propertyCount = result.Properties["memberOf"].Count;
                String dn;
                int equalsIndex, commaIndex;

                for (int propertyCounter = 0; propertyCounter < propertyCount;
                    propertyCounter++)
                {
                    dn = (String)result.Properties["memberOf"][propertyCounter];

                    equalsIndex = dn.IndexOf("=", 1);
                    commaIndex = dn.IndexOf(",", 1);
                    if (-1 == equalsIndex)
                    {
                        return null;
                    }
                    groupNames.Add(dn.Substring((equalsIndex + 1),
                                (commaIndex - equalsIndex) - 1));
                }

            return groupNames;

        }

        public bool CreateUser(string firstname, string lastname, string email, IEnumerable<string> groups, string company, out string username, out string setPassword, string password = null)
        {
            var companyConnection = ADConnection.Replace("OU=Companies", String.Format("OU={0},OU=Companies", company));

            var de = new DirectoryEntry(companyConnection, ADUser, ADPassword, AuthenticationTypes.Secure);

            if (String.IsNullOrWhiteSpace(firstname) || String.IsNullOrWhiteSpace(lastname) || String.IsNullOrWhiteSpace(email))
            {
                throw new ApplicationException("firstname, lastname, email must be supplied");
            }

            username = GetUniqueUsername(firstname, lastname, de);

            if (username == null)
            {
                throw new ApplicationException("Exhausted search space trying to locate a unique AD username");
            }

            setPassword = password ?? Guid.NewGuid().ToString();

            var userEntry = de.Children.Add(String.Format("CN={0}", email), "User");

            userEntry.Properties["userPrincipalName"].Value = firstname + lastname;
            userEntry.Properties["displayName"].Value = firstname + " " + lastname;
            userEntry.Properties["givenName"].Value = firstname;
            userEntry.Properties["sn"].Value = lastname;
            userEntry.Properties["sAMAccountName"].Value = username;
            userEntry.Properties["Mail"].Value = email;

            userEntry.CommitChanges();

            userEntry.Invoke("SetPassword", setPassword);
            userEntry.Properties["userAccountControl"].Value = 0x10000; // Password does not expire.

            userEntry.CommitChanges();
            userEntry.Close();

            var groupsConnection = ADConnection.Substring(0, ADConnection.IndexOf("/OU", StringComparison.InvariantCultureIgnoreCase));

            foreach (var group in groups)
            {
                var groupEntry = new DirectoryEntry(String.Format("{0}/CN={1},CN=Users,DC=traveledge,DC=tld", groupsConnection, group), ADUser, ADPassword, AuthenticationTypes.Secure);

                groupEntry.Properties["member"].Add(userEntry.Properties["distinguishedName"].Value);
                groupEntry.CommitChanges();

                groupEntry.Close();
            }
            
            de.Close();

            return true;
        }

        public bool DeleteUser(string username, string company)
        {
            var companyConnection = ADConnection.Replace("OU=Companies", String.Format("OU={0},OU=Companies", company));

            var de = new DirectoryEntry(companyConnection, ADUser, ADPassword, AuthenticationTypes.Secure);

            var searcher = new DirectorySearcher(de) { Filter = "(" + this.AttributeMapUsername + "=" + username + ")" };
            searcher.PropertiesToLoad.Add("sAMAccountName");

            var searchRes = searcher.FindOne();

            if (searchRes == null)
            {
                throw new ApplicationException("No user found in AD.");
            }

            var userEntry = searchRes.GetDirectoryEntry();

            if (userEntry == null)
            {
                throw new ApplicationException("No user found in AD.");
            }

            de.Children.Remove(userEntry);
            de.CommitChanges();

            return true;
        }

        /// <summary>
        /// Returns a unique username in the format of firstname.lastname000 to firstname.lastname.999; returns null if the search space has been exhausted.
        /// </summary>
        /// <param name="firstname"></param>
        /// <param name="lastname"></param>
        /// <param name="directoryEntry"></param>
        /// <returns></returns>
        public string GetUniqueUsername(string firstname, string lastname, DirectoryEntry directoryEntry)
        {
            var username = (firstname.RemoveSpecialCharacters() + "." + lastname.RemoveSpecialCharacters());

            //
            // Later on what we probably want to do is write a prettier username algorithm, but this will do.
            //
            if (username.Length > 16)
            {
                username = username.Substring(0, 16);
            }

            String finalUsername = null;

            for (var i = 0; i < 999; i++)
            {
                var testName = String.Format("{0}{1:D3}", username, i);

                var searcher = new DirectorySearcher(directoryEntry) { Filter = "(" + this.AttributeMapUsername + "=" + testName + ")" };
                searcher.PropertiesToLoad.Add("sAMAccountName");

                var searchRes = searcher.FindOne();

                // No match found, we've found a unique username.

                if (searchRes == null)
                {
                    finalUsername = testName;
                    break;
                }
            }

            return finalUsername;
        }

        /// <summary>
        /// Given the username (as specified in the AD membership provider via "AttributeMapUsername") this will
        /// call the "SetPassword" operation in AD.
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="newPassword">New password</param>
        public void SetPassword(string username, string newPassword)
        {
            DirectoryEntry de = new DirectoryEntry(ADConnection, ADUser, ADPassword, AuthenticationTypes.Secure);

            var searcher = new DirectorySearcher(de) { Filter = "(" + this.AttributeMapUsername + "=" + username + ")" };
            searcher.PropertiesToLoad.Add("sAMAccountName");

            var searchRes = searcher.FindOne();
            if (searchRes == null)
            {
                throw new ApplicationException("No user found in AD.");
            }
            var userEntry = searchRes.GetDirectoryEntry();
            if (userEntry == null)
            {
                throw new ApplicationException("No user found in AD.");
            }
            
            try
            {
                userEntry.Invoke("SetPassword", newPassword);
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException == null) throw;

                if (e.InnerException is UnauthorizedAccessException)
                {
                    throw new ApplicationException(
                        "Access Denied setting password in ActiveDirectory, check permissions of AD User",
                        e);
                }

                if (e.InnerException is COMException)
                {
                    var comException = e.InnerException as COMException;
                    if (comException.ErrorCode == -2147022651)
                        throw new BusinessException(AuthBusinessExceptionTypes.Reset_PasswordPolicy, comException.Message, e);
                }

                throw;
            }

            userEntry.CommitChanges();

            userEntry.Close();
            de.Close();
        }

        /// <summary>
        /// This will set the user in AD in the given groups, removing and adding the user as necessary.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="groups"></param>
        public void SetGroups(string username, IEnumerable<string> groups)
        {
            var de = new DirectoryEntry(ADConnection, ADUser, ADPassword, AuthenticationTypes.Secure);

            var searcher = new DirectorySearcher(de) { Filter = "(" + this.AttributeMapUsername + "=" + username + ")" };
            searcher.PropertiesToLoad.Add("sAMAccountName");

            var searchRes = searcher.FindOne();

            if (searchRes == null)
            {
                throw new ApplicationException("No user found in AD.");
            }

            var userEntry = searchRes.GetDirectoryEntry();

            if (userEntry == null)
            {
                throw new ApplicationException("No user found in AD.");
            }

            var distinguishedName = userEntry.Properties["distinguishedName"].Value;

            de.Close();

            var currentGroups = GetGroups(username);

            var addingGroups = groups.Except(currentGroups).ToList();
            var removingGroups = currentGroups.Except(groups).ToList();

            var groupsConnection = ADConnection.Substring(0, ADConnection.IndexOf("/OU", StringComparison.InvariantCultureIgnoreCase));

            foreach (var group in addingGroups)
            {
                var groupEntry = new DirectoryEntry(String.Format("{0}/CN={1},CN=Users,DC=traveledge,DC=tld", groupsConnection, group), ADUser, ADPassword, AuthenticationTypes.Secure);

                groupEntry.Properties["member"].Add(distinguishedName);
                groupEntry.CommitChanges();

                groupEntry.Close();
            }

            foreach (var group in removingGroups)
            {
                var groupEntry = new DirectoryEntry(String.Format("{0}/CN={1},CN=Users,DC=traveledge,DC=tld", groupsConnection, group), ADUser, ADPassword, AuthenticationTypes.Secure);

                groupEntry.Properties["member"].Remove(distinguishedName);
                groupEntry.CommitChanges();

                groupEntry.Close();
            }
        }
    }
}
