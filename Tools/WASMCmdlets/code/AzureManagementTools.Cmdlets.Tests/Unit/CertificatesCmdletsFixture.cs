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
    using Microsoft.Samples.AzureManagementTools.PowerShell.Certificates;
    using Microsoft.Samples.WindowsAzure.ServiceManagement;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class CertificatesCmdletsFixture
    {
        [TestMethod]
        public void GetCertificate()
        {
            var testThumbprintAlgorithm = "sha1";
            var testThumbprint = "3D6E34B526723E06C235BE8E5547784BF12C9F39";

            var mockChannel = new Mock<IServiceManagement>();
            mockChannel.Setup(m => m.BeginGetCertificate(TestConstants.SubscriptionId, TestConstants.HostedServiceName, testThumbprintAlgorithm, testThumbprint, It.IsAny<AsyncCallback>(), It.IsAny<object>()))
                       .Verifiable();
            mockChannel.Setup(m => m.EndGetCertificate(It.IsAny<IAsyncResult>()))
                       .Returns(new Certificate())
                       .Verifiable();

            var cmdlet = new GetCertificateCommand(mockChannel.Object)
            {
                Certificate = TestConstants.Certificate,
                SubscriptionId = TestConstants.SubscriptionId,
                ServiceName = TestConstants.HostedServiceName,
                ThumbprintAlgorithm = testThumbprintAlgorithm,
                Thumbprint = testThumbprint
            };

            var certificate = cmdlet.GetCertificateProcess();

            mockChannel.Verify();
            Assert.IsNotNull(certificate);
        }

        [TestMethod]
        public void GetCertificates()
        {
            var mockChannel = new Mock<IServiceManagement>();
            mockChannel.Setup(m => m.EndListCertificates(It.IsAny<IAsyncResult>()))
                       .Returns(new CertificateList())
                       .Verifiable();
            mockChannel.Setup(m => m.BeginListCertificates(TestConstants.SubscriptionId, TestConstants.HostedServiceName, It.IsAny<AsyncCallback>(), It.IsAny<object>()))
                       .Verifiable();

            var cmdlet = new GetCertificatesCommand(mockChannel.Object)
            {
                SubscriptionId = TestConstants.SubscriptionId,
                Certificate = TestConstants.Certificate,
                ServiceName = TestConstants.HostedServiceName
            };

            var certificates = cmdlet.GetCertificatesProcess();

            mockChannel.Verify();
            Assert.IsNotNull(certificates);
        }
    }
}
