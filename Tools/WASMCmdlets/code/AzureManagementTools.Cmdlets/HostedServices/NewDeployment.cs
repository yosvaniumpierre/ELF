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
    using System.IO;
    using System.Management.Automation;
    using System.ServiceModel;
    using Microsoft.Samples.AzureManagementTools.PowerShell.Helpers;
    using Microsoft.Samples.WindowsAzure.ServiceManagement;

    /// <summary>
    /// Create a new deployment. Note that there shouldn't be a deployment 
    /// of the same name or in the same slot when executing this command.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "Deployment")]
    public class NewDeploymentCommand : CmdletBase
    {
        public NewDeploymentCommand()
        {
        }

        public NewDeploymentCommand(IServiceManagement channel)
        {
            this.Channel = channel;
        }

        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Deployment slot. Staging | Production")]
        [ValidateSet(new string[] { "Staging", "Production" })]
        public string Slot
        {
            get;
            set;
        }

        [Parameter(Position = 1, Mandatory = true, HelpMessage = "Package location. This parameter should have the path or URI to a .cspkg in blob storage whose storage account is part of the same subscription/project.")]
        [ValidateNotNullOrEmpty]
        public string Package
        {
            get;
            set;
        }

        [Parameter(Position = 2, Mandatory = true, HelpMessage = "Configuration file path. This parameter should specifiy a .cscfg file on disk.")]
        [ValidateNotNullOrEmpty]
        public string Configuration
        {
            get;
            set;
        }

        [Parameter(Position = 3, Mandatory = true, HelpMessage = "Label name for the new deployment.")]
        [ValidateNotNullOrEmpty]
        public string Label
        {
            get;
            set;
        }

        [Parameter(Position = 4, HelpMessage = "Deployment name.")]
        public string Name
        {
            get;
            set;
        }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Hosted service name.")]
        [ValidateNotNullOrEmpty]
        public string ServiceName
        {
            get;
            set;
        }

        [Parameter(HelpMessage = "Storage service name.")]
        public string StorageServiceName
        {
            get;
            set;
        }

        public string NewDeploymentProcess()
        {
            this.ValidateParameters();

            Uri packageUrl;
            if (this.Package.StartsWith(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) ||
                this.Package.StartsWith(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
            {
                packageUrl = new Uri(this.Package);
            }
            else
            {
                var storageName = string.IsNullOrEmpty(this.StorageServiceName) ? this.ServiceName : this.StorageServiceName;
                packageUrl = this.RetryCall(s =>
                    AzureBlob.UploadPackageToBlob(
                    this.CreateChannel(),
                    storageName,
                    s,
                    this.ResolvePath(this.Package)));
            }

            var configurationFullPath = this.ResolvePath(this.Configuration);

            var deploymentInput = new CreateDeploymentInput
            {
                PackageUrl = packageUrl,
                Configuration = Utility.GetConfiguration(configurationFullPath),
                Label = ServiceManagementHelper.EncodeToBase64String(this.Label),
                Name = this.Name
            };

            using (new OperationContextScope((IContextChannel)Channel))
            {
                try
                {
                    this.RetryCall(s => this.Channel.CreateOrUpdateDeployment(
                        s,
                        this.ServiceName,
                        this.Slot,
                        deploymentInput));
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

                var operationId = this.NewDeploymentProcess();

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

        private void ValidateParameters()
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                this.Name = Guid.NewGuid().ToString();
            }
        }
    }
}