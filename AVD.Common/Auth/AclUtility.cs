using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVD.Common.Logging;

namespace AVD.Common.Auth
{
    /// <summary>
    /// This is a helper class with logic to tell if a access should be
    /// granted for a given path and role.
    /// </summary>
    /// <remarks>I named this a "utility" not "manager" as there is no DB access or requirements on any other class. This is
    /// important as the Website (not API) also uses this same class for page level auth checks.
    /// </remarks>
    public class AclUtility
    {
        private IEnumerable<UriAclEntryDto> Entries;

		/// <summary>Creates an ACL checker which will operate on the given entries
		/// </summary>
        /// <param name="entries">A list of entires to match against</param>
        public AclUtility(IEnumerable<UriAclEntryDto> entries)
        {
            // Do some preprocessing
            Entries = entries;
         
            // Need to order so the matching works 
            Entries = entries.OrderBy(t => t.Path);
        }

        /// <summary>
        /// This method takes in a set of ACL entries, a rooted path to a resource and a function
        /// which should return true if a given role is valid.
        /// 
        /// </summary>
        /// <param name="path">The path to check</param>
        /// <param name="isRoleValid">True if a given role is valid (e.g. the user is in the given role)</param>
        /// <returns>
        /// This function will return true if the path is not matched by anything in the list of entries
        /// or is matched, but isRoleValid() returns true for the corresponding role.
        /// </returns>
        public bool AreRolesValidForPath(string path, Func<string, bool> isRoleValid)
        {
            // Now pick the possible matches for the given path
            var options = Entries.Where(t => path.StartsWith(t.Path, StringComparison.InvariantCultureIgnoreCase)).OrderBy(tt => tt.Path.Length);

            // If there are no matches restricting the path, then return true
            if (!options.Any())
            {
                Logger.InstanceVerbose.Info("UriAclAuthorizeAttribute", "CanAccessPath",
                    "Allowing access as there were no matches for path='" + path + "'");
                return true;
            }

            foreach (var apiAclEntry in options)
            {
                // If at least one of the roles are valid, all good for this entry
                var authorized = apiAclEntry.RoleNames.Any(isRoleValid);

                if (!authorized)
                {
                    Logger.InstanceVerbose.Info("UriAclAuthorizeAttribute", "CanAccessPath",
                        "Denied access for user due to ApiACL value='" + apiAclEntry +
                        "' and path='" + path + "'");
                    return false;
                }
            }

            Logger.InstanceVerbose.Info("UriAclAuthorizeAttribute", "CanAccessPath",
                    "Allowing access user was authorized for all entries path='" + path + "'");

            return true;
        }
    }
}
