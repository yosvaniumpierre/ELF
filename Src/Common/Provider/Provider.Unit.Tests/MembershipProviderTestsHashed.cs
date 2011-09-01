using System.Collections.Specialized;
using System.Configuration.Provider;
using NUnit.Framework;

namespace Avanade.Provider.Unit.Tests
{
    [TestFixture]
    public class MembershipProviderTestsHashed
    {
        #region Fields

        private RepositoryMembershipProvider provider;

        #endregion Fields

        #region Methods

        [TearDown]
        public void CleanupTest()
        {
            provider = null;
        }

        [Test]
        [ExpectedException(typeof(ProviderException))]
        public void GetPasswordGivenGoodUserAndGoodAnswerThrowsException()
        {
            //Arrange
            const string name = "GoodUser";
            const string answer = "GoodAnswer";
            //Act
            provider.GetPassword(name, answer);
        }

        [SetUp]
        public void InitializeTest()
        {
            provider = new EncryptionProvider { Repository = MockRepositoryUtils.GetMockUserRepository() };
            var config = new NameValueCollection
                             {
                                 {"applicationName", "ProviderTestApp"},
                                 {"passwordFormat", "Hashed"},
                                 {"requiresQuestionAndAnswer", "true"}
                             };
            provider.Initialize("", config);
        }

        [Test]
        public void ResetPasswordGoodUserQandARequiredReturnsNewPassword()
        {
            //Arrange
            const string name = "HashUser";
            const string answer = "GoodAnswer";
            //Act
            var actual = provider.ResetPassword(name, answer);
            //Assert
            Assert.AreNotEqual("", actual);
        }

        [Test]
        public void ValidateUserGivenGoodUserGoodPasswordReturnsTrue()
        {
            //Arrange
            const string userName = "HashUser";
            const string userPass = "GoodPass";
            //Act
            var actual = provider.ValidateUser(userName, userPass);
            //Assert
            Assert.IsTrue(actual);
        }

        #endregion Methods
    }
}