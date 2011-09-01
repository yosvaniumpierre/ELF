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
    using Microsoft.Samples.WindowsAzure.ServiceManagement.ResourceModel;

    /// <summary>
    /// Gets the common configuration values for all logging buffers.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "CommonConfigurationLogs")]
    public class GetCommonConfigurationLogsCommand : DiagnosticsCmdletBase
    {
        [Parameter(Mandatory = true, HelpMessage = "The name of the role.")]
        [ValidateNotNullOrEmpty]
        public string RoleName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The role instance ID.")]
        [ValidateNotNullOrEmpty]
        public string InstanceId { get; set; }

        public DiagnosticCommonConfiguration GetCommonConfigurationLogsProcess()
        {
            DiagnosticCommonConfiguration commonConfiguration = null;
            var roleInstanceManager = this.DiagnosticManager.GetRoleInstanceDiagnosticManager(this.RoleName, this.InstanceId);
            var configuration = roleInstanceManager.GetCurrentConfiguration();

            if (configuration != null)
            {
                commonConfiguration = new DiagnosticCommonConfiguration
                {
                    OverallQuotaInMB = configuration.OverallQuotaInMB,
                    ConfigurationChangePollInterval = configuration.ConfigurationChangePollInterval
                };
            }

            return commonConfiguration;
        }

        protected override void ProcessRecord()
        {
            var commonConfiguration = this.GetCommonConfigurationLogsProcess();

            this.WriteObject(commonConfiguration);
        }
    }
}