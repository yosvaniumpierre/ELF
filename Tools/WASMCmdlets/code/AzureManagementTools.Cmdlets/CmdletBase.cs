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

namespace Microsoft.Samples.AzureManagementTools.PowerShell
{
    using System;
    using System.Globalization;
    using System.Management.Automation;
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Security;
    using System.ServiceModel.Web;
    using Microsoft.Samples.WindowsAzure.ServiceManagement;

    public class CmdletBase : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Account subscription Id.")]
        [ValidateNotNullOrEmpty]
        public string SubscriptionId
        {
            get;
            set;
        }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Account certificate.")]
        [ValidateNotNullOrEmpty]
        public X509Certificate2 Certificate
        {
            get;
            set;
        }

        [Parameter(HelpMessage = "Service Binding.")]
        public Binding ServiceBinding
        {
            get;
            set;
        }

        [Parameter(HelpMessage = "Service Endpoint.")]
        public string ServiceEndpoint
        {
            get;
            set;
        }

        [Parameter(HelpMessage = "The maximum string content length quota used by the management service.")]
        public int MaxStringContentLength
        {
            get;
            set;
        }

        protected IServiceManagement Channel
        {
            get;
            set;
        }

        protected static string RetrieveOperationId()
        {
            var operationId = string.Empty;

            if (WebOperationContext.Current.IncomingResponse != null)
            {
                operationId = WebOperationContext.Current.IncomingResponse.Headers[Constants.OperationTrackingIdHeader];
            }

            return operationId;
        }

        protected void WriteErrorDetails(CommunicationException exception)
        {
            ServiceManagementError error = null;
            ServiceManagementHelper.TryGetExceptionDetails(exception, out error);

            if (error == null)
            {
                WriteError(new ErrorRecord(exception, string.Empty, ErrorCategory.CloseError, null));
            }
            else
            {
                string errorDetails = string.Format(
                    CultureInfo.InvariantCulture,
                    "HTTP Status Code: {0} - HTTP Error Message: {1}",
                    error.Code,
                    error.Message);

                WriteError(new ErrorRecord(new CommunicationException(errorDetails), string.Empty, ErrorCategory.CloseError, null));
            }
        }

        protected override void ProcessRecord()
        {
            if (this.Channel == null)
            {
                this.Channel = this.CreateChannel();
            }
        }

        protected IServiceManagement CreateChannel()
        {
            return this.CreateChannel(Constants.VersionHeaderContent);
        }

        protected IServiceManagement CreateChannel(string versionHeaderContent)
        {
            if (this.ServiceBinding == null)
            {
                this.ServiceBinding = ConfigurationConstants.WebHttpBinding(this.MaxStringContentLength);
            }

            if (string.IsNullOrEmpty(this.ServiceEndpoint))
            {
                this.ServiceEndpoint = ConfigurationConstants.ServiceEndpoint;
            }

            return ServiceManagementHelper.CreateServiceManagementChannel(this.ServiceBinding, new Uri(this.ServiceEndpoint), this.Certificate, versionHeaderContent);
        }

        protected void RetryCall(Action<string> call)
        {
            this.RetryCall(this.SubscriptionId, call);
        }

        protected void RetryCall(string subscriptionId, Action<string> call)
        {
            try
            {
                try
                {
                    call(subscriptionId);
                }
                catch (MessageSecurityException ex)
                {
                    var webException = ex.InnerException as WebException;

                    if (webException == null)
                    {
                        throw;
                    }

                    var webResponse = webException.Response as HttpWebResponse;

                    if (webResponse != null && webResponse.StatusCode == HttpStatusCode.Forbidden)
                    {
                        this.Channel = this.CreateChannel();
                        if (subscriptionId.Equals(subscriptionId.ToUpper(CultureInfo.InvariantCulture)))
                        {
                            call(subscriptionId.ToLower(CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            call(subscriptionId.ToUpper(CultureInfo.InvariantCulture));
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            catch (MessageSecurityException ex)
            {
                var webException = ex.InnerException as WebException;

                if (webException == null)
                {
                    throw;
                }

                var webResponse = webException.Response as HttpWebResponse;

                if (webResponse != null && webResponse.StatusCode == HttpStatusCode.Forbidden)
                {
                    this.Channel = this.CreateChannel();
                    if (subscriptionId.Equals(subscriptionId.ToUpper(CultureInfo.InvariantCulture)))
                    {
                        call(subscriptionId.ToLower(CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        call(subscriptionId.ToUpper(CultureInfo.InvariantCulture));
                    }
                }
                else
                {
                    throw;
                }
            }
        }

        protected TResult RetryCall<TResult>(Func<string, TResult> call)
        {
            return this.RetryCall(this.SubscriptionId, call);
        }

        protected TResult RetryCall<TResult>(string subscriptionId, Func<string, TResult> call)
        {
            try
            {
                try
                {
                    return call(subscriptionId);
                }
                catch (MessageSecurityException ex)
                {
                    var webException = ex.InnerException as WebException;

                    if (webException == null)
                    {
                        throw;
                    }

                    var webResponse = webException.Response as HttpWebResponse;

                    if (webResponse != null && webResponse.StatusCode == HttpStatusCode.Forbidden)
                    {
                        if (subscriptionId.Equals(subscriptionId.ToUpper(CultureInfo.InvariantCulture)))
                        {
                            return call(subscriptionId.ToLower(CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            return call(subscriptionId.ToUpper(CultureInfo.InvariantCulture));
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            catch (MessageSecurityException ex)
            {
                var webException = ex.InnerException as WebException;

                if (webException == null)
                {
                    throw;
                }

                var webResponse = webException.Response as HttpWebResponse;

                if (webResponse != null && webResponse.StatusCode == HttpStatusCode.Forbidden)
                {
                    if (subscriptionId.Equals(subscriptionId.ToUpper(CultureInfo.InvariantCulture)))
                    {
                        return call(subscriptionId.ToLower(CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        return call(subscriptionId.ToUpper(CultureInfo.InvariantCulture));
                    }
                }
                else
                {
                    throw;
                }
            }
        }
    }
}