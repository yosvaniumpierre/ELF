using System;
using System.Collections;
using System.Collections.Generic;

namespace Avanade.Config.Impl
{
    /// <summary> 
    /// Simple class to lookup stored configuration settings by key. 
    /// Also provides type conversion methods. 
    /// GetInt("PageSize"); 
    /// GetBool("IsEnabled"); 
    /// </summary> 
    public class ConfigSourceDecorator : IConfigSource
    {
        #region Fields

        private readonly IConfigSource provider;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initialize w/ the actual provider.
        /// </summary>
        /// <param name="provider"></param>
        public ConfigSourceDecorator(IConfigSource provider)
        {
            this.provider = provider;
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Event handler for when the underlying config source changed.
        /// </summary>
        public event EventHandler OnConfigSourceChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Count
        /// </summary>
        public int Count
        {
            get { return provider.Count; }
        }

        /// <summary>
        /// Indicate if fixed size.
        /// </summary>
        public bool IsFixedSize
        {
            get { return provider.IsFixedSize; }
        }

        /// <summary>
        /// Is readonly
        /// </summary>
        public bool IsReadOnly
        {
            get { return provider.IsReadOnly; }
        }

        /// <summary>
        /// Whether or not this is synchronized.
        /// </summary>
        public bool IsSynchronized
        {
            get { return provider.IsSynchronized; }
        }

        /// <summary>
        /// Get the keys.
        /// </summary>
        public ICollection Keys
        {
            get { return provider.Keys; }
        }

        /// <summary>
        /// Name of the configuration source.
        /// </summary>
        public string Name
        {
            get
            {
                return provider.Name;
            }
            set
            {
                provider.Name = value;
            }
        }

        /// <summary>
        /// Get the list of sections.
        /// </summary>
        public List<string> Sections
        {
            get { return provider.Sections; }
        }

        /// <summary>
        /// The configuration source path.
        /// </summary>
        public string SourcePath
        {
            get { return provider.SourcePath; }
        }

        /// <summary>
        /// Get the synroot
        /// </summary>
        public object SyncRoot
        {
            get { return provider.SyncRoot; }
        }

        /// <summary>
        /// Get the values.
        /// </summary>
        public ICollection Values
        {
            get { return provider.Values; }
        }

        #endregion Properties

        #region Indexers

        /// <summary>
        /// Get / set section/key value.
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[string sectionName, string key]
        {
            get
            {
                return provider[sectionName, key];
            }
            set
            {
                provider[sectionName, key] = value;
            }
        }

        /// <summary>
        /// Get / set the value.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[object key]
        {
            get
            {
                return provider[key];
            }
            set
            {
                provider[key] = value;
            }
        }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// Add the sectionname, key, value.
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="overWrite"></param>
        public void Add(string sectionName, string key, object val, bool overWrite)
        {
            provider.Add(sectionName, key, val, overWrite);
        }

        /// <summary>
        /// Add the sectionname, key/value.
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public void Add(string sectionName, string key, object val)
        {
            provider.Add(sectionName, key, val);
        }

        /// <summary>
        /// Add the key/value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(object key, object value)
        {
            provider.Add(key, value);
        }

        /// <summary>
        /// Add multiple value to the same section/key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="overWrite"></param>
        public void AddMulti(string key, object val, bool overWrite)
        {
            provider.AddMulti(key, val, overWrite);
        }

        /// <summary>
        /// Clearn all the entries.
        /// </summary>
        public void Clear()
        {
            provider.Clear();
        }

        /// <summary>
        /// Check if the section/key exists.
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(string section, string key)
        {
            return provider.Contains(section, key);
        }

        /// <summary>
        /// Indicates whether the key exists.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(object key)
        {
            return provider.Contains(key);
        }

        /// <summary>
        /// Copies the array starting at the specified index.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(Array array, int index)
        {
            provider.CopyTo(array, index);
        }

        /// <summary>
        /// Get value of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            return provider.Get<T>(key);
        }

        /// <summary>
        /// Get the section/key value.
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get(string section, string key)
        {
            return provider.Get(section, key);
        }

        /// <summary>
        /// Get typed section/key value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string section, string key)
        {
            return provider.Get<T>(section, key);
        }

        /// <summary>
        /// Get value or default value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetDefault<T>(string key, T defaultValue)
        {
            return provider.GetDefault(key, defaultValue);
        }

        /// <summary>
        /// Get typed section/key value or default  value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetDefault<T>(string section, string key, T defaultValue)
        {
            return provider.GetDefault(section, key, defaultValue);
        }

        /// <summary>
        /// Get the enumerator.
        /// </summary>
        /// <returns></returns>
        public IDictionaryEnumerator GetEnumerator()
        {
            return provider.GetEnumerator();
        }

        /// <summary>
        /// Get section with name.
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public IConfigSection GetSection(string sectionName)
        {
            return provider.GetSection(sectionName);
        }

        /// <summary>
        /// Get section with specified name at index.
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="ndx"></param>
        /// <returns></returns>
        public IConfigSection GetSection(string sectionName, int ndx)
        {
            return provider.GetSection(sectionName, ndx);
        }

        /// <summary>
        /// GetEnumerator.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initialization after construction.
        /// </summary>
        public void Init()
        {
        }

        /// <summary>
        /// Load the configuration.
        /// </summary>
        public void Load()
        {
            provider.Load();
        }

        /// <summary>
        /// Remove the key.
        /// </summary>
        /// <param name="key"></param>
        public void Remove(object key)
        {
            provider.Remove(key);
        }

        /// <summary>
        /// Save the configuration
        /// </summary>
        public void Save()
        {
            provider.Save();
        }

        #endregion Methods
    }
}