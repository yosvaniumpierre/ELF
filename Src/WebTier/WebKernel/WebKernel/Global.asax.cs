using Avanade.BootStrapper;
using Avanade.BootStrapper.Web;
using NLog;

namespace Avanade.WebKernel
{
    using System;
    using System.Reflection;
    using System.Web;

    public class MvcApplication : HttpApplication
    {
        #region Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private IBootStrapper bootStrapper;

        #endregion Fields

        #region Methods

        protected void Application_End()
        {
            Logger.Info("Disposing the application...");

            bootStrapper.Dispose();
        }

        /// <summary>
        /// Handles the Error event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Application_Error(object sender, EventArgs e)
        {
            if (Response.StatusCode != 404)
            {
                Exception ex = Server.GetLastError();
                bootStrapper.Catch(ex);
            }
        }

        protected void Application_Start()
        {
            bootStrapper = BootStrapperFactory.Create();
            bootStrapper.Run(Assembly.GetExecutingAssembly());

            Logger.Info("Completed boot-strapping the application!");
        }

        #endregion Methods
    }
}