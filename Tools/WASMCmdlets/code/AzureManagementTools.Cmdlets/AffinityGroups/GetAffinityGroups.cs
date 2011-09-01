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
    /// Lists all affinity groups in the subscription.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AffinityGroups")]
    public class GetAffinityGroupsCommand : CmdletBase
    {
        public GetAffinityGroupsCommand()
        {
        }

        public GetAffinityGroupsCommand(IServiceManagement channel)
        {
            this.Channel = channel;
        }

        public AffinityGroupList GetAffinityGroupsProcess()
        {
            AffinityGroupList affinityGroups = null;

            try
            {
                affinityGroups = this.RetryCall(s => this.Channel.ListAffinityGroups(s));
                foreach (var affinityGroup in affinityGroups)
                {
                    if (!string.IsNullOrEmpty(affinityGroup.Label))
                    {
                        affinityGroup.Label = ServiceManagementHelper.DecodeFromBase64String(affinityGroup.Label);
                    }
                }
            }
            catch (CommunicationException ex)
            {
                this.WriteErrorDetails(ex);
            }

            return affinityGroups;
        }

        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();

                WriteObject(this.GetAffinityGroupsProcess(), true);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }
    }
}