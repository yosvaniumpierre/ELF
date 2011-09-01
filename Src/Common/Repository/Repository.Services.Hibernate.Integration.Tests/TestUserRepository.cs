namespace Avanade.Repository.Services.Hibernate.Integration.Tests
{
    using Core;

    using Domain.Security;

    using NUnit.Framework;

    using Security;

    [TestFixture]
    public class TestUserRepository
    {
        #region Fields

        private IUnitOfWork unitOfWork;
        private UserRepository userRepository;

        #endregion Fields

        #region Methods

        [SetUp]
        public void Setup()
        {
            var persistenceManager = new NHibernatePersistenceManager();
            persistenceManager.Init("Data Source=localhost;Initial Catalog=Test;Integrated Security=True", true);
            unitOfWork = persistenceManager.Create();
            userRepository = new UserRepository { UnitOfWork = unitOfWork };
        }

        [Test]
        public void TestSimpleCrud()
        {
            const string name1 = "Good1";
            const string appName1 = "Nothing1";
            const string name2 = "Good2";
            const string appName2 = "Nothing2";
            const string name3 = "Good3";
            const string appName3 = "Nothing3";
            const string email = "unit@test.com";

            unitOfWork.Start();

            var entity1 = new User { UserName = name1, ApplicationName = appName1 };
            var entity2 = new User { UserName = name2, ApplicationName = appName2 };
            userRepository.Add(entity1);
            userRepository.Add(entity2);
            userRepository.Add(new User { UserName = name3, ApplicationName = appName3 });
            Assert.AreEqual(entity1.Id, 1);
            Assert.AreEqual(entity2.Id, 2);

            unitOfWork.Commit();

            var retrieved = userRepository.GetUser(appName2, name2);

            Assert.NotNull(retrieved);
            Assert.AreEqual(2, retrieved.Id);
            Assert.AreEqual(name2, retrieved.UserName);
            Assert.AreEqual(appName2, retrieved.ApplicationName);

            unitOfWork.Start();

            retrieved.Email = email;
            userRepository.Update(retrieved);

            unitOfWork.Commit();

            var retrievedAfterUpdate = userRepository.GetUser(appName2, name2);

            Assert.AreEqual(email, retrievedAfterUpdate.Email);
            Assert.AreEqual(2, retrievedAfterUpdate.Id);

            unitOfWork.Start();

            userRepository.Remove(retrievedAfterUpdate);
            userRepository.Remove(x => x.UserName == name3 && x.ApplicationName == appName3);

            unitOfWork.Commit();

            var retrieveEntity2AfterRemove = userRepository.GetUser(appName2, name2);
            var retrieveEntity3AfterRemove = userRepository.GetUser(appName3, name3);

            Assert.Null(retrieveEntity2AfterRemove);
            Assert.Null(retrieveEntity3AfterRemove);
            Assert.AreEqual(1, userRepository.TotalCount());
        }

        #endregion Methods
    }
}