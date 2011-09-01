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

namespace Microsoft.Samples.AzureManagementTools.PowerShell.HostedServices
{
    using System;
    using System.Management.Automation;
    using System.ServiceModel;
    using Microsoft.Samples.WindowsAzure.ServiceManagement;

    /// <summary>
    /// Lists the versions of the guest operating system that are currently available in Windows Azure.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "OSVersions")]
    public class GetOSVersionsCommand : CmdletBase
    {
        public GetOSVersionsCommand()
        {
        }

        public GetOSVersionsCommand(IServiceManagement channel)
        {
            this.Channel = channel;
        }

        public OperatingSystemList GetOSVersionsProcess()
        {
            var operatingSystems = default(OperatingSystemList);

            try
            {
                operatingSystems = this.RetryCall(s => this.Channel.ListOperatingSystems(s));
            }
            catch (CommunicationException ex)
            {
                this.WriteErrorDetails(ex);
            }

            return operatingSystems;
        }

        protected override void ProcessRecord()
        {
            try
            {
                if (this.Channel == null)
                {
                    this.Channel = this.CreateChannel("2010-04-01");
                }

                var operatingSystems = this.GetOSVersionsProcess();
                foreach (var operatingSystem in operatingSystems)
                {
                    if (!string.IsNullOrEmpty(operatingSystem.Label))
                    {
                        operatingSystem.Label = ServiceManagementHelper.DecodeFromBase64String(operatingSystem.Label);
                    }
                }

                WriteObject(operatingSystems, true);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }
    }
}