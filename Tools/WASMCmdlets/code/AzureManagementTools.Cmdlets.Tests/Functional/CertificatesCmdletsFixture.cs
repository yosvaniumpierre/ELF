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
    using System.Globalization;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.Samples.AzureManagementTools.PowerShell.Certificates;
    using Microsoft.Samples.AzureManagementTools.PowerShell.Model;
    using Microsoft.Samples.AzureManagementTools.PowerShell.Tests.Helpers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CertificatesCmdletsFixture
    {
        private static string testCertificatePassword = "Passw0rd!";
        private static string testCertificateThumbprint = "AC62DA6DB383AFAF3BB27E4F253D6210A6412DE3";
        private static string testCertificateAlgorithm = "sha1";

        [TestMethod, Ignore]
        public void GetCertificates()
        {
            var cmdletName = "Get-Certificates";
            var cmdletParams = string.Format(
                CultureInfo.InvariantCulture,
                "{0} -serviceName {1}",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName);
            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(GetCertificatesCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
            Assert.IsInstanceOfType(result[0].ImmediateBaseObject, typeof(CertificateContext));
        }

        [TestMethod, Ignore]
        public void GetCertificate()
        {
            var cmdletName = "Get-Certificate";
            var cmdletParams = string.Format(
                CultureInfo.InvariantCulture,
                "{0} -serviceName {1} -thumbprintAlgorithm {2} -thumbprint {3}",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName,
                testCertificateAlgorithm,
                testCertificateThumbprint);
            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(GetCertificateCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var certificate = (CertificateContext)result[0].ImmediateBaseObject;

            Assert.IsFalse(string.IsNullOrEmpty(certificate.Data));
        }

        [TestMethod, Ignore]
        [DeploymentItem("Resources\\TestAzureMgmt.pfx")]
        public void AddCertificateUsingRelativePath()
        {
            var cmdletName = "Add-Certificate";
            var cmdletParams = string.Format(
                CultureInfo.InvariantCulture,
                "{0} -serviceName {1} -certificateToDeploy {2} -password {3}",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName,
                "TestAzureMgmt.pfx",
                testCertificatePassword);
            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(AddCertificateCommand));

            var ctx = (ManagementOperationContext)result[0].ImmediateBaseObject;

            Assert.IsTrue(!string.IsNullOrEmpty(ctx.OperationId));
            Assert.AreEqual<string>(TestConstants.HostedServiceName, ctx.ServiceName);
        }

        [TestMethod, Ignore]
        [DeploymentItem("Resources\\TestAzureMgmt.pfx")]
        public void AddCertificateUsingAbsolutePath()
        {
            var cmdletName = "Add-Certificate";
            var cmdletParams = string.Format(
                CultureInfo.InvariantCulture,
                "{0} -serviceName {1} -certificateToDeploy \"{2}\" -password {3}",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName,
                AppDomain.CurrentDomain.BaseDirectory + "\\TestAzureMgmt.pfx",
                testCertificatePassword);
            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(AddCertificateCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var ctx = (ManagementOperationContext)result[0].ImmediateBaseObject;

            Assert.IsTrue(!string.IsNullOrEmpty(ctx.OperationId));
            Assert.AreEqual<string>(TestConstants.HostedServiceName, ctx.ServiceName);
        }

        [TestMethod, Ignore]
        [DeploymentItem("Resources\\adatum.cer")]
        public void AddCertificateUsingCerFile()
        {
            var cmdletName = "Add-Certificate";
            var cmdletParams = string.Format(
                CultureInfo.InvariantCulture,
                "{0} -serviceName {1} -certificateToDeploy {2}",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName,
                "adatum.cer");
            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(AddCertificateCommand));

            var ctx = (ManagementOperationContext)result[0].ImmediateBaseObject;

            Assert.IsTrue(!string.IsNullOrEmpty(ctx.OperationId));
            Assert.AreEqual<string>(TestConstants.HostedServiceName, ctx.ServiceName);
        }

        [TestMethod, Ignore]
        public void AddCertificateUsingX509()
        {
            var cmdletName = "Add-Certificate";
            var cmdletParams = string.Format(
                CultureInfo.InvariantCulture,
                "{0} -serviceName {1} -certificateToDeploy ({2})",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName,
                TestConstants.CertificateCommand);
            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(AddCertificateCommand));

            var ctx = (ManagementOperationContext)result[0].ImmediateBaseObject;

            Assert.IsTrue(!string.IsNullOrEmpty(ctx.OperationId));
            Assert.AreEqual<string>(TestConstants.HostedServiceName, ctx.ServiceName);
        }

        [TestMethod, Ignore]
        public void DeleteCertificate()
        {
            var cmdletName = "Remove-Certificate";
            var cmdletParams = string.Format(
                CultureInfo.InvariantCulture,
                "{0} -serviceName {1} -thumbprintAlgorithm {2} -thumbprint {3}",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName,
                testCertificateAlgorithm,
                testCertificateThumbprint);
            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(RemoveCertificateCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var ctx = (ManagementOperationContext)result[0].ImmediateBaseObject;

            Assert.IsTrue(!string.IsNullOrEmpty(ctx.OperationId));
            Assert.AreEqual<string>(TestConstants.HostedServiceName, ctx.ServiceName);
        }
    }
}
