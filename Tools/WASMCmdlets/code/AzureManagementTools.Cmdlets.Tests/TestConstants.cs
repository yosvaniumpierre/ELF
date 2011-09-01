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

namespace Microsoft.Samples.AzureManagementTools.PowerShell.Tests
{
    using System.Globalization;
    using System.Security.Cryptography.X509Certificates;

    public static class TestConstants
    {
        public const string SubscriptionId = "8E3BBB0D-34FF-45D8-B0EB-098683F1291A";
        public const string StorageServiceName = "demosstorage";
        public const string HostedServiceName = "demosservices";
        public const string AffinityGroupName = "86987f1e-d90e-4f30-9ea7-1003d9541f4f";
        public const string AzurePackageLocation = "http://azuremanagementtools.blob.core.windows.net/test/TestCloudService.cspkg";
        public const string ProductionLabel = "ProductionLabelStaging";
        public const string StagingLabel = "TestLabelStaging";
        public const string ProductionDeploymentName = "TestProductionDeployment";
        public const string StagingDeploymentName = "TestStagingDeployment";
        public const string RoleName = "TestWebRole";
        private static string certificateThumbprint = "3D6E34B526723E06C235BE8E5547784BF12C9F39";

        public static string CommonParameters
        {
            get
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    "-subscriptionId {0} -certificate ({1})",
                    TestConstants.SubscriptionId,
                    TestConstants.CertificateCommand);
            }
        }

        public static string CertificateCommand
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, @"get-item cert:\LocalMachine\MY\{0}", certificateThumbprint.ToUpperInvariant());
            }
        }

        public static X509Certificate2 Certificate
        {
            get
            {
                var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadOnly);
                var certificates = store.Certificates.Find(X509FindType.FindByThumbprint, certificateThumbprint, false);

                return certificates.Count > 0 ? certificates[0] : null;
            }
        }
    }
}