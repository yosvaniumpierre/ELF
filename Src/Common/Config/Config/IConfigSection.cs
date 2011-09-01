using System.Collections;
using System.Collections.Generic;

namespace Avanade.Config
{
    /// <summary> 
    /// Base class for config settings. 
    /// This stores settings in 
    /// 1. At the root level ( similiar to AppSettings ). 
    /// 2. At a section level ( similar to GetSection("SectionName") ); 
    /// </summary> 
    /// <remarks> 
    /// The following properties are associated with 
    /// storing settings at the root level. 
    /// 1. Count 
    /// </remarks> 
    public interface IConfigSection : IDictionary
    {
        #region Properties

        /// <summary>
        /// The name of the this config section.
        /// </summary>
        string Name
        {
            get; set;
        }

        /// <summary>
        /// Get the names of the sections.
        /// </summary>
        List<string> Sections
        {
            get;
        }

        /// <summary>
        /// Get the section key value using the indexer.
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        object this[string sectionName, string key]
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Add to value to the section/key combination.
        /// </summary>
        /// <param name="sectionName">"ApplicationSettings"</param>
        /// <param name="key">PageSize</param>
        /// <param name="val">15</param>
        /// <param name="overWrite"></param>
        void Add(string sectionName, string key, object val, bool overWrite);

        /// <summary>
        /// Add section key/value item.
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        void Add(string sectionName, string key, object val);

        /// <summary>
        /// Add to value to the section/key combination.
        /// </summary>
        /// <param name="key">PageSize</param>
        /// <param name="val">15</param>
        /// <param name="overWrite"></param>
        void AddMulti(string key, object val, bool overWrite);

        /// <summary>
        /// Checks whether or not the key exists in the section.
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Contains(string section, string key);

        /// <summary>
        /// Get typed value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(string key);

        /// <summary>
        /// Get the section's key value.
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        object Get(string section, string key);

        /// <summary>
        /// Get the section's key's specific type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(string section, string key);

        /// <summary>
        /// Get key value if preset, default value otherwise.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        T GetDefault<T>(string key, T defaultValue);

        /// <summary>
        /// Get section/key value if preset, default value otherwise.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        T GetDefault<T>(string section, string key, T defaultValue);

        /// <summary>
        /// Get the section with the name specified.
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        IConfigSection GetSection(string sectionName);

        /// <summary>
        /// Get sectionlist with the specified name.
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="ndx"></param>
        /// <returns></returns>
        IConfigSection GetSection(string sectionName, int ndx);

        #endregion Methods
    }
}