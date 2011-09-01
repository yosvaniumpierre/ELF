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

namespace Microsoft.Samples.AzureManagementTools.PowerShell.Diagnostics
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Management.Automation;
    using Microsoft.WindowsAzure.Diagnostics;
    using Microsoft.WindowsAzure.Diagnostics.Management;

    public abstract class InstrumentationCmdletBase : DiagnosticsCmdletBase
    {
        [Parameter(Mandatory = true, HelpMessage = "The name of the role.")]
        [ValidateNotNullOrEmpty]
        public string RoleName { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, HelpMessage = "The role instance ID.")]
        public string InstanceId { get; set; }

        [Parameter(HelpMessage = "The maximum amount of file system storage available to the specified data buffer.")]
        public int BufferQuotaInMB { get; set; }

        [Parameter(HelpMessage = "The interval between scheduled transfers for this data buffer, in minutes.")]
        [ValidateNotNull]
        public int TransferPeriod { get; set; }

        protected void PerformInstrumentation()
        {
            foreach (var roleInstanceManager in this.RetrieveRoleInstanceManagers())
            {
                this.CancelAllPreviousTransfers(roleInstanceManager);
                
                var diagnosticConfiguration = roleInstanceManager.GetCurrentConfiguration();
                this.SetConfiguration(diagnosticConfiguration);

                roleInstanceManager.SetCurrentConfiguration(diagnosticConfiguration);
            }
        }

        protected abstract void SetConfiguration(DiagnosticMonitorConfiguration diagnosticConfig);

        private IEnumerable<RoleInstanceDiagnosticManager> RetrieveRoleInstanceManagers()
        {
            var roleInstanceManagers = new Collection<RoleInstanceDiagnosticManager>();

            if (string.IsNullOrEmpty(this.InstanceId))
            {
                // Get the role instance diagnostics manager for all instance of the a role
                foreach (var roleInstanceManager in this.DiagnosticManager.GetRoleInstanceDiagnosticManagersForRole(this.RoleName))
                {
                    roleInstanceManagers.Add(roleInstanceManager);
                }
            }
            else
            {
                var roleInstanceManager = this.DiagnosticManager.GetRoleInstanceDiagnosticManager(this.RoleName, this.InstanceId);
                roleInstanceManagers.Add(roleInstanceManager);
            }

            return roleInstanceManagers;
        }

        private void CancelAllPreviousTransfers(RoleInstanceDiagnosticManager roleInstanceManager)
        {
            foreach (var activeTransfer in roleInstanceManager.GetActiveTransfers())
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Stopping Pending Transfers for {0} with request id {1}\n",
                    activeTransfer.Key,
                    activeTransfer.Value.RequestId);

                this.WriteProgress(new ProgressRecord(0, "Diagnostic Instrumentation", message));

                foreach (var transferId in roleInstanceManager.CancelOnDemandTransfers(activeTransfer.Key))
                {
                    roleInstanceManager.EndOnDemandTransfer(transferId);
                }
            }
        }
    }
}