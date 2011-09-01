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

namespace Microsoft.Samples.AzureManagementTools.PowerShell.StorageServices
{
    using System;
    using System.Management.Automation;
    using System.ServiceModel;
    using Microsoft.Samples.AzureManagementTools.PowerShell.AffinityGroups;
    using Microsoft.Samples.WindowsAzure.ServiceManagement;

    /// <summary>
    /// List the properties for the specified storage account.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "StorageProperties")]
    public class GetStoragePropertiesCommand : CmdletBase
    {
        public GetStoragePropertiesCommand()
        {
        }

        public GetStoragePropertiesCommand(IServiceManagement channel)
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

        public StorageServiceProperties GetStoragePropertiesProcess()
        {
            StorageService storageService = null;

            try
            {
                storageService = this.RetryCall(s => this.Channel.GetStorageService(s, this.ServiceName));
                if (storageService != null)
                {
                    if (string.IsNullOrEmpty(storageService.StorageServiceProperties.Location) &&
                        !string.IsNullOrEmpty(storageService.StorageServiceProperties.AffinityGroup))
                    {
                        var affinityGroupCommand = new GetAffinityGroupCommand(this.Channel)
                        {
                            SubscriptionId = this.SubscriptionId,
                            Certificate = this.Certificate,
                            Name = storageService.StorageServiceProperties.AffinityGroup
                        };

                        var affinityGroup = affinityGroupCommand.GetAffinityGroupProcess();
                        storageService.StorageServiceProperties.Location = affinityGroup.Location;
                        storageService.StorageServiceProperties.AffinityGroup = affinityGroup.Label;
                    }

                    if (!string.IsNullOrEmpty(storageService.StorageServiceProperties.Label))
                    {
                        storageService.StorageServiceProperties.Label = ServiceManagementHelper.DecodeFromBase64String(storageService.StorageServiceProperties.Label);
                    }
                }
            }
            catch (CommunicationException ex)
            {
                this.WriteErrorDetails(ex);
            }

            return storageService.StorageServiceProperties;
        }

        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();

                WriteObject(this.GetStoragePropertiesProcess(), true);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }
    }
}
