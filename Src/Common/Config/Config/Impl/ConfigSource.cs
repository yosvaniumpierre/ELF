using System;

namespace Avanade.Config.Impl
{
    /// <summary> 
    /// Simple class to lookup stored configuration settings by key. 
    /// Also provides type conversion methods. 
    /// GetInt("PageSize"); 
    /// GetBool("IsEnabled"); 
    /// </summary> 
    public class ConfigSource : ConfigSection, IConfigSource
    {
        #region Constructors

        /// <summary>
        /// Default construction.
        /// </summary>
        public ConfigSource()
        {
            Init();
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
        /// The source file path.
        /// </summary>
        public virtual string SourcePath
        {
            get { return string.Empty; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Init()
        {
        }

        /// <summary>
        /// Load from datasource.
        /// </summary>
        public virtual void Load()
        {
        }

        /// <summary>
        /// Save to the datasource.
        /// </summary>
        public virtual void Save()
        {
        }

        #endregion Methods
    }
}