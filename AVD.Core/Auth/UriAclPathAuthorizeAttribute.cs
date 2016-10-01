using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using AVD.Common.Logging;

namespace AVD.Core.Auth
{
    /// <summary>
    /// Note this class is globally registered - make sure all calls within are threadsafe
    /// </summary>
    public class UriAclAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            Logger.InstanceVerbose.LogFunctionEntry(GetType().Name, "IsAuthorized");
            var sw = new Stopwatch();
            sw.Start();

            // If the attributes do not enable authorization then don't let them through
            if (!base.IsAuthorized(actionContext))
                return false;

            var path = actionContext.Request.RequestUri.AbsolutePath;

            // Check the custom authorization table to see if they have access
            var returnVal = AuthManager.IsAuthorized(path, System.Threading.Thread.CurrentPrincipal);

            sw.Stop();
            Logger.InstanceVerbose.Debug(GetType().Name, "IsAuthorized",
                "Time: " + sw.ElapsedMilliseconds + "ms");
            Logger.InstanceVerbose.LogFunctionExit(GetType().Name, "IsAuthorized");

            return returnVal;
        }
    }
}
