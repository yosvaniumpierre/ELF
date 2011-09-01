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
    using System.Linq;
    using Microsoft.Samples.AzureManagementTools.PowerShell.HostedServices;
    using Microsoft.Samples.AzureManagementTools.PowerShell.Model;
    using Microsoft.Samples.AzureManagementTools.PowerShell.Tests.Helpers;
    using Microsoft.Samples.WindowsAzure.ServiceManagement;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HostedServicesCmdletsFixture
    {
        private static string operationId = string.Empty;
        private static string testSlot = DeploymentSlotType.Staging;

        [TestMethod]
        public void GetHostedServices()
        {
            var cmdletName = "Get-HostedServices";
            var cmdletParams = TestConstants.CommonParameters;

            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(GetHostedServicesCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() > 0);
            Assert.IsInstanceOfType(result[0].ImmediateBaseObject, typeof(ManagementOperationContext));
        }

        [TestMethod]
        public void GetHostedService()
        {
            var cmdletName = "Get-HostedService";
            var cmdletParams = string.Format(CultureInfo.InvariantCulture, "{0} -serviceName {1}", TestConstants.CommonParameters, TestConstants.HostedServiceName);

            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(GetHostedServiceCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var hostedService = (Model.HostedServiceContext)result[0].ImmediateBaseObject;

            Assert.AreEqual<string>(TestConstants.HostedServiceName, hostedService.ServiceName);
            Assert.IsTrue(!string.IsNullOrEmpty(hostedService.Url.AbsoluteUri));
        }

        [TestMethod]
        public void GetHostedProperties()
        {
            var cmdletName = "Get-HostedProperties";
            var cmdletParams = string.Format(CultureInfo.InvariantCulture, "{0} -serviceName {1}", TestConstants.CommonParameters, TestConstants.HostedServiceName);

            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(GetHostedPropertiesCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var hostedProperties = (HostedServiceProperties)result[0].ImmediateBaseObject;

            Assert.IsTrue(!string.IsNullOrEmpty(hostedProperties.Label));
            Assert.IsTrue(!string.IsNullOrEmpty(hostedProperties.Description));
        }

        [TestMethod]
        public void GetDeployment()
        {
            var cmdletName = "Get-Deployment";
            var cmdletParams = string.Format(
                CultureInfo.InvariantCulture, 
                "{0} -serviceName {1} -slot {2}", 
                TestConstants.CommonParameters, 
                TestConstants.HostedServiceName, 
                testSlot);

            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(GetDeploymentCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var deployment = (DeploymentInfoContext)result[0].ImmediateBaseObject;

            Assert.IsNotNull(deployment);
            Assert.AreEqual<string>(testSlot, deployment.Slot);
        }

        [TestMethod]
        public void GetOSVersions()
        {
            var cmdletName = "Get-OSVersions";
            var cmdletParams = TestConstants.CommonParameters;

            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(GetOSVersionsCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() >= 4);
        }

        [TestMethod, Ignore]
        public void GetDeploymentAndDeleteIfExists()
        {
            var cmdletName = "Get-Deployment";
            var cmdletParams = string.Format(
                CultureInfo.InvariantCulture,
                "{0} -serviceName {1} -slot {2}",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName,
                testSlot);

            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(GetDeploymentCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var deployment = result[0].ImmediateBaseObject as Model.DeploymentInfoContext;

            if (deployment != null && !string.IsNullOrEmpty(deployment.Name))
            {
                Assert.AreEqual<string>(TestConstants.HostedServiceName, deployment.ServiceName);
                Assert.IsTrue(!string.IsNullOrEmpty(deployment.Url.AbsoluteUri));

                this.RemoveDeployment();
            }
        }

        [TestMethod, Ignore]
        [DeploymentItem("Resources\\ServiceConfiguration.cscfg")]
        public void CreateDeploymentUsingPackageFromBlob()
        {
            // OK
            var cmdletName = "New-Deployment";
            var cmdletParams = string.Format(
                CultureInfo.InvariantCulture,
                "{0} -serviceName {1} -slot {2} -package {3} -configuration {4} -name {5} -label {6}",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName,
                testSlot,
                TestConstants.AzurePackageLocation,
                "ServiceConfiguration.cscfg",
                TestConstants.StagingDeploymentName,
                TestConstants.StagingLabel);

            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(NewDeploymentCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var ctx = (ManagementOperationContext)result[0].ImmediateBaseObject;

            Assert.IsTrue(!string.IsNullOrEmpty(ctx.OperationId));
            Assert.AreEqual<string>(TestConstants.HostedServiceName, ctx.ServiceName);

            operationId = ctx.OperationId;
        }

        [TestMethod, Ignore]
        [DeploymentItem("Resources\\TestCloudService.cspkg")]
        [DeploymentItem("Resources\\ServiceConfiguration.cscfg")]
        public void CreateDeploymentUsingPackageFromFileSystemSpecifyingStorageServiceName()
        {
            // OK
            var cmdletName = "New-Deployment";
            var cmdletParams = string.Format(
                CultureInfo.InvariantCulture,
                "{0} -serviceName {1} -slot {2} -package {3} -configuration {4} -name {5} -label {6} -storageServiceName {7}",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName,
                testSlot,
                "TestCloudService.cspkg",
                "ServiceConfiguration.cscfg",
                TestConstants.StagingDeploymentName,
                TestConstants.StagingLabel,
                TestConstants.StorageServiceName);

            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(NewDeploymentCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var ctx = (ManagementOperationContext)result[0].ImmediateBaseObject;

            Assert.IsTrue(!string.IsNullOrEmpty(ctx.OperationId));
            Assert.AreEqual<string>(TestConstants.HostedServiceName, ctx.ServiceName);

            operationId = ctx.OperationId;
        }

        [TestMethod, Ignore]
        [DeploymentItem("Resources\\TestCloudService.cspkg")]
        [DeploymentItem("Resources\\ServiceConfiguration.cscfg")]
        public void CreateDeploymentUsingPackageFromFileSystem()
        {
            // OK
            var cmdletName = "New-Deployment";
            var cmdletParams = string.Format(
                CultureInfo.InvariantCulture,
                "{0} -serviceName {1} -slot {2} -package {3} -configuration {4} -name {5} -label {6}",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName,
                testSlot,
                "TestCloudService.cspkg",
                "ServiceConfiguration.cscfg",
                TestConstants.StagingDeploymentName,
                TestConstants.StagingLabel);

            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(NewDeploymentCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var ctx = (ManagementOperationContext)result[0].ImmediateBaseObject;

            Assert.IsTrue(!string.IsNullOrEmpty(ctx.OperationId));
            Assert.AreEqual<string>(TestConstants.HostedServiceName, ctx.ServiceName);

            operationId = ctx.OperationId;
        }

        [TestMethod, Ignore]
        [DeploymentItem("Resources\\ServiceConfiguration.cscfg")]
        public void CreateDeploymentAndWaitToCompleteOperation()
        {
            // OK
            var cmdletName = "New-Deployment";
            var cmdletParams = string.Format(
                CultureInfo.InvariantCulture,
                "{0} -serviceName {1} -slot {2} -package {3} -configuration {4} -label {5}",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName,
                testSlot,
                TestConstants.AzurePackageLocation,
                "ServiceConfiguration.cscfg",
                TestConstants.StagingDeploymentName);

            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(NewDeploymentCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var ctx = (ManagementOperationContext)result[0].ImmediateBaseObject;

            Assert.IsTrue(!string.IsNullOrEmpty(ctx.OperationId));
            Assert.AreEqual<string>(TestConstants.HostedServiceName, ctx.ServiceName);

            operationId = ctx.OperationId;

            var finalStatus = GetOperationStatus();

            Assert.AreEqual<string>(OperationState.Succeeded, finalStatus);
        }

        [TestMethod, Ignore]
        public void SwapDeployment()
        {
            // OK
            var cmdletName = "Move-Deployment";
            var cmdletParams = string.Format(
                CultureInfo.InvariantCulture,
                "{0} -serviceName {1} -name {2} -deploymentNameInProduction {3}",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName,
                TestConstants.StagingDeploymentName,
                TestConstants.ProductionDeploymentName);

            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(MoveDeploymentCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var ctx = (ManagementOperationContext)result[0].ImmediateBaseObject;

            Assert.IsTrue(!string.IsNullOrEmpty(ctx.OperationId));
            Assert.AreEqual<string>(TestConstants.HostedServiceName, ctx.ServiceName);

            operationId = ctx.OperationId;
        }

        [TestMethod, Ignore]
        public void RemoveDeployment()
        {
            // OK
            var cmdletName = "Remove-Deployment";
            var cmdletParams = string.Format(
                CultureInfo.InvariantCulture,
                "{0} -serviceName {1} -slot {2}",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName,
                testSlot);

            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(RemoveDeploymentCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var ctx = (ManagementOperationContext)result[0].ImmediateBaseObject;

            Assert.IsTrue(!string.IsNullOrEmpty(ctx.OperationId));
            Assert.AreEqual<string>(TestConstants.HostedServiceName, ctx.ServiceName);

            operationId = ctx.OperationId;
        }

        [TestMethod, Ignore]
        [DeploymentItem("Resources\\ServiceConfigurationWithMoreVMs.cscfg")]
        public void UpdateDeploymentConfiguration()
        {
            // OK
            var cmdletName = "Set-DeploymentConfiguration";
            var cmdletParams = string.Format(
                CultureInfo.InvariantCulture,
                "{0} -serviceName {1} -slot {2} -configuration {3}",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName,
                testSlot,
                "ServiceConfigurationWithMoreVMs.cscfg");

            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(SetDeploymentConfigurationCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var ctx = (ManagementOperationContext)result[0].ImmediateBaseObject;

            Assert.IsTrue(!string.IsNullOrEmpty(ctx.OperationId));
            Assert.AreEqual<string>(TestConstants.HostedServiceName, ctx.ServiceName);

            operationId = ctx.OperationId;
        }

        [TestMethod, Ignore]
        public void UpdateDeploymentStatus()
        {
            // OK
            var cmdletName = "Set-DeploymentStatus";
            var cmdletParams = string.Format(
                CultureInfo.InvariantCulture,
                "{0} -serviceName {1} -slot {2} -status {3}",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName,
                testSlot,
                DeploymentStatus.Running);

            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(SetDeploymentStatusCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var ctx = (ManagementOperationContext)result[0].ImmediateBaseObject;

            Assert.IsTrue(!string.IsNullOrEmpty(ctx.OperationId));
            Assert.AreEqual<string>(TestConstants.HostedServiceName, ctx.ServiceName);

            operationId = ctx.OperationId;
        }

        [TestMethod, Ignore]
        [DeploymentItem("Resources\\ServiceConfiguration.cscfg")]
        public void UpgradeDeploymentSingleRole()
        {
            // OK
            var testLabel = string.Format(CultureInfo.InvariantCulture, "TestLabel-{0}", Guid.NewGuid());
            var cmdletName = "Set-Deployment";
            var cmdletParams = string.Format(
                CultureInfo.InvariantCulture,
                "{0} -serviceName {1} -slot {2} -mode {3} -package {4} -configuration {5} -label {6} -roleName {7}",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName,
                testSlot,
                "Auto",
                TestConstants.AzurePackageLocation,
                "ServiceConfiguration.cscfg",
                testLabel,
                TestConstants.RoleName);

            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(SetDeploymentCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var ctx = (ManagementOperationContext)result[0].ImmediateBaseObject;

            Assert.IsTrue(!string.IsNullOrEmpty(ctx.OperationId));
            Assert.AreEqual<string>(TestConstants.HostedServiceName, ctx.ServiceName);

            operationId = ctx.OperationId;
        }

        [TestMethod, Ignore]
        [DeploymentItem("Resources\\TestCloudService.cspkg")]
        [DeploymentItem("Resources\\ServiceConfiguration.cscfg")]
        public void UpgradeDeploymentWholeServiceWithPackageFromFileSystem()
        {
            // OK
            var testLabel = string.Format(CultureInfo.InvariantCulture, "TestLabel-{0}", Guid.NewGuid());
            var cmdletName = "Set-Deployment";
            var cmdletParams = string.Format(
                CultureInfo.InvariantCulture,
                "{0} -serviceName {1} -slot {2} -mode {3} -package {4} -configuration {5} -label {6}",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName,
                testSlot,
                "Auto",
                "TestCloudService.cspkg",
                "ServiceConfiguration.cscfg",
                testLabel);

            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(SetDeploymentCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var ctx = (ManagementOperationContext)result[0].ImmediateBaseObject;

            Assert.IsTrue(!string.IsNullOrEmpty(ctx.OperationId));
            Assert.AreEqual<string>(TestConstants.HostedServiceName, ctx.ServiceName);

            operationId = ctx.OperationId;
        }

        [TestMethod, Ignore]
        [DeploymentItem("Resources\\TestCloudService.cspkg")]
        [DeploymentItem("Resources\\ServiceConfiguration.cscfg")]
        public void UpgradeDeploymentWholeServiceWithPackageFromFileSystemSpecifyingStorageServiceName()
        {
            // OK
            var testLabel = string.Format(CultureInfo.InvariantCulture, "TestLabel-{0}", Guid.NewGuid());
            var cmdletName = "Set-Deployment";
            var cmdletParams = string.Format(
                CultureInfo.InvariantCulture,
                "{0} -serviceName {1} -slot {2} -mode {3} -package {4} -configuration {5} -label {6} -storageServiceName {7}",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName,
                testSlot,
                "Auto",
                "TestCloudService.cspkg",
                "ServiceConfiguration.cscfg",
                testLabel,
                TestConstants.StorageServiceName);

            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(SetDeploymentCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var ctx = (ManagementOperationContext)result[0].ImmediateBaseObject;

            Assert.IsTrue(!string.IsNullOrEmpty(ctx.OperationId));
            Assert.AreEqual<string>(TestConstants.HostedServiceName, ctx.ServiceName);

            operationId = ctx.OperationId;
        }

        [TestMethod, Ignore]
        public void WalkUpgradeDomain()
        {
            // OK
            var cmdletName = "Set-WalkUpgradeDomain";
            var cmdletParams = string.Format(
                CultureInfo.InvariantCulture,
                "{0} -serviceName {1} -slot {2} -domainNumber {3}",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName,
                testSlot,
                0);

            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(SetWalkUpgradeDomainCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var ctx = (ManagementOperationContext)result[0].ImmediateBaseObject;

            Assert.IsTrue(!string.IsNullOrEmpty(ctx.OperationId));
            Assert.AreEqual<string>(TestConstants.HostedServiceName, ctx.ServiceName);

            operationId = ctx.OperationId;
        }

        [TestMethod, Ignore]
        [DeploymentItem("Resources\\ServiceConfiguration.cscfg")]
        [DeploymentItem("Resources\\ServiceConfigurationWithMoreVMs.cscfg")]
        public void IntegrationTest()
        {
            var status = string.Empty;

            // 1. Get deployment from staging. If exists, delete it
            this.GetDeploymentAndDeleteIfExists();
            status = GetOperationStatus();

            // 2. Create new deployment in staging
            this.CreateDeploymentUsingPackageFromBlob();
            status = GetOperationStatus();

            Assert.AreEqual<string>(OperationState.Succeeded, status);

            // 3. Move deployment to production
            this.SwapDeployment();
            status = GetOperationStatus();

            Assert.AreEqual<string>(OperationState.Succeeded, status);
            testSlot = "production";

            // 4. Change deployment status to running
            this.UpdateDeploymentStatus();
            status = GetOperationStatus();

            Assert.AreEqual<string>(OperationState.Succeeded, status);

            // 5. Update deployment configuration
            this.UpdateDeploymentConfiguration();
            status = GetOperationStatus();

            Assert.AreEqual<string>(OperationState.Succeeded, status);

            // 6. Delete deployment
            this.RemoveDeployment();
            status = GetOperationStatus();

            Assert.AreEqual<string>(OperationState.Succeeded, status);
        }

        private static string GetOperationStatus()
        {
            if (string.IsNullOrEmpty(operationId))
            {
                return string.Empty;
            }
            
            var cmdletName = "Get-OperationStatus";
            var cmdletParams = string.Format(
                CultureInfo.InvariantCulture,
                "{0} -operationId {1} -waitToComplete",
                TestConstants.CommonParameters,
                operationId);

            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(GetOperationStatusCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var status = result[0].ImmediateBaseObject.ToString();

            return status;
        }
    }
}