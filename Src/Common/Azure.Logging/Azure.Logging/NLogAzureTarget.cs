namespace Avanade.Azure.Logging
{
    using NLog;
    using NLog.Targets;

    [Target("AzureTarget")]
    public class NLogAzureTarget : TargetWithLayout
    {
        #region Constructors

        public NLogAzureTarget()
        {
            LogWriter = LogWriterFactory.CreateAzureWriter();
        }

        #endregion Constructors

        #region Properties

        protected LogWriter LogWriter
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        protected override void Write(LogEventInfo logEvent)
        {
            LogWriter.Log(logEvent);
        }

        #endregion Methods
    }
}