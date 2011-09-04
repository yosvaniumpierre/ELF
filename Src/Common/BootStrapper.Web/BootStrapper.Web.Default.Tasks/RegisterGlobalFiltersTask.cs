using System.Web.Mvc;
using Avanade.BootStrapper.Web.Task;
using Castle.Windsor;

namespace Avanade.BootStrapper.Web.Default.Tasks
{
    [TaskPriority(5)]
    public class RegisterGlobalFiltersTask : BaseTask
    {
        #region Fields

        private readonly GlobalFilterCollection filters;

        #endregion Fields

        #region Constructors

        public RegisterGlobalFiltersTask()
            : this(GlobalFilters.Filters)
        {
        }

        public RegisterGlobalFiltersTask(GlobalFilterCollection filters)
        {
            this.filters = filters;
        }

        #endregion Constructors

        #region Methods

        public override void Execute(IWindsorContainer container, BootStrapRuntime runtime)
        {
            filters.Add(new HandleErrorAttribute());
        }

        #endregion Methods
    }
}