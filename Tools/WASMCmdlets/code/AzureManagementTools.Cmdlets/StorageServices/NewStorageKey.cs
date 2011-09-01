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
    /// Regenerates storage keys with the key-type parameter specifying 
    /// which key to regenerate. Should have the storage account resource specified.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "StorageKey")]
    public class NewStorageKeyCommand : CmdletBase
    {
        public NewStorageKeyCommand()
        {
        }

        public NewStorageKeyCommand(IServiceManagement channel)
        {
            this.Channel = channel;
        }

        [Parameter(Position = 0, Mandatory = true, HelpMessage = "Key to regenerate. Primary | Secondary")]
        [ValidateSet(new string[] { "Primary", "Secondary" })]
        public string KeyType
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

        public StorageServiceKeys NewStorageKeyProcess()
        {
            StorageService storageService = null;
            var regenerateKeys = new RegenerateKeys();
            regenerateKeys.KeyType = this.KeyType;

            try
            {
                storageService = this.RetryCall(s => this.Channel.RegenerateStorageServiceKeys(s, this.ServiceName, regenerateKeys));
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

                WriteObject(this.NewStorageKeyProcess(), true);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }
    }
}
