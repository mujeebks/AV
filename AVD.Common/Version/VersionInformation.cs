using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using AVD.Common.Logging;
using AVD.Common.Version;

namespace AVD.Common.Version
{
    public class VersionInformation
    {
        public class VersionInfo
        {
            /// <summary>
            /// Version used when displaying as well as in the api calls
            /// </summary>
            /// <returns></returns>
            public string DisplayVersion()
            {
                return ReleaseVersion;
            }

            /// <summary>
            /// The release version (1.0.0-branch) - This is what is displayed.
            /// </summary>
            public string ReleaseVersion { get; set; }

            /// <summary>
            /// The build version (guid or number)
            /// </summary>
            public string BuildVersion { get; set; }
            
            /// <summary>
            /// The machine name this app is running on
            /// </summary>
            public string Machine { get; set; }

            ///// <summary>
            ///// The version of the main dll (1.0.0) Should be the same as the release version
            ///// </summary>
            //public string ProductVersion { get; set; }

            /// <summary>
            /// The environment this version was released too
            /// </summary>
            public string Environment { get; set; }

            /// <summary>
            /// The build config used
            /// </summary>
            public string BuildConfig { get; set; }

       

            public override string ToString()
            {
                return  ReleaseVersion + "," + BuildVersion + "," + BuildConfig + "," + Environment + "," + Machine;
            }
        }

        private static volatile VersionInfo versionInfo;
        private readonly static object lockObj = new object();

        public static VersionInfo Get()
        {
            if (versionInfo == null)
            {
                lock (lockObj)
                {
                    if (versionInfo == null)
                    {
                        versionInfo = get();
                    }
                }
            }

            return versionInfo;
        }

        private static VersionInfo get()
        {
            try
            {
                // Version # from the config
                var configuration = VersionConfigSettings.Instance;

                versionInfo = new VersionInfo
                {
                    ReleaseVersion = configuration.ReleaseVersion,
                    BuildVersion = configuration.BuildVersion,
                    BuildConfig = configuration.PublishConfig,
                    Environment = configuration.Environment,
                    Machine = configuration.Machine
                };
                //// Version # from the dll (should be the same as the build version)
                //Assembly assembly = Assembly.GetExecutingAssembly();
                //FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                //versionInfo.ProductVersion = fileVersionInfo.ProductVersion; // Set by TC embedded in the dll

                return versionInfo;
            }
            catch (Exception ex)
            {
                Logger.Instance.Warn("VersionInfo", "Get", ex);

                return new VersionInfo
                {
                    ReleaseVersion = "0.0.0",
                    BuildConfig = "Unknown",
                    BuildVersion = "Unknown",
                    Environment = "Unknown",
                    Machine = "Unknown"
                };
            }
        }
    }
}
