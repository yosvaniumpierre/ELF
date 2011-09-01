namespace Avanade.Config.Extension
{
    /// <summary>
    /// Base interface for a configuration source
    /// with dynamic events.
    /// </summary>
    public interface IConfigSourceDynamic : IConfigSourceBase
    {
        #region Methods

        /// <summary>
        /// Load the config settings from the underlying datasource via full life-cycle.
        /// </summary>
        void Load(object config);

        /// <summary>
        /// Called after loading.
        /// </summary>
        void OnAfterLoad();

        /// <summary>
        /// Called after saving.
        /// </summary>
        void OnAfterSave();

        /// <summary>
        /// Called before loading.
        /// </summary>
        void OnBeforeLoad();

        /// <summary>
        /// Called before saving.
        /// </summary>
        void OnBeforeSave();

        /// <summary>
        /// Called to load the config values.
        /// </summary>
        void OnLoad(object config);

        /// <summary>
        /// Called to save the values.
        /// </summary>
        void OnSave(object config);

        /// <summary>
        /// Save the config settings to the underlying datasource via full life-cycle.
        /// </summary>
        void Save(object config);

        #endregion Methods
    }
}