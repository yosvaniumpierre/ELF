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

////namespace Microsoft.Samples.AzureManagementTools.PowerShell.HostedServices
////{
////    using System;
////    using System.Collections.Generic;
////    using System.Linq;
////    using System.Text;
////    using System.Management.Automation;
////    using Microsoft.Samples.WindowsAzure.ServiceManagement;

////    /// <summary>
////    /// .
////    /// </summary>
////    [Cmdlet(VerbsCommon.Get, "HostedServiceStatus")]
////    public class GetHostedServiceStatus : CmdletBase
////    {
////        /// <summary>
////        /// Executes the commandlet.
////        /// </summary>
////        protected override void ProcessRecord()
////        {
////            try
////            {
////                var hostedServices = this.GetHostedServices();

////                foreach (var hostedService in hostedServices)
////                {
////                    WriteObject(hostedService);
////                }

////            }
////            catch (Exception ex)
////            {
////                WriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
////            }
////        }

////        private HostedServiceList GetHostedServices()
////        {
////            var channel = this.CreateChannel();
////            var hostedServices = channel.GetOperationStatus(this.SubscriptionId, );

////            return hostedServices;
////        }
////    }
////}
