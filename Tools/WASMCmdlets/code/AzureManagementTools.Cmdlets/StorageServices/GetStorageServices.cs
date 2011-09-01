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
    using Microsoft.Samples.WindowsAzure.ServiceManagement;

    /// <summary>
    /// Lists all storage services underneath the subscription.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "StorageServices")]
    public class GetStorageServicesCommand : CmdletBase
    {
        public GetStorageServicesCommand()
        {
        }

        public GetStorageServicesCommand(IServiceManagement channel)
        {
            this.Channel = channel;
        }

        public StorageServiceList GetStorageServicesProcess()
        {
            StorageServiceList storageServices = null;

            try
            {
                storageServices = this.RetryCall(s => this.Channel.ListStorageServices(s));
            }
            catch (CommunicationException ex)
            {
                this.WriteErrorDetails(ex);
            }

            return storageServices;
        }

        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();

                var storageServices = this.GetStorageServicesProcess();
                foreach (var service in storageServices)
                {
                    var ctx = new ManagementOperationContext();
                    ctx.SubscriptionId = this.SubscriptionId;
                    ctx.ServiceName = service.ServiceName;
                    ctx.Certificate = this.Certificate;

                    WriteObject(ctx);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }
    }
}
