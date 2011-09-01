using System;

namespace Avanade.Config.DataSource
{
    /// <summary>
    /// Class to store the key/values
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TVal"></typeparam>
    internal class KeyValueItem<TKey, TVal> : ICloneable
    {
        #region Fields

        private TKey key;
        private TVal val;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initialize the data with a valid value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public KeyValueItem(TKey key, TVal val)
        {
            this.key = key;
            this.val = val;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Get the null / empty object.
        /// </summary>
        public static KeyValueItem<TKey, TVal> Empty
        {
            get
            {
                TKey defaultKey = default(TKey);
                TVal defaultVal = default(TVal);
                return new KeyValueItem<TKey, TVal>(defaultKey, defaultVal);
            }
        }

        /// <summary>
        /// The key.
        /// </summary>
        public TKey Key
        {
            get { return key; }
        }

        /// <summary>
        /// The value.
        /// </summary>
        public TVal Value
        {
            get { return val; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Create shallow copy.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        /// Set the key. 
        /// </summary>
        /// <param name="configKey"></param>
        internal void SetKey(TKey configKey)
        {
            key = configKey;
        }

        /// <summary>
        /// Set the key. 
        /// </summary>
        internal void SetValue(TVal configValue)
        {
            val = configValue;
        }

        #endregion Methods
    }
}