using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Web.Security;
using Moq;
using NUnit.Framework;
using Avanade.Repository.Services.Security;

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
    public class MembershipProviderTestsDefault
    {
        #region Fields

        private RepositoryMembershipProvider provider;

        #endregion Fields

        #region Methods

        [Test]
        public void ChangePasswordBadUserBadPassReturnsFalse()
        {
            //Arrange
            const string user = "GoodUser";
            const string oldpass = "BadPass";
            const string newpass = "Bad";
            //Act
            bool actual = provider.ChangePassword(user, oldpass, newpass);
            //Assert
            Assert.IsFalse(actual);
        }

        [Test]
        [ExpectedException(typeof(MembershipPasswordException))]
        public void ChangePasswordGoodUserBadPassThrowsException()
        {
            //Arrange
            const string user = "GoodUser";
            const string oldpass = "GoodPass";
            const string newpass = "Bad";
            //Act
            provider.ChangePassword(user, oldpass, newpass);
            //Assert
        }

        [Test]
        public void ChangePasswordGoodUserGoodPassReturnsTrue()
        {
            //Arrange
            const string user = "GoodUser";
            const string oldpass = "GoodPass";
            const string newpass = "ABC123!?";
            //Act
            bool actual = provider.ChangePassword(user, oldpass, newpass);
            //Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void ChangePasswordQuestionAndAnswerBadUserReturnsFalse()
        {
            //Arrange
            const string user = "BadUser";
            const string pass = "BadPass";
            const string question = "Good";
            const string answer = "Answer";
            //Act
            bool actual = provider.ChangePasswordQuestionAndAnswer(user, pass, question, answer);
            //Assert
            Assert.IsFalse(actual);
        }

        [Test]
        public void ChangePasswordQuestionAndAnswerGoodUserReturnsTrue()
        {
            //Arrange
            const string user = "GoodUser";
            const string pass = "GoodPass";
            const string question = "Good";
            const string answer = "Answer";
            //Act
            bool actual = provider.ChangePasswordQuestionAndAnswer(user, pass, question, answer);
            //Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void CreateUserDupEmailReturnsDupEMail()
        {
            //Arrange
            const string user = "GoodUser";
            const string pass = "ABC123!?";
            const string email = "DupEmail";
            const bool approved = true;
            const string question = "Question";
            const string answer = "Answer";
            const int key = 1;
            MembershipCreateStatus result;
            //Act
            provider.CreateUser(user, pass, email, question, answer, approved, key, out result);
            //Assert
            Assert.AreEqual(MembershipCreateStatus.DuplicateEmail, result);
        }

        [Test]
        public void CreateUserDupUserReturnsDupUser()
        {
            //Arrange
            const string user = "GoodUser";
            const string pass = "ABC123!?";
            const string email = "NewEmail";
            const bool approved = true;
            const string question = "Question";
            const string answer = "Answer";
            const int key = 1;
            MembershipCreateStatus result;
            //Act
            provider.CreateUser(user, pass, email, question, answer, approved, key, out result);
            //Assert
            Assert.AreEqual(MembershipCreateStatus.DuplicateUserName, result);
        }

        [Test]
        public void CreateUserInvalidPasswordReturnsInvalidPassword()
        {
            //Arrange
            const string user = "NewUser";
            const string pass = "Bad";
            const string email = "NewEmail";
            const bool approved = true;
            const string question = "Question";
            const string answer = "Answer";
            const int key = 1;
            MembershipCreateStatus result;
            //Act
            provider.CreateUser(user, pass, email, question, answer, approved, key, out result);
            //Assert
            Assert.AreEqual(MembershipCreateStatus.InvalidPassword, result);
        }

        [Test]
        public void CreateUserValidDataReturnsSuccess()
        {
            //Arrange
            const string user = "NewUser";
            const string pass = "ABC123!?";
            const string email = "NewEmail";
            const bool approved = true;
            const string question = "Question";
            const string answer = "Answer";
            const int key = 1;
            MembershipCreateStatus result;
            //Act
            provider.CreateUser(user, pass, email, question, answer, approved, key, out result);
            //Assert
            Assert.AreEqual(MembershipCreateStatus.Success, result);
        }

        [Test]
        public void DeleteUserBadUserReturnsTrue()
        {
            //Arrange
            const string username = "BadUser";
            //Act
            bool actual = provider.DeleteUser(username, true);
            //Assert
            Assert.IsTrue(actual);
        }

        [Test]
        [ExpectedException(typeof(MemberAccessException))]
        public void DeleteUserExceptionUserThrowsException()
        {
            //Arrange
            const string username = "ExceptionUser";
            //Act
            provider.DeleteUser(username, true);
            //Assert
        }

        [Test]
        public void DeleteUserGoodUserReturnsTrue()
        {
            //Arrange
            const string username = "GoodUser";
            //Act
            bool actual = provider.DeleteUser(username, true);
            //Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void ExercisePublicProperties()
        {
            //Arrange
            //Act
            string applicationName = provider.ApplicationName;
            provider.ApplicationName = applicationName;
            int maxInvalidPasswordAttempts = provider.MaxInvalidPasswordAttempts;
            int minRequiredNonAlphanumericCharacters = provider.MinRequiredNonAlphanumericCharacters;
            int minRequiredPasswordLength = provider.MinRequiredPasswordLength;
            int passwordAttemptWindow = provider.PasswordAttemptWindow;
            string passwordStrengthRegularExpression = provider.PasswordStrengthRegularExpression;
            //Assert
            Assert.AreEqual(provider.ApplicationName, applicationName);
            Assert.AreEqual(provider.MaxInvalidPasswordAttempts, maxInvalidPasswordAttempts);
            Assert.AreEqual(provider.MinRequiredNonAlphanumericCharacters, minRequiredNonAlphanumericCharacters);
            Assert.AreEqual(provider.MinRequiredPasswordLength, minRequiredPasswordLength);
            Assert.AreEqual(provider.PasswordAttemptWindow, passwordAttemptWindow);
            Assert.AreEqual(provider.PasswordStrengthRegularExpression, passwordStrengthRegularExpression);
        }

        [Test]
        public void FindUserByEmailGivenBadEmailReturnsZeroRecords()
        {
            //Arrange
            const string email = "BadEmail";
            int recs;
            const int expectedRecs = 0;
            //Act
            MembershipUserCollection actual = provider.FindUsersByEmail(email, 0, 99, out recs);
            //Assert
            Assert.AreEqual(expectedRecs, recs);
            Assert.AreEqual(expectedRecs, actual.Count);
        }

        [Test]
        public void FindUserByEmailGivenDuplicateEmailReturnsTwoRecords()
        {
            //Arrange
            const string email = "DupEmail";
            int recs;
            const int expectedRecs = 2;
            //Act
            MembershipUserCollection actual = provider.FindUsersByEmail(email, 0, 99, out recs);
            //Assert
            Assert.AreEqual(expectedRecs, recs);
            Assert.AreEqual(expectedRecs, actual.Count);
        }

        [Test]
        [ExpectedException(typeof(MemberAccessException))]
        public void FindUserByEmailGivenExceptionThrowsMemberAccessException()
        {
            //Arrange
            const string email = "ExceptionUser";
            int recs;
            //Act
            provider.FindUsersByEmail(email, 0, 99, out recs);
            //Assert
        }

        [Test]
        public void FindUserByEmailGivenGoodEmailReturnsOneRecord()
        {
            //Arrange
            const string email = "GoodEmail";
            int recs;
            const int expectedRecs = 1;
            //Act
            MembershipUserCollection actual = provider.FindUsersByEmail(email, 0, 99, out recs);
            //Assert
            Assert.AreEqual(expectedRecs, recs);
            Assert.AreEqual(expectedRecs, actual.Count);
        }

        [Test]
        public void FindUserByNameGivenBadNameReturnsZeroRecords()
        {
            //Arrange
            const string name = "BadName";
            int recs;
            const int expectedRecs = 0;
            //Act
            MembershipUserCollection actual = provider.FindUsersByName(name, 0, 99, out recs);
            //Assert
            Assert.AreEqual(expectedRecs, recs);
            Assert.AreEqual(expectedRecs, actual.Count);
        }

        [Test]
        public void FindUserByNameGivenDuplicateNameReturnsTwoRecords()
        {
            //Arrange
            const string name = "DupName";
            int recs;
            const int expectedRecs = 2;
            //Act
            MembershipUserCollection actual = provider.FindUsersByName(name, 0, 99, out recs);
            //Assert
            Assert.AreEqual(expectedRecs, recs);
            Assert.AreEqual(expectedRecs, actual.Count);
        }

        [Test]
        [ExpectedException(typeof(MemberAccessException))]
        public void FindUserByNameGivenExceptionThrowsMemberAccessException()
        {
            //Arrange
            const string name = "ExceptionUser";
            int recs;
            //Act
            provider.FindUsersByName(name, 0, 99, out recs);
            //Assert
        }

        [Test]
        public void FindUserByNameGivenGoodNameReturnsOneRecord()
        {
            //Arrange
            const string name = "GoodName";
            int recs;
            const int expectedRecs = 1;
            //Act
            MembershipUserCollection actual = provider.FindUsersByName(name, 0, 99, out recs);
            //Assert
            Assert.AreEqual(expectedRecs, recs);
            Assert.AreEqual(expectedRecs, actual.Count);
        }

        [Test]
        [ExpectedException(typeof(MemberAccessException))]
        public void GetAllUsersGivenExceptionUserThrowsException()
        {
            //Arrange
            int tot;
            //Act
            provider.GetAllUsers(2, 99, out tot);
            //Assert
        }

        [Test]
        public void GetAllUsersGivenTwoUsersReturnsTwoUsers()
        {
            //Arrange
            const int expected = 2;
            int tot;
            //Act
            MembershipUserCollection actual = provider.GetAllUsers(0, 99, out tot);
            //Assert
            Assert.AreEqual(expected, actual.Count);
            Assert.AreEqual(expected, tot);
        }

        [Test]
        public void GetAllUsersGivenZeroUsersReturnsZeroUsers()
        {
            //Arrange
            const int expected = 0;
            int tot;
            //Act
            MembershipUserCollection actual = provider.GetAllUsers(1, 99, out tot);
            //Assert
            Assert.AreEqual(expected, actual.Count);
            Assert.AreEqual(expected, tot);
        }

        [Test]
        [ExpectedException(typeof(MemberAccessException))]
        public void GetPasswordGivenBadUserThrowsException()
        {
            //Arrange
            const string name = "BadUser";
            const string answer = "BadAnswer";
            //Act
            provider.GetPassword(name, answer);
            //Assert
        }

        [Test]
        public void GetPasswordGivenGoodUserAndBadAnswerWithoutRequireAnswerReturnsPassword()
        {
            //Arrange
            const string name = "BadAnswerUser";
            const string answer = "BadAnswer";
            const string expected = "GoodPass";
            //Act
            string actual = provider.GetPassword(name, answer);
            //Assert
            Assert.AreSame(expected, actual);
        }

        [Test]
        public void GetPasswordGivenGoodUserAndGoodAnswerReturnsPassword()
        {
            //Arrange
            const string name = "GoodUser";
            const string answer = "GoodAnswer";
            const string expected = "GoodPass";
            //Act
            string actual = provider.GetPassword(name, answer);
            //Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [ExpectedException(typeof(ProviderException))]
        public void GetPasswordWhenRetrievalDisabledThrowsException()
        {
            //Arrange
            var noPassProv = MockRepositoryUtils.GetProviderWithNoPasswordRetrievalOrReset();
            const string name = "BadUser";
            const string answer = "BadAnswer";
            //Act
            noPassProv.GetPassword(name, answer);
            //Assert
        }

        [Test]
        [ExpectedException(typeof(MemberAccessException))]
        public void GetUserBadUserIdThrowsException()
        {
            //Arrange
            const int id = 999;
            //Act
            provider.GetUser(id, true);
            //Assert
        }

        [Test]
        [ExpectedException(typeof(MemberAccessException))]
        public void GetUserBadUserThrowsException()
        {
            //Arrange
            const string name = "ExceptionUser";
            //Act
            provider.GetUser(name, true);
            //Assert
        }

        [Test]
        public void GetUserGoodUserIdOnlineReturnsUser()
        {
            //Arrange
            const int id = 1;
            //Act
            MembershipUser actual = provider.GetUser(id, true);
            //Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void GetUserGoodUserOnlineReturnsUser()
        {
            //Arrange
            const string name = "GoodUser";
            //Act
            MembershipUser actual = provider.GetUser(name, true);
            //Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        [ExpectedException(typeof(MemberAccessException))]
        public void GetUserNameByEmailExceptionUserThrowsException()
        {
            //Arrange
            const string email = "ExceptionEmail";
            //Act
            provider.GetUserNameByEmail(email);
            //Assert
        }

        [Test]
        [ExpectedException(typeof(ProviderException))]
        public void InitializeCheckEncryptionKeyFailsThrowsProviderException()
        {
            //Arrange
            var tmpProv = new EncryptionErrorProvider();
            //Act
            var config = new NameValueCollection { { "passwordFormat", "Hashed" } };
            tmpProv.Initialize("", config);
            //Assert
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InitializeNullConfigThrowsArgumentNullException()
        {
            //Arrange
            var mockRepository = new Mock<IUserRepository>();
            provider = new RepositoryMembershipProvider { Repository = mockRepository.Object };
            //Act
            provider.Initialize("", null);
            //Assert
        }

        [Test]
        public void InitializeNullNameSetsDefaultName()
        {
            //Arrange
            var mockRepository = new Mock<IUserRepository>();
            provider = new RepositoryMembershipProvider { Repository = mockRepository.Object };
            const string expected = "RepositoryMembershipProvider";
            //Act
            provider.Initialize("", ConfigUtils.GetMembershipNoPasswordConfig());
            string actual = provider.Name;
            //Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ResetPasswordGoodUserNoAnswerReturnsNewPassword()
        {
            //Arrange
            const string name = "GoodUser";
            //Act
            string actual = provider.ResetPassword(name, null);
            //Assert
            Assert.AreNotEqual("", actual);
        }

        [Test]
        [ExpectedException(typeof(MembershipPasswordException))]
        public void ResetPasswordLockedUserThrowsException()
        {
            //Arrange
            const string name = "LockedUser";
            //Act
            provider.ResetPassword(name, null);
            //Assert
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void ResetPasswordWhenRetrievalDisabledThrowsException()
        {
            //Arrange
            var noPassProv = MockRepositoryUtils.GetProviderWithNoPasswordRetrievalOrReset();
            const string name = "BadUser";
            const string answer = "BadAnswer";
            //Act
            noPassProv.ResetPassword(name, answer);
            //Assert
        }

        [SetUp]
        public void Setup()
        {
            provider = new RepositoryMembershipProvider { Repository = MockRepositoryUtils.GetMockUserRepository() };
            provider.Initialize("", ConfigUtils.GetMembershipNoPasswordConfig());
        }

        [TearDown]
        public void Teardown()
        {
            provider = null;
        }

        [Test]
        [ExpectedException(typeof(MemberAccessException))]
        public void UnlockUserExceptionUserThrowsException()
        {
            //Arrange
            const string name = "ExceptionUser";
            //Act
            provider.UnlockUser(name);
            //Assert
        }

        [Test]
        public void UnlockUserGoodUserReturnsTrue()
        {
            //Arrange
            const string name = "GoodUser";
            //Act
            bool actual = provider.UnlockUser(name);
            //Assert
            Assert.IsTrue(actual);
        }

        [Test]
        [ExpectedException(typeof(MemberAccessException))]
        public void UpdateUserBadUserThrowsException()
        {
            //Arrange
            //Act
            provider.UpdateUser(null);
            //Assert
        }

        [Test]
        public void UpdateUserGoodUserDoesNotThrowError()
        {
            //Arrange
            MembershipUser m = provider.GetUser("GoodUser", true);
            //Act
            provider.UpdateUser(m);
            //Assert
            Assert.IsTrue(true);
        }

        [Test]
        public void ValidateUserGivenBadUserBadPasswordReturnsFalse()
        {
            //Arrange
            const string userName = "BadUser";
            const string userPass = "BadPass";
            //Act
            bool actual = provider.ValidateUser(userName, userPass);
            //Assert
            Assert.IsFalse(actual);
        }

        [Test]
        [ExpectedException(typeof(MemberAccessException))]
        public void ValidateUserGivenExceptionThrowsMemberAccessException()
        {
            //Arrange
            const string userName = "ExceptionUser";
            const string userPass = "BadPass";
            //Act
            provider.ValidateUser(userName, userPass);
            //Assert
        }

        [Test]
        public void ValidateUserGivenGoodUserBadPasswordReturnsFalse()
        {
            //Arrange
            const string userName = "GoodUser";
            const string userPass = "BadPass";
            //Act
            bool actual = provider.ValidateUser(userName, userPass);
            //Assert
            Assert.IsFalse(actual);
        }

        [Test]
        public void ValidateUserGivenGoodUserGoodPasswordReturnsTrue()
        {
            //Arrange
            const string userName = "GoodUser";
            const string userPass = "GoodPass";
            //Act
            bool actual = provider.ValidateUser(userName, userPass);
            //Assert
            Assert.IsTrue(actual);
        }

        #endregion Methods
    }
}