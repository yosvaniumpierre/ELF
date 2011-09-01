﻿// ----------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    using System.Globalization;
    using System.Xml.Linq;

    public class RoleConfiguration
    {
        private readonly XNamespace ns = "http://schemas.microsoft.com/ServiceHosting/2008/10/ServiceConfiguration";

        public RoleConfiguration(XElement data)
        {
            this.Name = data.Attribute("name").Value;
            this.InstanceCount = int.Parse(data.Element(this.ns + "Instances").Attribute("count").Value, CultureInfo.InvariantCulture);

            this.Settings = new Dictionary<string, string>();

            foreach (var setting in data.Element(this.ns + "ConfigurationSettings").Descendants())
            {
                this.Settings.Add(setting.Attribute("name").Value, setting.Attribute("value").Value);
            }

            this.Certificates = new Dictionary<string, CertificateConfiguration>();

            foreach (var setting in data.Element(this.ns + "Certificates").Descendants())
            {
                var certificate = new CertificateConfiguration
                {
                    Thumbprint = setting.Attribute("thumbprint").Value,
                    ThumbprintAlgorithm = setting.Attribute("thumbprintAlgorithm").Value
                };

                this.Certificates.Add(setting.Attribute("name").Value, certificate);
            }
        }

        public string Name
        {
            get;
            set;
        }

        public int InstanceCount
        {
            get;
            set;
        }

        public Dictionary<string, string> Settings
        {
            get;
            protected set;
        }

        public Dictionary<string, CertificateConfiguration> Certificates
        {
            get;
            protected set;
        }

        internal XElement Serialize()
        {
            XElement roleElement = new XElement(this.ns + "Role");
            roleElement.SetAttributeValue("name", this.Name);
            XElement instancesElement = new XElement(this.ns + "Instances");
            instancesElement.SetAttributeValue("count", this.InstanceCount);
            roleElement.Add(instancesElement);

            XElement configurationSettingsElement = new XElement(this.ns + "ConfigurationSettings");
            roleElement.Add(configurationSettingsElement);

            foreach (var setting in this.Settings)
            {
                XElement settingElement = new XElement(this.ns + "Setting");
                settingElement.SetAttributeValue("name", setting.Key);
                settingElement.SetAttributeValue("value", setting.Value);
                configurationSettingsElement.Add(settingElement);
            }

            XElement certificatesElement = new XElement(this.ns + "Certificates");
            roleElement.Add(certificatesElement);

            foreach (var certificate in this.Certificates)
            {
                XElement certificateElement = new XElement(this.ns + "Certificate");
                certificateElement.SetAttributeValue("name", certificate.Key);
                certificateElement.SetAttributeValue("thumbprint", certificate.Value.Thumbprint);
                certificateElement.SetAttributeValue("thumbprintAlgorithm", certificate.Value.ThumbprintAlgorithm);
                certificatesElement.Add(certificateElement);
            }

            return roleElement;
        }
    }
}