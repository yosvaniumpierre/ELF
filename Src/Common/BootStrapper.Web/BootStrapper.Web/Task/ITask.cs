namespace Avanade.BootStrapper.Web.Task
{
    using Castle.Windsor;

    public interface ITask
    {
        #region Methods

        void Cleanup(IWindsorContainer container, BootStrapRuntime runtime);

        void Execute(IWindsorContainer container, BootStrapRuntime runtime);

        #endregion Methods
    }
}