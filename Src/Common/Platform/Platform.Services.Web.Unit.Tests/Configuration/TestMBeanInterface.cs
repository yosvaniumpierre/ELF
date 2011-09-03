namespace Avanade.Platform.Services.Web.Unit.Tests.Configuration
{
    using System;

    using Avanade.Platform.Services.Web.Configuration;

    using NUnit.Framework;

    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class TestMBeanInterface
    {
        #region Fields

        private DefaultConfigurationImpl defaultConfigurationImpl;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Sets up this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            defaultConfigurationImpl = new DefaultConfigurationImpl("TestData.ini");
        }

        /// <summary>
        /// Tests the verification.
        /// </summary>
        [Test]
        public void TestVerificationBoundary()
        {
            Assert.Throws<ArgumentNullException>(() => defaultConfigurationImpl.Verify(null));
            Assert.Throws<ArgumentNullException>(() => defaultConfigurationImpl.VerifySection("SomeSection", null));
            Assert.Throws<ArgumentNullException>(() => defaultConfigurationImpl.VerifySection("", "SomeKey"));

            Assert.IsEmpty(defaultConfigurationImpl.Verify("NonExistentKey"));
            Assert.IsEmpty(defaultConfigurationImpl.VerifySection("NonExistentSection", "NonExistentKey"));
        }

        /// <summary>
        /// Tests the verification interface.
        /// </summary>
        [Test]
        public void TestVerificationInterface()
        {
            var defaultValue = defaultConfigurationImpl.Verify("Setting1");
            Assert.AreEqual("Very", defaultValue);

            defaultValue = defaultConfigurationImpl.VerifySection("Test", "Success");
            Assert.AreEqual("true", defaultValue);
        }

        #endregion Methods
    }
}