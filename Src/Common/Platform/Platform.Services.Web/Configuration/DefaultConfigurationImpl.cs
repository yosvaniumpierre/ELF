namespace Avanade.Platform.Services.Web.Configuration
{
    using System;
    using System.Collections.Generic;

    using NLog;

    /// <summary>
    /// Represents a configuration file.
    /// </summary>
    public class DefaultConfigurationImpl : IConfiguration, DefaultConfigurationImplMBean
    {
        #region Fields

        /// <summary>
        /// Logger instance.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly KeyRequestHandler keyRequestHandler;
        private readonly SectionKeyRequestHandler sectionKeyRequestHandler;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultConfigurationImpl"/> class.
        /// </summary>
        public DefaultConfigurationImpl()
        {
            try
            {
                Logger.Info("Instantiating the component using the default constructor!");
                keyRequestHandler = new KeyRequestHandler();
                sectionKeyRequestHandler = new SectionKeyRequestHandler();
            }
            catch (Exception exception)
            {
                Logger.Error("Unknown problem encountered while instantiating the configuration component!", exception);
                throw;
            }
        }

        /// <summary>
        /// Adds the specified extension.
        /// Thus, the component can be extended to incorporate information from sources that were not originally anticipated.
        /// Such extensions can come from database tables or LDAP sources.
        /// 
        /// Initializes a new instance of the <see cref="DefaultConfigurationImpl"/> class.
        /// </summary>
        /// <param name="extensions">The extensions.</param>
        public DefaultConfigurationImpl(IKeyValueExtension[] extensions)
            : this()
        {
            try
            {
                Logger.Info("Extending the component with {0} extensions!", extensions.Length);
                foreach (var extension in extensions)
                {
                    Add(extension);
                }
            }
            catch (Exception exception)
            {
                Logger.Error("Unknown problem encountered while instantiating the configuration component using the Key-Value Extension!", exception);
                throw;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultConfigurationImpl"/> class.
        /// </summary>
        /// <param name="testConfigFile">The test config file.</param>
        public DefaultConfigurationImpl(string testConfigFile)
        {
            keyRequestHandler = new KeyRequestHandler(testConfigFile);
            sectionKeyRequestHandler = new SectionKeyRequestHandler(testConfigFile);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Looks for configuration value when a section is not needed.
        /// </summary>
        /// <param name="key">Key to lookup the value.</param>
        /// <returns>Instance of Setting or null.</returns>
        /// <exception cref="ArgumentNullException">Thrown when attempting to access a non-existent key in the default configuration.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when the key cannot be found.</exception>
        public IConfigSetting Get(string key)
        {
            DoValidateKey(key);

            return keyRequestHandler.Get(key);
        }

        /// <summary>
        /// Gets the configuration value based on the specified section name and key.
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        /// <param name="key">The key.</param>
        /// <returns>Instance of Setting or null.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either the section name or key are null or empty string.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when the section name or key cannot be found.</exception>
        public IConfigSetting Get(string sectionName, string key)
        {
            DoValidateSection(sectionName);

            if (string.IsNullOrEmpty(key))
            {
                string msg = string.Format("Key supplied for the section {0} was either a null or empty string", sectionName);
                Logger.Error(msg);
                throw new ArgumentNullException(msg);
            }

            return sectionKeyRequestHandler.Get(sectionName, key);
        }

        /// <summary>
        /// Performs a reinitialisation by re-reading all the configuration values and storing these in-memory.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if reinitialisation was successful; false for otherwise.
        /// </returns>
        public bool Reinitialise()
        {
            Logger.Info("Reinitialisation has started...");
            return keyRequestHandler.Reinitialise() && sectionKeyRequestHandler.Reinitialise();
        }

        /// <summary>
        /// Verifies a configuration value by supplying a configuration key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// Actual raw value or at worse, an empty string.
        /// </returns>
        public string Verify(string key)
        {
            DoValidateKey(key);

            try
            {
                return Get(key).RawValue;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Verifies a configuration value by looking up the specified section and through the use of the configuration key..
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        /// <param name="key">The key.</param>
        /// <returns>
        /// Actual raw value or at worse, an empty string.
        /// </returns>
        public string VerifySection(string sectionName, string key)
        {
            DoValidateSection(sectionName);
            DoValidateKey(key);

            try
            {
                return Get(sectionName, key).RawValue;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private static void DoValidateKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                const string msg = "Key supplied for the default section was either a null or empty string";
                Logger.Error(msg);
                throw new ArgumentNullException(msg);
            }
        }

        private static void DoValidateSection(string sectionName)
        {
            if (string.IsNullOrEmpty(sectionName))
            {
                const string msg = "Section name supplied was either a null or empty string";
                Logger.Error(msg);
                throw new ArgumentNullException(msg);
            }
        }

        /// <summary>
        /// Adds the specified extension.
        /// Thus, the component can be extended to incorporate information from sources that were not originally anticipated.
        /// Such extensions can come from database tables or LDAP sources.
        /// </summary>
        /// <param name="extension">The key-value pair extension.</param>
        private void Add(IKeyValueExtension extension)
        {
            if (extension == null)
            {
                Logger.Error("The KeyValueExtension is NULL! Why??!");
                return;
            }

            if (extension.Values.Count == 0)
            {
                Logger.Error("Why is the KeyValueExtension empty??!");
                return;
            }

            Logger.Info("Accepting a new component extension: {0}", extension);

            keyRequestHandler.KeyValueExtensionHandler.Add(extension);
        }

        #endregion Methods
    }
}