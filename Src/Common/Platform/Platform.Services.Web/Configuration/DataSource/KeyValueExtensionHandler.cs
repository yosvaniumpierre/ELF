namespace Avanade.Platform.Services.Web.Configuration.DataSource
{
    using System.Collections.Generic;

    using NLog;

    internal class KeyValueExtensionHandler : AbstractKeyConfigHandler
    {
        #region Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IDictionary<string, IConfigSetting> dictionary;

        #endregion Fields

        #region Constructors

        public KeyValueExtensionHandler()
        {
            dictionary = new Dictionary<string, IConfigSetting>();
        }

        #endregion Constructors

        #region Methods

        public void Add(IKeyValueExtension extension)
        {
            Logger.Info("Adding new extension: {0}", extension);

            foreach (var valuePair in extension.Values)
            {
                DoCacheSetting(valuePair.Key, valuePair.Value);
            }
        }

        public override bool Reinitialise()
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Reinitialisation does not apply to the Key-Value extension support!");
            }

            return NextHandler.Reinitialise();
        }

        protected override IConfigSetting DoGet(string key)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Look for values in the Key-Value Extension by using key: {0}", key);
            }

            if (dictionary.ContainsKey(key))
            {
                IConfigSetting value = dictionary[key];
                return value;
            }

            throw new KeyNotFoundException(string.Format("Key ({0}) cannot be found in the Key-Value Extension!", key));
        }

        protected override bool HasKey(string key)
        {
            return dictionary.ContainsKey(key);
        }

        private void DoCacheSetting(string key, string stringValue)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(stringValue))
            {
                Logger.Warn("Cache rejection because either the key ({0}) or value ({1}) or both is an empty string", key, stringValue);
                return;
            }

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Caching the extended key-value pair: key-{0}, value-{1}", key, stringValue);
            }
            bool isArray = stringValue.Contains(",");
            IConfigSetting value = new Setting(key, stringValue, isArray);
            dictionary.Add(key, value);
        }

        #endregion Methods
    }
}