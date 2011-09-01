namespace Avanade.BootStrapper.Web.Container.Ext
{
    using System.Reflection;

    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    public class WindsorInstaller : IWindsorInstaller
    {
        #region Methods

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.RegisterControllers(Assembly.GetExecutingAssembly());
        }

        #endregion Methods
    }
}