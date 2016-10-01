using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Web.Configuration;

using AVD.Common.Logging;

namespace AVD.Core.Providers.ActiveDirectory
{
    public class ActiveDirectoryConfigSettings
    {
        public bool LoadedWithoutErrors;

        private static volatile ActiveDirectoryConfigSettings instance;
        private static object syncRoot = new Object();

        public ActiveDirectoryConfigSettings()
        {
            this.Init();
        }

        public NameValueCollection RoleConfigurationCollection { get; set; }

        public static ActiveDirectoryConfigSettings Instance()
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new ActiveDirectoryConfigSettings();
                    }
                }
            }
            return instance;
        }

        private void Init()
        {
            var section = ConfigurationManager.GetSection("system.web/roleManager") as RoleManagerSection;

            if (section == null)
            {
                Logger.Instance.Warn(this.GetType().Name, "Init", "Could not find roleManager configuration section.");
                throw new ConfigurationErrorsException("Could not find roleManager configuration section.");
            }

            this.RoleConfigurationCollection = section.Providers[section.DefaultProvider].Parameters;
        }
    }
}
