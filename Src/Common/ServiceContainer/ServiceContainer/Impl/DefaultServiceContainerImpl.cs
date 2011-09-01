using NLog;

namespace Avanade.ServiceContainer.Impl
{
    public class DefaultServiceContainerImpl : IServiceContainer
    {
        #region Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private IBinder binderDelegate;
        private MBeanServerExtension mbeanServerExtension;

        #endregion Fields

        #region Methods

        public void Start()
        {
            Logger.Info("Starting the Service container by using the Windsor Delegate...");
            mbeanServerExtension = new MBeanServerExtension(9999);
            binderDelegate = new WindsorDelegate(mbeanServerExtension);

            mbeanServerExtension.AddMBean("WindsorContainer", binderDelegate);
            mbeanServerExtension.Start();
            Logger.Info("Added windsor container to the MBeanServer!");

            binderDelegate.Init();
            binderDelegate.Start();
        }

        public void Stop()
        {
            Logger.Info("Stopping the Service Container...");
            mbeanServerExtension.Stop();
            binderDelegate.Stop();
            binderDelegate.Cleanup();

            binderDelegate = null;
            mbeanServerExtension = null;
        }

        #endregion Methods
    }
}