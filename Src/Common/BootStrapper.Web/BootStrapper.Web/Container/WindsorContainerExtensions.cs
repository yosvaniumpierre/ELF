namespace Avanade.BootStrapper.Web.Container
{
    using System.IO;
    using System.Reflection;
    using System.Web;

    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Castle.Windsor.Installer;

    using Framework;

    using NLog;

    public static class WindsorContainerExtensions
    {
        #region Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion Fields

        #region Methods

        public static void InstallXmlConfigFile(this IWindsorContainer container, string configFileName)
        {
            var windsorConfigPath = Path.Combine(HttpRuntime.AppDomainAppPath, configFileName);

            if (File.Exists(windsorConfigPath))
            {
                Logger.Info("Castle Windsor config file exists at path: {0}", windsorConfigPath);
                container.Install(Configuration.FromXmlFile(windsorConfigPath));
            }
        }

        public static void RegisterControllers(this IWindsorContainer container, Assembly thisAssembly)
        {
            container.Register(AllTypes.FromAssembly(thisAssembly).BasedOn<BaseController>().Configure(c => c.LifeStyle.PerWebRequest));
        }

        #endregion Methods
    }
}