namespace Windows.Azure.Msbuild.AzureTools
{
    using System;

    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;

    [CoverageExclude(Reason.Framework)]
    public class AzureBlobClient : IAzureBlobClient
    {
        #region Fields

        private readonly CloudBlobClient blobClient;

        #endregion Fields

        #region Constructors

        public AzureBlobClient(CloudBlobClient client)
        {
            blobClient = client;
        }

        #endregion Constructors

        #region Methods

        public IAzureBlob GetBlobReference(string fileName)
        {
            var blob = blobClient.GetBlobReference(fileName);
            blob.UploadFile(fileName);
            return new AzureBlob(blob);
        }

        public IAzureBlobContainer GetContainerReference(string containerName)
        {
            var blobContainer = blobClient.GetContainerReference(containerName);
            return new AzureBlobContainer(blobContainer);
        }

        #endregion Methods
    }
}