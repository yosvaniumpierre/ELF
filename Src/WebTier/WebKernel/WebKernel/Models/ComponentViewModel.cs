namespace Avanade.WebKernel.Models
{
    using System.Collections.Generic;

    public class ComponentViewModel
    {
        #region Properties

        public IEnumerable<Component> Components
        {
            get; set;
        }

        #endregion Properties
    }
}