using Avanade.BootStrapper.Web.Attributes;
using Avanade.BootStrapper.Web.Container;

namespace Avanade.WebKernel.Container
{
    using System.Reflection;

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