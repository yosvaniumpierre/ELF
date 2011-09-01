namespace Windows.Azure.Msbuild.AzureTools
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Microsoft.WindowsAzure.StorageClient;

    [CoverageExclude(Reason.Delegate)]
    public class AzureBlob : IAzureBlob
    {
        #region Fields

        private readonly CloudBlob cloudBlob;

        #endregion Fields

        #region Constructors

        public AzureBlob(CloudBlob cloudBlob)
        {
            this.cloudBlob = cloudBlob;
        }

        #endregion Constructors

        #region Methods

        public bool DeleteIfExists()
        {
            return cloudBlob.DeleteIfExists();
        }

        public void UploadFile(string fileName)
        {
            cloudBlob.UploadFile(fileName);
        }

        public void UploadFromStream(Stream stream)
        {
            cloudBlob.UploadFromStream(stream);
        }

        #endregion Methods
    }
}