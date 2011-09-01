namespace Avanade.BootStrapper.Web.Default.Tasks.Mvc
{
    using System.Linq;
    using System.Web.Mvc;

    using Avanade.BootStrapper.Web.Container;
    using Avanade.BootStrapper.Web.Task;

    using Castle.Windsor;

    [TaskPriority(1)]
    public class FilterProviderSetupTask : BaseTask
    {
        #region Methods

        public override void Execute(IWindsorContainer container, BootStrapRuntime runtime)
        {
            // Setup the attribute filter provider so that the dependencies in the attributes can also be injected.
            // For example, AuthenticateAttribute needs to have an instance of IAuthenticatePlugin injected.
            var oldProvider = FilterProviders.Providers.Single(
                    f => f is FilterAttributeFilterProvider
                );
            FilterProviders.Providers.Remove(oldProvider);

            var provider = new WindsorFilterProvider(container);
            FilterProviders.Providers.Add(provider);
        }

        #endregion Methods
    }
}