namespace Avanade.Platform.Services.Web.Configuration
{
    using System.Collections.Generic;

    /// <summary>
    /// This is an extension to support getting Key-Value pairs from datasources that are currently not supported by the components or
    /// from datasources that are project-specific.
    /// </summary>
    public interface IKeyValueExtension
    {
        #region Properties

        /// <summary>
        /// Gets the Key-Value pair values.
        /// </summary>
        /// <value>The values.</value>
        IDictionary<string, string> Values
        {
            get;
        }

        #endregion Properties
    }
}