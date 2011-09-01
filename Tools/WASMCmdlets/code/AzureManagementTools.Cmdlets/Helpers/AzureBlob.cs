// ----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------------

namespace Microsoft.Samples.AzureManagementTools.PowerShell.Helpers
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.ServiceModel;
    using Microsoft.Samples.WindowsAzure.ServiceManagement;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;

    public static class AzureBlob
    {
        public static Uri UploadPackageToBlob(IServiceManagement channel, string storageName, string subscriptionId, string packagePath)
        {
            StorageService storageService = null;

            try
            {
                storageService = channel.GetStorageKeys(
                    subscriptionId,
                    storageName);
            }
            catch (CommunicationException)
            {
                throw;
            }

            string storageKey = storageService.StorageServiceKeys.Primary;

            return AzureBlob.UploadFile(storageName, storageKey, packagePath);
        }

        public static Uri UploadFile(string storageName, string storageKey, string filePath)
        {
            var baseAddress = string.Format(CultureInfo.InvariantCulture, ConfigurationConstants.BlobEndpointTemplate, storageName);
            var credentials = new StorageCredentialsAccountAndKey(storageName, storageKey);
            var client = new CloudBlobClient(baseAddress, credentials);
            
            string containerName = "mydeployments";
            string blobName = string.Format(
                CultureInfo.InvariantCulture,
                "{0}_{1}",
                DateTime.UtcNow.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture),
                Path.GetFileName(filePath));
            
            CloudBlobContainer container = client.GetContainerReference(containerName);
            container.CreateIfNotExist();
            CloudBlob blob = container.GetBlobReference(blobName);
            
            // blob.UploadFile(filePath);
            UploadBlobStream(blob, filePath);

            return new Uri(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}{1}{2}{3}",
                    client.BaseUri,
                    containerName,
                    client.DefaultDelimiter,
                    blobName));
        }

        private static void UploadBlobStream(CloudBlob blob, string sourceFile)
        {
            using (FileStream readStream = File.OpenRead(sourceFile))
            {
                byte[] buffer = new byte[1024 * 128];

                using (BlobStream blobStream = blob.OpenWrite())
                {
                    blobStream.BlockSize = 1024 * 128;

                    while (true)
                    {
                        int bytesCount = readStream.Read(buffer, 0, buffer.Length);

                        if (bytesCount <= 0)
                        {
                            break;
                        }

                        blobStream.Write(buffer, 0, bytesCount);
                    }
                }
            }
        }
    }
}
