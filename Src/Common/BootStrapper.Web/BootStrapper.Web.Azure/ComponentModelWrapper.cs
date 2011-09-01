namespace Avanade.BootStrapper.Web.Azure
{
    using System;

    using Castle.Core;

    internal class ComponentModelWrapper
    {
        #region Constructors

        internal ComponentModelWrapper(string key, ComponentModel componentModel)
        {
            Key = key;
            ComponentModel = componentModel;
        }

        #endregion Constructors

        #region Properties

        internal ComponentModel ComponentModel
        {
            get; private set;
        }

        internal string Key
        {
            get; private set;
        }

        #endregion Properties

        #region Methods

        internal bool ExistsImplementation(Type implementationType)
        {
            return ComponentModel.Implementation.Equals(implementationType);
        }

        #endregion Methods
    }
}