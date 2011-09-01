namespace Windows.Azure.Msbuild.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface ITaskLogger
    {
        #region Methods

        void LogMessage(string message, params object[] args);

        #endregion Methods
    }
}