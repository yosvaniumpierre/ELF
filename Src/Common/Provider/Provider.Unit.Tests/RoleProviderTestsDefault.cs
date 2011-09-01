using System;
using NUnit.Framework;

namespace Avanade.Provider.Unit.Tests
{
    /// <summary>
    ///This is a test class for NHibernateMembershipProviderTest and is intended
    ///to contain the majority NHibernateMembershipProviderTest Unit Tests.
    /// 
    /// Some of the tests require a valid machinekey configured specifically for testing
    /// purposes.  The app.config file included with this package has been preconfigured
    /// for this purpose.
    ///</summary>
    [TestFixture]
    public class RoleProviderTestsDefault
    {
        #region Fields

        private RepositoryRoleProvider provider;

        #endregion Fields

        #region Methods

        [Test]
        public void ExercisePublicProperties()
        {
            //Arrange
            //Act
            //Assert
            Assert.AreEqual(provider.ApplicationName, ConfigUtils.GetRoleConfig()["applicationName"]);
            Assert.AreEqual(provider.Description, "Repository-Based Role provider");
            Assert.AreEqual(provider.Name, "RepositoryRoleProvider");
        }

        [SetUp]
        public void Setup()
        {
            provider = new RepositoryRoleProvider { UserRepository = MockRepositoryUtils.GetMockUserRepository(), RoleRepository = MockRepositoryUtils.GetMockRoleRepository() };
            provider.Initialize("", ConfigUtils.GetRoleConfig());
        }

        [TearDown]
        public void Teardown()
        {
            provider = null;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestInitialiseBoundaryNullConfig()
        {
            provider.Initialize("whatever", null);
        }

        #endregion Methods
    }
}