using System.Web.Security;
using AVD.Core.Auth.Dtos;
using AVD.DataAccessLayer.Enums;

namespace AVD.Core.Auth
{
    public class AuthorizationFiltersConfiguration
    {
        public static bool Initialized { get; private set; }

        static AuthorizationFiltersConfiguration()
        {
            Initialized = false;
        }

        public static void Configure()
        {
            if (Initialized)
                return;
        }

        public static bool IsEmployee(AVDUserIdentity userIdentity)
        {
            return userIdentity.IsEmployee != null && (userIdentity.AgentId != null && userIdentity.IsEmployee.Value && !IsManager(userIdentity) && !IsRm(userIdentity));
        }

        public static bool IsManager(AVDUserIdentity userIdentity)
        {
            return userIdentity.IsManager != null && (userIdentity.AgentId != null && userIdentity.IsManager.Value &&
                                                      !Roles.IsUserInRole(RoleTypes.SuperAgent.ToString()));
        }

        public static bool IsRm(AVDUserIdentity userIdentity)
        {
            return userIdentity.AgencyId != null && Roles.IsUserInRole(RoleTypes.SuperAgent.ToString());
        }

        public static bool IsAdminOrSuperUser(AVDUserIdentity userIdentity)
        {
            return (Roles.IsUserInRole(RoleTypes.Administrator.ToString()) || Roles.IsUserInRole(RoleTypes.SuperUser.ToString()));
        }
        public static bool IsSystem(AVDUserIdentity userIdentity)
        {
            return Roles.IsUserInRole(RoleTypes.System.ToString());
        }
    }
}
