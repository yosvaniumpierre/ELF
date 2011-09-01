namespace Avanade.Azure.Logging
{
    using Microsoft.WindowsAzure.StorageClient;

    public class LogEntry : TableServiceEntity
    {
        #region Constructors

        public LogEntry()
        {
        }

        public LogEntry(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {
        }

        #endregion Constructors

        #region Properties

        public string ExceptionMessage
        {
            get; set;
        }

        public string Level
        {
            get; set;
        }

        public string Logger
        {
            get; set;
        }

        public string Message
        {
            get; set;
        }

        public string ExceptionStackTrace
        {
            get; set;
        }

        public string InnerExceptionMessage
        {
            get;
            set;
        }

        public string InnerExceptionStackTrace
        {
            get;
            set;
        }

        #endregion Properties
    }
}