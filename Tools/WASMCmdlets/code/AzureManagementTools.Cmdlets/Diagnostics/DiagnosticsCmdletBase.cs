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

namespace Microsoft.Samples.AzureManagementTools.PowerShell.Diagnostics
{
    using System.Management.Automation;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Diagnostics.Management;

    public class DiagnosticsCmdletBase : PSCmdlet
    {
        [Parameter(HelpMessage = "The name of the storage account. If you don't specify storage info the development storage is the default account.")]
        public string StorageAccountName { get; set; }

        [Parameter(HelpMessage = "The key of the storage account. If you don't specify storage info the development storage is the default account.")]
        public string StorageAccountKey { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The deployment ID.")]
        [ValidateNotNullOrEmpty]
        public string DeploymentId { get; set; }

        protected CloudStorageAccount StorageAccount { get; private set; }
        
        protected DeploymentDiagnosticManager DiagnosticManager { get; private set; }
        
        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            this.StorageAccount = this.GetStorageAccount();
            this.DiagnosticManager = new DeploymentDiagnosticManager(this.StorageAccount, this.DeploymentId);
        }

        private CloudStorageAccount GetStorageAccount()
        {
            CloudStorageAccount account = null;

            if (string.IsNullOrEmpty(this.StorageAccountName) || string.IsNullOrEmpty(this.StorageAccountKey))
            {
                account = CloudStorageAccount.DevelopmentStorageAccount;
            }
            else
            {
                var credentials = new StorageCredentialsAccountAndKey(this.StorageAccountName, this.StorageAccountKey);
                account = new CloudStorageAccount(credentials, true);
            }

            return account;
        }
    }
}