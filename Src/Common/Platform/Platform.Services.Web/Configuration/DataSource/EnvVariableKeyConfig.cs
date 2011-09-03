namespace Avanade.Platform.Services.Web.Configuration.DataSource
{
    using System;
    using System.Collections.Generic;

    using NLog;

    /// <summary>
    /// Description of EnvVariableKeyConfig.
    /// </summary>
    internal class EnvVariableKeyConfig : AbstractKeyConfigHandler
    {
        #region Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IDictionary<string, Setting> dictionary;

        #endregion Fields

        #region Constructors

        public EnvVariableKeyConfig()
        {
            dictionary = new Dictionary<string, Setting>();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Reinitialises this instance and its handler. 
        /// However, since the environment variables are read at runtime when the key is not available, 
        /// all that needs for reinitialisation is to clear the dictionary. 
        /// </summary>
        /// <returns></returns>
        public override bool Reinitialise()
        {
            Logger.Info("Clearing the environment variable dictionary...");
            dictionary.Clear();

            Logger.Info("Reinitialisation invoked on handler: {0}", NextHandler);
            return NextHandler.Reinitialise();
        }

        protected override IConfigSetting DoGet(string key)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Looking up the default value in environment variables by using key: {0}", key);
            }

            IConfigSetting value;

            if (dictionary.ContainsKey(key))
            {
                value = dictionary[key];
            }
            else
            {
                //If we get this far, it means that the environment variable already exists because of the HasKey check!
                string stringValue = Environment.GetEnvironmentVariable(key);
                value = DoCacheSetting(key, stringValue);
            }

            return value;
        }

        protected override bool HasKey(string key)
        {
            string stringValue = Environment.GetEnvironmentVariable(key);

            return dictionary.ContainsKey(key) || !string.IsNullOrEmpty(stringValue);
        }

        private Setting DoCacheSetting(string key, string stringValue)
        {
            bool isArray = stringValue.Contains(",");
            Setting value = new Setting(key, stringValue, isArray);
            dictionary.Add(key, value);

            return value;
        }

        #endregion Methods
    }
}