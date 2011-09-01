using System.Collections.Generic;

namespace Avanade.Config
{
    public interface IConfig
    {
        #region Properties

        /// <summary>
        /// Current config.
        /// </summary>
        IConfigSource Current
        {
            get;
        }

        /// <summary>
        /// The name of this config provider.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// The names of the sections in this config provider.
        /// </summary>
        List<string> Sections
        {
            get;
        }

        /// <summary>
        /// The full path to the source for this config provider.
        /// </summary>
        string SourcePath
        {
            get;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Convenience method for checking if config key exists.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool ContainsKey(string key);

        /// <summary>
        /// Convenience method for checking if config key exists.
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        bool ContainsKey(string section, string key);

        /// <summary>
        /// Convenience method for getting typed config value from current config provider.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object Get(string key);

        /// <summary>
        /// Convenience method for getting typed config value from current config provider.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(string key);

        /// <summary>
        /// Convenience method for getting typed config value from current config provider using index position of key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexOfKey"></param>
        /// <returns></returns>
        T Get<T>(int indexOfKey);

        /// <summary>
        /// Convenience method for getting section/key value from current config.
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        object Get(string section, string key);

        /// <summary>
        /// Convenience method for getting section/key value from current config.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(string section, string key);

        /// <summary>
        /// Convenience method for getting section/key value from current config.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue">The default value to use if key is not available.</param>
        /// <returns></returns>
        T GetDefault<T>(string section, string key, T defaultValue);

        /// <summary>
        /// Get the configuration section with the specified name.
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        IConfigSection GetSection(string sectionName);

        /// <summary>
        /// Convenience method for getting typed config value from current config provider.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetString(string key);

        /// <summary>
        /// Convenience method for getting section/key value from current config.
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetString(string section, string key);

        /// <summary>
        /// Save the configuration.
        /// </summary>
        void Save();

        #endregion Methods
    }
}