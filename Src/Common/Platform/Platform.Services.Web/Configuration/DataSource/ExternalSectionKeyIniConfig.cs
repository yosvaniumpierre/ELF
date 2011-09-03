namespace Avanade.Platform.Services.Web.Configuration.DataSource
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Avanade.Platform.Services.Web.Configuration.Utils;

    using NLog;

    internal class ExternalSectionKeyIniConfig : AbstractSectionKeyConfigHandler
    {
        #region Fields

        /// <summary>
        /// Logger instance.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly string externalFileConfig;

        #endregion Fields

        #region Constructors

        private ExternalSectionKeyIniConfig(string externalFileConfig)
        {
            this.externalFileConfig = externalFileConfig;

            Logger.Info("External config file used: {0}", externalFileConfig);
        }

        private ExternalSectionKeyIniConfig(string externalFileConfig, IDictionary<string, IConfigSettingCollection> settingsGroups)
            : this(externalFileConfig)
        {
            SettingGroups = settingsGroups;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the groups found in the configuration file.
        /// </summary>
        private IDictionary<string, IConfigSettingCollection> SettingGroups
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public static ExternalSectionKeyIniConfig Create(FileInfo fileInfo)
        {
            return fileInfo.Exists ? new ExternalSectionKeyIniConfig(fileInfo.FullName, IniFileProcessor.Load(fileInfo)) :
                new ExternalSectionKeyIniConfig(fileInfo.FullName);
        }

        /// <summary>
        /// Reinitialises this instance.
        /// This method has to cope with two scenarios:
        /// <li>
        /// <ol>Embedded Ini file - it is not envisaged that the DLL will be changed and hence, reinitialisation will not occur.</ol>
        /// <ol>External config file - this is a very possible scenario and thus, the external file info needs to be reused for subsequent reinitialisation.</ol>
        /// </li>
        /// </summary>
        /// <returns></returns>
        public override bool Reinitialise()
        {
            Logger.Info("Performing reinitialisation of the external secion-based Ini file...");

            //If the external config file is null or empty, then just reinitalise the next handler in the chain.
            if (string.IsNullOrEmpty(externalFileConfig))
            {
                Logger.Info("Unable to find info on the external config file...reinitialising the next handler in the chain!");
                return NextHandler.Reinitialise();
            }

            //At this stage, we can assume that the path to the external config file is available.
            //Check whether the external config file exists
            FileInfo externalConfigFileInfo = new FileInfo(externalFileConfig);

            // However, need to clear the internal dictionary of all the previous configuration values.
            // But check whether it is null or not since it could be that the previous initialisation
            // there was no external file available for initialisation. If it is not available,
            // then the SettingGroups will be null.
            if (SettingGroups != null)
            {
                SettingGroups.Clear();
            }

            if (externalConfigFileInfo.Exists)
            {
                Logger.Info(
                    "Since an external config file reference exists, then reload the external config file: {0}",
                    externalConfigFileInfo.FullName);
                SettingGroups = IniFileProcessor.Load(externalConfigFileInfo);
            }

            Logger.Info("Reinitialisation invoked on handler: {0}", NextHandler);
            return NextHandler.Reinitialise();
        }

        protected override IConfigSetting DoGet(string section, string key)
        {
            if (String.IsNullOrEmpty(key))
            {
                return null;
            }

            return SettingGroups[section].Get(key);
        }

        protected override bool HasKey(string sectionName, string key)
        {
            if (SettingGroups == null)
            {
                return false;
            }

            var containsSection = SettingGroups.ContainsKey(sectionName);
            var containsKey = false;

            if (containsSection)
            {
                containsKey = SettingGroups[sectionName].ContainsKey(key);
            }

            return containsSection && containsKey;
        }

        #endregion Methods
    }
}