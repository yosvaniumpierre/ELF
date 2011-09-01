namespace Avanade.AppKernel
{
    using System.Reflection;

    using BootStrapper.Web.Attributes;
    using BootStrapper.Web.Container;

    using Castle.Facilities.Startable;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    public class WindsorInstaller : IWindsorInstaller
    {
        #region Methods

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.RegisterControllers(Assembly.GetExecutingAssembly());

            container.AddFacility<StartableFacility>(s => s.DeferredStart()).Register(
                Component.For<AuthenticateAttribute>().ImplementedBy<AuthenticateAttribute>().LifeStyle.Singleton,
                Component.For<IWindsorContainer>().Instance(container).LifeStyle.Singleton
                );
        }

        #endregion Methods
    }
}