using System.Web.Mvc;
using System.Web.Routing;
using Avanade.BootStrapper.Web.Task;
using Castle.Windsor;

namespace Avanade.BootStrapper.Web.Default.Tasks
{
    [TaskPriority(9)]
    public class RegisterRoutesTask : BaseTask
    {
        #region Fields

        private readonly RouteCollection routes;

        #endregion Fields

        #region Constructors

        public RegisterRoutesTask()
            : this(RouteTable.Routes)
        {
        }

        public RegisterRoutesTask(RouteCollection routes)
        {
            this.routes = routes;
        }

        #endregion Constructors

        #region Methods

        public override void Execute(IWindsorContainer container, BootStrapRuntime runtime)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        #endregion Methods
    }
}