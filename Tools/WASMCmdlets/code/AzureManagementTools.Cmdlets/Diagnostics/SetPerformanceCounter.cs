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
    /// Sets the buffer configuration for performance counter data.
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "PerformanceCounter")]
    public class SetPerformanceCounterCommand : InstrumentationCmdletBase
    {
        public SetPerformanceCounterCommand()
        {
        }

        [Parameter(HelpMessage = "The performance counters specifier using standard Windows counter syntax.")]
        [ValidateNotNullOrEmpty]
        public PerformanceCounterConfiguration[] PerformanceCounters { get; set; }

        public void SetPerformanceCounterProcess()
        {
            this.PerformInstrumentation();
        }

        protected override void ProcessRecord()
        {
            this.SetPerformanceCounterProcess();
        }

        protected override void SetConfiguration(DiagnosticMonitorConfiguration diagnosticConfig)
        {
            diagnosticConfig.PerformanceCounters.BufferQuotaInMB = this.BufferQuotaInMB;
            diagnosticConfig.PerformanceCounters.ScheduledTransferPeriod = TimeSpan.FromMinutes(this.TransferPeriod);

            diagnosticConfig.PerformanceCounters.DataSources.Clear();

            if (this.PerformanceCounters != null)
            {
                foreach (var performanceCounters in this.PerformanceCounters)
                {
                    var performanceCounterConfiguration = new PerformanceCounterConfiguration
                    {
                        CounterSpecifier = performanceCounters.CounterSpecifier,
                        SampleRate = performanceCounters.SampleRate
                    };

                    diagnosticConfig.PerformanceCounters.DataSources.Add(performanceCounterConfiguration);
                }
            }
        }
    }
}