using System;
using System.Linq;
using System.Text;
using AVD.Common.Configuration;

namespace AVD.Core.Users
{
    public class UserConfigSettings : ConfigSettingsBase
    {
        private static volatile UserConfigSettings instance;
        private static readonly object SyncRoot = new Object();

        public UserConfigSettings()
        {
            Init();
        }

        public static UserConfigSettings Instance()
        {
            if (instance == null)
            {
                lock (SyncRoot)
                {
                    if (instance == null)
                    {
                        instance = new UserConfigSettings();
                    }
                }
            }
            return instance;
        }

        public string ForgotPasswordResetAdxRelPath { get; set; }
        public int ForgotPasswordTokenExpiryInHours { get; set; }
        public string WelcomeAdxRelPath { get; set; }
    }
}
