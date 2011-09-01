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
    using System.Xml.Linq;
    using Microsoft.Samples.AzureManagementTools.PowerShell.Helpers;
    using Microsoft.Samples.AzureManagementTools.PowerShell.Model;
    using Microsoft.Samples.WindowsAzure.ServiceManagement;

    /// <summary>
    /// Initiates an in-place upgrade of the specified deployment.
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "Deployment")]
    public class SetDeploymentCommand : CmdletBase
    {
        public SetDeploymentCommand()
        {
        }

        public SetDeploymentCommand(IServiceManagement channel)
        {
            this.Channel = channel;
        }

        [Parameter(HelpMessage = "Name of role to upgrade.")]
        public string RoleName
        {
            get;
            set;
        }

        [Parameter(Mandatory = true, HelpMessage = "Package location. This parameter should have the path or URI to a .cspkg in blob storage whose storage account is part of the same subscription/project.")]
        [ValidateNotNullOrEmpty]
        public string Package
        {
            get;
            set;
        }

        [Parameter(Mandatory = true, HelpMessage = "Label name for the new deployment.")]
        [ValidateNotNullOrEmpty]
        public string Label
        {
            get;
            set;
        }

        [Parameter(HelpMessage = "Upgrade mode. Auto | Manual")]
        [ValidateSet(new string[] { UpgradeType.Auto, UpgradeType.Manual })]
        public string Mode
        {
            get;
            set;
        }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Service name")]
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

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Deployment slot. Staging | Production")]
        [ValidateSet(new string[] { "Staging", "Production" })]
        public string Slot
        {
            get;
            set;
        }

        [Parameter(HelpMessage = "Configuration file path. This parameter should specifiy a .cscfg file on disk.")]
        [ValidateNotNullOrEmpty]
        public string Configuration
        {
            get;
            set;
        }

        [Parameter(ValueFromPipeline = true)]
        public DeploymentInfoContext DeploymentInfo
        {
            get;
            set;
        }

        [Parameter(Position = 15)]
        public ScriptBlock ConfigurationScript
        {
            get;
            set;
        }

        public string SetDeploymentProcess()
        {
            this.ValidateParameters();

            string configString = string.Empty;
            if (this.ConfigurationScript != null)
            {
                var selectedRoleConfiguration = this.DeploymentInfo.RolesConfiguration[this.RoleName];
                this.SessionState.PSVariable.Set("SelectedRoleConfiguration", selectedRoleConfiguration);

                this.ConfigurationScript.InvokeReturnAsIs(null);

                selectedRoleConfiguration = (RoleConfiguration)((PSObject)this.SessionState.PSVariable.Get("SelectedRoleConfiguration").Value).ImmediateBaseObject;
                this.DeploymentInfo.RolesConfiguration[this.RoleName] = selectedRoleConfiguration;
                this.SessionState.PSVariable.Remove("SelectedRoleConfiguration");

                var xmlDocument = this.DeploymentInfo.SerializeRolesConfiguration();
                var xml = xmlDocument.ToString(SaveOptions.DisableFormatting);
                configString = ServiceManagementHelper.EncodeToBase64String(xml);
            }
            else
            {
                if (string.IsNullOrEmpty(this.Configuration))
                {
                    configString = this.DeploymentInfo.Configuration;
                }
                else
                {
                    var configurationFullPath = this.ResolvePath(this.Configuration);
                    configString = Utility.GetConfiguration(configurationFullPath);
                }
            }

            Uri packageUrl;
            if (this.Package.StartsWith(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) ||
                this.Package.StartsWith(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
            {
                packageUrl = new Uri(this.Package);
            }
            else
            {
                string storageName = string.IsNullOrEmpty(this.StorageServiceName) ? this.ServiceName : this.StorageServiceName;
                packageUrl = this.RetryCall(s => AzureBlob.UploadPackageToBlob(
                    this.CreateChannel(),
                    storageName,
                    s,
                    this.ResolvePath(this.Package)));
            }

            var upgradeDeploymentInput = new UpgradeDeploymentInput
            {
                Mode = this.Mode,
                Configuration = configString,
                PackageUrl = packageUrl,
                Label = ServiceManagementHelper.EncodeToBase64String(this.Label)
            };

            if (!string.IsNullOrEmpty(this.RoleName))
            {
                upgradeDeploymentInput.RoleToUpgrade = this.RoleName;
            }

            using (new OperationContextScope((IContextChannel)Channel))
            {
                try
                {
                    this.RetryCall(s => this.Channel.UpgradeDeploymentBySlot(
                        s,
                        this.ServiceName,
                        this.Slot,
                        upgradeDeploymentInput));
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

                var operationId = this.SetDeploymentProcess();

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
            if (string.IsNullOrEmpty(this.Mode))
            {
                this.Mode = "Auto";
            }
        }
    }
}
