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
    /// Walks the specified upgrade domain.
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "WalkUpgradeDomain")]
    public class SetWalkUpgradeDomainCommand : CmdletBase
    {
        public SetWalkUpgradeDomainCommand()
        {
        }

        public SetWalkUpgradeDomainCommand(IServiceManagement channel)
        {
            this.Channel = channel;
        }

        [Parameter(Position = 0, Mandatory = true, HelpMessage = "Domain number.")]
        [ValidateNotNullOrEmpty]
        public int DomainNumber
        {
            get;
            set;
        }

        [Parameter(Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Deployment slot. Staging | Production")]
        [ValidateSet(new string[] { "Staging", "Production" })]
        public string Slot
        {
            get;
            set;
        }

        [Parameter(Position = 2, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Service name")]
        [ValidateNotNullOrEmpty]
        public string ServiceName
        {
            get;
            set;
        }

        public string SetWalkUpgradeDomainProcess()
        {
            var walkUpgradeDomain = new WalkUpgradeDomainInput()
            {
                UpgradeDomain = this.DomainNumber
            };

            using (new OperationContextScope((IContextChannel)Channel))
            {
                try
                {
                    this.RetryCall(s => this.Channel.WalkUpgradeDomainBySlot(
                        s,
                        this.ServiceName,
                        this.Slot,
                        walkUpgradeDomain));
                }
                catch (CommunicationException ex)
                {
                    this.WriteErrorDetails(ex);
                }

                return RetrieveOperationId();
            }
        }

        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();

                var operationId = this.SetWalkUpgradeDomainProcess();

                var ctx = new ManagementOperationContext();
                ctx.SubscriptionId = this.SubscriptionId;
                ctx.ServiceName = this.ServiceName;
                ctx.Certificate = this.Certificate;
                ctx.OperationId = operationId;

                WriteObject(ctx);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }
    }
}
