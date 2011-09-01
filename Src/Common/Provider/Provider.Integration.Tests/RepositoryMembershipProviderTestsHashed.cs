using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Web.Security;
using Avanade.Repository.Services;
using Avanade.Repository.Services.Hibernate.Core;
using NUnit.Framework;
using Avanade.Repository.Services.Hibernate.Security;
using Avanade.Repository.Services.Security;

namespace Avanade.Provider.Integration.Tests
{
    /// <summary>
    /// These tests are based on a membership provider set up for 
    /// Hashed passwords.
    /// </summary>
    [TestFixture]
    public class RepositoryMembershipProviderTestsHashed
    {
        #region Fields

        private static NameValueCollection testConfig;
        private static List<UserParameters> testUsers;

        private RepositoryMembershipProvider provider;
        private IUserRepository testRepository;

        #endregion Fields

        #region Methods

        //You can use the following additional attributes as you write your tests:
        //Use ClassInitialize to run code before running the first test in the class
        [TestFixtureSetUp]
        public static void FixtureSetup()
        {
            //Set up a test configuration to use.
            testConfig = new NameValueCollection { { "applicationName", "ProviderTestApp" }, { "passwordFormat", "Hashed" } };

            IUserRepository repository = GetRepository(true);
            var providerSetup = new EncryptionProvider { Repository = repository };
            providerSetup.Initialize("RepositoryMembershipProvider", testConfig);
            testUsers = TestUtils.GetTestUsers(5, "Hashed");
            repository.UnitOfWork.Start();
            foreach (var u in testUsers)
            {
                MembershipCreateStatus status;
                providerSetup.CreateUser(u.Username, u.Password, u.Email, u.PasswordQuestion,
                    u.PasswordAnswer, u.IsApproved, u.ProviderUserKey, out status);
            }
            repository.UnitOfWork.Commit();
            repository.UnitOfWork.Dispose();
        }

        //
        //Use ClassCleanup to run code after all tests in a class have run
        [TestFixtureTearDown]
        public static void FixtureTeardown()
        {
            //We will remove our sample users
            IUserRepository repository = GetRepository(false);
            var providerTearDown = new EncryptionProvider { Repository = repository };
            providerTearDown.Initialize("", testConfig);
            repository.UnitOfWork.Start();
            foreach (var user in testUsers)
            {
                providerTearDown.DeleteUser(user.Username, true);
            }
            repository.UnitOfWork.Commit();
            repository.UnitOfWork.Dispose();
        }

        [Test]
        [ExpectedException(typeof(ProviderException))]
        public void HashedGetPasswordAnyAnswerThrowsException()
        {
            UserParameters u = testUsers[0];
            const string answer = "KittyCatsLikeTuna";
            provider.GetPassword(u.Username, answer);
        }

        [Test]
        public void HashedValidateUserGoodPasswordReturnsTrue()
        {
            UserParameters u = testUsers[0];
            bool result = provider.ValidateUser(u.Username, u.Password);
            Assert.IsTrue(result);
        }

        //
        //Use TestInitialize to run code before running each test
        [SetUp]
        public void Setup()
        {
            testRepository = GetRepository(false);
            provider = new EncryptionProvider { Repository = testRepository };
            provider.Initialize("", testConfig);
            testRepository.UnitOfWork.Start();
        }

        //
        //Use TestCleanup to run code after each test has run
        [TearDown]
        public void Teardown()
        {
            testRepository.UnitOfWork.Rollback();
            provider = null;
        }

        private static IUserRepository GetRepository(bool runMigrations)
        {
            var persistenceManager = new NHibernatePersistenceManager();
            persistenceManager.Init("Data Source=localhost;Initial Catalog=Test;Integrated Security=True", runMigrations);
            IUnitOfWork unitOfWork = persistenceManager.Create();
            return new UserRepository { UnitOfWork = unitOfWork };
        }

        #endregion Methods

        #region Other

        //

        #endregion Other
    }
}