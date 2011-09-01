namespace Windows.Azure.Msbuild.AzureTools
{
    using Microsoft.WindowsAzure.StorageClient;

    public interface IAzureBlobClientFactory
    {
        #region Methods

        IAzureBlobClient Create(CloudBlobClient blobClient);

        #endregion Methods
    }
}