namespace Windows.Azure.Msbuild
{
    public interface ITaskLogger
    {
        #region Methods

        void LogMessage(string message, params object[] args);

        #endregion Methods
    }
}