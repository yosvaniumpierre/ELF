using System;
using System.Collections.Generic;
using System.IO;

namespace Avanade.ServiceContainer.Impl
{
    /// <summary>
    /// Description of WindsorDelegate.
    /// </summary>
    internal class WindsorDelegate : AbstractWindsorAdapter
    {
        #region Fields

        /// <summary>
        /// logging instance
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WindsorDelegate));

        private readonly List<IDisposable> disposableList = new List<IDisposable>();

        #endregion Fields

        #region Constructors

        public WindsorDelegate(MBeanServerExtension mbeanServerExtension)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Initialising the Service Container Windsor delegate...");
            }

            //Add an event listener in order to determine the component creation order
            Container.Kernel.ComponentCreated += (componentModel, instance) =>
                mbeanServerExtension.AddMBean(componentModel.Service.Name, instance);

            DoInitialise();
            Logger.Info("Created and initialised the Castle Windsor delegate!");
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Disposes a list of objects that have implemented the IDisposable interface but is not found in the container.
        /// </summary>
        public override void Stop()
        {
            base.Stop();

            disposableList.ForEach(disposable => disposable.Dispose());
            Logger.Info("Listed disposables have also been stopped inside the container!");

            disposableList.Clear();
        }

        protected override FileInfo DoGetConfigFilePath()
        {
            Logger.InfoFormat("Looking for Castle configuration from the app.config/web.config file");
            return null;
        }

        protected override void DoInstall(IWindsorContainer container)
        {
            ExternalExtension externalExtension = new ExternalExtension();
            disposableList.Add(externalExtension);

            //Add more extensions here
            container.Install(externalExtension);
        }

        #endregion Methods
    }
}