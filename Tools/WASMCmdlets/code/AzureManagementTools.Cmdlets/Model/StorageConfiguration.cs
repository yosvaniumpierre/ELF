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
    using System.Globalization;
    using System.Xml.Linq;

    public class StorageConfiguration
    {
        public StorageConfiguration(XElement data)
        {
            this.Name = data.Attribute("name").Value;
            this.SizeInMB = int.Parse(data.Attribute("sizeInMB").Value, CultureInfo.InvariantCulture);
        }

        public string Name
        {
            get;
            set;
        }

        public int SizeInMB
        {
            get;
            set;
        }

        internal XElement Serialize()
        {
            XElement storageElement = new XElement("LocalStorage");
            storageElement.SetAttributeValue("name", this.Name);
            storageElement.SetAttributeValue("sizeInMB", this.SizeInMB);
            return storageElement;
        }
    }
}
