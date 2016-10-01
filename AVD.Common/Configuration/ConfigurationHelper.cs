using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVD.Common.Configuration
{
    public static class ConfigurationHelper
    {
        /// <summary>
        /// Returns a bool for the given settingName key value pair.  If the setting is not found, false is returned.
        /// The setting must contain the value of "YES" case insensitive in order to be returned as true.
        /// </summary>
        public static bool GetAppSettingAsBool(string settingName)
        {
            string setting = ConfigurationManager.AppSettings[settingName];

            if (string.IsNullOrWhiteSpace(setting)) return false;

            bool settingEnabled = string.Equals(setting, "yes", StringComparison.OrdinalIgnoreCase);

            return settingEnabled;
        }
    }
}
