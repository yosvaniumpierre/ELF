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
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Management.Automation;
    using Microsoft.Samples.AzureManagementTools.PowerShell.StorageServices;
    using Microsoft.Samples.AzureManagementTools.PowerShell.Tests.Helpers;
    using Microsoft.Samples.WindowsAzure.ServiceManagement;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class StorageServicesPipeliningFixture
    {
        [TestMethod]
        public void ShouldRetrieveStorageProperties()
        {
            //// 1. Retrieve all storage services
            //// 2. Search a specific service
            //// 3. List service properties

            var command = string.Format(
                CultureInfo.InvariantCulture,
                "Get-StorageServices {0} | where {{$_.ServiceName -eq \"{1}\"}} | Get-StorageProperties",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName);

            var cmdlets = new Dictionary<string, Type>()
            {
                { "Get-StorageServices", typeof(GetStorageServicesCommand) },
                { "Get-StorageProperties", typeof(GetStoragePropertiesCommand) }
            };

            Collection<PSObject> result = CmdletTestingHelper.RunCmdlet(cmdlets, command);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var storageProperties = (StorageServiceProperties)result[0].ImmediateBaseObject;

            Assert.IsTrue(!string.IsNullOrEmpty(storageProperties.Label));
        }

        [TestMethod]
        public void ShouldRetrieveStorageKeys()
        {
            //// 1. Retrieve all storage services
            //// 2. Search a specific service
            //// 3. Retrieve storage keys

            var command = string.Format(
                CultureInfo.InvariantCulture,
                "Get-StorageServices {0} | where {{$_.ServiceName -eq \"{1}\"}} | Get-StorageKeys",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName);

            var cmdlets = new Dictionary<string, Type>()
            {
                { "Get-StorageServices", typeof(GetStorageServicesCommand) },
                { "Get-StorageKeys", typeof(GetStorageKeysCommand) }
            };

            Collection<PSObject> result = CmdletTestingHelper.RunCmdlet(cmdlets, command);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var storageKeys = (StorageServiceKeys)result[0].ImmediateBaseObject;

            Assert.IsTrue(!string.IsNullOrEmpty(storageKeys.Primary));
            Assert.IsTrue(!string.IsNullOrEmpty(storageKeys.Secondary));
        }

        [TestMethod, Ignore]
        public void ShouldRegenerateStorageKey()
        {
            //// 1. Retrieve all storage services
            //// 2. Search a specific service
            //// 3. Regenerate secondary storage key

            var command = string.Format(
                CultureInfo.InvariantCulture,
                "Get-StorageServices {0} | where {{$_.ServiceName -eq \"{1}\"}} | New-StorageKey {2}",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName,
                "Secondary");

            var cmdlets = new Dictionary<string, Type>()
            {
                { "Get-StorageServices", typeof(GetStorageServicesCommand) },
                { "New-StorageKey", typeof(NewStorageKeyCommand) }
            };

            Collection<PSObject> result = CmdletTestingHelper.RunCmdlet(cmdlets, command);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var storageKeys = (StorageServiceKeys)result[0].ImmediateBaseObject;

            Assert.IsTrue(!string.IsNullOrEmpty(storageKeys.Primary));
            Assert.IsTrue(!string.IsNullOrEmpty(storageKeys.Secondary));
        }
    }
}