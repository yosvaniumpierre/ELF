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

namespace Microsoft.Samples.AzureManagementTools.PowerShell.Certificates
{
    using System;
    using System.Management.Automation;
    using System.ServiceModel;
    using Microsoft.Samples.WindowsAzure.ServiceManagement;

    /// <summary>
    /// Deletes the specified certificate.
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "Certificate")]
    public class RemoveCertificateCommand : CmdletBase
    {
        public RemoveCertificateCommand()
        {
        }

        public RemoveCertificateCommand(IServiceManagement channel)
        {
            this.Channel = channel;
        }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Hosted Service Name.")]
        [ValidateNotNullOrEmpty]
        public string ServiceName
        {
            get;
            set;
        }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Certificate thumbprint algorithm.")]
        [ValidateNotNullOrEmpty]
        public string ThumbprintAlgorithm
        {
            get;
            set;
        }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Certificate thumbprint.")]
        [ValidateNotNullOrEmpty]
        public string Thumbprint
        {
            get;
            set;
        }

        public string RemoveCertificateProcess()
        {
            using (new OperationContextScope((IContextChannel)Channel))
            {
                try
                {
                    this.RetryCall(s => this.Channel.DeleteCertificate(s, this.ServiceName, this.ThumbprintAlgorithm, this.Thumbprint));
                }
                catch (CommunicationException ex)
                {
                    this.WriteErrorDetails(ex);
                }

                return RetrieveOperationId();
            }
        }

        /// <summary>
        /// Executes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();

                var operationId = this.RemoveCertificateProcess();

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