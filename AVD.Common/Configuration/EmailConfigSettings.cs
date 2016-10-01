using System;
using System.Linq;
using System.Text;

namespace AVD.Common.Configuration
{
    public sealed class EmailConfigSettings : ConfigSettingsBase
    {
        private static volatile  EmailConfigSettings instance;
        private static object syncRoot = new Object();

        public static EmailConfigSettings Instance()
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new EmailConfigSettings();
                    }
                }
            }
            return instance;
        }
        
        // cruise line commission values
        public string SmtpHostName { get; set; }
        public int SmtpPortNumber { get; set; }
        public bool SmtpEnableSsl { get; set; }
        public bool SmtpUseDefaultCredentials { get; set; }
        public string SmtpUserName { get; set; }
        public string SmtpPassword { get; set; }
        public string AttachmentsFolder { get; set; }
        public string EoPdfLicenseKey { get; set; }
        public string EmailSenderAddressForAgent { get; set; }
        public string DefaultEmailSenderAddressForClient { get; set; }
        public string DefaultSupportEmailAddress { get; set; }

        public EmailConfigSettings()
        {
            this.Init();
        }
    }
}
