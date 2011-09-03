using System.IO;
using Castle.Windsor.Configuration.Interpreters;

namespace Avanade.BootStrapper.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web;

    using Castle.MicroKernel;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Castle.Windsor.Installer;

    using NLog;

    using Task;

    /// <summary>
    /// This implementation of the IBootStrapper class is specifically targeted for web applications running in traditional hosted environments.
    /// It is an application framework built to incorporate MVC component extensions (i.e., where each assembly has Controller, Model and View features).
    /// To make this work, the framework relies on the work done by David Ebbo (http://blog.davidebbo.com/2011/06/precompile-your-mvc-views-using.html).
    /// <para>
    /// As mentioned, in the simplest sense, an MVC component extension will be one assembly consisting of Controllers, Models and Views. 
    /// The layout of the classes and html assets are arranged according to the MVC configurations.
    /// More complex component extensions can consist of clusters of extensions.
    /// </para>
    /// <para>
    /// To be sure, this application framework is dependent on:
    /// <list type="bullet">
    ///     <item>
    ///         <description>.NET Framework 4</description>
    ///     </item>
    ///     <item>
    ///         <description>ASP.NET MVC 3</description>
    ///     </item>
    /// </list>
    /// </para>
    /// </summary>
    public class WebBootStrapper : IBootStrapper
    {
        #region Fields

        protected const string WindsorConfig = @"CastleWindsor.config";

        protected BootStrapRuntime Runtime;
        protected IList<BaseTask> Tasks;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion Fields
        
        #region Properties

        public IWindsorContainer Container
        {
            get; private set;
        }

        #endregion Properties

        #region Methods

        public virtual void Catch(Exception exception)
        {
            Logger.ErrorException("Unexpected error encountered!", exception);
        }

        public virtual void Dispose()
        {
            if (Tasks == null)
            {
                return;
            }

            foreach (var task in Tasks)
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Running the cleanup in the task: {0}", task);
                }
                task.Cleanup(Container, Runtime);
            }
            Logger.Info("Disposing the Windsor Container now...");
            Container.Dispose();
        }

        public virtual object GetService(Type serviceType)
        {
            return Container.Resolve(serviceType);
        }

        /// <summary>
        /// BootStrapper will load the Windsor configuration file before performing the fluent registration of the component services.
        /// </summary>
        /// <param name="callingAssembly"></param>
        public virtual void Run(Assembly callingAssembly)
        {
            try
            {
                Runtime = CreateBootStrapRuntime();

                ConfigureLogger();
                Logger.Info("Running the BootStrapper now...");

                Container = CreateContainer(callingAssembly);

                ExecuteTasks();
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Unknown problem encountered while spinning up the BootStrapper!", exception);
            }
        }

        protected virtual void ConfigureLogger()
        {
            //do nothing
        }

        protected virtual BootStrapRuntime CreateBootStrapRuntime()
        {
            return new BootStrapRuntime();
        }

        protected virtual IWindsorContainer CreateContainer(Assembly callingAssembly)
        {
            Logger.Info("Creating the Windsor Container for use within standard ASP.NET hosted environment!");
            var windsorConfigPath = Path.Combine(HttpRuntime.AppDomainAppPath, WindsorConfig);
            var exists = File.Exists(windsorConfigPath);
            Logger.Info("Status of CastleWindsor config file ({0}): {1}", WindsorConfig, exists);
            var container = exists ? new WindsorContainer(new XmlInterpreter(windsorConfigPath)) : new WindsorContainer();
            container.Kernel.ComponentRegistered += KernelComponentRegistered;

            container.Register(AllTypes.FromAssemblyInDirectory(new AssemblyFilter(HttpRuntime.BinDirectory, BootStrapRuntime.TasksMask)).
                BasedOn<BaseTask>().Configure(c => c.LifeStyle.Transient));

            // This will find all the IWindsorInstall instances in the assemblies and install all the components registered in these installers.
            container.Install(
                    FromAssembly.Instance(callingAssembly),
                    FromAssembly.InDirectory(new AssemblyFilter(HttpRuntime.BinDirectory, BootStrapRuntime.ExtensionMask)),
                    FromAssembly.InDirectory(new AssemblyFilter(HttpRuntime.BinDirectory, BootStrapRuntime.PluginMask))
                    );

            return container;
        }

        protected virtual void ExecuteTasks()
        {
            Logger.Info("Executing the tasks...");

            Type priorityType = typeof (TaskPriorityAttribute);

            Tasks = Container.ResolveAll<BaseTask>().OrderBy(t =>
                                                              {
                                                                  Type taskType = t.GetType();

                                                                  var priority =
                                                                      taskType.GetCustomAttributes(priorityType, false)
                                                                          .SingleOrDefault() as TaskPriorityAttribute;

                                                                  if (priority != null)
                                                                  {
                                                                      return priority.Priority;
                                                                  }

                                                                  return Int32.MaxValue;
                                                              })
                .ToList();

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Number of tasks to execute: {0}", Tasks.Count);
            }

            foreach (var task in Tasks)
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Executing task: {0}", task);
                }
                task.Execute(Container, Runtime);
            }
        }

        private void KernelComponentRegistered(string key, IHandler handler)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Component Registered: Key-{0}, Service-{1}, Implementation-{2}",
                key, handler.ComponentModel.Service, handler.ComponentModel.Implementation);
            }
        }

        #endregion Methods
    }
}