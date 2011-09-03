namespace Avanade.Platform.Services
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Main interface for reading application-specific configuration parameters. 
    /// The component essentially uses the 'Chain of Responsibility' pattern to pass on the handling of the configuration retrieval. 
    /// 
    /// There will be 2 'Chain' handlers - namely, the Key Request Handler and the Section-Key Request Handler.
    /// 
    /// The elements within the Key Request handler (sorted according to the order of search) are:
    /// <list type="number">
    ///     <item>
    ///         <description>App.Config/Web.Config file</description>
    ///     </item>
    ///     <item>
    ///         <description>System Environment Variables</description>
    ///     </item>
    ///     <item>
    ///         <description>Custom Key-Value extensions from others sources (e.g., database or LDAP)</description>
    ///     </item>
    ///     <item>
    ///         <description>Default section of an external ini file</description>
    ///     </item>
    ///     <item>
    ///         <description>Default section of the embedded ini file in the DLL</description>
    ///     </item>
    /// </list>
    /// <para>
    /// The elements within the Section-Key Request handler (sorted according to the order of search) are:
    /// <list type="number">
    ///     <item>
    ///         <description>External ini configuration file</description>
    ///     </item>
    ///     <item>
    ///         <description>Embedded ini file in the DLL</description>
    ///     </item>
    /// </list>
    /// </para>
    /// 
    /// The interface was deliberately designed to be fail-safe.
    /// </summary>
    /// <exception cref="KeyNotFoundException">Thrown when a section or key cannot be found</exception>
    /// <exception cref="ArgumentNullException">Thrown when null values are passed into the SettingGroups / Setting arguments</exception>
    /// <exception cref="FormatException">Thrown when the value cannot be converted to the desired raw data type</exception>
    /// <example>configuration.SettingGroups["Bad"].Settings["NoName"].GetValueAsString()</example>
    /// <description>
    /// Do not instantiate the DefaultConfigurationImpl class - rather use the Configuration Singleton to get a reference.
    /// This point is important because each IConfiguration instance caches the property value and creating each new
    /// instance will defeat this feature.
    /// 
    /// Also, this interface provides convenience methods to return the property in Int, Bool, decrypted string, String, Int Array,
    /// Bool array and String array. The examples of these can be found in the NUnit test class.
    /// </description>
    public interface IConfiguration
    {
        #region Methods

        /// <summary>
        /// Looks for configuration value when a section is not needed.
        /// </summary>
        /// <param name="key">Key to lookup the value.</param>
        /// <returns>Instance of Setting or null.</returns>
        /// <exception cref="ArgumentNullException">Thrown when attempting to access a non-existent key in the default configuration.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when the key cannot be found.</exception>
        IConfigSetting Get(string key);

        /// <summary>
        /// Gets the configuration value based on the specified section name and key.
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        /// <param name="key">The key.</param>
        /// <returns>Instance of Setting or null.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either the section name or key are null or empty string.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when the section name or key cannot be found.</exception>
        IConfigSetting Get(string sectionName, string key);

        #endregion Methods
    }
}