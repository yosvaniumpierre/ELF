namespace Avanade.Platform.Services.Web.Unit.Tests.Configuration
{
    using System.Collections.Generic;

    using Avanade.Platform.Services.Web.Configuration;

    using NUnit.Framework;

    /// <summary>
    /// Description of NoTestDataTest.
    /// </summary>
    [TestFixture]
    public class NoTestDataTest
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
            defaultConfigurationImpl = new DefaultConfigurationImpl();
        }

        /// <summary>
        /// 
        /// </summary>
        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TestDefaultLoad()
        {
            defaultConfigurationImpl.Get("NotThere").GetValueAsString();
        }

        #endregion Methods
    }
}