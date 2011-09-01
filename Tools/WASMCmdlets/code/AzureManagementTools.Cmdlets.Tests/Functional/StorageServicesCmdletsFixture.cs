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

namespace Microsoft.Samples.AzureManagementTools.PowerShell.Tests.Functional
{
    using System.Globalization;
    using Microsoft.Samples.AzureManagementTools.PowerShell.StorageServices;
    using Microsoft.Samples.AzureManagementTools.PowerShell.Tests.Helpers;
    using Microsoft.Samples.WindowsAzure.ServiceManagement;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class StorageServicesCmdletsFixture
    {
        [TestMethod]
        public void GetStorageServices()
        {
            var cmdletName = "Get-StorageServices";
            var result = CmdletTestingHelper.RunCmdlet(cmdletName, TestConstants.CommonParameters, typeof(GetStorageServicesCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
            Assert.IsInstanceOfType(result[0].ImmediateBaseObject, typeof(ManagementOperationContext));
        }

        [TestMethod]
        public void GetStorageProperties()
        {
            var cmdletName = "Get-StorageProperties";
            var cmdletParams = string.Format(CultureInfo.InvariantCulture, "{0} -serviceName \"{1}\"", TestConstants.CommonParameters, TestConstants.StorageServiceName);
            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(GetStoragePropertiesCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var storageProperties = (StorageServiceProperties)result[0].ImmediateBaseObject;

            Assert.IsTrue(!string.IsNullOrEmpty(storageProperties.Label));
        }

        [TestMethod]
        public void GetStorageKeys()
        {
            var cmdletName = "Get-StorageKeys";
            var cmdletParams = string.Format(CultureInfo.InvariantCulture, "{0} -serviceName \"{1}\"", TestConstants.CommonParameters, TestConstants.StorageServiceName);
            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(GetStorageKeysCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var storageKeys = (StorageServiceKeys)result[0].ImmediateBaseObject;

            Assert.IsTrue(!string.IsNullOrEmpty(storageKeys.Primary));
            Assert.IsTrue(!string.IsNullOrEmpty(storageKeys.Secondary));
        }

        [TestMethod]
        public void RegenerateStorageKeys()
        {
            var cmdletName = "Get-StorageKeys";
            var cmdletParams = string.Format(CultureInfo.InvariantCulture, "{0} -serviceName \"{1}\"", TestConstants.CommonParameters, TestConstants.StorageServiceName);
            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(GetStorageKeysCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var originalStorageKeys = (StorageServiceKeys)result[0].ImmediateBaseObject;

            cmdletName = "New-StorageKey";
            cmdletParams = string.Format(CultureInfo.InvariantCulture, "{0} -serviceName \"{1}\" -keyType \"{2}\"", TestConstants.CommonParameters, TestConstants.StorageServiceName, "Secondary");
            result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(NewStorageKeyCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var regeneratedStorageKeys = (StorageServiceKeys)result[0].ImmediateBaseObject;

            Assert.IsNotNull(regeneratedStorageKeys);
            Assert.AreEqual<string>(originalStorageKeys.Primary, regeneratedStorageKeys.Primary);
            Assert.AreNotEqual<string>(originalStorageKeys.Secondary, regeneratedStorageKeys.Secondary);
        }
    }
}
