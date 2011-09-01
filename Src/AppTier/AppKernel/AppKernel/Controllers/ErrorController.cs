namespace Avanade.AppKernel.Controllers
{
    using System.Net;
    using System.Web.Mvc;

    using Avanade.BootStrapper.Web.Framework;

    using NLog;

    public class ErrorController : BaseController
    {
        #region Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion Fields

        #region Methods

        public ActionResult Http404(string url)
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            var model = new NotFoundViewModel();
            // If the url is relative ('NotFound' route) then replace with Requested path
            model.RequestedUrl = Request.Url.OriginalString.Contains(url) & Request.Url.OriginalString != url ?
                Request.Url.OriginalString : url;
            // Dont get the user stuck in a 'retry loop' by
            // allowing the Referrer to be the same as the Request
            model.ReferrerUrl = Request.UrlReferrer != null &&
                Request.UrlReferrer.OriginalString != model.RequestedUrl ?
                Request.UrlReferrer.OriginalString : null;

            // TODO: insert ILogger here

            return View("NotFound", model);
        }

        //
        // GET: /Error/
        public ActionResult Index(string aspxerrorpath)
        {
            //Logger.Info("Exception captured: " + exception.GetType());
            //Logger.Info("Resolution exception: " + exception.ResolutionInstruction);
            //if (exception is CustomAppException)
            //{
            //    ViewData.Model = exception as CustomAppException;
            //}
            //else
            //{
            //    ViewData.Model = ExceptionFactory.Wrap(exception);
            //}
            if (!string.IsNullOrEmpty(aspxerrorpath))
            {
                ViewData["Path"] = aspxerrorpath;
            }
            return View("Error");
        }

        public ActionResult NotFound()
        {
            return View("NotFound");
        }

        #endregion Methods
    }

    public class NotFoundViewModel
    {
        #region Properties

        public string ReferrerUrl
        {
            get;
            set;
        }

        public string RequestedUrl
        {
            get;
            set;
        }

        #endregion Properties
    }
}