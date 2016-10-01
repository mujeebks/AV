using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using System.Web.Security;
using AVD.Common.Logging;
using AVD.Core.Auth;
using AVD.DataAccessLayer;
using AVD.DataAccessLayer.Enums;
using AVD.DataAccessLayer.Models;
using AVD.DataAccessLayer.Repositories;

namespace AVD.Core.Providers.ActiveDirectory
{
    /// <summary>
    /// This class is responsonsible for using the AD workers to get data from AD
    /// and convert it into classes and dtos understanable by the TE/WVS application.
    /// 
    /// Note communication can happen via the AD Membership provider or querying AD directly.
    /// </summary>
    public class ActiveDirectoryManager
    {
        private IActiveDirectoryWorker ActiveDirectoryWorker { get; set; }
        private Func<IRepository<Role>> RoleRepositoryFactory { get; set; }
 
        public ActiveDirectoryManager(IActiveDirectoryWorker worker, Func<IRepository<Role>> roleRepository = null)
        {
            RoleRepositoryFactory = roleRepository ?? (() => RepositoryFactory.Get<Role>());
            ActiveDirectoryWorker = worker;
        }

        public bool CreateUser(string firstname, string lastname, string email, List<RoleTypes> roleTypes, string company, out string username, out string setPassword, string password = null)
        {
            var groups = RoleTypesToActiveDirectoryGroups(roleTypes);
            return ActiveDirectoryWorker.CreateUser(firstname, lastname, email, groups, company, out username, out setPassword, password);
        }
         
        public List<String> GetRoles(String username)
        {
            var adRoles = ActiveDirectoryWorker.GetGroups(username);

            if (adRoles == null)
            {
                // Doesn't look like we have a record of the user
                return new List<string>();
            }

            var roles = RoleRepositoryFactory().GetAll();

            var userRoles =
                adRoles.Select(adGroupName => roles.SingleOrDefault(r => r.ADRoleName == adGroupName)).ToList();

            if (userRoles.Any(t => t == null))
            {
                Logger.Instance.Warn(typeof(ActiveDirectoryManager).Name, "GetRoles", "Could not find a matching TE role for corrs AD Group for " + username);
            }

            return userRoles.Where(t => t != null).Select(tt => tt.Name).ToList();
        } 

        public void SetPassword(string username, string newPassword)
        {
            ActiveDirectoryWorker.SetPassword(username, newPassword);
        }

        public void SetRoles(string username, List<RoleTypes> roleTypes)
        {
            var groups = RoleTypesToActiveDirectoryGroups(roleTypes);
            ActiveDirectoryWorker.SetGroups(username, groups);
        }

        private IEnumerable<string> RoleTypesToActiveDirectoryGroups(IEnumerable<RoleTypes> roleTypes)
        {
            using (var roles = RepositoryFactory.Get<Role>())
            {
                var roleTypesId = roleTypes.Select(t => (int) t);

                return roles.Get(x => roleTypesId.Contains(x.RoleId)).Where(x => x.ADRoleName != null).Select(t => t.ADRoleName);
            }
        }
    }
}
