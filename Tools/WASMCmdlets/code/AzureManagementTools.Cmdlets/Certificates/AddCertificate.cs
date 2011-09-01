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
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceModel;
    using Microsoft.Samples.AzureManagementTools.PowerShell.Helpers;
    using Microsoft.Samples.WindowsAzure.ServiceManagement;

    /// <summary>
    /// Upload a service certificate for the specified hosted service.
    /// </summary>
    [Cmdlet(VerbsCommon.Add, "Certificate")]
    public class AddCertificateCommand : CmdletBase
    {
        public AddCertificateCommand()
        {
        }

        public AddCertificateCommand(IServiceManagement channel)
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

        [Parameter(Mandatory = true, HelpMessage = "Certificate to deploy.")]
        [ValidateNotNullOrEmpty]
        public object CertificateToDeploy
        {
            get;
            set;
        }

        [Parameter(HelpMessage = "Certificate password.")]
        public string Password
        {
            get;
            set;
        }

        public string AddCertificateProcess()
        {
            this.ValidateParameters();

            var certData = this.GetCertificateData();

            var certificateFile = new CertificateFile
            {
                Data = Convert.ToBase64String(certData),
                Password = this.Password,
                CertificateFormat = "pfx"
            };

            using (new OperationContextScope((IContextChannel)Channel))
            {
                try
                {
                    this.RetryCall(s => this.Channel.AddCertificates(s, this.ServiceName, certificateFile));
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

                var operationId = this.AddCertificateProcess();

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
            this.Password = this.Password == null ? string.Empty : this.Password;
        }

        private byte[] GetCertificateData()
        {
            var cert = new X509Certificate2();
            byte[] certData = null;

            if (((this.CertificateToDeploy is PSObject) && ((PSObject)this.CertificateToDeploy).ImmediateBaseObject is X509Certificate) ||
                (this.CertificateToDeploy is X509Certificate))
            {
                cert = ((PSObject)this.CertificateToDeploy).ImmediateBaseObject as X509Certificate2;

                try
                {
                    certData = cert.HasPrivateKey ? cert.Export(X509ContentType.Pfx) : cert.Export(X509ContentType.Pkcs12);
                }
                catch (CryptographicException)
                {
                    certData = cert.HasPrivateKey ? cert.RawData : cert.Export(X509ContentType.Pkcs12);
                }
            }
            else
            {
                cert.Import(this.ResolvePath(this.CertificateToDeploy.ToString()), this.Password, X509KeyStorageFlags.Exportable);
                certData = cert.HasPrivateKey ? cert.Export(X509ContentType.Pfx, this.Password) : cert.Export(X509ContentType.Pkcs12);
            }

            return certData;
        }
    }
}