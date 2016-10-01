using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AVD.UnitTests.Common.Configuration
{
    [TestClass]
    public class ConfigSettingsTests : UnitTestBase
    {
        [TestMethod]
        public void LoadSetting_String()
        {
            var s = new TestConfigSettings();
            s.Init();
            Assert.IsTrue(s.LoadedWithoutErrors);
            Assert.AreEqual("Value", s.TestString);
        }
        
        [TestMethod]
        public void LoadSetting_Int32()
        {
            var s = new TestConfigSettings();
            s.Init();
            Assert.IsTrue(s.LoadedWithoutErrors);
            Assert.AreEqual(1, s.TestInt32);
        }

        [TestMethod]
        public void LoadSetting_StringArray()
        {
            var s = new TestConfigSettings();
            s.Init();
            Assert.IsTrue(s.LoadedWithoutErrors);
            Assert.AreEqual(3, s.TestStringArray.Count());
        }
    }
}
