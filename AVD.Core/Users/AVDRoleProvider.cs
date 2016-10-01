using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web.Security;
using LinqKit;
using AVD.Common.Caching;
using AVD.Common.Logging;
using AVD.Core;
using AVD.Core.Auth;
using AVD.DataAccessLayer;
using AVD.DataAccessLayer.Enums;
using AVD.DataAccessLayer.Models;
using AVD.Core.Providers.ActiveDirectory;

namespace AVD.Core.Users
{
    public class AVDRoleProvider : RoleProvider
    {

        private ActiveDirectoryManager ActiveDirectoryManager { get; set; }

        public override void Initialize(string name, NameValueCollection config)
        {
            //
            // Initialize values from web.config.
            //

            if (config == null)
                throw new ArgumentNullException("config");

            if (String.IsNullOrWhiteSpace(name))
                name = "TERoleProvider";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "TravelEdge Role provider");
            }

            // Initialize the abstract base class.
            base.Initialize(name, config);

            if (String.IsNullOrWhiteSpace(config["applicationName"]))
            {
                ApplicationName = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;
            }
            else
            {
                ApplicationName = config["applicationName"];
            }

            // Get the worker type to use. This contains specific AD querying functionality
            // that isn't provided by the AD membership provider. We load this up dynamically
            // so that we have a chance to mock it for unit tests (plus change the implementation
            // later if need be)

            var workerType = config["ADWorkerType"];

            Type type;

            if (workerType == null)
                type = typeof(ActiveDirectoryWorker);
            else
            {
                type = Type.GetType(workerType);

                if (type == null)
                    throw new ApplicationException("Can not find type " + workerType);
            }

            var activeDirectoryMembershipProviderName = config["activeDirectoryMembershipProviderName"];

            object instance;

            try
            {
                if (activeDirectoryMembershipProviderName != null)
                    instance = Activator.CreateInstance(type, config);
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
            if (activeDirectoryWorker != null)
                ActiveDirectoryManager = new ActiveDirectoryManager(activeDirectoryWorker);
            else
                throw new ApplicationException("The type " + instance.GetType().Name + " does not implement interface" + typeof(IActiveDirectoryWorker).Name);
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            if (usernames.Any(String.IsNullOrWhiteSpace))
            {
                throw new ArgumentNullException("usernames");
            }

            if (roleNames.Any(String.IsNullOrWhiteSpace))
            {
                throw new ArgumentNullException("roleNames");
            }

            var roleTypes = new List<RoleTypes>();

            foreach (var role in roleNames)
            {
                RoleTypes rt;

                if (!Enum.TryParse(role, out rt))
                {
                    throw new ApplicationException("Failed to convert String rolename to RoleTypes value: " + role);
                }

                roleTypes.Add(rt);
            }

            foreach (var user in usernames)
            {
                var currentRoleTypes = new List<RoleTypes>();

                GetRolesForUser(user).ForEach(x =>
                {
                    RoleTypes rt;

                    Enum.TryParse(x, out rt);

                    currentRoleTypes.Add(rt);
                });

                var newRoles = currentRoleTypes.Union(roleTypes).ToList();

                ActiveDirectoryManager.SetRoles(user, newRoles);

                var key = AuthManager.GetCacheKeyForRoles(user);
                (ServiceLocator.CachingManager).Remove<string[]>(key);
            }
        }

        public override string ApplicationName
        {
            get; set;
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            var key = AuthManager.GetCacheKeyForRoles(username);

            string[] rolesFromCache = ServiceLocator.CachingManager.GetOrAddToCache(key,
                () =>
                {
                    var roles = ActiveDirectoryManager.GetRoles(username).ToList();

                    // Add the generic ADX access role.
                    if (roles.Any())
                        roles.Add(RoleTypes.WVS.ToString());

                    // Add the WVTAgent or AffiliateAgent role (and other feature roles that are DB and not AD driven)
                    if (roles.Contains(RoleTypes.Agent.ToString()))
                    {
                        // get user roles here
                    }

                    return roles.ToArray();
                });

            return rolesFromCache;
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            return GetRolesForUser(username).Contains(roleName);
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            if (usernames.Any(String.IsNullOrWhiteSpace))
            {
                throw new ArgumentNullException("usernames");
            }

            if (roleNames.Any(String.IsNullOrWhiteSpace))
            {
                throw new ArgumentNullException("roleNames");
            }

            var roleTypes = new List<RoleTypes>();

            foreach (var role in roleNames)
            {
                RoleTypes rt;

                if (!Enum.TryParse(role, out rt))
                {
                    throw new ApplicationException("Failed to convert String rolename to RoleTypes value: " + role);
                }

                roleTypes.Add(rt);
            }

            foreach (var user in usernames)
            {
                var currentRoleTypes = new List<RoleTypes>();

                GetRolesForUser(user).ForEach(x =>
                {
                    RoleTypes rt;

                    Enum.TryParse(x, out rt);

                    currentRoleTypes.Add(rt);
                });

                var newRoles = currentRoleTypes.Except(roleTypes).ToList();

                ActiveDirectoryManager.SetRoles(user, newRoles);

                var key = AuthManager.GetCacheKeyForRoles(user);
                (ServiceLocator.CachingManager).Remove<string[]>(key);
            }
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

    }
}
