namespace Windows.Azure.Msbuild.AzureTools
{
    public interface IAzureBlobContainer
    {
        #region Methods

        void Cleanup();

        bool CreateIfNotExists();

        IAzureBlob GetBlobReference(string fileName);

        IAzureBlob GetBlockBlobReference(string fileName);

        #endregion Methods
    }
}