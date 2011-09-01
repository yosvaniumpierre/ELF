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
    using System.Management.Automation.Runspaces;
    using Microsoft.Samples.AzureManagementTools.PowerShell.Diagnostics;
    using Microsoft.Samples.AzureManagementTools.PowerShell.Tests.Helpers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Diagnostics;

    [TestClass, Ignore]
    public class DiagnosticsCmdletsFixture
    {
        private static string deploymentId = "a60db6385b7b46f1848b5ab022e7ac38";
        private static string testRoleName = "TestWebRole";
        private static string testInstanceId = "TestWebRole_IN_0";
        private static string diagnosticsCommonParams = string.Format(
            CultureInfo.InvariantCulture,
            "-storageAccountName {0} -storageAccountKey {1} -deploymentId {2}",
            TestConstants.StorageServiceName,
            "8z7i2IDTEwT+/TXRPGknjhSvSk7zH41jsDhT2cVdoAB9Ax+gxtFFMx+IqHjt8IMufwYJzYbrDh3NjwofQ/ht9g==",
            deploymentId);

        [TestInitialize]
        public void InitializeTest()
        {
            this.StopActiveTranfers();
        }
        
        [TestMethod]
        public void GetDiagnosticAwareRoleInstances()
        {
            // Ok
            var cmdletName = "Get-DiagnosticAwareRoleInstances";
            var cmdletParams = string.Format(
                CultureInfo.InvariantCulture,
                "{0} -roleName {1}",
                diagnosticsCommonParams,
                testRoleName);
            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(GetDiagnosticAwareRoleInstancesCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public void GetDiagnosticAwareRoles()
        {
            // Ok
            var cmdletName = "Get-DiagnosticAwareRoles";
            var cmdletParams = diagnosticsCommonParams;
            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(GetDiagnosticAwareRolesCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public void GetDiagnosticConfiguration()
        {
            // Ok
            var cmdletName = "Get-DiagnosticConfiguration";
            var cmdletParams = string.Format(
                CultureInfo.InvariantCulture,
                "{0} -bufferName {1} -roleName {2} -instanceId {3}",
                diagnosticsCommonParams,
                DataBufferName.WindowsEventLogs,
                testRoleName,
                testInstanceId);
            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(GetDiagnosticConfigurationCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public void GetCommonConfigurationLogs()
        {
            // Ok
            var cmdletName = "Get-CommonConfigurationLogs";
            var cmdletParams = string.Format(
                CultureInfo.InvariantCulture,
                "{0} -roleName {1} -instanceId {2}",
                diagnosticsCommonParams,
                testRoleName,
                testInstanceId);
            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(GetCommonConfigurationLogsCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public void SetCommonConfigurationLogs()
        {
            // Ok
            var cmdletName = "Set-CommonConfigurationLogs";
            var cmdletParams = string.Format(
                CultureInfo.InvariantCulture,
                "{0} -roleName {1} -instanceId {2} -overallQuotaInMB {3}",
                diagnosticsCommonParams,
                testRoleName,
                testInstanceId,
                6 * 1024);
            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(SetCommonConfigurationLogsCommand));

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void SetFileBasedLog()
        {
            // Ok
            var testDirectoryConfiguration1 = new DirectoryConfiguration
            {
                Path = @"C:\Resources\directory\a60db6385b7b46f1848b5ab022e7ac38.TestWebRole.DiagnosticStore\FailedReqLogFiles",
                DirectoryQuotaInMB = 512,
                Container = "wad-iis-failedreqlogfiles"
            };

            var testDirectoryConfiguration2 = new DirectoryConfiguration
            {
                Path = @"C:\Resources\directory\a60db6385b7b46f1848b5ab022e7ac38.TestWebRole.DiagnosticStore\LogFiles",
                DirectoryQuotaInMB = 512,
                Container = "wad-iis-logfiles"
            };

            var testDirectoryConfiguration3 = new DirectoryConfiguration
            {
                Path = @"C:\Resources\directory\a60db6385b7b46f1848b5ab022e7ac38.TestWebRole.DiagnosticStore\CrashDumps",
                DirectoryQuotaInMB = 512,
                Container = "wad-crash-dumps"
            };

            var command = new Command("Set-FileBasedLog");
            command.Parameters.Add("storageAccountName", TestConstants.StorageServiceName);
            command.Parameters.Add("storageAccountKey", "8z7i2IDTEwT+/TXRPGknjhSvSk7zH41jsDhT2cVdoAB9Ax+gxtFFMx+IqHjt8IMufwYJzYbrDh3NjwofQ/ht9g==");
            command.Parameters.Add("deploymentId", deploymentId);
            command.Parameters.Add("roleName", testRoleName);
            command.Parameters.Add("instanceId", testInstanceId);
            command.Parameters.Add("directoriesConfiguration", new DirectoryConfiguration[] { testDirectoryConfiguration1, testDirectoryConfiguration2, testDirectoryConfiguration3 });

            var result = CmdletTestingHelper.RunCmdlet(command, typeof(SetFileBasedLogCommand));

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void SetInfrastructureLog()
        {
            // Ok
            var cmdletName = "Set-InfrastructureLog";
            var cmdletParams = string.Format(
                CultureInfo.InvariantCulture,
                "{0} -roleName {1} -instanceId {2} -logLevelFilter {3}",
                diagnosticsCommonParams,
                testRoleName,
                testInstanceId,
                LogLevel.Verbose);
            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(SetInfrastructureLogCommand));

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void SetPerformanceCounter()
        {
            // Ok
            var testPerformanceCounter = new PerformanceCounterConfiguration
            {
                CounterSpecifier = @"\memory\Available MBytes",
                SampleRate = TimeSpan.FromSeconds(30)
            };

            var command = new Command("Set-PerformanceCounter");
            command.Parameters.Add("storageAccountName", TestConstants.StorageServiceName);
            command.Parameters.Add("storageAccountKey", "8z7i2IDTEwT+/TXRPGknjhSvSk7zH41jsDhT2cVdoAB9Ax+gxtFFMx+IqHjt8IMufwYJzYbrDh3NjwofQ/ht9g==");
            command.Parameters.Add("deploymentId", deploymentId);
            command.Parameters.Add("roleName", testRoleName);
            command.Parameters.Add("instanceId", testInstanceId);
            command.Parameters.Add("performanceCounters", new PerformanceCounterConfiguration[] { testPerformanceCounter });

            var result = CmdletTestingHelper.RunCmdlet(command, typeof(SetPerformanceCounterCommand));

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void SetWindowsAzureLog()
        {
            // Ok
            var cmdletName = "Set-WindowsAzureLog";
            var cmdletParams = string.Format(
                CultureInfo.InvariantCulture,
                "{0} -roleName {1} -instanceId {2} -logLevelFilter {3}",
                diagnosticsCommonParams,
                testRoleName,
                testInstanceId,
                LogLevel.Verbose);
            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(SetWindowsAzureLogCommand));

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void SetWindowsEventLog()
        {
            // Ok
            var command = new Command("Set-WindowsEventLog");
            command.Parameters.Add("storageAccountName", TestConstants.StorageServiceName);
            command.Parameters.Add("storageAccountKey", "8z7i2IDTEwT+/TXRPGknjhSvSk7zH41jsDhT2cVdoAB9Ax+gxtFFMx+IqHjt8IMufwYJzYbrDh3NjwofQ/ht9g==");
            command.Parameters.Add("deploymentId", deploymentId);
            command.Parameters.Add("roleName", testRoleName);
            command.Parameters.Add("instanceId", testInstanceId);
            command.Parameters.Add("logLevelFilter", LogLevel.Verbose);
            command.Parameters.Add("eventLogs", new string[] { "UserData!*", "System!*" });

            var result = CmdletTestingHelper.RunCmdlet(command, typeof(SetWindowsEventLogCommand));

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void StartOnDemandTransfer()
        {
            // Ok
            var command = new Command("Start-OnDemandTransfer");
            command.Parameters.Add("storageAccountName", TestConstants.StorageServiceName);
            command.Parameters.Add("storageAccountKey", "8z7i2IDTEwT+/TXRPGknjhSvSk7zH41jsDhT2cVdoAB9Ax+gxtFFMx+IqHjt8IMufwYJzYbrDh3NjwofQ/ht9g==");
            command.Parameters.Add("deploymentId", deploymentId);
            command.Parameters.Add("roleName", testRoleName);
            ////command.Parameters.Add("instanceId", testInstanceId);
            command.Parameters.Add("dataBufferName", DataBufferName.PerformanceCounters);
            command.Parameters.Add("from", DateTime.Now - TimeSpan.FromHours(1));
            command.Parameters.Add("to", DateTime.Now);

            var result = CmdletTestingHelper.RunCmdlet(command, typeof(StartOnDemandTransferCommand));

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void StopActiveTranfers()
        {
            // Ok
            var command = new Command("Stop-ActiveTranfer");
            command.Parameters.Add("storageAccountName", TestConstants.StorageServiceName);
            command.Parameters.Add("storageAccountKey", "8z7i2IDTEwT+/TXRPGknjhSvSk7zH41jsDhT2cVdoAB9Ax+gxtFFMx+IqHjt8IMufwYJzYbrDh3NjwofQ/ht9g==");
            command.Parameters.Add("deploymentId", deploymentId);
            command.Parameters.Add("roleName", testRoleName);
            command.Parameters.Add("instanceId", testInstanceId);

            var result = CmdletTestingHelper.RunCmdlet(command, typeof(StopActiveTransferCommand));

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetActiveTransfers()
        {
            // Ok
            var command = new Command("Get-ActiveTransfers");
            command.Parameters.Add("storageAccountName", TestConstants.StorageServiceName);
            command.Parameters.Add("storageAccountKey", "8z7i2IDTEwT+/TXRPGknjhSvSk7zH41jsDhT2cVdoAB9Ax+gxtFFMx+IqHjt8IMufwYJzYbrDh3NjwofQ/ht9g==");
            command.Parameters.Add("deploymentId", deploymentId);
            command.Parameters.Add("roleName", testRoleName);
            ////command.Parameters.Add("instanceId", testInstanceId);

            var result = CmdletTestingHelper.RunCmdlet(command, typeof(GetActiveTransfersCommand));

            Assert.IsNotNull(result);
        }
    }
}
