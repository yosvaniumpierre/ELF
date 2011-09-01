using System.Collections.Generic;
using System.Web.Configuration;

namespace Avanade.Provider.Unit.Tests
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

    public static class TestUtils
    {
        #region Methods

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