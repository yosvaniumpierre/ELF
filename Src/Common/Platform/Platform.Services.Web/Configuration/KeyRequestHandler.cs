namespace Avanade.Platform.Services.Web.Configuration
{
    using System;
    using System.IO;
    using System.Web;

    using DataSource;

    using NLog;

    internal class KeyRequestHandler : IKeyConfigHandler
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

        private readonly IKeyConfigHandler topMostHandler;

        private static KeyValueExtensionHandler keyValueExtensionHandler;

        #endregion Fields

        #region Constructors

        public KeyRequestHandler()
        {
            try
            {
                topMostHandler = CreateHandlerChain("");
            }
            catch (Exception exception)
            {
                Logger.Error("Problem encountered when creating the key handler!", exception);
            }
        }

        public KeyRequestHandler(string testConfigFile)
        {
            try
            {
                topMostHandler = CreateHandlerChain(testConfigFile);
            }
            catch (Exception exception)
            {
                Logger.Error("Problem encountered when creating the key handler!", exception);
            }
        }

        #endregion Constructors

        #region Properties

        public KeyValueExtensionHandler KeyValueExtensionHandler
        {
            get { return keyValueExtensionHandler; }
        }

        #endregion Properties

        #region Methods

        public IConfigSetting Get(string key)
        {
            return topMostHandler.Get(key);
        }

        public bool Reinitialise()
        {
            return topMostHandler.Reinitialise();
        }

        private static IKeyConfigHandler CreateHandlerChain(string testConfigFile)
        {
            AppWebKeyConfigFile appWebKeyConfigFile = new AppWebKeyConfigFile();
            EnvVariableKeyConfig envVariableKeyConfig = new EnvVariableKeyConfig();
            keyValueExtensionHandler = new KeyValueExtensionHandler();
            ExternalDefaultIniConfig externalDefaultIniConfig = DoCreateIniConfig();
            EmbeddedIniConfig embeddedIniConfig = EmbeddedIniConfig.Create(testConfigFile);

            //Chain it up!
            appWebKeyConfigFile.SetNextHandler(envVariableKeyConfig);
            envVariableKeyConfig.SetNextHandler(keyValueExtensionHandler);
            keyValueExtensionHandler.SetNextHandler(externalDefaultIniConfig);
            externalDefaultIniConfig.SetNextHandler(embeddedIniConfig);

            return appWebKeyConfigFile;
        }

        private static ExternalDefaultIniConfig DoCreateIniConfig()
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Searching external file folder for the ini file: {0}", ExternalConfigFileFolder);
            }

            //Now let's create another instance of the ExternalDefaultIniConfig by using an external resource.
            //The external resource will be the first to be checked for configuration.
            var fileInfo = new FileInfo(Path.Combine(HttpRuntime.AppDomainAppPath, ExternalConfigFile));

            //The top-most config chain must still be created even when the external file cannot be found so that the calls can be delegated to the next
            //handler in the chain.

            ExternalDefaultIniConfig externalIniKeyConfigDecorator;

            if (fileInfo.Exists)
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Found {0} in the {1} folder!!", ExternalConfigFile, fileInfo.FullName);
                }
                externalIniKeyConfigDecorator = ExternalDefaultIniConfig.Create(fileInfo);
            }
            else
            {
                Logger.Info("Unable to find {0} in the folder ({1}) relative to the app.config or web.config",
                  ExternalConfigFile, ExternalConfigFileFolder);
                externalIniKeyConfigDecorator = ExternalDefaultIniConfig.Create(fileInfo);
            }

            return externalIniKeyConfigDecorator;
        }

        #endregion Methods
    }
}