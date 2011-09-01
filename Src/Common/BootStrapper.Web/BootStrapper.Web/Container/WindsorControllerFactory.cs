namespace Avanade.BootStrapper.Web.Container
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Castle.MicroKernel;

    using Framework;

    using NLog;

    public class WindsorControllerFactory : DefaultControllerFactory
    {
        #region Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IKernel kernel;

        #endregion Fields

        #region Constructors

        public WindsorControllerFactory(IKernel kernel)
        {
            this.kernel = kernel;
        }

        #endregion Constructors

        #region Methods

        public override IController CreateController(RequestContext requestContext, string controllerName)
        {
            IController controller = base.CreateController(requestContext, controllerName);

            if (controller == null)
            {
                var customException = ExceptionFactory.WhenControllerNonExistent(controllerName);
                Logger.Warn(customException.Message, customException);
            }

            return controller;
        }

        public new void ReleaseController(IController controller)
        {
            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }

            kernel.ReleaseComponent(controller);
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                return null;
            }
            try
            {
                return (IController)kernel.Resolve(controllerType);
            }
            catch (Exception exception)
            {
                var customException = ExceptionFactory.WhenControllerInstantiationFailed(exception);
                Logger.Error(customException.Message, customException);
                return null;
            }
        }

        #endregion Methods
    }
}