namespace Avanade.BootStrapper.Web.Task
{
    using System.IO;

    public sealed class EmbeddedViewResource
    {
        #region Constructors

        public EmbeddedViewResource(string viewsFolder, string fileName, string manifestResourceName)
        {
            ResourceFolder = viewsFolder;
            FileName = fileName;
            ManifestResourceName = manifestResourceName;
        }

        #endregion Constructors

        #region Properties

        public string FileName
        {
            get; private set;
        }

        public string ManifestResourceName
        {
            get; private set;
        }

        public byte[] ResourceBytes
        {
            get; set;
        }

        public string ResourceFolder
        {
            get; private set;
        }

        public string ResourcePath
        {
            get { return Path.Combine(ResourceFolder, FileName); }
        }

        #endregion Properties

        #region Methods

        public string GetFileFolder(BootStrapRuntime runtime, string rootFolder)
        {
            return Path.Combine(runtime.BinDirectory, string.Format("{0}/{1}", rootFolder, ResourceFolder));
        }

        public string GetFilePath(BootStrapRuntime runtime, string rootFolder)
        {
            return Path.Combine(GetFileFolder(runtime, rootFolder), FileName);
        }

        public override string ToString()
        {
            return string.Format("ResourceFolder: {0}, FileName: {1}, ManifestResourceName: {2}",
                ResourceFolder, FileName, ManifestResourceName);
        }

        #endregion Methods
    }
}