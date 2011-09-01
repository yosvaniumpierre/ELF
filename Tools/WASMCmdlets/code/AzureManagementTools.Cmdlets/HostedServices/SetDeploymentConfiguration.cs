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
    /// Change the deployment's configuration.
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "DeploymentConfiguration")]
    public class SetDeploymentConfigurationCommand : CmdletBase
    {
        public SetDeploymentConfigurationCommand()
        {
        }

        public SetDeploymentConfigurationCommand(IServiceManagement channel)
        {
            this.Channel = channel;
        }

        [Parameter(Position = 0)]
        public ScriptBlock ScriptConfiguration
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

        [Parameter(Position = 2, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "This parameter should specifiy a .cscfg file on disk.")]
        [ValidateNotNullOrEmpty]
        public string Configuration
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

        [Parameter(ValueFromPipeline = true)]
        public DeploymentInfoContext DeploymentInfo
        {
            get;
            set;
        }

        public string SetDeploymentConfigurationProcess()
        {
            string configString = string.Empty;
            if (this.ScriptConfiguration != null)
            {
                var underScoreVariable = this.SessionState.PSVariable.Get("_");
                this.SessionState.PSVariable.Set("_", this.DeploymentInfo);
                this.ScriptConfiguration.InvokeReturnAsIs(null);
                this.SessionState.PSVariable.Set("_", underScoreVariable);

                var xmlDocument = this.DeploymentInfo.SerializeRolesConfiguration();
                var xml = xmlDocument.ToString(SaveOptions.DisableFormatting);
                configString = ServiceManagementHelper.EncodeToBase64String(xml);
            }
            else
            {
                var configurationFullPath = this.ResolvePath(this.Configuration);
                configString = Utility.GetConfiguration(configurationFullPath);
            }

            var changeConfiguration = new ChangeConfigurationInput
            {
                Configuration = configString
            };

            using (new OperationContextScope((IContextChannel)Channel))
            {
                try
                {
                    this.RetryCall(s => this.Channel.ChangeConfigurationBySlot(
                        s,
                        this.ServiceName,
                        this.Slot,
                        changeConfiguration));
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

                var operationId = this.SetDeploymentConfigurationProcess();

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
