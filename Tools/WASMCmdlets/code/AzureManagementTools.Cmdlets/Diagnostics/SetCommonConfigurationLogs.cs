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
    using System.Management.Automation;
    using Microsoft.WindowsAzure.Diagnostics;

    /// <summary>
    /// Sets the common configuration values for all logging buffers.
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "CommonConfigurationLogs")]
    public class SetCommonConfigurationLogsCommand : InstrumentationCmdletBase
    {
        public SetCommonConfigurationLogsCommand()
        {
        }

        [Parameter(HelpMessage = "Sets the total amount of file system storage allocated for all logging buffers.")]
        public int OverallQuotaInMB { get; set; }

        public void SetCommonConfigurationLogsProcess()
        {
            this.PerformInstrumentation();
        }

        protected override void ProcessRecord()
        {
            this.SetCommonConfigurationLogsProcess();
        }

        protected override void SetConfiguration(DiagnosticMonitorConfiguration diagnosticConfig)
        {
            // The configuration change polling interval cannot be set remotely.
            // diagnosticConfig.ConfigurationChangePollInterval = TimeSpan.FromSeconds(this.ConfigurationChangePollInterval);
            diagnosticConfig.OverallQuotaInMB = this.OverallQuotaInMB;
        }
    }
}