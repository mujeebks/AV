using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AVD.Common.Logging;

namespace AVD.Core.Auth
{
    /// <summary>
    /// This will impersonate the system account and then reverse the impersonation after
    /// </summary>
    /// <remarks>WARNING - CHECK WITH LEAD DEV BEFORE USING THIS IN PRODUCTION CODE. Incorrect usage will
    /// result in security holes. </remarks>
    public class ImpersonatedAccount : IDisposable
    {
        public const string USERNAME_SYSAVDMACCOUNT = "SYSAVDM.ACCOUNT";

        private IPrincipal lastPrincipal { get; set; }

        public ImpersonatedAccount(String username)
        {
            Logger.Instance.LogFunctionEntry("ImpersonatedAccount", "ImpersonatedAccount", username);

            // Could be NULL if not a web project
            if(System.Threading.Thread.CurrentPrincipal != null)
                lastPrincipal = System.Threading.Thread.CurrentPrincipal;
            else if (HttpContext.Current != null)
                lastPrincipal = HttpContext.Current.User;

            // Impersonate the user
            AuthManager.SetCurrentPrincipalWithAVDIdentity(new GenericIdentity(username));

            Logger.Instance.LogFunctionExit("ImpersonatedAccount", "ImpersonatedAccount");
        }

        public void Dispose()
        {
            string currentUserName = null;
            var currentIdentity = System.Threading.Thread.CurrentPrincipal;

            if (currentIdentity != null)
                currentUserName = currentIdentity.Identity.Name;
           
            // Set the old principal back
            if(HttpContext.Current != null )
                HttpContext.Current.User = lastPrincipal;

            System.Threading.Thread.CurrentPrincipal = lastPrincipal;

            Logger.Instance.Info("ImpersonatedAccount", "Dispose", "From '" + currentUserName + "' to '" + (lastPrincipal == null ? "null" : lastPrincipal.Identity.Name) + "'");
        }
    }
}
