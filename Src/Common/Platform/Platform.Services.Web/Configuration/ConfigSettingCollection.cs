namespace Avanade.Platform.Services.Web.Configuration
{
    using System.Collections.Generic;

    /// <summary>
    /// A group of settings from a configuration file.
    /// </summary>
    internal class ConfigSettingCollection : IConfigSettingCollection
    {
        #region Constructors

        internal ConfigSettingCollection(string name, IEnumerable<IConfigSetting> settings)
        {
            Name = name;
            Settings = new Dictionary<string, IConfigSetting>();

            foreach (IConfigSetting setting in settings)
                Settings.Add(setting.Name, setting);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets the settings found in the group.
        /// </summary>
        /// <value>The settings.</value>
        private Dictionary<string, IConfigSetting> Settings
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public bool ContainsKey(string key)
        {
            return Settings.ContainsKey(key);
        }

        public IConfigSetting Get(string key)
        {
            return Settings[key];
        }

        #endregion Methods
    }
}