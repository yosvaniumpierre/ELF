using System.Web.Mvc;
using Avanade.BootStrapper.Web.Container;
using Avanade.BootStrapper.Web.Task;
using Castle.Windsor;
using NLog;

namespace Avanade.BootStrapper.Web.Default.Tasks
{
    [TaskPriority(1)]
    public class ControllerFactoryTask : BaseTask
    {
        #region Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion Fields

        #region Methods

        public override void Execute(IWindsorContainer container, BootStrapRuntime runtime)
        {
            Logger.Info("Overriding the choice of the ControllerFactory with a Windsor-centric one...");

            // Resolve the IControllerFactory for MVC usage
            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(container.Kernel));
        }

        #endregion Methods
    }
}