namespace Windows.Azure.Msbuild.AzureTools
{
    using System;

    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;

    [CoverageExclude(Reason.Framework)]
    public class DevBlobClient : IAzureBlobClient
    {
        #region Fields

        private readonly CloudBlobClient blobClient;

        #endregion Fields

        #region Constructors

        public DevBlobClient(int timeoutInMinutes = 30, int parallelOperationThreadCount = 1)
        {
            var cloudStorageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            blobClient = cloudStorageAccount.CreateCloudBlobClient();
            blobClient.Timeout = new TimeSpan(0, timeoutInMinutes, 0);
            blobClient.ParallelOperationThreadCount = parallelOperationThreadCount;
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