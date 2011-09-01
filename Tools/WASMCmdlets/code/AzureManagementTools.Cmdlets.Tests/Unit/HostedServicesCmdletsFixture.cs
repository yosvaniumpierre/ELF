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
    using Microsoft.Samples.AzureManagementTools.PowerShell.HostedServices;
    using Microsoft.Samples.WindowsAzure.ServiceManagement;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class HostedServicesCmdletsFixture
    {
        [TestMethod]
        public void GetHostedServices()
        {
            var testHostedServiceList = new HostedServiceList();
            testHostedServiceList.Capacity = 0;

            var mockChannel = new Mock<IServiceManagement>();
            mockChannel.Setup(m => m.BeginListHostedServices(TestConstants.SubscriptionId, It.IsAny<AsyncCallback>(), It.IsAny<object>()))
                       .Verifiable();
            mockChannel.Setup(m => m.EndListHostedServices(It.IsAny<IAsyncResult>()))
                       .Returns(testHostedServiceList)
                       .Verifiable();

            var cmdlet = new GetHostedServicesCommand(mockChannel.Object)
            {
                Certificate = TestConstants.Certificate,
                SubscriptionId = TestConstants.SubscriptionId
            };

            var hostedServiceList = cmdlet.GetHostedServicesProcess();

            mockChannel.Verify();
            Assert.IsNotNull(hostedServiceList);
        }

        [TestMethod]
        public void GetHostedService()
        {
            var testHostedService = new HostedService
            {
                ServiceName = TestConstants.HostedServiceName
            };

            var mockChannel = new Mock<IServiceManagement>();
            mockChannel.Setup(m => m.BeginGetHostedService(TestConstants.SubscriptionId, TestConstants.HostedServiceName, It.IsAny<AsyncCallback>(), It.IsAny<object>()))
                       .Verifiable();
            mockChannel.Setup(m => m.EndGetHostedService(It.IsAny<IAsyncResult>()))
                       .Returns(testHostedService)
                       .Verifiable();

            var cmdlet = new GetHostedServiceCommand(mockChannel.Object)
            {
                Certificate = TestConstants.Certificate,
                SubscriptionId = TestConstants.SubscriptionId,
                ServiceName = TestConstants.HostedServiceName
            };

            var hostedService = cmdlet.GetHostedServiceProcess();

            mockChannel.Verify();
            Assert.IsNotNull(hostedService);
            Assert.AreEqual<string>(testHostedService.ServiceName, hostedService.ServiceName);
        }

        [TestMethod]
        public void GetHostedProperties()
        {
            var testHostedServiceProperties = new HostedServiceProperties
            {
                Label = TestConstants.StagingLabel
            };

            var mockChannel = new Mock<IServiceManagement>();
            mockChannel.Setup(m => m.BeginGetHostedService(TestConstants.SubscriptionId, TestConstants.HostedServiceName, It.IsAny<AsyncCallback>(), It.IsAny<object>()))
                       .Verifiable();
            mockChannel.Setup(m => m.EndGetHostedService(It.IsAny<IAsyncResult>()))
                       .Returns(new HostedService() { HostedServiceProperties = testHostedServiceProperties })
                       .Verifiable();

            var cmdlet = new GetHostedPropertiesCommand(mockChannel.Object)
            {
                Certificate = TestConstants.Certificate,
                SubscriptionId = TestConstants.SubscriptionId,
                ServiceName = TestConstants.HostedServiceName
            };

            var hostedProperties = cmdlet.GetHostedPropertiesProcess();

            mockChannel.Verify();
            Assert.IsNotNull(hostedProperties);
            Assert.AreEqual<string>(testHostedServiceProperties.Label, hostedProperties.Label);
        }

        [TestMethod]
        public void GetDeployment()
        {
            var testSlot = "Staging";
            var testDeployment = new Deployment
            {
                DeploymentSlot = testSlot
            };

            var mockChannel = new Mock<IServiceManagement>();
            mockChannel.Setup(m => m.BeginGetDeploymentBySlot(TestConstants.SubscriptionId, TestConstants.HostedServiceName, testSlot, It.IsAny<AsyncCallback>(), It.IsAny<object>()))
                       .Verifiable();
            mockChannel.Setup(m => m.EndGetDeploymentBySlot(It.IsAny<IAsyncResult>()))
                       .Returns(testDeployment)
                       .Verifiable();

            var cmdlet = new GetDeploymentCommand(mockChannel.Object)
            {
                Certificate = TestConstants.Certificate,
                SubscriptionId = TestConstants.SubscriptionId,
                ServiceName = TestConstants.HostedServiceName,
                Slot = testSlot
            };

            var deployment = cmdlet.GetDeploymentProcess();
            
            mockChannel.Verify();
            Assert.IsNotNull(deployment);
            Assert.AreEqual<string>(testDeployment.Label, deployment.Label);
        }
    }
}
