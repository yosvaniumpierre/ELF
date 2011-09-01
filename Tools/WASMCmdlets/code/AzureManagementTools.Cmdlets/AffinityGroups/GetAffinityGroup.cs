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

namespace Microsoft.Samples.AzureManagementTools.PowerShell.AffinityGroups
{
    using System;
    using System.Management.Automation;
    using System.ServiceModel;
    using Microsoft.Samples.WindowsAzure.ServiceManagement;

    /// <summary>
    /// List the properties for the specified affinity group.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AffinityGroup")]
    public class GetAffinityGroupCommand : CmdletBase
    {
        public GetAffinityGroupCommand()
        {
        }

        public GetAffinityGroupCommand(IServiceManagement channel)
        {
            this.Channel = channel;
        }

        [Parameter(Position = 0, Mandatory = true, HelpMessage = "Affinity Group name")]
        [ValidateNotNullOrEmpty]
        public string Name
        {
            get;
            set;
        }

        public AffinityGroup GetAffinityGroupProcess()
        {
            AffinityGroup affinityGroup = null;

            try
            {
                affinityGroup = this.RetryCall(s => this.Channel.GetAffinityGroup(s, this.Name));
                if (affinityGroup != null && !string.IsNullOrEmpty(affinityGroup.Label))
                {
                    affinityGroup.Label = ServiceManagementHelper.DecodeFromBase64String(affinityGroup.Label);
                }
            }
            catch (CommunicationException ex)
            {
                this.WriteErrorDetails(ex);
            }

            return affinityGroup;
        }

        /// <summary>
        /// Executes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();

                WriteObject(this.GetAffinityGroupProcess(), true);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }
    }
}