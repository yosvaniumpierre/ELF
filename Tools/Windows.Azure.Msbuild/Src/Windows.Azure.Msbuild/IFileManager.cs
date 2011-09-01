namespace Windows.Azure.Msbuild
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public interface IFileManager
    {
        #region Methods

        Stream GetFile(string pathToFile);

        #endregion Methods
    }
}