using System;
using System.Collections;
using System.Collections.Generic;

namespace Avanade.Config.Impl
{
    /// <summary>
    /// Config source multi - list of multiple config sources.
    /// Need to hit make this thread safe.
    /// </summary>
    public class ConfigSourceMulti : ConfigSection, IConfigSource
    {
        #region Fields

        private readonly List<IConfigSource> configSources;
        private string sourcePath;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Config source list.
        /// </summary>
        /// <param name="sources"></param>
        public ConfigSourceMulti(List<IConfigSource> sources)
        {
            configSources = sources;

            // Iterate through each config and get the name/paths.
            configSources.ForEach( configSource =>
            {
                configSource.OnConfigSourceChanged += new EventHandler(configSource_OnConfigSourceChanged);
                sourcePath += configSource.SourcePath + ",";
            });
            sourcePath = sourcePath.Replace("/", "\\");
            Merge();
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Notifies subscribers when any configsource was changed.
        /// </summary>
        public event EventHandler OnConfigSourceChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Get the source paths.
        /// e.g. c:\app\prod.config,c:\app\dev.config
        /// </summary>
        public string SourcePath
        {
            get { return sourcePath; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Initialization after construction.
        /// </summary>
        public void Init()
        {
        }

        /// <summary>
        /// Load settings.
        /// </summary>
        public void Load()
        {
            configSources.ForEach(configSource => configSource.Load());
        }

        /// <summary>
        /// Save the sources.
        /// </summary>
        public void Save()
        {
            configSources.ForEach(configSource => configSource.Save());
        }

        /// <summary>
        /// Merge all the config sources.
        /// </summary>
        protected virtual void Merge()
        {
            // The config sources are from highest inhertance to lowest.
            // e.g. prod,qa,dev.
            // prod inherits qa, inherited by dev.
            // This merge has to be done in reverse order.
            for(int ndx = configSources.Count - 1; ndx >= 0; ndx--)
            {
                var configSource = configSources[ndx];
                Merge(configSource, this);
            }
        }

        /// <summary>
        /// Merge with config source specified.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        protected virtual void Merge(IConfigSection source, IConfigSection dest)
        {
            // Get all the sections.
            foreach (DictionaryEntry entry in source)
            {
                // Create new config section.
                if (entry.Value is IConfigSection)
                {
                    IConfigSection newDest = null;
                    if (dest.Contains(entry.Key))
                    {
                        newDest = dest.GetSection(entry.Key.ToString());
                    }
                    else
                    {
                        newDest = new ConfigSection(entry.Key.ToString());
                        dest.Add(newDest.Name, newDest);
                    }
                    Merge(entry.Value as IConfigSection, newDest);
                }
                else // Just overwrite the keys.
                {
                    dest[entry.Key] = entry.Value;
                }
            }
        }

        /// <summary>
        /// Event handler for on config source changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void configSource_OnConfigSourceChanged(object sender, EventArgs e)
        {
            if (OnConfigSourceChanged != null)
                OnConfigSourceChanged(sender, e);
        }

        #endregion Methods
    }
}