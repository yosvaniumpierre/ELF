namespace Avanade.Platform.Services.Web.Unit.Tests.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Avanade.Platform.Services.Web.Configuration;

    using NUnit.Framework;

    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class DefaultConfigurationTest
    {
        #region Fields

        private DefaultConfigurationImpl defaultConfigurationImpl;

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
        /// 
        /// </summary>
        [Test]
        public void TestDefaultEnvVariablePath()
        {
            string value = defaultConfigurationImpl.Get("Path").GetValueAsString();
            Assert.IsNotEmpty(value);

            //Test once more time to ensure total coverage and that the value is retrieved from the cache.
            value = defaultConfigurationImpl.Get("Path").GetValueAsString();
            Assert.IsNotEmpty(value);
        }

        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void TestInterfaceDefaultGroupBoundary()
        {
            Assert.Throws<ArgumentNullException>(() => defaultConfigurationImpl.Get(null).GetValueAsString());
            Assert.Throws<KeyNotFoundException>(() => defaultConfigurationImpl.Get("NonExistentKey").GetValueAsString());
        }

        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void TestInterfaceDefaultGroupGood()
        {
            string defaultValue = defaultConfigurationImpl.Get("Setting1").GetValueAsString();
            Assert.AreEqual("Very", defaultValue);

            //Value should be cached now.
            defaultValue = defaultConfigurationImpl.Get("Setting1").GetValueAsString();
            Assert.AreEqual("Very", defaultValue);

            int intValue = defaultConfigurationImpl.Get("Setting2").GetValueAsInt();
            Assert.AreEqual(1, intValue);

            int intUltimateValue = defaultConfigurationImpl.Get("Setting3").GetValueAsInt();
            Assert.AreEqual(42, intUltimateValue);
        }

        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void TestInterfaceDefaultGroupStringUrl()
        {
            string url = defaultConfigurationImpl.Get("Url").GetValueAsString();
            Assert.AreEqual("tcp://localhost:61616", url);
        }

        #endregion Methods
    }
}