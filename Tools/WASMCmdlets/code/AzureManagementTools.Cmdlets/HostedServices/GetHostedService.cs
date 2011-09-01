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
    /// Retrieve a specified hosted account.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "HostedService")]
    public class GetHostedServiceCommand : CmdletBase
    {
        public GetHostedServiceCommand()
        {
        }

        public GetHostedServiceCommand(IServiceManagement channel)
        {
            this.Channel = channel;
        }

        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string ServiceName
        {
            get;
            set;
        }

        public HostedService GetHostedServiceProcess()
        {
            HostedService hostedService = null;

            try
            {
                hostedService = this.RetryCall(s => this.Channel.GetHostedService(s, this.ServiceName));
            }
            catch (CommunicationException ex)
            {
                this.WriteErrorDetails(ex);
            }

            return hostedService;
        }

        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();

                var service = this.GetHostedServiceProcess();
                var ctx = new Model.HostedServiceContext(service);
                ctx.SubscriptionId = this.SubscriptionId;
                ctx.ServiceName = service.ServiceName != null ? service.ServiceName : this.ServiceName;
                ctx.Certificate = this.Certificate;

                WriteObject(ctx);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }
    }
}
