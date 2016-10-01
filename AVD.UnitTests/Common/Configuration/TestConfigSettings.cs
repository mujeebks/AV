using System;
using System.Linq;
using System.Text;
using AVD.Common.Configuration;

namespace AVD.UnitTests.Common.Configuration
{
    public class TestConfigSettings : ConfigSettingsBase
    {
        private static volatile TestConfigSettings instance;
        private static readonly object SyncRoot = new Object();

        public TestConfigSettings()
        {
            Init();
        }

        public static TestConfigSettings Instance()
        {
            if (instance == null)
            {
                lock (SyncRoot)
                {
                    if (instance == null)
                    {
                        instance = new TestConfigSettings();
                    }
                }
            }
            return instance;
        }

        public String TestString { get; set; }
        public Int32 TestInt32 { get; set; }
        public String[] TestStringArray { get; set; }
        public String UnitTestsImpersonatedAgentUsername { get; set; }
    }
}
