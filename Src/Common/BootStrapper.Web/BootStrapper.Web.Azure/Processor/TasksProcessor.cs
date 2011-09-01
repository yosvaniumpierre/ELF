namespace Avanade.BootStrapper.Web.Azure.Processor
{
    using System.Reflection;

    using Avanade.BootStrapper.Web.Task;

    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    internal class TasksProcessor : BaseAssemblyItemProcessor
    {
        #region Fields

        private readonly IWindsorContainer container;

        #endregion Fields

        #region Constructors

        internal TasksProcessor(IWindsorContainer container)
        {
            this.container = container;
        }

        #endregion Constructors

        #region Methods

        public override bool Process(AssemblyItem assemblyItem)
        {
            if (IsMatch(assemblyItem.Name, BootStrapRuntime.TasksMask))
            {
                var assembly = Assembly.Load(assemblyItem.Bytes);
                container.Register(AllTypes.FromAssembly(assembly).BasedOn<BaseTask>().Configure(c => c.LifeStyle.Transient));
                return true;
            }
            return false;
        }

        #endregion Methods
    }
}