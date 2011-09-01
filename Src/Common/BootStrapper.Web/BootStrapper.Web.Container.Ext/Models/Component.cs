namespace Avanade.BootStrapper.Web.Container.Ext.Models
{
    public class Component
    {
        #region Constructors

        internal Component(string serviceName, string implementation)
        {
            ServiceName = serviceName;
            Implementation = implementation;
        }

        #endregion Constructors

        #region Properties

        public string Implementation
        {
            get; private set;
        }

        public string ServiceName
        {
            get; private set;
        }

        #endregion Properties
    }
}