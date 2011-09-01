using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Avanade.Config.Impl
{
    /// <summary> 
    /// Simple class to lookup stored configuration settings by key. 
    /// Also provides type conversion methods. 
    /// GetInt("PageSize"); 
    /// GetBool("IsEnabled"); 
    /// </summary> 
    public class ConfigSection : OrderedDictionary, IConfigSection
    {
        #region Constructors

        /// <summary> 
        /// Allow default constructor. 
        /// </summary> 
        public ConfigSection()
        {
        }

        /// <summary>
        /// Initialize the config section w/ the name.
        /// </summary>
        /// <param name="name"></param>
        public ConfigSection(string name)
        {
            Name = name;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Name of config section.
        /// </summary>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// The names of all the sections.
        /// </summary>
        public List<string> Sections
        {
            get
            {
                var sections = new List<string>();
                foreach (DictionaryEntry entry in this)
                {
                    // Single entry section associated w/ key.
                    if (entry.Value is IConfigSection)
                        sections.Add(entry.Key.ToString());

                        // Check for list of sections with same key.
                    else if (entry.Value is List<object>)
                    {
                        var items = (List<object>)entry.Value;
                        if (items[0] is IConfigSection)
                            sections.Add(entry.Key.ToString());
                    }
                }
                return sections;
            }
        }

        #endregion Properties

        #region Indexers

        /// <summary> 
        /// Get / set the value using both the section name and key. 
        /// e.g. "globalsettings", "pageSize"
        /// </summary> 
        /// <param name="sectionName"></param> 
        /// <param name="key"></param> 
        /// <returns></returns> 
        public object this[string sectionName, string key]
        {
            get
            {
                object section = this[sectionName];
                if (section is IDictionary)
                {
                    return ((IDictionary)section)[key];
                }
                return null;
            }
            set
            {
                object section = this[sectionName];
                if (section is IDictionary)
                {
                    ((IDictionary)section)[key] = value;
                }
                else if ( section == null)
                {
                    Add(sectionName, key, value);
                }
            }
        }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// Add the key value to the section specified.
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="overWrite"></param>
        public virtual void Add(string sectionName, string key, object val, bool overWrite)
        {
            ConfigSectionUtils.Add(this, sectionName, key, val, overWrite);
        }

        /// <summary>
        /// Add the key value to the section specified.
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public virtual void Add(string sectionName, string key, object val)
        {
            ConfigSectionUtils.Add(this, sectionName, key, val, false);
        }

        /// <summary>
        /// Add key value with option of overwriting value of existing key
        /// or adding to a list of values associated w/ the same key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="overWrite"></param>
        public virtual void AddMulti(string key, object val, bool overWrite)
        {
            ConfigSectionUtils.Add(this, key, val, overWrite);
        }

        /// <summary>
        /// Checks whether or not the key exists in the section.
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(string section, string key)
        {
            IConfigSection configSection = GetSection(section);
            if (configSection == null) return false;

            return configSection.Contains(key);
        }

        /// <summary>
        /// Get typed root setting by string key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual T Get<T>(string key)
        {
            object result = this[key];
            var converted = ConverterUtil.ConvertTo<T>(result);
            return converted;
        }

        /// <summary>
        /// Get section key value.
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get(string section, string key)
        {
            return this[section, key];
        }

        /// <summary>
        /// Get typed section key value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string section, string key)
        {
            object result = this[section, key];
            var converted = (T)ConverterUtil.ConvertObj<T>(result);
            return converted;
        }

        /// <summary>
        /// Validate and return the default value if the key is not present.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetDefault<T>(string key, T defaultValue)
        {
            // Validate and return default value.
            if (!Contains(key)) return defaultValue;
            return Get<T>(key);
        }

        /// <summary>
        /// Get section/key value if present, default value otherwise.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetDefault<T>(string section, string key, T defaultValue)
        {
            // Validate and return default value.
            if (!Contains(section, key)) return defaultValue;
            return Get<T>(section, key);
        }

        /// <summary> 
        /// Get a section. 
        /// </summary> 
        /// <param name="sectionName"></param> 
        /// <returns></returns> 
        public IConfigSection GetSection(string sectionName)
        {
            return this[sectionName] as IConfigSection;
        }

        /// <summary> 
        /// Get a section associated with the specified key at the specified index.
        /// </summary> 
        /// <param name="sectionName"></param>
        /// <param name="ndx"></param>
        /// <returns></returns> 
        public IConfigSection GetSection(string sectionName, int ndx)
        {
            return ConfigSectionUtils.Get<IConfigSection>(this, sectionName, ndx);
        }

        #endregion Methods
    }
}