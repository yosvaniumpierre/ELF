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

namespace Microsoft.Samples.AzureManagementTools.PowerShell.Model
{
    using System;
    using System.Collections.Generic;

    public class CertificateContext : ManagementOperationContext
    {
        private Microsoft.Samples.WindowsAzure.ServiceManagement.Certificate innerCertificate;

        public CertificateContext(Microsoft.Samples.WindowsAzure.ServiceManagement.Certificate innerCertificate)
        {
            this.innerCertificate = innerCertificate;
        }

        public Uri Url
        {
            get
            {
                return this.innerCertificate.CertificateUrl;
            }
        }

        public string Data
        {
            get
            {
                return this.innerCertificate.Data;
            }
        }

        public string Thumbprint
        {
            get
            {
                return this.innerCertificate.Thumbprint;
            }
        }

        public string ThumbprintAlgorithm
        {
            get
            {
                return this.innerCertificate.ThumbprintAlgorithm;
            }
        }
    }
}
