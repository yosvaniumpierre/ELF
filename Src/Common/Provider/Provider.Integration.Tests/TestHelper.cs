using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.Configuration;
using Avanade.Domain.Security;
using Avanade.Repository.Services.Security;
using Moq;

namespace Avanade.Provider.Integration.Tests
{
    public struct PasswordQandA
    {
        #region Properties

        public string Answer
        {
            get;
            set;
        }

        public string Question
        {
            get;
            set;
        }

        #endregion Properties
    }

    public class EncryptionErrorProvider : RepositoryMembershipProvider
    {
        #region Methods

        protected override MachineKeySection GetMachineKeySection()
        {
            var mk = new MachineKeySection { ValidationKey = "AutoGenerate" };
            return mk;
        }

        #endregion Methods
    }

    public class EncryptionProvider : RepositoryMembershipProvider
    {
        #region Methods

        protected override MachineKeySection GetMachineKeySection()
        {
            var mk = new MachineKeySection { ValidationKey = "7D30287B722BF7141915476F0609FFD604CBB5243D8574F85BA5B496FA58D3EE49A8CE1E07E958F145967495A56E5B6298082070C0488F7B4FC42EDE9956422E" };
            return mk;
        }

        #endregion Methods
    }

    public static class MockRepositoryUtils
    {
        #region Methods

        public static IUserRepository GetMockRepository()
        {
            const string appName = "ProviderTestApp";
            var goodUser = new User
            {
                Id = 1,
                Password = "GoodPass",
                IsApproved = true,
                CreationDate = DateTime.Now,
                PasswordAnswer = "GoodAnswer",
                LastLockedOutDate = DateTime.Now,
                LastPasswordChangedDate = DateTime.Now,
                LastLoginDate = DateTime.Now,
                LastActivityDate = DateTime.Now,
                FailedPasswordAnswerAttemptWindowStart = DateTime.Now,
                FailedPasswordAttemptWindowStart = DateTime.Now,
                UserName = "GoodUser",
                ApplicationName = appName
            };

            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(v => v.GetUser(appName, "GoodUser")).Returns(goodUser);
            mockRepo.Setup(v => v.GetUser(appName, "LockedUser")).Returns(new User { IsLockedOut = true });
            mockRepo.Setup(v => v.GetUser(appName, "BadAnswerUser")).Returns(new User { PasswordAnswer = "GoodAnswer", Password = "GoodPass" });
            mockRepo.Setup(v => v.GetUser(appName, "EncryptUser")).Returns(new User { PasswordAnswer = "SerLEVf28XZ/mBLKLgqulBDfUK05rOsefCL0gd+WRDE=", Password = "Hei77AsDaWtwcvWYAJFawnB0X7BiukYVd+62O6lthNY=", IsApproved = true });
            mockRepo.Setup(v => v.GetUser(appName, "HashUser")).Returns(new User { PasswordAnswer = "/jGKx1DvdLPnZk1ZuQaz2fSFdws=", Password = "UAFsjFEtDHxMGwlRE/ICHnUehCs=", IsApproved = true });
            mockRepo.Setup(v => v.GetUser(appName, "NewUser")).Returns((User)null);
            mockRepo.Setup(v => v.GetUser(appName, "ExceptionUser")).Throws(new Exception());
            mockRepo.Setup(v => v.GetUserById(appName, 1)).Returns(goodUser);
            mockRepo.Setup(v => v.GetUserById(appName, 999)).Throws(new Exception());
            mockRepo.Setup(v => v.FindUsersByEmail("GoodEmail", 0, 99, appName)).Returns(GetSetupUsers(1));
            mockRepo.Setup(v => v.FindUsersByEmail("BadEmail", 0, 99, appName)).Returns(new List<User>());
            mockRepo.Setup(v => v.FindUsersByEmail("DupEmail", 0, 99, appName)).Returns(GetSetupUsers(2));
            mockRepo.Setup(v => v.FindUsersByEmail("ExceptionMail", 0, 99, appName)).Throws(new Exception());
            mockRepo.Setup(v => v.FindUsersByName("GoodName", 0, 99, appName)).Returns(GetSetupUsers(1));
            mockRepo.Setup(v => v.FindUsersByName("BadName", 0, 99, appName)).Returns(new List<User>());
            mockRepo.Setup(v => v.FindUsersByName("DupName", 0, 99, appName)).Returns(GetSetupUsers(2));
            mockRepo.Setup(v => v.FindUsersByName("ExceptionMail", 0, 99, appName)).Throws(new Exception());
            mockRepo.Setup(v => v.GetUserNameByEmail(appName, "NewEmail")).Returns("");
            mockRepo.Setup(v => v.GetUserNameByEmail(appName, "DupEmail")).Returns("DupUser");
            mockRepo.Setup(v => v.GetUserNameByEmail(appName, "ExceptionEmail")).Throws(new Exception());
            mockRepo.Setup(v => v.GetAllUsers(0, 99, appName)).Returns(GetSetupUsers(2));
            mockRepo.Setup(v => v.GetAllUsers(1, 99, appName)).Returns(new List<User>());
            mockRepo.Setup(v => v.GetAllUsers(2, 99, appName)).Throws(new Exception());
            return mockRepo.Object;
        }

        public static RepositoryMembershipProvider GetProviderWithNoPasswordRetrievalOrReset()
        {
            var repository = new Mock<IUserRepository>();
            var prov = new RepositoryMembershipProvider { Repository = repository.Object };
            var config = new NameValueCollection { { "enablePasswordRetrieval", "false" }, { "enablePasswordReset", "false" } };
            prov.Initialize("", config);
            return prov;
        }

        public static List<User> GetSetupUsers(int numUsers)
        {
            var uList = new List<User>();
            for (int i = 0; i < numUsers; i++)
            {
                var u = new User { CreationDate = DateTime.Now, UserName = "SampleUser" + i, Id = i };
                uList.Add(u);
            }
            return uList;
        }

        #endregion Methods
    }

    public static class TestUtils
    {
        #region Methods

        public static NameValueCollection GetComplexConfig()
        {
            var config = new NameValueCollection
                             {
                                 {"applicationName", "ProviderTestApp"},
                                 {"maxInvalidPasswordAttempts", "3"},
                                 {"passwordAttemptWindow", "10"},
                                 {"minRequiredNonAlphanumericCharacters", "1"},
                                 {"minRequiredPasswordLength", "7"},
                                 {"passwordStrengthRegularExpression", "^.*(?=.{6,})(?=.*[a-z])(?=.*[A-Z]).*$"},
                                 {"enablePasswordReset", "true"},
                                 {"enablePasswordRetrieval", "true"},
                                 {"requiresQuestionAndAnswer", "true"},
                                 {"requiresUniqueEmail", "true"}
                             };

            return config;
        }

        public static NameValueCollection GetNoPasswordConfig()
        {
            var config = new NameValueCollection
                             {
                                 {"applicationName", "ProviderTestApp"},
                                 {"maxInvalidPasswordAttempts", "5"},
                                 {"passwordAttemptWindow", "10"},
                                 {"minRequiredNonAlphanumericCharacters", "1"},
                                 {"minRequiredPasswordLength", "7"},
                                 {"passwordStrengthRegularExpression", String.Empty},
                                 {"enablePasswordReset", "true"},
                                 {"enablePasswordRetrieval", "false"},
                                 {"requiresQuestionAndAnswer", "false"},
                                 {"requiresUniqueEmail", "true"},
                                 {"passwordFormat", "Clear"}
                             };

            return config;
        }

        public static List<UserParameters> GetTestUsers(int numUsers, string prefix)
        {
            var t = new List<UserParameters>();
            for (int i = 0; i < numUsers; i++)
            {
                var u = new UserParameters { Username = prefix + "TestUser" + i, Password = prefix + "!Password?" + i };

                u.Email = u.Username + "@testdomain.com";
                u.PasswordQuestion = prefix + "TestPasswordQuestion" + i;
                u.PasswordAnswer = prefix + "TestPasswordAnswer" + i;
                u.IsApproved = true;
                u.ProviderUserKey = null;

                t.Add(u);
            }
            return t;
        }

        public static UserParameters GetValidUser()
        {
            var u = new UserParameters
            {
                Username = "TestUserName",
                Password = "!Password?123",
                Email = "user@domain.com",
                PasswordQuestion = "TestPasswordQuestion",
                PasswordAnswer = "TestPasswordAnswer",
                IsApproved = false,
                ProviderUserKey = null
            };

            return u;
        }

        #endregion Methods
    }

    public class UserParameters
    {
        #region Properties

        public string Email
        {
            get;
            set;
        }

        public bool IsApproved
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

        public string PasswordAnswer
        {
            get;
            set;
        }

        public string PasswordQuestion
        {
            get;
            set;
        }

        public object ProviderUserKey
        {
            get;
            set;
        }

        public string Username
        {
            get;
            set;
        }

        #endregion Properties
    }
}