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

//---------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.  
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
//---------------------------------------------------------------------------------

namespace Microsoft.Samples.WindowsAzure.ServiceManagement
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Web;

    /// <summary>
    /// The certificate related part of the API
    /// </summary>
    public partial interface IServiceManagement
    {
        /// <summary>
        /// Lists the certificates associated with a given subscription.
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebGet(UriTemplate = @"{subscriptionId}/services/hostedservices/{serviceName}/certificates")]
        IAsyncResult BeginListCertificates(string subscriptionId, string serviceName, AsyncCallback callback, object state);
        CertificateList EndListCertificates(IAsyncResult asyncResult);

        /// <summary>
        /// Gets public data for the given certificate.
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebGet(UriTemplate = @"{subscriptionId}/services/hostedservices/{serviceName}/certificates/{thumbprintalgorithm}-{thumbprint_in_hex}")]
        IAsyncResult BeginGetCertificate(string subscriptionId, string serviceName, string thumbprintalgorithm, string thumbprint_in_hex, AsyncCallback callback, object state);
        Certificate EndGetCertificate(IAsyncResult asyncResult);

        /// <summary>
        /// Adds certificates to the given subscription. 
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "POST", UriTemplate = @"{subscriptionId}/services/hostedservices/{serviceName}/certificates")]
        IAsyncResult BeginAddCertificates(string subscriptionId, string serviceName, CertificateFile input, AsyncCallback callback, object state);
        void EndAddCertificates(IAsyncResult asyncResult);

        /// <summary>
        /// Deletes the given certificate.
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "DELETE", UriTemplate = @"{subscriptionId}/services/hostedservices/{serviceName}/certificates/{thumbprintalgorithm}-{thumbprint_in_hex}")]
        IAsyncResult BeginDeleteCertificate(string subscriptionId, string serviceName, string thumbprintalgorithm, string thumbprint_in_hex, AsyncCallback callback, object state);
        void EndDeleteCertificate(IAsyncResult asyncResult);
    }

    public static partial class ServiceManagementExtensionMethods
    {
        public static CertificateList ListCertificates(this IServiceManagement proxy, string subscriptionId, string serviceName)
        {
            return proxy.EndListCertificates(proxy.BeginListCertificates(subscriptionId, serviceName, null, null));
        }

        public static Certificate GetCertificate(this IServiceManagement proxy, string subscriptionId, string serviceName, string algorithm, string thumbprint)
        {
            return proxy.EndGetCertificate(proxy.BeginGetCertificate(subscriptionId, serviceName, algorithm, thumbprint, null, null));
        }

        public static void AddCertificates(this IServiceManagement proxy, string subscriptionId, string serviceName, CertificateFile input)
        {
            proxy.EndAddCertificates(proxy.BeginAddCertificates(subscriptionId, serviceName, input, null, null));
        }

        public static void DeleteCertificate(this IServiceManagement proxy, string subscriptionId, string serviceName, string algorithm, string thumbprint)
        {
            proxy.EndDeleteCertificate(proxy.BeginDeleteCertificate(subscriptionId, serviceName, algorithm, thumbprint, null, null));
        }
    }

    [CollectionDataContract(Name = "Certificates", ItemName = "Certificate", Namespace = Constants.ServiceManagementNS)]
    public class CertificateList : List<Certificate>
    {
        public CertificateList()
        {
        }

        public CertificateList(IEnumerable<Certificate> certificateList)
            : base(certificateList)
        {
        }
    }

    [DataContract(Namespace = Constants.ServiceManagementNS)]
    public class Certificate : IExtensibleDataObject
    {
        [DataMember(Order = 1, EmitDefaultValue = false)]
        public Uri CertificateUrl { get; set; }

        [DataMember(Order = 2, EmitDefaultValue = false)]
        public string Thumbprint { get; set; }

        [DataMember(Order = 3, EmitDefaultValue = false)]
        public string ThumbprintAlgorithm { get; set; }

        [DataMember(Order = 4, EmitDefaultValue = false)]
        public string Data { get; set; }

        public ExtensionDataObject ExtensionData { get; set; }
    }

    [DataContract(Namespace = Constants.ServiceManagementNS)]
    public class CertificateFile : IExtensibleDataObject
    {
        [DataMember(Order = 1)]
        public string Data { get; set; }

        [DataMember(Order = 2)]
        public string CertificateFormat { get; set; }

        [DataMember(Order = 3)]
        public string Password { get; set; }

        public ExtensionDataObject ExtensionData { get; set; }
    }
}
