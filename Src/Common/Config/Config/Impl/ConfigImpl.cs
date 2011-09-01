using System.Collections.Generic;

namespace Avanade.Config.Impl
{
    /// <summary>
    /// Utility class for the configuration reflector.
    /// </summary>
    public class ConfigImpl : IConfig
    {
        #region Fields

        /// <summary>
        /// Current configuration source based on the current Environment.
        /// </summary>
        private readonly IConfigSource current;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Current config.
        /// </summary>
        public IConfigSource Current
        {
            get { return current; }
        }

        /// <summary>
        /// The name of this config provider.
        /// </summary>
        public string Name
        {
            get { return current.Name; }
        }

        /// <summary>
        /// The names of the sections in this config provider.
        /// </summary>
        public List<string> Sections
        {
            get { return current.Sections; }
        }

        /// <summary>
        /// The full path to the source for this config provider.
        /// </summary>
        public string SourcePath
        {
            get { return current.SourcePath; }
        }

        #endregion Properties

        public ConfigImpl(IConfigSource configSource)
        {
            current = configSource;
        }

        #region Methods

        /// <summary>
        /// Convenience method for checking if config key exists.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return current.Contains(key);
        }

        /// <summary>
        /// Convenience method for checking if config key exists.
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string section, string key)
        {
            return current.Contains(section, key);
        }

        /// <summary>
        /// Convenience method for getting typed config value from current config provider.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get(string key)
        {
            return current.Get<object>(key);
        }

        /// <summary>
        /// Convenience method for getting typed config value from current config provider.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            return current.Get<T>(key);
        }

        /// <summary>
        /// Convenience method for getting typed config value from current config provider using index position of key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="indexOfKey"></param>
        /// <returns></returns>
        public T Get<T>(int indexOfKey)
        {
            // BUG: Indexing by numbers are not supported.
            return current.Get<T>(indexOfKey.ToString());
        }

        /// <summary>
        /// Convenience method for getting section/key value from current config.
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get(string section, string key)
        {
            return current.Get(section, key);
        }

        /// <summary>
        /// Convenience method for getting section/key value from current config.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string section, string key)
        {
            return current.Get<T>(section, key);
        }

        /// <summary>
        /// Convenience method for getting section/key value from current config.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue">The default value to use if key is not available.</param>
        /// <returns></returns>
        public T GetDefault<T>(string section, string key, T defaultValue)
        {
            return current.GetDefault(section, key, defaultValue);
        }

        /// <summary>
        /// Get the configuration section with the specified name.
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public IConfigSection GetSection(string sectionName)
        {
            return current.GetSection(sectionName);
        }

        /// <summary>
        /// Convenience method for getting typed config value from current config provider.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetString(string key)
        {
            return current.Get<string>(key);
        }

        /// <summary>
        /// Convenience method for getting section/key value from current config.
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetString(string section, string key)
        {
            return current.Get<string>(section, key);
        }

        /// <summary>
        /// Save the configuration.
        /// </summary>
        public void Save()
        {
            current.Save();
        }

        #endregion Methods
    }
}