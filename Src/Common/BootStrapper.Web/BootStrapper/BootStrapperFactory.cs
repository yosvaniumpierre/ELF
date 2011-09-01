namespace Avanade.BootStrapper
{
    using System;
    using System.Configuration;

    using NLog;

    using Web;
    using Web.Azure;

    public static class BootStrapperFactory
    {
        #region Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion Fields

        #region Methods

        public static IBootStrapper Create()
        {
            string bootStrapEnvironment = ConfigurationManager.AppSettings["BootStrapEnvironment"];
            Logger.Info("Value of the BootStrapEnvironment: " + bootStrapEnvironment);

            if (string.IsNullOrEmpty(bootStrapEnvironment))
            {
                return new WebBootStrapper();
            }

            if (bootStrapEnvironment.Equals("azure", StringComparison.OrdinalIgnoreCase))
            {
                return new AzureBootStrapper();
            }

            if (bootStrapEnvironment.Equals("web", StringComparison.OrdinalIgnoreCase) )
            {
                return new WebBootStrapper();
            }

            return new SureFireBootStrapper();
        }

        #endregion Methods
    }
}