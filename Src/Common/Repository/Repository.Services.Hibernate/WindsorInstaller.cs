namespace Avanade.Repository.Services.Hibernate
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    using Core;

    using Security;

    using Services.Security;

    public class WindsorInstaller : IWindsorInstaller
    {
        #region Methods

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IPersistenceManager, INHibernatePersistenceManager>().ImplementedBy<NHibernatePersistenceManager>(),
                Component.For<IUserRepository>().ImplementedBy<UserRepository>().LifeStyle.PerWebRequest,
                Component.For<IRoleRepository>().ImplementedBy<RoleRepository>().LifeStyle.PerWebRequest,
                Component.For<IProfileRepository>().ImplementedBy<ProfileRepository>().LifeStyle.PerWebRequest,
                Component.For<IUnitOfWork>().UsingFactoryMethod(kernel => kernel.Resolve<IPersistenceManager>().Create()).LifeStyle.PerWebRequest
                );
        }

        #endregion Methods
    }
}