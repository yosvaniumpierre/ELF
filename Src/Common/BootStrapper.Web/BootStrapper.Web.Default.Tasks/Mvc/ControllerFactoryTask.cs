namespace Avanade.BootStrapper.Web.Default.Tasks.Mvc
{
    using System.Web.Mvc;

    using Castle.Windsor;

    using Container;

    using NLog;

    using Task;

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