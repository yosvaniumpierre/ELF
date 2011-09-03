namespace Avanade.Platform.Services.Web.Configuration
{
    /// <summary>
    /// A group of settings from a configuration file.
    /// </summary>
    internal interface IConfigSettingCollection
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        /// <value>The name.</value>
        string Name
        {
            get;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether the specified key exists in this group.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	<c>true</c> if the specified key exists; otherwise, <c>false</c>.
        /// </returns>
        bool ContainsKey(string key);

        /// <summary>
        /// Gets the specified settings based on the supplied key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        IConfigSetting Get(string key);

        #endregion Methods
    }
}