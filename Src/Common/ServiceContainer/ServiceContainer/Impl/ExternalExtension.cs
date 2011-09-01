using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using NLog;

namespace Avanade.ServiceContainer.Impl
{
    /// <summary>
    /// Description of ExternalExtension.
    /// </summary>
    internal class ExternalExtension : IWindsorInstaller, IDisposable
    {
        #region Fields

        /// <summary>
        /// logging instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        private static IScheduler scheduler;

        #endregion Fields

        #region Methods

        public void Dispose()
        {
            scheduler.Shutdown(false);
            Logger.Info("Shutdown completed for the Quartz Scheduler!!");
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Adding the External services now...");
            }

            DoConfigureQuartz(container);
        }

        private static void DoConfigureQuartz(IWindsorContainer container)
        {
            try
            {
                // This option of creating an instance of the scheduler is recommended by the Quartz.Net. (see - http://quartznet.sourceforge.net/tutorial/lesson_10.html)
                // However, an issue was discovered when this assembly was used in MOSS 2007.
                // See here - http://groups.google.com/group/quartznet/browse_thread/thread/745f455999dbfb4f
                // See here - http://jira.opensymphony.com/browse/QRTZNET-207

                //// construct a scheduler factory
                //ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
                //if (Logger.IsDebugEnabled)
                //{
                //    Logger.DebugFormat("Scheduler Factory use: {0}", schedulerFactory);
                //}
                //// get a scheduler
                //var scheduler = schedulerFactory.GetScheduler();
                //container.Register(Component.For<IScheduler>().Instance(scheduler));

                DirectSchedulerFactory schedulerFactory = DirectSchedulerFactory.Instance;
                try
                {
                    schedulerFactory.CreateVolatileScheduler(10); // 10 threads
                }
                catch (SchedulerException schedulerException)
                {
                    Logger.Error("Problem encountered while creating the scheduler!", schedulerException);
                }
                scheduler = schedulerFactory.GetScheduler("SimpleQuartzScheduler");
                container.Register(Component.For<IScheduler>().Instance(scheduler));
                scheduler.Start();
            }
            catch (Exception exception)
            {
                Logger.Error("Problem encountered while creating and starting a Quartz.Net scheduler", exception);
            }
        }

        #endregion Methods
    }
}