namespace Avanade.Platform.Services.Web.Unit.Tests.Configuration
{
    using System.Collections.Generic;

    using Web.Configuration;

    class KeyValueExtension : IKeyValueExtension
    {
        #region Fields

        private readonly IDictionary<string, string> dictionary;

        #endregion Fields

        #region Constructors

        public KeyValueExtension()
        {
            dictionary = new Dictionary<string, string> {{"Extension", "TestValue"}, {"", ""}};
        }

        #endregion Constructors

        #region Properties

        public IDictionary<string, string> Values
        {
            get { return dictionary; }
        }

        #endregion Properties
    }
}