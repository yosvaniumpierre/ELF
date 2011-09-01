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
    /// List the certificates for the specified hosted service.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "Certificates")]
    public class GetCertificatesCommand : CmdletBase
    {
        public GetCertificatesCommand()
        {
        }

        public GetCertificatesCommand(IServiceManagement channel)
        {
            this.Channel = channel;
        }

        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Hosted Service Name.")]
        [ValidateNotNullOrEmpty]
        public string ServiceName
        {
            get;
            set;
        }

        public CertificateList GetCertificatesProcess()
        {
            CertificateList certificates = null;

            try
            {
                certificates = this.RetryCall(s => this.Channel.ListCertificates(s, this.ServiceName));
            }
            catch (CommunicationException ex)
            {
                this.WriteErrorDetails(ex);
            }

            return certificates;
        }

        /// <summary>
        /// Executes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();

                var certificates = this.GetCertificatesProcess();
                foreach (var certificate in certificates)
                {
                    var ctx = new Model.CertificateContext(certificate);
                    ctx.SubscriptionId = this.SubscriptionId;
                    ctx.ServiceName = this.ServiceName;
                    ctx.Certificate = this.Certificate;

                    WriteObject(ctx);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }
    }
}