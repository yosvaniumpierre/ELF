namespace Avanade.BootStrapper.Web.Azure.Processor
{
    using System;
    using System.Reflection;
    using System.Web.Compilation;

    internal class DefaultAssemblyItemProcessor : BaseAssemblyItemProcessor
    {
        #region Methods

        public override bool Process(AssemblyItem assemblyItem)
        {
            //load the assembly from blob into currentdomain.
            AppDomain.CurrentDomain.Load(assemblyItem.Bytes);

            return true;
        }

        #endregion Methods
    }
}