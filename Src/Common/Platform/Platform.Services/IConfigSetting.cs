namespace Avanade.Platform.Services
{
    /// <summary>
    /// A single setting from a configuration file.
    /// </summary>
    public interface IConfigSetting
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is array.
        /// </summary>
        /// <value><c>True</c> If this instance is array; otherwise, <c>False</c>.</value>
        bool IsArray
        {
            get;
        }

        /// <summary>
        /// Gets or sets the name of the setting.
        /// </summary>
        /// <value>The name.</value>
        string Name
        {
            get;
        }

        /// <summary>
        /// Gets or sets the raw value of the setting.
        /// </summary>
        /// <value>The raw value.</value>
        string RawValue
        {
            get;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Attempts to return the setting's value as a bool.
        /// </summary>
        /// <returns>A bool representation of the value.</returns>
        bool GetValueAsBool();

        /// <summary>
        /// Attempts to return the setting's value as an array of bools.
        /// </summary>
        /// <returns>An bool array representation of the value.</returns>
        bool[] GetValueAsBoolArray();

        /// <summary>
        /// Attempts to return the setting's value as a decrypted string.
        /// </summary>
        /// <returns>Decrypted string representation of the value.</returns>
        string GetValueAsDecryptedString();

        /// <summary>
        /// Attempts to return the setting's value as a float.
        /// </summary>
        /// <returns>A float representation of the value.</returns>
        float GetValueAsFloat();

        /// <summary>
        /// Attempts to return the setting's value as an array of floats.
        /// </summary>
        /// <returns>An float array representation of the value.</returns>
        float[] GetValueAsFloatArray();

        /// <summary>
        /// Attempts to return the setting's value as an integer.
        /// </summary>
        /// <returns>An integer representation of the value.</returns>
        int GetValueAsInt();

        /// <summary>
        /// Attempts to return the setting's value as an array of integers.
        /// </summary>
        /// <returns>An integer array representation of the value.</returns>
        int[] GetValueAsIntArray();

        /// <summary>
        /// Attempts to return the setting's value as a string.
        /// </summary>
        /// <returns>A string representation of the value.</returns>
        string GetValueAsString();

        /// <summary>
        /// Attempts to return the setting's value as an array of strings.
        /// </summary>
        /// <returns>An string array representation of the value.</returns>
        string[] GetValueAsStringArray();

        #endregion Methods
    }
}