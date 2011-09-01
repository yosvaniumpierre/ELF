namespace Avanade.Azure.Logging
{
    using System;

    using NLog;

    public class LogWriter
    {
        #region Fields

        protected TableHelper TableHelper;

        private const string TableName = "AzureNLogTableStorage";

        private readonly string partitionKey;

        #endregion Fields

        #region Constructors

        internal LogWriter(TableHelper instance)
        {
            TableHelper = instance;
            TableHelper.CreateTable(TableName);
            partitionKey = DateTime.Today.DayOfWeek.ToString();
        }

        #endregion Constructors

        #region Methods

        public void Log(LogEventInfo logEvent)
        {
            string timeStamp = FormatTimeStamp(logEvent.TimeStamp);
            string loggerName = logEvent.LoggerName;

            var exceptionStackTrace = String.Empty;
            var exceptionMessage = String.Empty;
            var innerExceptionMessage = string.Empty;
            var innerExceptionStackTrace = string.Empty;

            var exception = logEvent.Exception;
            if (exception != null)
            {
                exceptionStackTrace = exception.StackTrace;
                exceptionMessage = exception.Message;
            }

            if (exception != null && exception.InnerException != null)
            {
                innerExceptionMessage = exception.InnerException.Message;
                innerExceptionStackTrace = exception.InnerException.StackTrace;
            }

            var entity = new LogEntry(partitionKey, timeStamp) 
            { 
                Message = logEvent.FormattedMessage, 
                Level = logEvent.Level.Name, 
                Logger = loggerName, 
                ExceptionMessage = exceptionMessage, 
                ExceptionStackTrace = exceptionStackTrace,
                InnerExceptionMessage = innerExceptionMessage,
                InnerExceptionStackTrace = innerExceptionStackTrace
            };

            TableHelper.InsertEntity(TableName, entity);
        }

        internal static string FormatTimeStamp(DateTime timeStamp)
        {
            return String.Format("{0:yyyy-MM-dd HH:mm:ss.ffffZ}", timeStamp);
        }

        /// <summary>
        /// Do consider the possibility (regardless of how remote it can be) that the timeStamp will be the same for 2 events on separate processes.
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <param name="logLevel"></param>
        /// <param name="message"></param>
        /// <param name="loggerName"></param>
        /// <param name="stackTrace"></param>
        internal void Log(string timeStamp, string logLevel, string message, string loggerName, string stackTrace)
        {
        }

        #endregion Methods
    }
}