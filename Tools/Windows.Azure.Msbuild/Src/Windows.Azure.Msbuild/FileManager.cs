namespace Windows.Azure.Msbuild
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    [CoverageExclude(Reason.Humble)]
    public class FileManager : IFileManager
    {
        #region Methods

        public Stream GetFile(string pathToFile)
        {
            return new FileStream(pathToFile, FileMode.Open, FileAccess.Read);
        }

        #endregion Methods
    }
}