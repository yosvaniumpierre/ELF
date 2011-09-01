namespace Avanade.Platform.Services.Web
{
    using System.IO;

    public class LocalFileStore : IFileStore
    {
        #region Methods

        public byte[] Read(string filePath)
        {
            return File.ReadAllBytes(filePath);
        }

        public void Save(string filePath, byte[] content)
        {
            File.WriteAllBytes(filePath, content);
        }

        #endregion Methods
    }
}