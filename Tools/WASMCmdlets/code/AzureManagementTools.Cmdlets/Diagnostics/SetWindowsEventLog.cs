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
    /// Sets the buffer configuration for Windows event logs.
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "WindowsEventLog")]
    public class SetWindowsEventLogCommand : InstrumentationCmdletBase
    {
        public SetWindowsEventLogCommand()
        {
        }

        [Parameter(HelpMessage = "The name of the Windows Event Logs.")]
        [ValidateNotNullOrEmpty]
        public string[] EventLogs { get; set; }

        [Parameter(HelpMessage = "The logging level by which to filter records when performing a scheduled transfer.")]
        public LogLevel LogLevelFilter { get; set; }

        public void SetWindowsEventLogProcess()
        {
            this.PerformInstrumentation();
        }

        protected override void ProcessRecord()
        {
            this.SetWindowsEventLogProcess();
        }

        protected override void SetConfiguration(DiagnosticMonitorConfiguration diagnosticConfig)
        {
            diagnosticConfig.WindowsEventLog.BufferQuotaInMB = this.BufferQuotaInMB;
            diagnosticConfig.WindowsEventLog.ScheduledTransferPeriod = TimeSpan.FromMinutes(this.TransferPeriod);
            diagnosticConfig.WindowsEventLog.ScheduledTransferLogLevelFilter = this.LogLevelFilter;

            diagnosticConfig.WindowsEventLog.DataSources.Clear();

            if (this.EventLogs != null)
            {
                foreach (var dataSource in this.EventLogs)
                {
                    diagnosticConfig.WindowsEventLog.DataSources.Add(dataSource);
                }
            }
        }
    }
}