using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVD.Common.Configuration;
using AVD.Common.Logging;

namespace AVD.Core
{
    public class EMRMCoreConfigSettings : ConfigSettingsBase
    {
        private static volatile EMRMCoreConfigSettings instance;
        private static object syncRoot = new Object();

        public static EMRMCoreConfigSettings Instance()
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new EMRMCoreConfigSettings();
                    }
                }
            }
            return instance;
        }

        private EMRMCoreConfigSettings()
        {
            Init();
        }

        /// <summary>
        /// Returns the base path for adx shared resources feeds
        /// </summary>
        public String AdxSharedResourcesFeedsPath { get; set; }

        public String ENVIRONMENT_TYPE
        {
            get
            {
                if (ConfigurationManager.AppSettings["ENVIRONMENT_TYPE"] == null)
                {
                    Logger.Instance.Error("AdxCoreConfigSettings", "ENVIRONMENT_TYPE", "ENVIRONMENT_TYPE not set in config");
                    return "UNKNOWN";
                }

                return ConfigurationManager.AppSettings["ENVIRONMENT_TYPE"];
            }
        }

        /// <summary>
        /// Returns the base path for adx shared resources feeds
        /// </summary>
        public String AdxUrl { get; set; }
    }
}
