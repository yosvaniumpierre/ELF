namespace Avanade.BootStrapper.Web.Azure.Processor
{
    using System;

    using Castle.Windsor;
    using Castle.Windsor.Installer;

    internal class PluginProcessor : BaseAssemblyItemProcessor
    {
        #region Fields

        private readonly IWindsorContainer container;

        #endregion Fields

        #region Constructors

        internal PluginProcessor(IWindsorContainer container)
        {
            this.container = container;
        }

        #endregion Constructors

        #region Methods

        public override bool Process(AssemblyItem assemblyItem)
        {
            if (IsMatch(assemblyItem.Name, BootStrapRuntime.PluginMask))
            {
                var assembly = AppDomain.CurrentDomain.Load(assemblyItem.Bytes);
                container.Install(FromAssembly.Instance(assembly));

                ResourceUtil.Extract(assembly);

                return true;
            }
            return false;
        }

        #endregion Methods
    }
}