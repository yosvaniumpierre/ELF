using System;

namespace Avanade.Config
{
    /// <summary>
    /// Base interface for a configuration source.
    /// </summary>
    public interface IConfigSourceBase
    {
        #region Events

        /// <summary>
        /// Event handler when the configuration store changes.
        /// </summary>
        event EventHandler OnConfigSourceChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Name of the source.
        /// This cane be the file path.
        /// </summary>
        string SourcePath
        {
            get;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Called after construction
        /// </summary>
        void Init();

        /// <summary>
        /// Load the config settings from the underlying datasource.
        /// </summary>
        void Load();

        /// <summary>
        /// Save the config settings to the underlying datasource.
        /// </summary>
        void Save();

        #endregion Methods
    }
}