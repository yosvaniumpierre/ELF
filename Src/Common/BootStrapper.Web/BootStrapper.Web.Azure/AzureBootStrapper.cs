namespace Avanade.BootStrapper.Web.Azure
{
    using System.Reflection;
    using System.Web.Mvc;

    using Castle.MicroKernel;
    using Castle.Windsor;
    using Castle.Windsor.Installer;

    using Container;

    using NLog;

    /// <summary>
    /// This implementation of the IBootStrapper class is specifically targeted for web applications running in Windows Azure environments.
    /// </summary>
    public class AzureBootStrapper : WebBootStrapper
    {
        #region Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly ServiceLocator ServiceLocator = new ServiceLocator();

        #endregion Fields

        #region Methods

        protected override IWindsorContainer CreateContainer(Assembly callingAssembly)
        {
            Logger.Info("Creating the Windsor Container for use within Azure!");

            var container = new WindsorContainer();
            container.Kernel.ComponentRegistered += KernelComponentRegistered;

            var processingStrategy = new AssemblyItemStrategy(container);

            // Make sure that it picks up the IWindsorInstallers from the calling assembly.
            // That is, the application may already have IWindsorInstaller instances.
            container.Install(FromAssembly.Instance(callingAssembly));

            var assemblyItems = AssemblyStorage.Extract();

            Logger.Info("Number of assemblies extracted from the Blob Storage: {0}", assemblyItems.Count);

            foreach (var assemblyItem in assemblyItems)
            {
                processingStrategy.Handle(assemblyItem);
            }

            container.InstallXmlConfigFile(WindsorConfig);

            return container;
        }

        protected override void ExecuteTasks()
        {
            base.ExecuteTasks();

            Logger.Info("Overriding the default ControllerFactory with an Azure-capable one...");

            ControllerBuilder.Current.SetControllerFactory(new AzureControllerFactory(Container.Kernel, ServiceLocator));
        }

        private static void KernelComponentRegistered(string key, IHandler handler)
        {
            ServiceLocator.Add(key, handler.ComponentModel);
        }

        #endregion Methods
    }
}