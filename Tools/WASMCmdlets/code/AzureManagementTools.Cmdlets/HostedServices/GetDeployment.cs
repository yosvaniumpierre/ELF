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
    using Microsoft.Samples.AzureManagementTools.PowerShell.Model;
    using Microsoft.Samples.WindowsAzure.ServiceManagement;

    /// <summary>
    /// View details of a specified deployment.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "Deployment")]
    public class GetDeploymentCommand : CmdletBase
    {
        public GetDeploymentCommand()
        {
        }

        public GetDeploymentCommand(IServiceManagement channel)
        {
            this.Channel = channel;
        }

        [Parameter(Position = 0, Mandatory = true, HelpMessage = "Deployment slot. Staging | Production")]
        [ValidateSet(new string[] { "Staging", "Production" })]
        public string Slot
        {
            get;
            set;
        }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Service name.")]
        public string ServiceName
        {
            get;
            set;
        }

        public Deployment GetDeploymentProcess()
        {
            var deployment = new Deployment();

            try
            {
                deployment = this.RetryCall(s => this.Channel.GetDeploymentBySlot(s, this.ServiceName, this.Slot));
            }
            catch (EndpointNotFoundException)
            {
                // Empty Deployment
            }
            catch (CommunicationException ex)
            {
                this.WriteErrorDetails(ex);
            }

            return deployment;
        }

        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();

                var deployment = this.GetDeploymentProcess();

                if (string.IsNullOrEmpty(deployment.DeploymentSlot))
                {
                    deployment.DeploymentSlot = this.Slot;
                }

                var ctx = new DeploymentInfoContext(deployment);
                ctx.SubscriptionId = this.SubscriptionId;
                ctx.ServiceName = this.ServiceName;
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