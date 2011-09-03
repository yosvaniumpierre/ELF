namespace Avanade.Platform.Services.Web.Configuration.DataSource
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    using Avanade.Platform.Services.Web.Configuration.Utils;

    using NLog;

    internal class EmbeddedIniConfig : AbstractKeyConfigHandler, ISectionKeyConfigHandler
    {
        #region Fields

        private const string DefaultSection = "Default";

        /// <summary>
        /// Embedded configuration file.
        /// </summary>
        private const string EmbeddedConfigFile = "ParameterData.ini";

        /// <summary>
        /// Logger instance.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion Fields

        #region Constructors

        private EmbeddedIniConfig(IDictionary<string, IConfigSettingCollection> settingGroups)
        {
            //prevent instantiation.
            SettingGroups = settingGroups;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the groups found in the configuration file.
        /// </summary>
        private IDictionary<string, IConfigSettingCollection> SettingGroups
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public static EmbeddedIniConfig Create(string testConfigFile)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            string assemblyNameSpace = assembly.GetName().Name;

            string fullQualifiedResourceName;

            if (string.IsNullOrEmpty(testConfigFile))
            {
                fullQualifiedResourceName = assemblyNameSpace + "." + EmbeddedConfigFile;    // it should look like this - "MyNamespace.MyTextFile.txt"
            }
            else
            {
                fullQualifiedResourceName = assemblyNameSpace + "." + testConfigFile;
            }

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug(string.Format("Looking for embedded resource inside the component - {0}", fullQualifiedResourceName));
            }

            //This is to populate the delegate with an embedded resource
            //This will be the last config to be checked for availability
            Stream resourceStream = assembly.GetManifestResourceStream(fullQualifiedResourceName);
            EmbeddedIniConfig baseKeyConfigDecorator = new EmbeddedIniConfig(IniFileProcessor.Load(resourceStream));

            return baseKeyConfigDecorator;
        }

        public IConfigSetting Get(string section, string key)
        {
            return SettingGroups[section].Get(key);
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
            Logger.Info("Bottom of the handler stack reached!");
            return true;
        }

        protected override IConfigSetting DoGet(string key)
        {
            if (String.IsNullOrEmpty(key))
            {
                return null;
            }

            return SettingGroups[DefaultSection].Get(key);
        }

        protected override bool HasKey(string key)
        {
            if (SettingGroups == null)
            {
                return false;
            }

            return SettingGroups[DefaultSection].ContainsKey(key);
        }

        #endregion Methods
    }
}