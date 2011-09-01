using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Web.Security;
using Avanade.Domain.Security;
using Avanade.Repository.Services;
using Avanade.Repository.Services.Hibernate.Core;
using Avanade.Repository.Services.Hibernate.Security;
using Avanade.Repository.Services.Security;
using NUnit.Framework;

namespace Avanade.Provider.Integration.Tests
{
    /// <summary>
    ///This is a test class for RepositoryMembershipProviderTest and is intended
    ///to contain the majority RepositoryMembershipProviderTest Integration Tests.
    /// 
    /// This test requires a properly configured database that is referenced in the 
    /// app.config file included with this package (change it to appropriate values).
    /// 
    /// To fully support the unit tests, your web config will need to include encrypt and decrypt
    /// keys, since some of the functions are not supported with auto-generated keys.  
    ///</summary>
    [TestFixture]
    public class RepositoryMembershipProviderTestsDefault
    {
        #region Fields

        private static NameValueCollection testConfig;
        private static List<UserParameters> testUsers;

        private RepositoryMembershipProvider provider;
        private IUserRepository testRepository;

        #endregion Fields

        #region Methods

        [TestFixtureSetUp]
        public static void FixtureSetup()
        {
            //Set up a test configuration to use.
            testConfig = new NameValueCollection { { "applicationName", "ProviderTestApp" } };

            IUserRepository repository = GetRepository(true);
            var providerSetup = new RepositoryMembershipProvider { Repository = repository };
            providerSetup.Initialize("", testConfig);
            testUsers = TestUtils.GetTestUsers(5, "Default");
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

        [TestFixtureTearDown]
        public static void FixtureTeardown()
        {
            //We will remove our sample users
            IUserRepository repository = GetRepository(false);
            var providerTearDown = new RepositoryMembershipProvider { Repository = repository };
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
        public void ChangePasswordQuestionAndAnswerBadPasswordReturnsFalse()
        {
            UserParameters u = testUsers[0];
            const string badpass = "BadPassword";
            bool result = provider.ChangePasswordQuestionAndAnswer(u.Username, badpass, u.PasswordQuestion, u.PasswordAnswer);
            //Cleanup
            Assert.IsFalse(result);
        }

        [Test]
        public void ChangePasswordQuestionAndAnswerValidUserQandAChanged()
        {
            UserParameters uparm = testUsers[3];
            User u = provider.Repository.GetUser(provider.ApplicationName, uparm.Username);
            var oldQa = new PasswordQandA { Answer = u.PasswordAnswer, Question = u.PasswordQuestion };
            var newQa = new PasswordQandA { Answer = "Lettuce And Carrots", Question = "What Do Rabbits Eat?" };

            provider.ChangePasswordQuestionAndAnswer(uparm.Username, uparm.Password, newQa.Question, newQa.Answer);
            User uCur = provider.Repository.GetUser(provider.ApplicationName, uparm.Username);
            oldQa.Question = uCur.PasswordQuestion;
            oldQa.Answer = uCur.PasswordAnswer;

            //Cleanup
            provider.ChangePasswordQuestionAndAnswer(uparm.Username, uparm.Password, uparm.PasswordQuestion, uparm.PasswordAnswer);
            Assert.AreEqual(oldQa, newQa);
        }

        [Test]
        public void ChangePasswordQuestionAndAnswerValidUserReturnsTrue()
        {
            UserParameters u = testUsers[0];
            const string newquestion = "question";
            const string newanswer = "answer";
            bool result = provider.ChangePasswordQuestionAndAnswer(u.Username, u.Password, newquestion, newanswer);
            //Cleanup
            if (result) provider.ChangePasswordQuestionAndAnswer(u.Username, u.Password, u.PasswordQuestion, u.PasswordAnswer);
            Assert.IsTrue(result);
        }

        [Test]
        [ExpectedException(typeof(MembershipPasswordException))]
        public void ChangePasswordTestBadNewPassThrowsException()
        {
            UserParameters u = testUsers[0];
            const string newpass = "";
            provider.ChangePassword(u.Username, u.Password, newpass);
        }

        [Test]
        public void ChangePasswordTestBadOldPassReturnsFalse()
        {
            UserParameters u = testUsers[0];
            const string newpass = "!Password??999";
            const string badpass = "!!!!BadPass999";
            bool result = provider.ChangePassword(u.Username, badpass, newpass);
            Assert.IsFalse(result);
        }

        [Test]
        public void ChangePasswordTestSamePasswordsReturnsTrue()
        {
            UserParameters u = testUsers[0];
            bool result = provider.ChangePassword(u.Username, u.Password, u.Password);
            Assert.IsTrue(result);
        }

        [Test]
        public void ChangePasswordTestUnapprovedUserReturnsFalse()
        {
            UserParameters u = testUsers[0];
            //Change user to unapproved
            var user = provider.GetUser(u.Username, true);
            user.IsApproved = false;
            provider.UpdateUser(user);

            const string newpass = "!Password??999";
            bool result = provider.ChangePassword(u.Username, u.Password, newpass);
            //Cleanup
            if (result) provider.ChangePassword(u.Username, newpass, u.Password);
            user.IsApproved = true;
            provider.UpdateUser(user);

            Assert.IsFalse(result);
        }

        [Test]
        public void ChangePasswordTestValidUserPasswordChanged()
        {
            UserParameters uparm = testUsers[2];
            const string newpass = "!Password??999";
            provider.ChangePassword(uparm.Username, uparm.Password, newpass);
            string curpass = provider.GetPassword(uparm.Username, uparm.PasswordAnswer);
            //Cleanup
            provider.ChangePassword(uparm.Username, newpass, uparm.Password);
            Assert.AreEqual(newpass, curpass);
        }

        [Test]
        public void ChangePasswordTestValidUserReturnsTrue()
        {
            UserParameters u = testUsers[0];
            const string newpass = "!Password??999";
            bool result = provider.ChangePassword(u.Username, u.Password, newpass);
            //Cleanup
            if (result) provider.ChangePassword(u.Username, newpass, u.Password);
            Assert.IsTrue(result);
        }

        [Test]
        public void CreateUserTestDuplicateEMailReturnsDuplicateEmail()
        {
            UserParameters u = TestUtils.GetValidUser();
            u.Email = testUsers[0].Email;
            MembershipCreateStatus status;
            const MembershipCreateStatus statusExpected = MembershipCreateStatus.DuplicateEmail;
            provider.CreateUser(u.Username, u.Password, u.Email, u.PasswordQuestion,
                                u.PasswordAnswer, u.IsApproved, u.ProviderUserKey, out status);
            Assert.AreEqual(statusExpected, status);
        }

        [Test]
        public void CreateUserTestDuplicateUsernameReturnsDuplicateUsername()
        {
            UserParameters u = TestUtils.GetValidUser();
            u.Username = testUsers[0].Username;
            MembershipCreateStatus status;
            const MembershipCreateStatus statusExpected = MembershipCreateStatus.DuplicateUserName;
            provider.CreateUser(u.Username, u.Password, u.Email, u.PasswordQuestion,
                                u.PasswordAnswer, u.IsApproved, u.ProviderUserKey, out status);
            Assert.AreEqual(statusExpected, status);
        }

        [Test]
        public void CreateUserTestInvalidPasswordReturnsInvalidPassword()
        {
            UserParameters u = TestUtils.GetValidUser();
            u.Password = "";
            MembershipCreateStatus status;
            const MembershipCreateStatus statusExpected = MembershipCreateStatus.InvalidPassword;
            provider.CreateUser(u.Username, u.Password, u.Email, u.PasswordQuestion,
                                u.PasswordAnswer, u.IsApproved, u.ProviderUserKey, out status);
            Assert.AreEqual(statusExpected, status);
        }

        [Test]
        public void CreateUserTestInvalidPasswordReturnsSuccess()
        {
            UserParameters u = TestUtils.GetValidUser();
            MembershipCreateStatus status;
            const MembershipCreateStatus statusExpected = MembershipCreateStatus.Success;
            provider.CreateUser(u.Username, u.Password, u.Email, u.PasswordQuestion,
                                u.PasswordAnswer, u.IsApproved, u.ProviderUserKey, out status);
            //Cleanup
            if (statusExpected == status) provider.DeleteUser(u.Username, true);
            Assert.AreEqual(statusExpected, status);
        }

        [Test]
        public void CreateUserTestValidTestUserReturnsSuccess()
        {
            UserParameters u = TestUtils.GetValidUser();
            MembershipCreateStatus status;
            const MembershipCreateStatus statusExpected = MembershipCreateStatus.Success;
            provider.CreateUser(u.Username, u.Password, u.Email, u.PasswordQuestion,
                                u.PasswordAnswer, u.IsApproved, u.ProviderUserKey, out status);
            //Cleanup
            if (statusExpected == status) provider.DeleteUser(u.Username, true);
            Assert.AreEqual(statusExpected, status);
        }

        [Test]
        public void FindUsersByEmailInvalidUserReturnsNoRecords()
        {
            const string email = "InvalidEmailAddress";
            int total;
            provider.FindUsersByEmail(email, 0, 5, out total);
            Assert.AreEqual(total, 0);
        }

        [Test]
        public void FindUsersByEmailValidUserReturnsOneRecord()
        {
            UserParameters u = testUsers[0];
            int total;
            provider.FindUsersByEmail(u.Email, 0, 5, out total);
            Assert.AreEqual(total, 1);
        }

        [Test]
        public void FindUsersByNameInvalidUserReturnsNoRecords()
        {
            const string badname = "InvalidUsername";
            int total;
            provider.FindUsersByName(badname, 0, 5, out total);
            Assert.AreEqual(total, 0);
        }

        [Test]
        public void FindUsersByNameValidUserReturnsOneRecord()
        {
            UserParameters u = testUsers[0];
            int total;
            provider.FindUsersByName(u.Username, 0, 5, out total);
            Assert.AreEqual(total, 1);
        }

        [Test]
        public void GetAllUsersFourPerPageReturnsFourRecords()
        {
            int total;
            //We should at least get four of our five test users
            provider.GetAllUsers(0, 4, out total);
            Assert.AreEqual(total, 4);
        }

        [Test]
        public void GetNumberOfUsersOnlineFourPerPageReturnsFourRecords()
        {
            //We should at least get four of our five test users
            int total = provider.GetNumberOfUsersOnline();
            Assert.IsFalse(total < 0);
        }

        [Test]
        public void GetPasswordAnswerNotRequiredReturnsGoodPassword()
        {
            UserParameters u = testUsers[0];
            const string answer = "KittyCatsLikeTuna";
            string password = provider.GetPassword(u.Username, answer);
            Assert.AreEqual(password, u.Password);
        }

        [Test]
        [ExpectedException(typeof(MembershipPasswordException))]
        public void GetPasswordBadAnswerThrowsException()
        {
            UserParameters u = testUsers[0];
            var nhTemp = new RepositoryMembershipProvider { Repository = testRepository };
            nhTemp.Initialize("", TestUtils.GetComplexConfig());
            const string answer = "KittyCatsLikeTuna";
            nhTemp.GetPassword(u.Username, answer);
        }

        [Test]
        [ExpectedException(typeof(ProviderException))]
        public void GetPasswordNoRetrievalThrowsException()
        {
            UserParameters u = testUsers[0];
            var nhTemp = new RepositoryMembershipProvider { Repository = testRepository };
            nhTemp.Initialize("", TestUtils.GetNoPasswordConfig());
            const string answer = "KittyCatsLikeTuna";
            nhTemp.GetPassword(u.Username, answer);
        }

        [Test]
        public void GetPasswordValidAnswerReturnsGoodPassword()
        {
            UserParameters u = testUsers[0];
            string password = provider.GetPassword(u.Username, u.PasswordAnswer);
            Assert.AreEqual(password, u.Password);
        }

        [Test]
        public void GetUserValidUserReturnsObject()
        {
            UserParameters u = testUsers[0];
            int total;
            MembershipUserCollection users = provider.FindUsersByName(u.Username, 0, 5, out total);
            MembershipUser mu = provider.GetUser(users[u.Username].ProviderUserKey, true);
            Assert.AreEqual(mu.UserName, u.Username);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InitializeEmptyConfigThrowsException()
        {
            //Arrange
            var nhpTemp = new RepositoryMembershipProvider();
            //Act
            nhpTemp.Initialize("RepositoryMembershipProvider", null);
        }

        [Test]
        public void InitializeNullNameSetsDefault()
        {
            //Arrange
            var nhpTemp = new RepositoryMembershipProvider { Repository = testRepository };
            //Act
            nhpTemp.Initialize(null, testConfig);
            //Assert
            Assert.IsNotNull(nhpTemp.Repository);
        }

        [Test]
        public void InitializeValidParmsDoesNotThrowException()
        {
            //Arrange
            var nhpTemp = new RepositoryMembershipProvider { Repository = testRepository };
            //Act
            nhpTemp.Initialize("RepositoryMembershipProvider", testConfig);
            //Assert
            Assert.IsNotNull(nhpTemp.Repository);
        }

        [Test]
        public void PropertiesExerciseAllGetsAndSets()
        {
            var nhTemp = new RepositoryMembershipProvider();
            nhTemp.Initialize("RepositoryMembershipProvider", TestUtils.GetComplexConfig());
            nhTemp.ApplicationName = "TempApp";
            Assert.AreEqual(nhTemp.ApplicationName, "TempApp");
            Assert.AreEqual(nhTemp.MaxInvalidPasswordAttempts, 3);
            Assert.AreEqual(nhTemp.MinRequiredNonAlphanumericCharacters, 1);
            Assert.AreEqual(nhTemp.MinRequiredPasswordLength, 7);
            Assert.AreEqual(nhTemp.PasswordAttemptWindow, 10);
            Assert.AreEqual(nhTemp.PasswordStrengthRegularExpression, "^.*(?=.{6,})(?=.*[a-z])(?=.*[A-Z]).*$");
        }

        [Test]
        public void ResetPasswordAnswerNotRequiredReturnsNewPassword()
        {
            UserParameters u = testUsers[0];
            const string badanswer = "BadPassword";
            string newPass = provider.ResetPassword(u.Username, badanswer);
            provider.ChangePassword(u.Username, newPass, u.Password);
            Assert.AreNotEqual(newPass, "");
        }

        [Test]
        [ExpectedException(typeof(MembershipPasswordException))]
        public void ResetPasswordBadAnswerThrowsException()
        {
            UserParameters u = testUsers[0];
            const string badanswer = "BadPassword";
            var nhTemp = new RepositoryMembershipProvider();
            nhTemp.Initialize("RepositoryMembershipProvider", TestUtils.GetComplexConfig());
            nhTemp.ResetPassword(u.Username, badanswer);
        }

        [Test]
        [ExpectedException(typeof(MembershipPasswordException))]
        public void ResetPasswordBadUserThrowsException()
        {
            const string baduser = "InvalidUser";
            provider.ResetPassword(baduser, baduser);
        }

        [Test]
        public void ResetPasswordGoodUserReturnsNewPassword()
        {
            UserParameters u = testUsers[1];
            string newPass = provider.ResetPassword(u.Username, u.PasswordAnswer);
            provider.ChangePassword(u.Username, newPass, u.Password);
            Assert.AreNotEqual(newPass, "");
        }

        [Test]
        [ExpectedException(typeof(ProviderException))]
        public void ResetPasswordNullAnswerThrowsException()
        {
            UserParameters u = testUsers[0];
            var nhTemp = new RepositoryMembershipProvider { Repository = testRepository };
            nhTemp.Initialize("", TestUtils.GetComplexConfig());
            nhTemp.ResetPassword(u.Username, null);
        }

        [SetUp]
        public void Setup()
        {
            testRepository = GetRepository(false);
            provider = new RepositoryMembershipProvider { Repository = testRepository };
            provider.Initialize("", testConfig);
            testRepository.UnitOfWork.Start();
        }

        [TearDown]
        public void Teardown()
        {
            testRepository.UnitOfWork.Rollback();
            provider = null;
        }

        [Test]
        public void UnlockUserInvalidUserReturnsTrue()
        {
            bool result = provider.UnlockUser("TheKingOfFrance");
            Assert.IsFalse(result);
        }

        [Test]
        public void UnlockUserValidUserReturnsTrue()
        {
            UserParameters u = testUsers[4];
            bool result = provider.UnlockUser(u.Username);
            Assert.IsTrue(result);
        }

        [Test]
        public void ValidateUserBadPasswordReturnsFalse()
        {
            UserParameters u = testUsers[0];
            bool result = provider.ValidateUser(u.Username, String.Empty);
            Assert.IsFalse(result);
        }

        [Test]
        public void ValidateUserBadUserReturnsFalse()
        {
            bool result = provider.ValidateUser("TheKingOfFrance", String.Empty);
            Assert.IsFalse(result);
        }

        [Test]
        public void ValidateUserGoodPasswordReturnsTrue()
        {
            UserParameters u = testUsers[0];
            bool result = provider.ValidateUser(u.Username, u.Password);
            Assert.IsTrue(result);
        }

        private static IUserRepository GetRepository(bool runMigrations)
        {
            var persistenceManager = new NHibernatePersistenceManager();
            persistenceManager.Init("Data Source=localhost;Initial Catalog=Test;Integrated Security=True", runMigrations);
            IUnitOfWork unitOfWork = persistenceManager.Create();
            return new UserRepository { UnitOfWork = unitOfWork };
        }

        #endregion Methods
    }
}