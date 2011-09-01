namespace Avanade.BootStrapper.Web.Container.Ext.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using Attributes;

    using Castle.Windsor;

    using Framework;

    using Models;

    using NLog;

    public class ContainerController : BaseController
    {
        #region Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion Fields

        #region Methods

        [JsonPox]
        public ViewResult Index()
        {
            var containerAccessor = HttpContext.ApplicationInstance as IContainerAccessor;
            if (containerAccessor == null)
            {
                Logger.Warn("Do check why the HttpApplication did not implement the IContainerAccessor?!");
                return View();
            }

            IList<Component> components = containerAccessor.Container.Kernel.GetAssignableHandlers(typeof (object)).Select(handler =>
                new Component(handler.ComponentModel.Service.FullName, handler.ComponentModel.Implementation.FullName)).ToList();

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Number of components discovered: {0}", components.Count);
            }

            var componentViewModel = new ComponentViewModel(components);
            ViewData.Model = componentViewModel;

            return View();
        }

        #endregion Methods
    }
}