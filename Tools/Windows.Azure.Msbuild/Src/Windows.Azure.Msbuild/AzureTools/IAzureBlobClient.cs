namespace Windows.Azure.Msbuild.AzureTools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IAzureBlobClient
    {
        #region Methods

        IAzureBlob GetBlobReference(string fileName);

        IAzureBlobContainer GetContainerReference(string containerName);

        #endregion Methods
    }
}