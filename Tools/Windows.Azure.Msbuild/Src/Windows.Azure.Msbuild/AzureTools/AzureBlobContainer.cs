namespace Windows.Azure.Msbuild.AzureTools
{
    using Microsoft.WindowsAzure.StorageClient;

    [CoverageExclude(Reason.Delegate)]
    public class AzureBlobContainer : IAzureBlobContainer
    {
        #region Fields

        private readonly CloudBlobContainer container;

        #endregion Fields

        #region Constructors

        public AzureBlobContainer(CloudBlobContainer container)
        {
            this.container = container;
        }

        #endregion Constructors

        #region Methods

        public void Cleanup()
        {
            //Indicate that any snapshots should be deleted.
            var options = new BlobRequestOptions
                              {
                                  DeleteSnapshotsOption = DeleteSnapshotsOption.IncludeSnapshots,
                                  UseFlatBlobListing = true
                              };

            //Specify a flat blob listing, so that only CloudBlob objects will be returned.
            //The Delete method exists only on CloudBlob, not on IListBlobItem.

            //Enumerate through the blobs in the container, deleting both blobs and their snapshots.
            foreach (CloudBlob blob in container.ListBlobs(options))
            {
                blob.Delete(options);
            }
        }

        public bool CreateIfNotExists()
        {
            return container.CreateIfNotExist();
        }

        public void Delete()
        {
            container.Delete();
        }

        public IAzureBlob GetBlobReference(string fileName)
        {
            var blob = container.GetBlobReference(fileName);
            return new AzureBlob(blob);
        }

        public IAzureBlob GetBlockBlobReference(string fileName)
        {
            var blob = container.GetBlockBlobReference(fileName);
            return new AzureBlockBlob(blob);
        }

        #endregion Methods
    }
}