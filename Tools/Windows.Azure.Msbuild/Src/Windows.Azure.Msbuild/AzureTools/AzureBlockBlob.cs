namespace Windows.Azure.Msbuild.AzureTools
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Microsoft.WindowsAzure.StorageClient;

    public class AzureBlockBlob : IAzureBlob
    {
        #region Fields

        private readonly CloudBlockBlob blob;

        #endregion Fields

        #region Constructors

        public AzureBlockBlob(CloudBlockBlob blob)
        {
            this.blob = blob;
        }

        #endregion Constructors

        #region Methods

        public bool DeleteIfExists()
        {
            return blob.DeleteIfExists();
        }

        public void UploadFile(string fileName)
        {
            blob.UploadFile(fileName);
        }

        public void UploadFromStream(Stream stream)
        {
            blob.UploadFromStream(stream);
        }

        #endregion Methods
    }
}