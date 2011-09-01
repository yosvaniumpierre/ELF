namespace Avanade.AppKernel
{
    using System;
    using System.Reflection;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Controllers;
    using BootStrapper;
    using BootStrapper.Web;

    using Castle.Windsor;

    // Note: For instructions on enabling IIS6 or IIS7 classic mode,
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : HttpApplication, IServiceProvider, IContainerAccessor
    {
        #region Fields

        private static IBootStrapper bootStrapper;

        #endregion Fields

        #region Properties

        public IWindsorContainer Container
        {
            get { return bootStrapper.Container; }
        }

        #endregion Properties

        #region Methods

        public object GetService(Type serviceType)
        {
            return bootStrapper.GetService(serviceType);
        }

        protected void Application_End()
        {
            if (bootStrapper == null)
            {
                return;
            }
            bootStrapper.Dispose();
        }

        /// <summary>
        /// Handles the Error event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();

            bootStrapper.Catch(exception);

            var httpException = exception as HttpException;
            Response.Clear();
            Server.ClearError();
            var routeData = new RouteData();
            routeData.Values["controller"] = "Error";
            routeData.Values["action"] = "Index";
            routeData.Values["exception"] = exception;
            Response.TrySkipIisCustomErrors = true;
            Response.StatusCode = 500;
            if (httpException != null)
            {
                Response.StatusCode = httpException.GetHttpCode();
                switch (Response.StatusCode)
                {
                    case 403:
                        routeData.Values["action"] = "NotFound";
                        break;
                    case 404:
                        routeData.Values["action"] = "NotFound";
                        break;
                }
            }

            IController errorsController = new ErrorController();
            var rc = new RequestContext(new HttpContextWrapper(Context), routeData);
            errorsController.Execute(rc);
        }

        protected void Application_Start()
        {
            // Now do the boot-strapping for the application.
            bootStrapper = BootStrapperFactory.Create();
            bootStrapper.Run(Assembly.GetExecutingAssembly());
        }

        #endregion Methods
    }
}