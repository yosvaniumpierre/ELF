namespace Avanade.BootStrapper.Web.Azure.Processor
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Castle.Windsor;
    using Castle.Windsor.Installer;

    using NLog;

    using WebActivator;

    internal static class AssemblyExtensions
    {
        #region Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion Fields

        #region Methods

        public static IEnumerable<T> GetActivationAttributes<T>(this Assembly assembly)
            where T : BaseActivationMethodAttribute
        {
            const bool inherit = false;
            List<T> attributes = assembly.GetCustomAttributes(typeof(T), inherit).OfType<T>().ToList();

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Number of attributes discovered: {0}", attributes.Count());
                attributes.ForEach(x => Logger.Debug("Assembly attribute discovered: {0}", x));
            }

            return attributes;
        }

        #endregion Methods
    }

    internal class ExtensionProcessor : BaseAssemblyItemProcessor
    {
        #region Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IWindsorContainer container;

        #endregion Fields

        #region Constructors

        internal ExtensionProcessor(IWindsorContainer container)
        {
            this.container = container;
        }

        #endregion Constructors

        #region Methods

        public override bool Process(AssemblyItem assemblyItem)
        {
            if (IsMatch(assemblyItem.Name, BootStrapRuntime.ExtensionMask))
            {
                var assembly = Assembly.Load(assemblyItem.Bytes);
                container.Install(FromAssembly.Instance(assembly));

                RunActivationMethods<PreApplicationStartMethodAttribute>(assembly);

                return true;
            }
            return false;
        }

        private static void RunActivationMethods<T>(Assembly assembly)
            where T : BaseActivationMethodAttribute
        {
            var attributes = assembly.GetActivationAttributes<T>();
            if (attributes == null)
            {
                return;
            }
            foreach (T baseActivationMethodAttribute in attributes)
            {
                try
                {
                    baseActivationMethodAttribute.InvokeMethod();
                }
                catch (TargetInvocationException ex)
                {
                    Logger.ErrorException("Unknown exception while invoking the WebActivator assembly attribute!", ex);
                }
            }
        }

        #endregion Methods
    }
}