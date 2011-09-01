using Avanade.ServiceContainer.Impl;
using System.Threading;
using NUnit.Framework;

namespace Avanade.ServiceContainer.Tests
{
    [TestFixture]
    public class TestServiceContainer
    {
        #region Fields

        private IServiceContainer serviceContainer;

        #endregion Fields

        #region Methods

        [SetUp]
        public void Setup()
        {
            serviceContainer = new DefaultServiceContainerImpl();
        }

        [TearDown]
        public void Teardown()
        {
            serviceContainer.Stop();
        }

        [Test]
        public void TestContainer()
        {
            serviceContainer.Start();

            //This is to give the MBeanServer some bit of time to spin up.
            for (int i = 0; i < 50; i++)
                Thread.Sleep(10);
        }

        #endregion Methods
    }
}