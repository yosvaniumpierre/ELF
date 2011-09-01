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
    using Microsoft.Samples.AzureManagementTools.PowerShell.AffinityGroups;
    using Microsoft.Samples.WindowsAzure.ServiceManagement;

    /// <summary>
    /// List the properties for the specified hosted account.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "HostedProperties")]
    public class GetHostedPropertiesCommand : CmdletBase
    {
        public GetHostedPropertiesCommand()
        {
        }

        public GetHostedPropertiesCommand(IServiceManagement channel)
        {
            this.Channel = channel;
        }

        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Service name.")]
        [ValidateNotNullOrEmpty]
        public string ServiceName
        {
            get;
            set;
        }

        public HostedServiceProperties GetHostedPropertiesProcess()
        {
            HostedService hostedService = null;

            try
            {
                hostedService = this.RetryCall(s => this.Channel.GetHostedService(s, this.ServiceName));

                if (hostedService != null)
                {
                    if (string.IsNullOrEmpty(hostedService.HostedServiceProperties.Location) &&
                        !string.IsNullOrEmpty(hostedService.HostedServiceProperties.AffinityGroup))
                    {
                        var affinityGroupCommand = new GetAffinityGroupCommand(this.Channel)
                        {
                            SubscriptionId = this.SubscriptionId,
                            Certificate = this.Certificate,
                            Name = hostedService.HostedServiceProperties.AffinityGroup
                        };

                        var affinityGroup = affinityGroupCommand.GetAffinityGroupProcess();
                        hostedService.HostedServiceProperties.Location = affinityGroup.Location;
                        hostedService.HostedServiceProperties.AffinityGroup = affinityGroup.Label;
                    }

                    if (!string.IsNullOrEmpty(hostedService.HostedServiceProperties.Label))
                    {
                        hostedService.HostedServiceProperties.Label = ServiceManagementHelper.DecodeFromBase64String(hostedService.HostedServiceProperties.Label);
                    }
                }
            }
            catch (CommunicationException ex)
            {
                this.WriteErrorDetails(ex);
            }

            return hostedService.HostedServiceProperties;
        }

        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();
                
                WriteObject(this.GetHostedPropertiesProcess());
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }
    }
}
