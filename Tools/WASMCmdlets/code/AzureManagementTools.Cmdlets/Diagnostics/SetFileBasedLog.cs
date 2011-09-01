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
    /// Sets the buffer configuration for file-based logs.
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "FileBasedLog")]
    public class SetFileBasedLogCommand : InstrumentationCmdletBase
    {
        public SetFileBasedLogCommand()
        {
        }

        [Parameter(HelpMessage = "List of configured directories for file-based logs.")]
        [ValidateNotNullOrEmpty]
        public DirectoryConfiguration[] DirectoriesConfiguration { get; set; }

        public void SetFileBasedLogProcess()
        {
            this.PerformInstrumentation();
        }

        protected override void ProcessRecord()
        {
            this.SetFileBasedLogProcess();
        }

        protected override void SetConfiguration(DiagnosticMonitorConfiguration diagnosticConfig)
        {
            diagnosticConfig.Directories.BufferQuotaInMB = this.BufferQuotaInMB;
            diagnosticConfig.Directories.ScheduledTransferPeriod = TimeSpan.FromMinutes(this.TransferPeriod);
            
            diagnosticConfig.Directories.DataSources.Clear();

            if (this.DirectoriesConfiguration != null)
            {
                foreach (var directory in this.DirectoriesConfiguration)
                {
                    var directoryConfiguration = new DirectoryConfiguration
                    {
                        Path = directory.Path,
                        Container = directory.Container,
                        DirectoryQuotaInMB = directory.DirectoryQuotaInMB
                    };

                    diagnosticConfig.Directories.DataSources.Add(directoryConfiguration);
                }
            }
        }
    }
}