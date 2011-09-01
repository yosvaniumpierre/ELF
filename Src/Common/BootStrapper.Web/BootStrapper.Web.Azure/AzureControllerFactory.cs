namespace Avanade.BootStrapper.Web.Azure
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Castle.MicroKernel;

    using Framework;

    using NLog;

    public class AzureControllerFactory : DefaultControllerFactory
    {
        #region Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IKernel kernel;
        private readonly ServiceLocator serviceLocator;

        #endregion Fields

        #region Constructors

        public AzureControllerFactory(IKernel kernel, ServiceLocator serviceLocator)
        {
            this.kernel = kernel;
            this.serviceLocator = serviceLocator;
        }

        #endregion Constructors

        #region Methods

        public override IController CreateController(RequestContext requestContext, string controllerName)
        {
            if (requestContext == null)
            {
                throw new ArgumentNullException("requestContext");
            }
            if (string.IsNullOrEmpty(controllerName))
            {
                throw new ArgumentException("ControllerName for creation is either null or empty string!", "controllerName");
            }

            Type controllerType = serviceLocator.GetControllerType(controllerName);

            return GetControllerInstance(requestContext, controllerType);
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
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Getting a controller instance: Type-{0}", controllerType);
                }

                ComponentModelWrapper componentModelWrapper;
                var exists = serviceLocator.ExistsImplementation(controllerType, out componentModelWrapper);
                if (exists)
                {
                    return (IController)kernel.Resolve(componentModelWrapper.Key, componentModelWrapper.ComponentModel.Service);
                }
                return (IController)kernel.Resolve(controllerType);
            }
            catch (Exception exception)
            {
                var customException = ExceptionFactory.WhenControllerInstantiationFailed(exception);
                Logger.ErrorException(customException.Message, exception);
                return null;
            }
        }

        #endregion Methods
    }
}