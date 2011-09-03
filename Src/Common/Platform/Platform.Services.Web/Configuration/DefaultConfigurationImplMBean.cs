namespace Avanade.Platform.Services.Web.Configuration
{
    /// <summary>
    /// This is an interface specifically defined for invocation within an JMX/NetMX environment.
    /// </summary>
    public interface DefaultConfigurationImplMBean
    {
        #region Methods

        /// <summary>
        /// Performs a reinitialisation by re-reading all the configuration values and storing these in-memory.
        /// </summary>
        /// <returns>true if reinitialisation was successful; false for otherwise.</returns>
        bool Reinitialise();

        /// <summary>
        /// Verifies a configuration value by supplying a configuration key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Actual raw value or at worse, an empty string.</returns>
        string Verify(string key);

        /// <summary>
        /// Verifies a configuration value by looking up the specified section and through the use of the configuration key..
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        /// <param name="key">The key.</param>
        /// <returns>Actual raw value or at worse, an empty string.</returns>
        string VerifySection(string sectionName, string key);

        #endregion Methods
    }
}