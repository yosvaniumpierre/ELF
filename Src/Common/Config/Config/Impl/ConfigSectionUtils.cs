using System.Collections;
using System.Collections.Generic;

namespace Avanade.Config.Impl
{
    /// <summary>
    /// Config section utils for adding/removing from both a map and list.
    /// </summary>
    internal class ConfigSectionUtils
    {
        #region Methods

        /// <summary>
        /// Add item to map checking for duplicate keys.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="overWrite"></param>
        public static void Add(IDictionary dictionary, string key, object val, bool overWrite)
        {
            object result = dictionary[key];
            if (result == null || overWrite)
            {
                dictionary[key] = val;
                return;
            }
            // Make this a list.
            List<object> valueList = null;
            if (result is List<object>)
            {
                valueList = (List<object>)result;
                valueList.Add(val);
            }
            else
            {
                valueList = new List<object>();
                valueList.Add(result);
                valueList.Add(val);
                dictionary[key] = valueList;
            }
        }

        /// <summary>
        /// Add key / value pair to the section specified.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="sectionName">E.g. "server"</param>
        /// <param name="key">"name"</param>
        /// <param name="val">"myserver01"</param>
        /// <param name="overWrite">true</param>
        public static void Add(IDictionary dictionary, string sectionName, 
            string key, object val, bool overWrite)
        {
            object result = dictionary[sectionName];
            IConfigSection section = null;

            // Handle null.
            if (result is IConfigSection)
            {
                section = (IConfigSection)result;
            }
            else if (result == null)
            {
                section = new ConfigSection(sectionName);
                dictionary.Add(sectionName, section);
            }
            else if (result is List<object>)
            {
                section = (IConfigSection)((List<object>)result)[0];
            }
            Add(section, key, val, overWrite);
        }

        /// <summary>
        /// Get the entry at the specified index of the multivalue list 
        /// associated with the key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key">"post"</param>
        /// <param name="ndx">1</param>
        /// <returns></returns>
        public static T Get<T>(IDictionary dictionary, string key, int ndx)
        {
            object result = dictionary[key];
            if (result == null) return default(T);

            if (result is T) return (T)result;

            if (result is List<object>)
            {
                List<object> valueList = result as List<object>;
                if (ndx < 0 || ndx >= valueList.Count)
                    return default(T);

                return (T)valueList[ndx];
            }
            return default(T);
        }

        #endregion Methods
    }
}