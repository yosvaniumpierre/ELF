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
    /// The hosted services related part of the external API
    /// </summary>
    public partial interface IServiceManagement
    {
        /// <summary>
        /// Lists the hosted services associated with a given subscription.
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebGet(UriTemplate = @"{subscriptionId}/services/hostedservices")]
        IAsyncResult BeginListHostedServices(string subscriptionId, AsyncCallback callback, object state);
        HostedServiceList EndListHostedServices(IAsyncResult asyncResult);

        /// <summary>
        /// Gets the properties for the specified hosted service.
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebGet(UriTemplate = @"{subscriptionId}/services/hostedservices/{serviceName}")]
        IAsyncResult BeginGetHostedService(string subscriptionId, string serviceName, AsyncCallback callback, object state);
        HostedService EndGetHostedService(IAsyncResult asyncResult);

        /// <summary>
        /// Gets the detailed properties for the specified hosted service. 
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebGet(UriTemplate = @"{subscriptionId}/services/hostedservices/{serviceName}?embed-detail={embedDetail}")]
        IAsyncResult BeginGetHostedServiceWithDetails(string subscriptionId, string serviceName, bool embedDetail, AsyncCallback callback, object state);
        HostedService EndGetHostedServiceWithDetails(IAsyncResult asyncResult);
    }

    public static partial class ServiceManagementExtensionMethods
    {
        public static HostedServiceList ListHostedServices(this IServiceManagement proxy, string subscriptionId)
        {
            return proxy.EndListHostedServices(proxy.BeginListHostedServices(subscriptionId, null, null));
        }

        public static HostedService GetHostedService(this IServiceManagement proxy, string subscriptionId, string serviceName)
        {
            return proxy.EndGetHostedService(proxy.BeginGetHostedService(subscriptionId, serviceName, null, null));
        }

        public static HostedService GetHostedServiceWithDetails(this IServiceManagement proxy, string subscriptionId, string serviceName, bool embedDetail)
        {
            return proxy.EndGetHostedServiceWithDetails(proxy.BeginGetHostedServiceWithDetails(subscriptionId, serviceName, embedDetail, null, null));
        }
    }

    /// <summary>
    /// List of host services
    /// </summary>
    [CollectionDataContract(Name = "HostedServices", ItemName = "HostedService", Namespace = Constants.ServiceManagementNS)]
    public class HostedServiceList : List<HostedService>
    {
        public HostedServiceList()
        {
        }

        public HostedServiceList(IEnumerable<HostedService> hostedServices)
            : base(hostedServices)
        {
        }
    }
    
    [DataContract(Namespace = Constants.ServiceManagementNS)]
    public class HostedService : IExtensibleDataObject
    {
        [DataMember(Order = 1)]
        public Uri Url { get; set; }

        [DataMember(Order = 2, EmitDefaultValue = false)]
        public string ServiceName { get; set; }

        [DataMember(Order = 3, EmitDefaultValue = false)]
        public HostedServiceProperties HostedServiceProperties { get; set; }

        [DataMember(Order = 4, EmitDefaultValue = false)]
        public DeploymentList Deployments { get; set; }

        public ExtensionDataObject ExtensionData { get; set; }
    }

    [CollectionDataContract(Name = "Deployments", ItemName = "Deployment", Namespace = Constants.ServiceManagementNS)]
    public class DeploymentList : List<Deployment>
    {
        public DeploymentList()
        {
        }

        public DeploymentList(IEnumerable<Deployment> deployments)
            : base(deployments)
        {
        }
    }

    /// <summary>
    /// Hosted Service in the Resource Model 
    /// </summary>
    [DataContract(Namespace = Constants.ServiceManagementNS)]
    public class HostedServiceProperties : IExtensibleDataObject
    {
        [DataMember(Order = 1)]
        public string Description { get; set; }

        [DataMember(Order = 2, EmitDefaultValue = false)]
        public string AffinityGroup { get; set; }

        [DataMember(Order = 3, EmitDefaultValue = false)]
        public string Location { get; set; }

        [DataMember(Order = 4)]
        public string Label { get; set; }

        public ExtensionDataObject ExtensionData { get; set; }
    }
}
