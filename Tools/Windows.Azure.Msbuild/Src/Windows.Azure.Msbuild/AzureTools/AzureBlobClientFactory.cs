namespace Windows.Azure.Msbuild.AzureTools
{
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;

    [CoverageExclude(Reason.Humble)]
    public class AzureBlobClientFactory : IAzureBlobClientFactory
    {
        #region Methods

        public IAzureBlobClient Create(CloudBlobClient blobClient)
        {
            return new AzureBlobClient(blobClient);
        }

        #endregion Methods
    }
}