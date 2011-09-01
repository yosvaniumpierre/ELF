using System;
using System.Collections.Specialized;

namespace Avanade.Provider.Unit.Tests
{
    public static class ConfigUtils
    {
        #region Methods

        public static NameValueCollection GetMembershipComplexConfig()
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

        public static NameValueCollection GetMembershipNoPasswordConfig()
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
                                 {"enablePasswordRetrieval", "true"},
                                 {"requiresQuestionAndAnswer", "false"},
                                 {"requiresUniqueEmail", "true"},
                                 {"passwordFormat", "Clear"}
                             };

            return config;
        }

        public static NameValueCollection GetRoleConfig()
        {
            var config = new NameValueCollection
                             {
                                 {"applicationName", "ProviderTestApp"},
                             };

            return config;
        }

        #endregion Methods
    }
}