using Avanade.BootStrapper.Web.Framework;

namespace Avanade.WebKernel.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    using Models;

    public class HomeController : BaseController
    {
        #region Methods

        public ActionResult Index()
        {
            var component1 = new Component { Service = "Service1", Implementation = "Impl1" };
            var component2 = new Component { Service = "Service2", Implementation = "Impl2" };

            var componentViewModel = new ComponentViewModel { Components = new List<Component> { component1, component2 } };

            ViewData.Model = componentViewModel.Components;

            return View(componentViewModel.Components);
        }

        #endregion Methods
    }
}