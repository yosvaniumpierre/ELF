namespace Avanade.BootStrapper.Web.Framework
{
    using System.Web.Mvc;

    using Attributes;

    using NLog;

    [RevertToHttp]
    public abstract class BaseController : Controller
    {
        #region Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion Fields

        #region Methods

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext != null && filterContext.Exception != null)
            {
                string controller = filterContext.RouteData.Values["controller"].ToString();
                string action = filterContext.RouteData.Values["action"].ToString();
                string loggerName = string.Format("{0}Controller.{1}", controller, action);

                Logger.Warn("Base Controller caught an exception from {0}", GetType().Name);

                LogManager.GetLogger(loggerName).Error(string.Format("Thrown from within {0}: {1}", GetType().Name,
                    filterContext.Exception.Message), filterContext.Exception);
            }
        }

        #endregion Methods
    }
}