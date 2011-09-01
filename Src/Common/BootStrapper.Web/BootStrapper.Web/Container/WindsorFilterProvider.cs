namespace Avanade.BootStrapper.Web.Container
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    using Castle.Windsor;

    using NLog;

    /// <summary>
    /// This is a hackish approach to intercepting the filter attributes before it is used inside MVC.
    /// This hack is undertaken for 2 reasons:
    /// (1) Attributes are .NET in nature and as such, are directly created by the Framework;
    /// (2) Castle Windsor does not support injection of dependencies into an existing instance.
    /// </summary>
    public class WindsorFilterProvider : FilterAttributeFilterProvider
    {
        #region Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IWindsorContainer container;

        #endregion Fields

        #region Constructors

        public WindsorFilterProvider(IWindsorContainer container)
        {
            this.container = container;
        }

        #endregion Constructors

        #region Methods

        protected override IEnumerable<FilterAttribute> GetActionAttributes(
            ControllerContext controllerContext,
            ActionDescriptor actionDescriptor)
        {
            var attributes = base.GetActionAttributes(controllerContext, actionDescriptor);

            return AugmentAttributeList(attributes);
        }

        protected override IEnumerable<FilterAttribute> GetControllerAttributes(
            ControllerContext controllerContext,
            ActionDescriptor actionDescriptor)
        {
            var attributes = base.GetControllerAttributes(controllerContext, actionDescriptor);

            return AugmentAttributeList(attributes);
        }

        private IEnumerable<FilterAttribute> AugmentAttributeList(IEnumerable<FilterAttribute> attributes)
        {
            var augmentedList = new List<FilterAttribute>();
            foreach (var attribute in attributes)
            {
                Type type = attribute.GetType();
                var instance = container.GetService(type);

                if (instance == null)
                {
                    augmentedList.Add(attribute);
                }
                else
                {
                    if (Logger.IsDebugEnabled)
                    {
                        Logger.Debug("From the container: Type-{0}, Instance-{1}", type, instance);
                    }
                    augmentedList.Add((FilterAttribute)instance);
                }
            }
            return augmentedList;
        }

        #endregion Methods
    }
}