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
    using System.Globalization;
    using System.Management.Automation;
    using System.ServiceModel;
    using Microsoft.Samples.WindowsAzure.ServiceManagement;

    /// <summary>
    /// Swaps the deployments in production and stage.
    /// </summary>
    [Cmdlet(VerbsCommon.Move, "Deployment")]
    public class MoveDeploymentCommand : CmdletBase
    {
        public MoveDeploymentCommand()
        {
        }

        public MoveDeploymentCommand(IServiceManagement channel)
        {
            this.Channel = channel;
        }

        [Parameter(Position = 0, HelpMessage = "Name for the new deployment in production.")]
        public string DeploymentNameInProduction
        {
            get;
            set;
        }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Service name.")]
        [ValidateNotNullOrEmpty]
        public string ServiceName
        {
            get;
            set;
        }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Source deployment name.")]
        [ValidateNotNullOrEmpty]
        public string Name
        {
            get;
            set;
        }

        public string MoveDeploymentProcess()
        {
            if (string.IsNullOrEmpty(this.DeploymentNameInProduction))
            {
                this.DeploymentNameInProduction = this.EnsureDeploymentNameInProduction();
            }

            var swapDeploymentInput = new SwapDeploymentInput()
            {
                SourceDeployment = this.Name,
                Production = this.DeploymentNameInProduction
            };

            using (new OperationContextScope((IContextChannel)Channel))
            {
                try
                {
                    this.RetryCall(s => this.Channel.SwapDeployment(s, this.ServiceName, swapDeploymentInput));
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

                var operationId = this.MoveDeploymentProcess();

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

        private string EnsureDeploymentNameInProduction()
        {
            var deployment = new Deployment();

            try
            {
                deployment = this.RetryCall(s => this.Channel.GetDeploymentBySlot(s, this.ServiceName, DeploymentSlotType.Production));
            }
            catch (CommunicationException)
            {
            }

            return string.IsNullOrEmpty(deployment.Name) ? Guid.NewGuid().ToString() : deployment.Name;
        }
    }
}
