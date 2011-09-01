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
    using Microsoft.Samples.WindowsAzure.ServiceManagement.ResourceModel;
    using Microsoft.WindowsAzure.Diagnostics;
    using Microsoft.WindowsAzure.Diagnostics.Management;
    using Microsoft.WindowsAzure.StorageClient;

    /// <summary>
    /// Returns the set of active transfers, with associated transfer information.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "ActiveTransfers")]
    public class GetActiveTransfersCommand : DiagnosticsCmdletBase
    {
        [Parameter(Mandatory = true, HelpMessage = "The name of the role.")]
        [ValidateNotNullOrEmpty]
        public string RoleName { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, HelpMessage = "The role instance ID.")]
        public string InstanceId { get; set; }

        public IEnumerable<ActiveTransfer> GetActiveTransfersProcess()
        {
            var roleInstanceManagers = new List<RoleInstanceDiagnosticManager>();

            if (string.IsNullOrEmpty(this.InstanceId))
            {
                // Get the role instance diagnostics manager for all instance of the a role
                roleInstanceManagers.AddRange(this.DiagnosticManager.GetRoleInstanceDiagnosticManagersForRole(this.RoleName));
            }
            else
            {
                roleInstanceManagers.Add(this.DiagnosticManager.GetRoleInstanceDiagnosticManager(this.RoleName, this.InstanceId));
            }

            var listTransfers = new List<ActiveTransfer>();
            foreach (var roleInstanceManager in roleInstanceManagers)
            {
                IDictionary<DataBufferName, OnDemandTransferInfo> activeTransfers = roleInstanceManager.GetActiveTransfers();
            
                foreach (KeyValuePair<DataBufferName, OnDemandTransferInfo> tranfer in activeTransfers)
                {
                    var dataBufferName = tranfer.Key;
                    var transferInfo = tranfer.Value;

                    var activeTransfer = new ActiveTransfer
                    {
                        DataBufferName = dataBufferName.ToString(),
                        NotificationQueueName = transferInfo.NotificationQueueName,
                        RequestId = transferInfo.RequestId.ToString(),
                        DeploymentId = transferInfo.DeploymentId,
                        RoleName = transferInfo.RoleName,
                        InstanceId = transferInfo.RoleInstanceId,
                        CurrentStatus = this.RetrieveCurrentStatus(transferInfo.NotificationQueueName, transferInfo.RequestId.ToString())
                    };

                    listTransfers.Add(activeTransfer);
                }
            }

            return listTransfers;
        }

        protected override void ProcessRecord()
        {
            var activeTransfers = this.GetActiveTransfersProcess();
            this.WriteObject(activeTransfers, true);
        }

        private string RetrieveCurrentStatus(string notificationQueueName, string requestId)
        {
            if (!string.IsNullOrEmpty(notificationQueueName) && !string.IsNullOrEmpty(requestId))
            {
                if (this.IsPublishRequestFinished(new Guid(requestId), notificationQueueName))
                {
                    return "Published (OK to end/cancel)";
                }

                return "Not yet Published (Do not end/cancel)";
            }

            return "Not available (no queue name)";
        }

        private bool IsPublishRequestFinished(Guid requestId, string queueName)
        {
            CloudQueue queue = this.StorageAccount.CreateCloudQueueClient().GetQueueReference(queueName);

            if (queue.Exists())
            {
                foreach (CloudQueueMessage msg in queue.GetMessages(0x20))
                {
                    OnDemandTransferInfo info = OnDemandTransferInfo.FromQueueMessage(msg);
                    if (requestId == info.RequestId)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}