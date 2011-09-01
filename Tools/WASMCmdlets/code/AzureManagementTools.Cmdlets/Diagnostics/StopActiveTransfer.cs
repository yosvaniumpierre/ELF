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
    using System;
    using System.Collections.Generic;
    using System.Management.Automation;
    using Microsoft.WindowsAzure.Diagnostics;
    using Microsoft.WindowsAzure.Diagnostics.Management;

    /// <summary>
    /// Stops active on-demand transfer of the specified transfer id.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Stop, "ActiveTransfer")]
    public class StopActiveTransferCommand : DiagnosticsCmdletBase
    {
        [Parameter(Mandatory = true, HelpMessage = "The name of the role.")]
        [ValidateNotNullOrEmpty]
        public string RoleName { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, HelpMessage = "The role instance ID.")]
        public string InstanceId { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, HelpMessage = "The request ID.")]
        public string TransferId { get; set; }

        [Parameter(HelpMessage = "Name of the Data Buffer.")]
        public string DataBufferName { get; set; }

        public void StopActiveTransferProcess()
        {
            var roleInstanceManagers = new List<RoleInstanceDiagnosticManager>();

            if (string.IsNullOrEmpty(this.InstanceId))
            {
                roleInstanceManagers.AddRange(this.DiagnosticManager.GetRoleInstanceDiagnosticManagersForRole(this.RoleName));
            }
            else
            {
                roleInstanceManagers.Add(this.DiagnosticManager.GetRoleInstanceDiagnosticManager(this.RoleName, this.InstanceId));
            }
            
            if (!string.IsNullOrEmpty(this.TransferId))
            {
                if (string.IsNullOrEmpty(this.InstanceId))
                {
                    throw new ArgumentNullException("InstanceId");
                }
                
                roleInstanceManagers[0].EndOnDemandTransfer(new Guid(this.TransferId));
            }
            else if (!string.IsNullOrEmpty(this.DataBufferName))
            {
                foreach (var roleInstanceManager in roleInstanceManagers)
                {
                    var dataBufferName = (DataBufferName) Enum.Parse(typeof(DataBufferName), this.DataBufferName);
                    roleInstanceManager.CancelOnDemandTransfers(dataBufferName);
                }
            }
            else
            {
                foreach (var roleInstanceManager in roleInstanceManagers)
                {
                    var activetransfers = roleInstanceManager.GetActiveTransfers();

                    foreach (var activeTransfer in activetransfers)
                    {
                        roleInstanceManager.EndOnDemandTransfer(activeTransfer.Value.RequestId);
                    }
                }
            }
        }

        protected override void ProcessRecord()
        {
            this.StopActiveTransferProcess();
        }
    }
}