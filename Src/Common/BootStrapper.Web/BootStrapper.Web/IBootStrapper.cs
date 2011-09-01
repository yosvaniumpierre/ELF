namespace Avanade.BootStrapper.Web
{
    using System;
    using System.Reflection;

    using Castle.Windsor;

    public interface IBootStrapper
    {
        #region Properties

        IWindsorContainer Container
        {
            get;
        }

        #endregion Properties

        #region Methods

        void Catch(Exception exception);

        void Dispose();

        object GetService(Type serviceType);

        /// <summary>
        /// BootStrapper will load the Windsor configuration file before performing the fluent registration of the component services.
        /// </summary>
        /// <param name="callingAssembly"></param>
        void Run(Assembly callingAssembly);

        #endregion Methods
    }
}