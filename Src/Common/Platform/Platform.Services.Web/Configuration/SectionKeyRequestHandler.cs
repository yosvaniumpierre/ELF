namespace Avanade.Platform.Services.Web.Configuration
{
    using System;
    using System.IO;
    using System.Web;

    using DataSource;

    using NLog;

    internal class SectionKeyRequestHandler : ISectionKeyConfigHandler
    {
        #region Fields

        /// <summary>
        /// External configuration file.
        /// </summary>
        private const string ExternalConfigFile = "Configuration.ini";

        /// <summary>
        /// Folder relative to the app.config and web.config file location.
        /// </summary>
        private const string ExternalConfigFileFolder = "ConfigFolder";

        /// <summary>
        /// Logger instance.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ISectionKeyConfigHandler topMostHandler;

        #endregion Fields

        #region Constructors

        public SectionKeyRequestHandler()
        {
            try
            {
                topMostHandler = CreateHandlerChain("");
            }
            catch (Exception exception)
            {
                Logger.Error("Problem encountered when creating the section-key handler!", exception);
            }
        }

        public SectionKeyRequestHandler(string testConfigFile)
        {
            try
            {
                topMostHandler = CreateHandlerChain(testConfigFile);
            }
            catch (Exception exception)
            {
                Logger.Error("Problem encountered when creating the section-key handler!", exception);
            }
        }

        #endregion Constructors

        #region Methods

        public IConfigSetting Get(string section, string key)
        {
            return topMostHandler.Get(section, key);
        }

        public bool Reinitialise()
        {
            return topMostHandler.Reinitialise();
        }

        private static ISectionKeyConfigHandler CreateHandlerChain(string testConfigFile)
        {
            ExternalSectionKeyIniConfig externalSectionKeyIniConfigFile = DoCreateIniConfig();
            EmbeddedIniConfig embeddedIniConfig = EmbeddedIniConfig.Create(testConfigFile);

            //Chain it up!
            externalSectionKeyIniConfigFile.SetNextHandler(embeddedIniConfig);

            return externalSectionKeyIniConfigFile;
        }

        private static ExternalSectionKeyIniConfig DoCreateIniConfig()
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Searching external file folder for the ini file: {0}", ExternalConfigFileFolder);
            }

            //Now let's create another instance of the ExternalSectionKeyIniConfig by using an external resource.
            //The external resource will be the first to be checked for configuration.
            var fileInfo = new FileInfo(Path.Combine(HttpRuntime.AppDomainAppPath, ExternalConfigFile));

            ExternalSectionKeyIniConfig externalExternalSectionKeyIniKeyConfigDecorator;

            if (fileInfo.Exists)
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Found {0} in the {1} folder!!", ExternalConfigFile, fileInfo.FullName);
                }
                externalExternalSectionKeyIniKeyConfigDecorator = ExternalSectionKeyIniConfig.Create(fileInfo);
            }
            else
            {
                Logger.Info("Unable to find {0} in the folder ({1}) relative to the app.config or web.config",
                  ExternalConfigFile, ExternalConfigFileFolder);
                externalExternalSectionKeyIniKeyConfigDecorator = ExternalSectionKeyIniConfig.Create(fileInfo);
            }

            return externalExternalSectionKeyIniKeyConfigDecorator;
        }

        #endregion Methods
    }
}