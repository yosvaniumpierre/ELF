namespace Avanade.BootStrapper.Web.Default.Tasks.Mvc
{
    using System.Web.Mvc;

    using Castle.Windsor;

    using Task;

    [TaskPriority(1)]
    public class AreaRegistrationTask : BaseTask
    {
        #region Methods

        public override void Execute(IWindsorContainer container, BootStrapRuntime runtime)
        {
            AreaRegistration.RegisterAllAreas();
        }

        #endregion Methods
    }
}