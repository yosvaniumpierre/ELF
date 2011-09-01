namespace Avanade.Azure.Logging
{
    using NLog.Targets;

    [Target("EmulatorTarget")]
    public class NLogEmulatorTarget : NLogAzureTarget
    {
        #region Constructors

        public NLogEmulatorTarget()
        {
            LogWriter = LogWriterFactory.CreateDevelopmentWriter();
        }

        #endregion Constructors
    }
}