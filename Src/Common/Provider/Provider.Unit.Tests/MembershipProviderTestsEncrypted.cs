using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Web.Security;
using NUnit.Framework;

namespace Avanade.Provider.Unit.Tests
{
    [TestFixture]
    public class MembershipProviderTestsEncrypted
    {
        #region Fields

        private RepositoryMembershipProvider provider;

        #endregion Fields

        #region Methods

        [Test]
        public void ChangePasswordGoodUserGoodPassReturnsTrue()
        {
            //Arrange
            const string user = "EncryptUser";
            const string oldpass = "GoodPass";
            const string newpass = "ABC123!?";
            //Act
            var actual = provider.ChangePassword(user, oldpass, newpass);
            //Assert
            Assert.IsTrue(actual);
        }

        [Test]
        [ExpectedException(typeof(MembershipPasswordException))]
        public void GetPasswordGivenGoodUserAndBadAnswerWithRequireAnswerThrowsException()
        {
            //Arrange
            const string name = "EncryptUser";
            const string answer = "BadAnswer";
            //Act
            provider.GetPassword(name, answer);
            //Assert
        }

        [Test]
        public void GetPasswordGivenGoodUserAndGoodAnswerReturnsPassword()
        {
            //Arrange
            const string name = "EncryptUser";
            const string answer = "GoodAnswer";
            const string expected = "GoodPass";
            //Act
            var actual = provider.GetPassword(name, answer);
            //Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetPasswordGivenGoodUserAndGoodAnswerWithRequireAnswerReturnsPassword()
        {
            //Arrange
            const string name = "EncryptUser";
            const string answer = "GoodAnswer";
            const string expected = "GoodPass";
            //Act
            var actual = provider.GetPassword(name, answer);
            //Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [ExpectedException(typeof(MembershipPasswordException))]
        public void ResetPasswordBadAnswerQandARequiredThrowsException()
        {
            //Arrange
            const string name = "EncryptUser";
            const string answer = "BadAnswer";
            //Act
            provider.ResetPassword(name, answer);
            //Assert
        }

        [Test]
        public void ResetPasswordGoodUserQandARequiredReturnsNewPassword()
        {
            //Arrange
            const string name = "EncryptUser";
            const string answer = "GoodAnswer";
            //Act
            var actual = provider.ResetPassword(name, answer);
            //Assert
            Assert.AreNotEqual("", actual);
        }

        [Test]
        [ExpectedException(typeof(ProviderException))]
        public void ResetPasswordNullAnswerQandARequiredThrowsException()
        {
            //Arrange
            const string name = "GoodUser";
            //Act
            provider.ResetPassword(name, null);
            //Assert
        }

        [SetUp]
        public void Setup()
        {
            provider = new EncryptionProvider { Repository = MockRepositoryUtils.GetMockUserRepository() };
            var config = new NameValueCollection
                             {
                                 {"applicationName", "ProviderTestApp"},
                                 {"requiresQuestionAndAnswer", "true"},
                                 {"passwordFormat", "Encrypted"}
                             };
            provider.Initialize("", config);
        }

        [TearDown]
        public void Teardown()
        {
            provider = null;
        }

        #endregion Methods
    }
}