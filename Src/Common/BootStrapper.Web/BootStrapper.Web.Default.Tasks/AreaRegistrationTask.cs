using System.Web.Mvc;
using Avanade.BootStrapper.Web.Task;
using Castle.Windsor;

namespace Avanade.BootStrapper.Web.Default.Tasks
{
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