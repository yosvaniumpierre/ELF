using Avanade.Platform.Services.Web.Configuration;

namespace Avanade.Platform.Services.Web.Unit.Tests.Configuration
{
    using System.Collections.Generic;

    using NUnit.Framework;

    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class TestAppConfigFile
    {
        #region Fields

        private IConfiguration defaultConfigurationImpl;

        #endregion Fields

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        [SetUp]
        public void Init()
        {
            //Interface is not used because the Init method had to be run
            //This is intentional since this component was designed to be run from within NKernel
            defaultConfigurationImpl = new DefaultConfigurationImpl("TestData.ini");
        }

        /// <summary>
        /// Tests the component read config.
        /// </summary>
        [Test]
        public void TestComponentReadConfig()
        {
            string value = defaultConfigurationImpl.Get("sectionA", "keyA").GetValueAsString();
            Assert.AreEqual("valueA", value);

            value = defaultConfigurationImpl.Get("sectionB", "keyB").GetValueAsString();
            Assert.AreEqual("valueB", value);
        }

        /// <summary>
        /// Tests read external config.
        /// </summary>
        [Test]
        public void TestExternalReadConfig()
        {
            var sortedList = new SortedList<string, SortedList<string, string>>();

            if (sortedList.ContainsKey("sectionA"))
            {
                SortedList<string, string> details = sortedList["sectionA"];

                Assert.AreEqual("valueA", details["keyA"]);
                Assert.AreEqual("valueAA", details["keyAA"]);
            }
            else if (sortedList.ContainsKey("sectionB"))
            {
                SortedList<string, string> details = sortedList["sectionB"];

                Assert.AreEqual("valueB", details["keyB"]);
            }
            else
            {
                Assert.Fail();
            }
        }

        #endregion Methods
    }
}