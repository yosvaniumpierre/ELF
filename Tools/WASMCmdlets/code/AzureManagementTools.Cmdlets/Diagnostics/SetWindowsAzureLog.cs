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
    /// Sets the buffer configuration for basic Windows Azure logs.
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "WindowsAzureLog")]
    public class SetWindowsAzureLogCommand : InstrumentationCmdletBase
    {
        public SetWindowsAzureLogCommand()
        {
        }

        [Parameter(HelpMessage = "The logging level by which to filter records when performing a scheduled transfer.")]
        public LogLevel LogLevelFilter { get; set; }

        public void SetWindowsAzureLogProcess()
        {
            this.PerformInstrumentation();
        }

        protected override void ProcessRecord()
        {
            this.SetWindowsAzureLogProcess();
        }

        protected override void SetConfiguration(DiagnosticMonitorConfiguration diagnosticConfig)
        {
            diagnosticConfig.Logs.BufferQuotaInMB = this.BufferQuotaInMB;
            diagnosticConfig.Logs.ScheduledTransferPeriod = TimeSpan.FromMinutes(this.TransferPeriod);
            diagnosticConfig.Logs.ScheduledTransferLogLevelFilter = this.LogLevelFilter;
        }
    }
}