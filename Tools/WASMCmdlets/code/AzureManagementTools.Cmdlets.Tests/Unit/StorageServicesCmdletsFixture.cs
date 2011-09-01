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

namespace Microsoft.Samples.AzureManagementTools.PowerShell.Tests.Unit
{
    using System;
    using Microsoft.Samples.AzureManagementTools.PowerShell.StorageServices;
    using Microsoft.Samples.WindowsAzure.ServiceManagement;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class StorageServicesCmdletsFixture
    {
        [TestMethod]
        public void GetStorageServices()
        {
            var testStorageServiceList = new StorageServiceList();
            testStorageServiceList.Capacity = 0;
            
            var mockChannel = new Mock<IServiceManagement>();
            mockChannel.Setup(m => m.BeginListStorageServices(TestConstants.SubscriptionId, It.IsAny<AsyncCallback>(), It.IsAny<object>()))
                       .Verifiable();
            mockChannel.Setup(m => m.EndListStorageServices(It.IsAny<IAsyncResult>()))
                       .Returns(testStorageServiceList)
                       .Verifiable();

            var cmdlet = new GetStorageServicesCommand(mockChannel.Object)
            {
                Certificate = TestConstants.Certificate,
                SubscriptionId = TestConstants.SubscriptionId
            };

            var storageServiceList = cmdlet.GetStorageServicesProcess();

            mockChannel.Verify();
            Assert.IsNotNull(storageServiceList);
        }

        [TestMethod]
        public void GetStorageKeys()
        {
            var testStorageServiceKeys = new StorageServiceKeys
            {
                Primary = "testPromaryKey",
                Secondary = "testSecondaryKey"
            };

            var mockChannel = new Mock<IServiceManagement>();
            mockChannel.Setup(m => m.BeginGetStorageKeys(TestConstants.SubscriptionId, TestConstants.StorageServiceName, It.IsAny<AsyncCallback>(), It.IsAny<object>()))
                       .Verifiable();
            mockChannel.Setup(m => m.EndGetStorageKeys(It.IsAny<IAsyncResult>()))
                       .Returns(new StorageService() { StorageServiceKeys = testStorageServiceKeys })
                       .Verifiable();

            var cmdlet = new GetStorageKeysCommand(mockChannel.Object)
            {
                Certificate = TestConstants.Certificate,
                SubscriptionId = TestConstants.SubscriptionId,
                ServiceName = TestConstants.StorageServiceName
            };

            var storageKeys = cmdlet.GetStorageKeysProcess();

            mockChannel.Verify();
            Assert.IsNotNull(storageKeys);
            Assert.AreEqual<string>(testStorageServiceKeys.Primary, storageKeys.Primary);
            Assert.AreEqual<string>(testStorageServiceKeys.Secondary, storageKeys.Secondary);
        }

        [TestMethod]
        public void GetStorageProperties()
        {
            var testStorageServiceProperties = new StorageServiceProperties
            {
                Label = TestConstants.StagingLabel
            };

            var mockChannel = new Mock<IServiceManagement>();
            mockChannel.Setup(m => m.BeginGetStorageService(TestConstants.SubscriptionId, TestConstants.StorageServiceName, It.IsAny<AsyncCallback>(), It.IsAny<object>()))
                       .Verifiable();
            mockChannel.Setup(m => m.EndGetStorageService(It.IsAny<IAsyncResult>()))
                       .Returns(new StorageService() { StorageServiceProperties = testStorageServiceProperties })
                       .Verifiable();

            var cmdlet = new GetStoragePropertiesCommand(mockChannel.Object)
            {
                Certificate = TestConstants.Certificate,
                SubscriptionId = TestConstants.SubscriptionId,
                ServiceName = TestConstants.StorageServiceName
            };

            var storageProperties = cmdlet.GetStoragePropertiesProcess();

            mockChannel.Verify();
            Assert.IsNotNull(storageProperties);
            Assert.AreEqual<string>(storageProperties.Label, storageProperties.Label);
        }

        [TestMethod]
        public void RegenerateStorageKeys()
        {
            var testStorageServiceKeys = new StorageServiceKeys
            {
                Primary = "testPromaryKey",
                Secondary = "newSecondaryKey"
            };
            
            var mockChannel = new Mock<IServiceManagement>();
            mockChannel.Setup(m => m.BeginRegenerateStorageServiceKeys(TestConstants.SubscriptionId, TestConstants.StorageServiceName, It.IsAny<RegenerateKeys>(), It.IsAny<AsyncCallback>(), It.IsAny<object>()))
                       .Verifiable();
            mockChannel.Setup(m => m.EndRegenerateStorageServiceKeys(It.IsAny<IAsyncResult>()))
                       .Returns(new StorageService() { StorageServiceKeys = testStorageServiceKeys })
                       .Verifiable();

            var cmdlet = new NewStorageKeyCommand(mockChannel.Object)
            {
                Certificate = TestConstants.Certificate,
                SubscriptionId = TestConstants.SubscriptionId,
                ServiceName = TestConstants.StorageServiceName,
                KeyType = "Secondary"
            };

            var regeneratedStorageKeys = cmdlet.NewStorageKeyProcess();

            Assert.IsNotNull(regeneratedStorageKeys);
            Assert.AreEqual<string>(testStorageServiceKeys.Primary, regeneratedStorageKeys.Primary);
            Assert.AreEqual<string>(testStorageServiceKeys.Secondary, regeneratedStorageKeys.Secondary);
        }
    }
}