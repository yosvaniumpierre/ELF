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

namespace Microsoft.Samples.AzureManagementTools.PowerShell.Tests.Unit
{
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.Samples.AzureManagementTools.PowerShell.Helpers;
    using Microsoft.Samples.AzureManagementTools.PowerShell.Model;
    using Microsoft.Samples.WindowsAzure.ServiceManagement;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    /// <summary>
    /// This is a test class for DeploymentInfoTest and is intended
    /// to contain all DeploymentInfoTest Unit Tests
    /// </summary>
    [TestClass()]
    public class DeploymentInfoFixture
    {
        [TestMethod]
        [DeploymentItem(@"Resources\DeploymentInfoFixture.ShouldLoadTest.cscfg")]
        public void ShouldLoadFromApiObject()
        {
            var deployment = new Deployment();
            deployment.Name = "deploymentName";
            
            var xmlReader = XmlReader.Create(@"DeploymentInfoFixture.ShouldLoadTest.cscfg");
            var xmlConfig = XDocument.Load(xmlReader);
            string config = xmlConfig.ToString(SaveOptions.DisableFormatting);
            deployment.Configuration = ServiceManagementHelper.EncodeToBase64String(config);
            deployment.RoleInstanceList = new RoleInstanceList();
            
            var deploymentInfo = new DeploymentInfoContext(deployment);

            Assert.AreEqual(1, deploymentInfo.RolesConfiguration.Count);
            Assert.AreEqual(1, deploymentInfo.RolesConfiguration["TestWebRole"].InstanceCount);
            Assert.AreEqual("TestWebRole", deploymentInfo.RolesConfiguration["TestWebRole"].Name);
            Assert.AreEqual("AzureManagementTools", deploymentInfo.RolesConfiguration["TestWebRole"].Settings["productName"]);
        }
    }
}
