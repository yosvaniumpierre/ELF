namespace Avanade.BootStrapper.Web.Azure
{
    using System.Collections.Generic;

    using Castle.Windsor;

    using NLog;

    using Processor;

    internal class AssemblyItemStrategy
    {
        #region Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IList<IAssemblyItemProcessor> processorList;

        #endregion Fields

        #region Constructors

        internal AssemblyItemStrategy(IWindsorContainer container)
        {
            processorList = new List<IAssemblyItemProcessor>
                                {
                                    new ExtensionProcessor(container),
                                    new PluginProcessor(container),
                                    new TasksProcessor(container),
                                    new DefaultAssemblyItemProcessor()
                                };
        }

        #endregion Constructors

        #region Methods

        internal void Handle(AssemblyItem assemblyItem)
        {
            foreach (var assemblyItemProcessor in processorList)
            {
                bool processed = assemblyItemProcessor.Process(assemblyItem);

                if (Logger.IsDebugEnabled && processed)
                {
                    Logger.Debug("Outcome of processing: {0}, using Processor: {1}, for Assembly Item: {2}",
                        processed, assemblyItemProcessor, assemblyItem);
                }

                if(processed)
                {
                    break;
                }
            }
        }

        #endregion Methods
    }
}