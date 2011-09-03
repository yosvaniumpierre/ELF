namespace Avanade.BootStrapper.Web.Container
{
    using System.Reflection;

    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    using Framework;

    using NLog;

    public static class WindsorContainerExtensions
    {
        #region Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion Fields

        #region Methods

        public static void RegisterControllers(this IWindsorContainer container, Assembly thisAssembly)
        {
            container.Register(AllTypes.FromAssembly(thisAssembly).BasedOn<BaseController>().Configure(c => c.LifeStyle.PerWebRequest));
        }

        #endregion Methods
    }
}