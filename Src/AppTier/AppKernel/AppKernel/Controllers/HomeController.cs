namespace Avanade.AppKernel.Controllers
{
    using System.Web.Mvc;

    using Avanade.BootStrapper.Web.Framework;

    public class HomeController : BaseController
    {
        #region Methods

        public ViewResult Index()
        {
            return View();
        }

        #endregion Methods
    }
}