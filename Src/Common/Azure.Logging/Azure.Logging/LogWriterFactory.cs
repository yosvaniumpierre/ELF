namespace Avanade.Azure.Logging
{
    using System.Configuration;

    internal static class LogWriterFactory
    {
        #region Methods

        internal static LogWriter CreateAzureWriter()
        {
            const string connectionStringKey = "LogStorage";
            var connectionStringCollection = ConfigurationManager.ConnectionStrings[connectionStringKey];

            if (connectionStringCollection != null)
            {
                string storageConnectionString = connectionStringCollection.ConnectionString;
                if (!string.IsNullOrEmpty(storageConnectionString))
                {
                    var createdTableHelper = TableHelper.UseConnectionString(storageConnectionString);
                    if (createdTableHelper != null)
                    {
                        return new LogWriter(createdTableHelper);
                    }
                }
            }
            return null;
        }

        internal static LogWriter CreateDevelopmentWriter()
        {
            var createdTableHelper = TableHelper.UseDevelopmentStorageAccount();
            if (createdTableHelper != null)
            {
                return new LogWriter(createdTableHelper);
            }
            return null;
        }

        #endregion Methods
    }
}