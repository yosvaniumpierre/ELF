namespace Avanade.BootStrapper.Web.Task
{
    using Castle.Windsor;

    /// <summary>
    /// Look at this to understand the pitfalls of virtual and override - http://www.blackrabbitcoder.net/archive/2011/07/21/c.net-little-pitfalls-the-default-is-to-hide-not-override.aspx.
    /// </summary>
    public abstract class BaseTask : ITask
    {
        #region Methods

        public virtual void Cleanup(IWindsorContainer container, BootStrapRuntime runtime)
        {
            //This is an optional implementation.
        }

        public abstract void Execute(IWindsorContainer container, BootStrapRuntime runtime);

        #endregion Methods
    }
}