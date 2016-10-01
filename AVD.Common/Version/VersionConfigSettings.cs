using System;
using System.Linq;
using AVD.Common.Configuration;

namespace AVD.Common.Version
{
    public class VersionConfigSettings : ConfigSettingsBase
    {
        private static volatile VersionConfigSettings instance;

        private static object obj = new object();

        public static VersionConfigSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new VersionConfigSettings();
                        }
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// The release version (for the public)
        /// </summary>
        public String ReleaseVersion { get; set; }

        /// <summary>
        /// The environment this version was released too
        /// </summary>
        public String Environment { get; set; }

        /// <summary>
        /// The build version (github hash or TC unique build #)
        /// </summary>
        public String BuildVersion { get; set; }

        /// <summary>
        /// The build config used
        /// </summary>
        public String PublishConfig { get; set; }
        
        /// <summary>
        /// The machine this app is running on
        /// </summary>
        public String Machine { get; set; }
    }
}