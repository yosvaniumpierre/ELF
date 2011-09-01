using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Avanade.Domain.Security;
using Avanade.Repository.Services.Security;
using Moq;

namespace Avanade.Provider.Unit.Tests
{
    public static class MockRepositoryUtils
    {
        #region Methods

        public static IRoleRepository GetMockRoleRepository()
        {
            const string appName = "ProviderTestApp";
            var user = new User
            {
                Id = 1,
                Password = "RolePass1",
                IsApproved = true,
                CreationDate = DateTime.Now,
                PasswordAnswer = "RoleAnswer1",
                LastLockedOutDate = DateTime.Now,
                LastPasswordChangedDate = DateTime.Now,
                LastLoginDate = DateTime.Now,
                LastActivityDate = DateTime.Now,
                FailedPasswordAnswerAttemptWindowStart = DateTime.Now,
                FailedPasswordAttemptWindowStart = DateTime.Now,
                UserName = "RoleUser1",
                ApplicationName = appName
            };
            var goodRole = new Role
            {
                Id = 1,
                RoleName = "GoodRole",
                ApplicationName = appName,
                UsersInRole = new List<User> { user }
            };

            var mockRepo = new Mock<IRoleRepository>();
            mockRepo.Setup(v => v.GetRole(appName, "GoodRole")).Returns(goodRole);
            mockRepo.Setup(v => v.RoleExists(appName, "GoodRole")).Returns(true);
            mockRepo.Setup(v => v.GetRoles(appName)).Returns(GetSetupRoles(2));
            return mockRepo.Object;
        }

        public static IUserRepository GetMockUserRepository()
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

        public static List<Role> GetSetupRoles(int numRoles)
        {
            var roles = new List<Role>();
            for (int i = 0; i < numRoles; i++)
            {
                var u = new Role { RoleName = "SampleRole" + i, Id = i };
                roles.Add(u);
            }
            return roles;
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
}