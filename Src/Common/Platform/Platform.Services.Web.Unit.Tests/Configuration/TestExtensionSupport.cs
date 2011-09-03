using Avanade.Platform.Services.Web.Configuration;

namespace Avanade.Platform.Services.Web.Unit.Tests.Configuration
{
    using System.IO;

    using NUnit.Framework;

    [TestFixture]
    class TestExtensionSupport
    {
        #region Fields

        private IConfiguration configuration;

        #endregion Fields

        #region Methods

        [Test]
        public void TestExtensionViaConstructor()
        {
            configuration = new DefaultConfigurationImpl(new IKeyValueExtension[] { new KeyValueExtension() });

            IConfigSetting setting = configuration.Get("Extension");

            Assert.IsNotNull(setting);
            Assert.AreEqual("TestValue", setting.GetValueAsString());
        }

        #endregion Methods
    }
}