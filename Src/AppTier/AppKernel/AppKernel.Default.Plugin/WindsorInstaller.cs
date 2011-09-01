namespace Avanade.AppKernel.Default.Plugin
{
    using Avanade.BootStrapper.Web.AddIn;

    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    public class WindsorInstaller : IWindsorInstaller
    {
        #region Methods

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IAuthenticatePlugin>().ImplementedBy<DefaultAuthenticationPlugin>().LifeStyle.Singleton
                );
        }

        #endregion Methods
    }
}