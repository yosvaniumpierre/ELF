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
    /// Displays the primary and secondary keys for the account. Should have 
    /// the storage account resource specified.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "StorageKeys")]
    public class GetStorageKeysCommand : CmdletBase
    {
        public GetStorageKeysCommand()
        {
        }

        public GetStorageKeysCommand(IServiceManagement channel)
        {
            this.Channel = channel;
        }

        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Service name.")]
        [ValidateNotNullOrEmpty]
        public string ServiceName
        {
            get;
            set;
        }

        public StorageServiceKeys GetStorageKeysProcess()
        {
            StorageService storageService = null;
            
            try
            {
                storageService = this.RetryCall(s => this.Channel.GetStorageKeys(s, this.ServiceName));
            }
            catch (CommunicationException ex)
            {
                this.WriteErrorDetails(ex);
            }

            return storageService.StorageServiceKeys;
        }

        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();

                WriteObject(this.GetStorageKeysProcess(), true);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }
    }
}