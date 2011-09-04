using Avanade.BootStrapper.Web.Task;
using Castle.Windsor;

namespace Avanade.BootStrapper.Web.Default.Tasks
{
    [TaskPriority(1)]
    public class VirtualPathProviderTask : BaseTask
    {
        #region Methods

        public override void Execute(IWindsorContainer container, BootStrapRuntime runtime)
        {
            // To facilitate serving of views from the DLL as embedded resources
            // http://www.chrisvandesteeg.nl/2010/11/22/embedding-pre-compiled-razor-views-in-your-dll/#axzz1Hy1JZrjy
            //HostingEnvironment.RegisterVirtualPathProvider(new AssemblyResourceVirtualPathProvider());
        }

        #endregion Methods
    }
}