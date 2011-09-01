namespace Windows.Azure.Msbuild
{
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    public class LoggingHelperWrapper : ITaskLogger
    {
        #region Fields

        private TaskLoggingHelper logger;

        #endregion Fields

        #region Constructors

        public LoggingHelperWrapper(ITask taskInstance)
        {
            logger = new TaskLoggingHelper(taskInstance);
        }

        #endregion Constructors

        #region Methods

        public void LogMessage(string message, params object[] args)
        {
            logger.LogMessage(message, args);
        }

        #endregion Methods
    }
}