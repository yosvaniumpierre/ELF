namespace Avanade.BootStrapper.Web.Container.Ext.App_Start
{
    using System;
    using System.Web.Mvc;
    using System.Web.WebPages;

    using NLog;

    using RazorGenerator.Mvc;

    public static class PreApplicationStartCode
    {
        #region Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static bool startMethodExecuted;

        #endregion Fields

        #region Methods

        public static void Start()
        {
            if (startMethodExecuted)
            {
                return;
            }

            try
            {
                var engine = new PrecompiledMvcEngine(typeof(PreApplicationStartCode).Assembly);

                ViewEngines.Engines.Insert(0, engine);

                // StartPage lookups are done by WebPages.
                VirtualPathFactoryManager.RegisterVirtualPathFactory(engine);

                startMethodExecuted = true;

                Logger.Info("Executed the PreApplicationStart action!");
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Unexpected exception while Start method is invoked!", exception);
            }
        }

        #endregion Methods
    }
}