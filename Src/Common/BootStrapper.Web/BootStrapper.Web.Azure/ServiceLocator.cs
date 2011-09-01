namespace Avanade.BootStrapper.Web.Azure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Castle.Core;

    using NLog;

    public class ServiceLocator
    {
        #region Fields

        private static readonly IList<ComponentModelWrapper> ComponentList = new List<ComponentModelWrapper>();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion Fields

        #region Methods

        public Type GetControllerType(string controllerName)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Finding a controller that matches the name '{0}'.", controllerName);
            }
            foreach (var component in ComponentList)
            {
                var controllerType = component.Key.Split('.').LastOrDefault();
                if (string.IsNullOrEmpty(controllerType))
                {
                    continue;
                }

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Isolated the controller type: {0}", controllerType);
                }

                var controllerShortName =
                    controllerType.Split(new[] {"Controller"}, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

                if (string.IsNullOrEmpty(controllerShortName))
                {
                    continue;
                }

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Name of the controller isolated: {0}", controllerShortName);
                }

                if (controllerShortName.Equals(controllerName))
                {
                    Type type = component.ComponentModel.Implementation;

                    if (Logger.IsDebugEnabled)
                    {
                        Logger.Debug("Type match found for ControllerName ('{0}'): {1}", controllerName, type);
                    }

                    return type;
                }
            }
            return null;
        }

        internal void Add(string key, ComponentModel model)
        {
            Logger.Info("Component Registered: Key-{0}, Service-{1}, Implementation-{2}",
                key, model.Service, model.Implementation);
            ComponentList.Add(new ComponentModelWrapper(key, model));
        }

        internal bool ExistsImplementation(Type implementationType, out ComponentModelWrapper componentModel)
        {
            foreach (var componentModelWrapper in ComponentList)
            {
                bool exists = componentModelWrapper.ExistsImplementation(implementationType);
                if (exists)
                {
                    componentModel = componentModelWrapper;
                    return true;
                }
            }
            componentModel = null;
            return false;
        }

        #endregion Methods
    }
}