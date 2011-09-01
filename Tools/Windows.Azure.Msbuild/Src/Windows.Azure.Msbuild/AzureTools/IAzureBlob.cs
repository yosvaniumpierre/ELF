namespace Windows.Azure.Msbuild.AzureTools
{
    using System.IO;

    public interface IAzureBlob
    {
        #region Methods

        bool DeleteIfExists();

        void UploadFile(string fileName);

        void UploadFromStream(Stream stream);

        #endregion Methods
    }
}