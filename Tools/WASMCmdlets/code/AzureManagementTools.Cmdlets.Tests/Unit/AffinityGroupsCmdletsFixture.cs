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
    using Microsoft.Samples.AzureManagementTools.PowerShell.AffinityGroups;
    using Microsoft.Samples.WindowsAzure.ServiceManagement;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class AffinityGroupsCmdletsFixture
    {
        [TestMethod]
        public void GetAffinityGroups()
        {
            var testAffinityGroupList = new AffinityGroupList();
            testAffinityGroupList.Capacity = 0;

            var mockChannel = new Mock<IServiceManagement>();
            mockChannel.Setup(m => m.BeginListAffinityGroups(TestConstants.SubscriptionId, It.IsAny<AsyncCallback>(), It.IsAny<object>()))
                       .Verifiable();
            mockChannel.Setup(m => m.EndListAffinityGroups(It.IsAny<IAsyncResult>()))
                       .Returns(testAffinityGroupList)
                       .Verifiable();

            var cmdlet = new GetAffinityGroupsCommand(mockChannel.Object)
            {
                Certificate = TestConstants.Certificate,
                SubscriptionId = TestConstants.SubscriptionId
            };

            var affinityGroupList = cmdlet.GetAffinityGroupsProcess();

            mockChannel.Verify();
            Assert.IsNotNull(affinityGroupList);
        }

        [TestMethod]
        public void GetAffinityGroupProperties()
        {
            var testAffinityGroupProperties = new AffinityGroup
            {
                Name = "testAffinityGroup"
            };

            var mockChannel = new Mock<IServiceManagement>();
            mockChannel.Setup(m => m.BeginGetAffinityGroup(TestConstants.SubscriptionId, TestConstants.AffinityGroupName, It.IsAny<AsyncCallback>(), It.IsAny<object>()))
                       .Verifiable();
            mockChannel.Setup(m => m.EndGetAffinityGroup(It.IsAny<IAsyncResult>()))
                       .Returns(testAffinityGroupProperties)
                       .Verifiable();

            var cmdlet = new GetAffinityGroupCommand(mockChannel.Object)
            {
                Certificate = TestConstants.Certificate,
                SubscriptionId = TestConstants.SubscriptionId, 
                Name = TestConstants.AffinityGroupName
            };

            var affinityGroupProperties = cmdlet.GetAffinityGroupProcess();

            mockChannel.Verify();
            Assert.IsNotNull(affinityGroupProperties);
            Assert.AreEqual<string>(testAffinityGroupProperties.Name, affinityGroupProperties.Name);
        }
    }
}
