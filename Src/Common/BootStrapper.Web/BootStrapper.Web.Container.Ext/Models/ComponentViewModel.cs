namespace Avanade.BootStrapper.Web.Container.Ext.Models
{
    using System.Collections.Generic;

    public class ComponentViewModel
    {
        #region Constructors

        internal ComponentViewModel(IEnumerable<Component> components)
        {
            Components = components;
        }

        #endregion Constructors

        #region Properties

        public IEnumerable<Component> Components
        {
            get; private set;
        }

        #endregion Properties
    }
}