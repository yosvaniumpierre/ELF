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
    using System.Management.Automation;
    using Microsoft.WindowsAzure.Diagnostics;

    /// <summary>
    /// Gets the buffer configuration for the specified buffer name.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "DiagnosticConfiguration")]
    public class GetDiagnosticConfigurationCommand : DiagnosticsCmdletBase
    {
        [Parameter(Mandatory = true, HelpMessage = "The name of the source buffer.")]
        [ValidateNotNullOrEmpty]
        [ValidateSet("DiagnosticInfrastructureLogs", "Directories", "Logs", "PerformanceCounters", "WindowsEventLogs")]
        public string BufferName { get; set; }

        [Parameter(Mandatory = true, HelpMessage = "The name of the role.")]
        [ValidateNotNullOrEmpty]
        public string RoleName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The role instance ID.")]
        [ValidateNotNullOrEmpty]
        public string InstanceId { get; set; }

        public DiagnosticDataBufferConfiguration GetDiagnosticConfigurationProcess()
        {
            var roleInstanceManager = this.DiagnosticManager.GetRoleInstanceDiagnosticManager(this.RoleName, this.InstanceId);

            DiagnosticDataBufferConfiguration diagnosticConfiguration = null;
            var dataBufferName = (DataBufferName)Enum.Parse(typeof(DataBufferName), this.BufferName);
            var configuration = roleInstanceManager.GetCurrentConfiguration();

            if (configuration != null)
            {
                switch (dataBufferName)
                {
                    case DataBufferName.DiagnosticInfrastructureLogs:
                        diagnosticConfiguration = configuration.DiagnosticInfrastructureLogs;
                        break;
                    case DataBufferName.Directories:
                        diagnosticConfiguration = configuration.Directories;
                        break;
                    case DataBufferName.Logs:
                        diagnosticConfiguration = configuration.Logs;
                        break;
                    case DataBufferName.PerformanceCounters:
                        diagnosticConfiguration = configuration.PerformanceCounters;
                        break;
                    case DataBufferName.WindowsEventLogs:
                        diagnosticConfiguration = configuration.WindowsEventLog;
                        break;
                }
            }

            return diagnosticConfiguration;
        }

        protected override void ProcessRecord()
        {
            var diagnosticConfigurations = this.GetDiagnosticConfigurationProcess();

            this.WriteObject(diagnosticConfigurations, true);
        }
    }
}