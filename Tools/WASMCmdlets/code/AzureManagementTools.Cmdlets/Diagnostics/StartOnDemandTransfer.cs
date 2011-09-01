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
    using Microsoft.WindowsAzure.StorageClient;

    /// <summary>
    /// Starts an on-demand transfer of the specified data buffer.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Start, "OnDemandTransfer")]
    public class StartOnDemandTransferCommand : DiagnosticsCmdletBase
    {
        private OnDemandTransferOptions transferOptions;

        [Parameter(Mandatory = true, HelpMessage = "The name of the source buffer.")]
        [ValidateNotNullOrEmpty]
        [ValidateSet("DiagnosticInfrastructureLogs", "Directories", "Logs", "PerformanceCounters", "WindowsEventLogs")]
        public string DataBufferName { get; set; }

        [Parameter(Mandatory = true, HelpMessage = "The name of the role.")]
        [ValidateNotNullOrEmpty]
        public string RoleName { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, HelpMessage = "The role instance ID.")]
        public string InstanceId { get; set; }

        [Parameter(HelpMessage = "The start of the time window for which event data is to be transferred.")]
        public DateTime From { get; set; }

        [Parameter(HelpMessage = "The end of the time window for which event data is to be transferred.")]
        public DateTime To { get; set; }

        [Parameter(HelpMessage = "The filter level for event data that has been logged with level information.")]
        public LogLevel LogLevelFilter { get; set; }

        [Parameter(HelpMessage = "The name of the queue where transfer completion notification can optionally be sent.")]
        public string NotificationQueueName { get; set; }

        public IEnumerable<Guid> StartOnDemandTransferProcess()
        {   
            this.transferOptions = new OnDemandTransferOptions();
            this.transferOptions.LogLevelFilter = this.LogLevelFilter;

            if (!string.IsNullOrEmpty(this.NotificationQueueName))
            {
                this.transferOptions.NotificationQueueName = this.NotificationQueueName.ToLowerInvariant();

                var queueClient = this.StorageAccount.CreateCloudQueueClient();
                queueClient.RetryPolicy = RetryPolicies.Retry(3, TimeSpan.FromSeconds(5));

                var notificationQueue = queueClient.GetQueueReference(this.transferOptions.NotificationQueueName);
                notificationQueue.CreateIfNotExist();
            }

            if (this.From != null)
            {
                this.transferOptions.From = this.From.ToUniversalTime();
            }

            if (this.To != null)
            {
                this.transferOptions.To = this.To.ToUniversalTime();
            }

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

            var transferList = new List<Guid>();
            foreach (var roleInstanceManager in roleInstanceManagers)
            {
                var dataBuffer = (DataBufferName)Enum.Parse(typeof(DataBufferName), this.DataBufferName);

                var transferId = roleInstanceManager.BeginOnDemandTransfer(dataBuffer, this.transferOptions);
                transferList.Add(transferId);
            }

            return transferList;
        }

        protected override void ProcessRecord()
        {
            var transferList = this.StartOnDemandTransferProcess();

            this.WriteObject(transferList, true);
        }
    }
}