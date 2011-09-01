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
    using Microsoft.Samples.AzureManagementTools.PowerShell.HostedServices;
    using Microsoft.Samples.AzureManagementTools.PowerShell.Model;
    using Microsoft.Samples.AzureManagementTools.PowerShell.Tests.Helpers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HostedServicesPipeliningFixture
    {
        [TestMethod]
        public void ShouldRetrieveHostedProperties()
        {
            // OK
            var command = string.Format(
                CultureInfo.InvariantCulture,
                "Get-HostedServices {0} | where {{$_.ServiceName -eq \"{1}\"}} | Get-HostedProperties",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName);

            var cmdlets = new Dictionary<string, Type>()
            {
                { "Get-HostedServices", typeof(GetHostedServicesCommand) },
                { "Get-HostedProperties", typeof(GetHostedPropertiesCommand) }
            };

            Collection<PSObject> result = CmdletTestingHelper.RunCmdlet(cmdlets, command);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod, Ignore]
        public void ShouldRetrieveDeploymentFromHostedService()
        {
            // OK
            var testSlot = "Staging";
            var command = string.Format(
                CultureInfo.InvariantCulture,
                "Get-HostedServices {0} | where {{$_.ServiceName -eq \"{1}\"}} | Get-Deployment {2}",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName,
                testSlot);

            var cmdlets = new Dictionary<string, Type>()
            {
                { "Get-HostedServices", typeof(GetHostedServicesCommand) },
                { "Get-Deployment", typeof(GetDeploymentCommand) }
            };

            Collection<PSObject> result = CmdletTestingHelper.RunCmdlet(cmdlets, command);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var deploymentInfo = result[0].ImmediateBaseObject as DeploymentInfoContext;

            Assert.AreEqual<string>(testSlot, deploymentInfo.Slot);
        }

        [TestMethod, Ignore]
        [DeploymentItem("Resources\\ServiceConfigurationWithMoreVMs.cscfg")]
        public void ShouldUpdateAllDeployConfiguration()
        {
            // OK
            var command = string.Format(
                CultureInfo.InvariantCulture,
                "Get-HostedService {1} {0} | Get-Deployment {2} | Set-DeploymentConfiguration -Configuration {3}",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName,
                "Staging",
                "ServiceConfigurationWithMoreVMs.cscfg");

            var cmdlets = new Dictionary<string, Type>()
            {
                { "Get-HostedService", typeof(GetHostedServiceCommand) },
                { "Get-Deployment", typeof(GetDeploymentCommand) }, 
                { "Set-DeploymentConfiguration", typeof(SetDeploymentConfigurationCommand) }
            };

            Collection<PSObject> result = CmdletTestingHelper.RunCmdlet(cmdlets, command);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var ctx = result[0].ImmediateBaseObject as ManagementOperationContext;
            Assert.IsFalse(string.IsNullOrEmpty(ctx.OperationId));
        }

        [TestMethod, Ignore]
        public void ShouldUpdateRoleInstanceNumberFromDeployConfiguration()
        {
            // OK
            var command = string.Format(
                CultureInfo.InvariantCulture,
                "Get-HostedServices {0} | where {{$_.ServiceName -eq \"{1}\"}} | Get-Deployment {2} | Set-DeploymentConfiguration {{$_.RolesConfiguration[\"{3}\"].InstanceCount += 1}}",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName,
                "Staging",
                TestConstants.RoleName);

            var cmdlets = new Dictionary<string, Type>()
            {
                { "Get-HostedServices", typeof(GetHostedServicesCommand) },
                { "Get-Deployment", typeof(GetDeploymentCommand) }, 
                { "Set-DeploymentConfiguration", typeof(SetDeploymentConfigurationCommand) }
            };

            Collection<PSObject> result = CmdletTestingHelper.RunCmdlet(cmdlets, command);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var ctx = result[0].ImmediateBaseObject as ManagementOperationContext;
            Assert.IsFalse(string.IsNullOrEmpty(ctx.OperationId));
        }

        [TestMethod, Ignore]
        public void ShouldUpdateCertificateSettingsFromDeployConfiguration()
        {
            // Ok
            var command = string.Format(
                CultureInfo.InvariantCulture,
                "Get-HostedServices {0} | where {{$_.ServiceName -eq \"{1}\"}} | Get-Deployment {2} | Set-DeploymentConfiguration {{$_.RolesConfiguration[\"{3}\"].Certificates[\"{4}\"].Thumbprint = \"{5}\"}}",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName,
                "Staging",
                TestConstants.RoleName,
                "Certificate1",
                "0000000000000000000000000000000000000000");

            var cmdlets = new Dictionary<string, Type>()
            {
                { "Get-HostedServices", typeof(GetHostedServicesCommand) },
                { "Get-Deployment", typeof(GetDeploymentCommand) }, 
                { "Set-DeploymentConfiguration", typeof(SetDeploymentConfigurationCommand) }
            };

            Collection<PSObject> result = CmdletTestingHelper.RunCmdlet(cmdlets, command);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var ctx = result[0].ImmediateBaseObject as ManagementOperationContext;
            Assert.IsFalse(string.IsNullOrEmpty(ctx.OperationId));
        }

        [TestMethod, Ignore]
        public void ShouldUpdateOsVersionFromDeployConfiguration()
        {
            // OK
            var command = string.Format(
                CultureInfo.InvariantCulture,
                "Get-HostedService {1} {0} | Get-Deployment {2} | Set-DeploymentConfiguration {{ $_.OSVersion = '{3}' }}",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName,
                "Staging",
                "WA-GUEST-OS-1.0_200912-01");

            var cmdlets = new Dictionary<string, Type>()
            {
                { "Get-HostedService", typeof(GetHostedServiceCommand) },
                { "Get-Deployment", typeof(GetDeploymentCommand) }, 
                { "Set-DeploymentConfiguration", typeof(SetDeploymentConfigurationCommand) }
            };

            Collection<PSObject> result = CmdletTestingHelper.RunCmdlet(cmdlets, command);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var ctx = result[0].ImmediateBaseObject as ManagementOperationContext;
            Assert.IsFalse(string.IsNullOrEmpty(ctx.OperationId));
        }

        [TestMethod, Ignore]
        public void ShouldSwapDeployments()
        {
            // OK
            var command = string.Format(
                CultureInfo.InvariantCulture,
                "Get-HostedServices {0} | where {{$_.ServiceName -eq \"{1}\"}} | Get-Deployment {2} | Move-Deployment",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName,
                "Staging");

            var cmdlets = new Dictionary<string, Type>()
            {
                { "Get-HostedServices", typeof(GetHostedServicesCommand) },
                { "Get-Deployment", typeof(GetDeploymentCommand) }, 
                { "Move-Deployment", typeof(MoveDeploymentCommand) }
            };

            Collection<PSObject> result = CmdletTestingHelper.RunCmdlet(cmdlets, command);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var ctx = result[0].ImmediateBaseObject as ManagementOperationContext;
            Assert.IsFalse(string.IsNullOrEmpty(ctx.OperationId));
        }

        [TestMethod, Ignore]
        [DeploymentItem("Resources\\ServiceConfiguration.cscfg")]
        public void ShouldCreateNewDeployment()
        {
            // OK
            var command = string.Format(
                CultureInfo.InvariantCulture,
                "Get-HostedServices {0} | where {{$_.ServiceName -eq \"{1}\"}} | New-Deployment {2} \"{3}\" \"{4}\" {5}",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName,
                "Staging",
                TestConstants.AzurePackageLocation,
                "ServiceConfiguration.cscfg",
                TestConstants.StagingLabel);

            var cmdlets = new Dictionary<string, Type>()
            {
                { "Get-HostedServices", typeof(GetHostedServicesCommand) },
                { "New-Deployment", typeof(NewDeploymentCommand) }
            };

            Collection<PSObject> result = CmdletTestingHelper.RunCmdlet(cmdlets, command);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var ctx = result[0].ImmediateBaseObject as ManagementOperationContext;
            Assert.IsFalse(string.IsNullOrEmpty(ctx.OperationId));
        }

        [TestMethod, Ignore]
        public void ShouldDeleteDeployment()
        {
            // OK
            var command = string.Format(
                CultureInfo.InvariantCulture,
                "Get-HostedServices {0} | where {{$_.ServiceName -eq \"{1}\"}} | Get-Deployment {2} | Remove-Deployment",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName,
                "Production");

            var cmdlets = new Dictionary<string, Type>()
            {
                { "Get-HostedServices", typeof(GetHostedServicesCommand) },
                { "Get-Deployment", typeof(GetDeploymentCommand) }, 
                { "Remove-Deployment", typeof(RemoveDeploymentCommand) }
            };

            Collection<PSObject> result = CmdletTestingHelper.RunCmdlet(cmdlets, command);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var ctx = result[0].ImmediateBaseObject as ManagementOperationContext;
            Assert.IsFalse(string.IsNullOrEmpty(ctx.OperationId));
        }

        [TestMethod, Ignore]
        public void ShouldChangeDeploymentStatus()
        {
            // OK
            var command = string.Format(
                CultureInfo.InvariantCulture,
                "Get-HostedService {1} {0} | Get-Deployment {2} | Set-DeploymentStatus {3}",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName,
                "Staging",
                "Running");

            var cmdlets = new Dictionary<string, Type>
            {
                { "Get-HostedService", typeof(GetHostedServiceCommand) },
                { "Get-Deployment", typeof(GetDeploymentCommand) }, 
                { "Set-DeploymentStatus", typeof(SetDeploymentStatusCommand) }
            };

            Collection<PSObject> result = CmdletTestingHelper.RunCmdlet(cmdlets, command);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var ctx = result[0].ImmediateBaseObject as ManagementOperationContext;
            Assert.IsFalse(string.IsNullOrEmpty(ctx.OperationId));
        }

        [TestMethod, Ignore]
        public void ShouldUpgradeDeploymentWithoutUpdateConfiguration()
        {
            // OK
            var command = string.Format(
                CultureInfo.InvariantCulture,
                "Get-HostedService {1} {0} | Get-Deployment {2} | Set-Deployment -package {3} -label {4}",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName,
                "Staging",
                TestConstants.AzurePackageLocation,
                TestConstants.StagingLabel);

            var cmdlets = new Dictionary<string, Type>()
            {
                { "Get-HostedService", typeof(GetHostedServiceCommand) },
                { "Get-Deployment", typeof(GetDeploymentCommand) }, 
                { "Set-Deployment", typeof(SetDeploymentCommand) }
            };

            Collection<PSObject> result = CmdletTestingHelper.RunCmdlet(cmdlets, command);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var ctx = result[0].ImmediateBaseObject as ManagementOperationContext;
            Assert.IsFalse(string.IsNullOrEmpty(ctx.OperationId));
        }

        [TestMethod, Ignore]
        [DeploymentItem("Resources\\ServiceConfigurationWithMoreVMs.cscfg")]
        public void ShouldUpgradeDeploymentAndUpdateAllConfiguration()
        {
            // OK
            var command = string.Format(
                CultureInfo.InvariantCulture,
                "Get-HostedService {1} {0} | Get-Deployment {2} | Set-Deployment -roleName {3} -package {4} -label {5} -configuration {6}",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName,
                "Staging",
                TestConstants.RoleName,
                TestConstants.AzurePackageLocation,
                TestConstants.StagingLabel,
                "ServiceConfigurationWithMoreVMs.cscfg");

            var cmdlets = new Dictionary<string, Type>()
            {
                { "Get-HostedService", typeof(GetHostedServiceCommand) },
                { "Get-Deployment", typeof(GetDeploymentCommand) },
                { "Set-Deployment", typeof(SetDeploymentCommand) }
            };

            Collection<PSObject> result = CmdletTestingHelper.RunCmdlet(cmdlets, command);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var ctx = result[0].ImmediateBaseObject as ManagementOperationContext;
            Assert.IsFalse(string.IsNullOrEmpty(ctx.OperationId));
        }

        [TestMethod, Ignore]
        public void ShouldUpgradeDeploymentAndUpdateASpecifiedSettingAndInstanceNumber()
        {
            // OK
            var command = string.Format(
                CultureInfo.InvariantCulture,
                "Get-HostedService {1} {0} | Get-Deployment {2} | Set-Deployment -roleName {3} -package {4} -label {5} {{ $SelectedRoleConfiguration.Settings[\"productName\"] = \"{6}\"; $SelectedRoleConfiguration.InstanceCount += 1 }}",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName,
                "Staging",
                TestConstants.RoleName,
                TestConstants.AzurePackageLocation,
                TestConstants.StagingLabel,
                "AzureManagementToolsUpdated");

            var cmdlets = new Dictionary<string, Type>()
            {
                { "Get-HostedService", typeof(GetHostedServiceCommand) },
                { "Get-Deployment", typeof(GetDeploymentCommand) }, 
                { "Set-Deployment", typeof(SetDeploymentCommand) }
            };

            Collection<PSObject> result = CmdletTestingHelper.RunCmdlet(cmdlets, command);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var ctx = result[0].ImmediateBaseObject as ManagementOperationContext;

            Assert.IsFalse(string.IsNullOrEmpty(ctx.OperationId));
        }

        [TestMethod, Ignore]
        public void ShouldSetWalkUpgradeDomain()
        {
            // OK
            var command = string.Format(
                CultureInfo.InvariantCulture,
                "Get-HostedService {1} {0} | Get-Deployment {2} | Set-WalkUpgradeDomain {3}",
                TestConstants.CommonParameters,
                TestConstants.HostedServiceName,
                "Staging",
                "0");

            var cmdlets = new Dictionary<string, Type>()
            {
                { "Get-HostedService", typeof(GetHostedServiceCommand) },
                { "Get-Deployment", typeof(GetDeploymentCommand) }, 
                { "Set-WalkUpgradeDomain", typeof(SetWalkUpgradeDomainCommand) }
            };

            Collection<PSObject> result = CmdletTestingHelper.RunCmdlet(cmdlets, command);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var ctx = result[0].ImmediateBaseObject as ManagementOperationContext;
            Assert.IsFalse(string.IsNullOrEmpty(ctx.OperationId));
        }
    }
}
