namespace Avanade.Platform.Services.Web.Configuration.DataSource
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Web;
    using System.Web.Configuration;

    using NLog;

    /// <summary>
    /// Description of AppWebKeyConfigFile.
    /// </summary>
    internal class AppWebKeyConfigFile : AbstractKeyConfigHandler
    {
        #region Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IDictionary<string, IConfigSetting> dictionary;

        #endregion Fields

        #region Constructors

        public AppWebKeyConfigFile()
        {
            dictionary = new Dictionary<string, IConfigSetting>();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Reinitialises this instance and its handler. 
        /// However, since the App.Config and Web.Config values are read at runtime when the key is not available, 
        /// all that needs for reinitialisation is to clear the dictionary. 
        /// </summary>
        /// <returns></returns>
        public override bool Reinitialise()
        {
            Logger.Info("Clearing the App.Config and Web.Config dictionary...");
            dictionary.Clear();

            Logger.Info("Reinitialisation invoked on handler: {0}", NextHandler);
            return NextHandler.Reinitialise();
        }

        protected override IConfigSetting DoGet(string key)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Looking up the default value of App.Config or Web.Config by using key: {0}", key);
            }

            IConfigSetting value;

            if (dictionary.ContainsKey(key))
            {
                value = dictionary[key];
            }
            else
            {
                //Try to get from the App.Config or Web.Config first
                var stringValue = HttpContext.Current == null ? ConfigurationManager.AppSettings[key] : WebConfigurationManager.AppSettings[key];

                value = DoCacheSetting(key, stringValue);
            }

            return value;
        }

        protected override bool HasKey(string key)
        {
            var stringValue = HttpContext.Current == null ? ConfigurationManager.AppSettings[key] :
                WebConfigurationManager.AppSettings[key];

            return dictionary.ContainsKey(key) || !string.IsNullOrEmpty(stringValue);
        }

        private IConfigSetting DoCacheSetting(string key, string stringValue)
        {
            bool isArray = stringValue.Contains(",");
            IConfigSetting value = new Setting(key, stringValue, isArray);
            dictionary.Add(key, value);

            return value;
        }

        #endregion Methods
    }
}