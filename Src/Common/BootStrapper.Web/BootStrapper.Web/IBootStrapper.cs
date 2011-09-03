namespace Avanade.BootStrapper.Web
{
    using System;
    using System.Reflection;

    using Castle.Windsor;

    /// <summary>
    /// This class will bootstrap the web application.
    /// Typically, bootstrapping a web application will begin with the global.asax getting an instance of the IBootStrapper.
    /// With a reference to IBootStrapper, the web application can be stripped bare with all initialisation delegated to the bootstrapper..
    /// <para>
    /// Bootstrapping the web application will load a bunch of assemblies:
    /// <list type="number">
    ///     <item>
    ///         <description>*.Tasks.dll (Task assembly) - contain tasks used for bootstrapping the application</description>
    ///     </item>
    ///     <item>
    ///         <description>*.Ext.dll (Extension assembly) - component with embedded MVC elements (i.e., Views, Controller and Model)</description>
    ///     </item>
    ///     <item>
    ///         <description>*.Plugin.dll (Plugin assembly) - component without any MVC elements</description>
    ///     </item>
    /// </list>
    /// </para>
    /// <seealso cref="WebBootStrapper"/>
    /// </summary>
    public interface IBootStrapper
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        IWindsorContainer Container
        {
            get;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Catches all uncaught exceptions that are caught in Global.asax but delegated to the bootstrapper.
        /// </summary>
        /// <param name="exception">Global.asax delegated uncaught exception</param>
        void Catch(Exception exception);

        /// <summary>
        /// Cleans up the resources handled by the bootstrapper.
        /// </summary>
        void Dispose();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        object GetService(Type serviceType);

        /// <summary>
        /// BootStrapper will load the Windsor configuration file before performing the fluent registration of the component services.
        /// </summary>
        /// <param name="callingAssembly"></param>
        void Run(Assembly callingAssembly);

        #endregion Methods
    }
}