namespace Avanade.Platform.Services.Web.Configuration
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;

    using ComLib.Cryptography;

    /// <summary>
    /// A single setting from a configuration file.
    /// </summary>
    internal class Setting : IConfigSetting
    {
        #region Constructors

        internal Setting(string name, string value, bool isArray)
        {
            Name = name;
            RawValue = value;
            IsArray = isArray;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is array.
        /// </summary>
        /// <value><c>True</c> If this instance is array; otherwise, <c>False</c>.</value>
        public bool IsArray
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets the name of the setting.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets the raw value of the setting.
        /// </summary>
        /// <value>The raw value.</value>
        public string RawValue
        {
            get; private set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Attempts to return the setting's value as a bool.
        /// </summary>
        /// <returns>A bool representation of the value.</returns>
        public bool GetValueAsBool()
        {
            return bool.Parse(RawValue);
        }

        /// <summary>
        /// Attempts to return the setting's value as an array of bools.
        /// </summary>
        /// <returns>An bool array representation of the value.</returns>
        public bool[] GetValueAsBoolArray()
        {
            string[] parts = RawValue.Split(',');

            var valueParts = new bool[parts.Length];

            for (int i = 0; i < parts.Length; i++)
                valueParts[i] = bool.Parse(parts[i]);

            return valueParts;
        }

        /// <summary>
        /// Attempts to return the setting's value as a decrypted string.
        /// </summary>
        /// <returns>Decrypted string representation of the value.</returns>
        public string GetValueAsDecryptedString()
        {
            if (!RawValue.StartsWith("\"") && !RawValue.EndsWith("\""))
            {
                return Crypto.Decrypt(RawValue);
            }

            return Crypto.Decrypt(RawValue.Substring(1, RawValue.Length - 2));
        }

        /// <summary>
        /// Attempts to return the setting's value as a float.
        /// </summary>
        /// <returns>A float representation of the value.</returns>
        public float GetValueAsFloat()
        {
            return float.Parse(RawValue, CultureInfo.InvariantCulture.NumberFormat);
        }

        /// <summary>
        /// Attempts to return the setting's value as an array of floats.
        /// </summary>
        /// <returns>An float array representation of the value.</returns>
        public float[] GetValueAsFloatArray()
        {
            string[] parts = RawValue.Split(',');

            var valueParts = new float[parts.Length];

            for (int i = 0; i < parts.Length; i++)
                valueParts[i] = float.Parse(parts[i], CultureInfo.InvariantCulture.NumberFormat);

            return valueParts;
        }

        /// <summary>
        /// Attempts to return the setting's value as an integer.
        /// </summary>
        /// <returns>An integer representation of the value.</returns>
        public int GetValueAsInt()
        {
            return int.Parse(RawValue, CultureInfo.InvariantCulture.NumberFormat);
        }

        /// <summary>
        /// Attempts to return the setting's value as an array of integers.
        /// </summary>
        /// <returns>An integer array representation of the value.</returns>
        public int[] GetValueAsIntArray()
        {
            var parts = RawValue.Split(',');

            var valueParts = new int[parts.Length];

            for (var i = 0; i < parts.Length; i++)
                valueParts[i] = int.Parse(parts[i], CultureInfo.InvariantCulture.NumberFormat);

            return valueParts;
        }

        /// <summary>
        /// Attempts to return the setting's value as a string.
        /// </summary>
        /// <returns>A string representation of the value.</returns>
        public string GetValueAsString()
        {
            if (!RawValue.StartsWith("\"") && !RawValue.EndsWith("\""))
            {
                return RawValue;
            }

            return RawValue.Substring(1, RawValue.Length - 2);
        }

        /// <summary>
        /// Attempts to return the setting's value as an array of strings.
        /// </summary>
        /// <returns>An string array representation of the value.</returns>
        public string[] GetValueAsStringArray()
        {
            Match match = Regex.Match(RawValue, "[a-zA-Z\\d\\s]*[,]*");
            var values = new List<string>();

            while (match.Success)
            {
                string value = match.Value;

                if (value.EndsWith(","))
                {
                    value = value.Substring(0, value.Length - 1);
                }
                if(!string.IsNullOrEmpty(value))
                {
                    values.Add(value);
                }
                match = match.NextMatch();
            }

            return values.ToArray();
        }

        #endregion Methods
    }
}