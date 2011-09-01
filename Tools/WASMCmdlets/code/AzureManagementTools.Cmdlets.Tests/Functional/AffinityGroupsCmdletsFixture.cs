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

namespace Microsoft.Samples.AzureManagementTools.PowerShell.Tests.Functional
{
    using System.Globalization;
    using System.Linq;
    using Microsoft.Samples.AzureManagementTools.PowerShell.AffinityGroups;
    using Microsoft.Samples.AzureManagementTools.PowerShell.Tests.Helpers;
    using Microsoft.Samples.WindowsAzure.ServiceManagement;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AffinityGroupsCmdletsFixture
    {
        [TestMethod]
        public void GetAffinityGroups()
        {
            var cmdletName = "Get-AffinityGroups";
            var cmdletParams = TestConstants.CommonParameters;

            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(GetAffinityGroupsCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() > 0);
            Assert.IsInstanceOfType(result[0].ImmediateBaseObject, typeof(AffinityGroup));
        }

        [TestMethod]
        public void GetAffinityGroup()
        {
            var cmdletName = "Get-AffinityGroup";
            var cmdletParams = string.Format(CultureInfo.InvariantCulture, "{0} -name {1}", TestConstants.CommonParameters, TestConstants.AffinityGroupName);

            var result = CmdletTestingHelper.RunCmdlet(cmdletName, cmdletParams, typeof(GetAffinityGroupCommand));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            var affinityGroupProperties = (AffinityGroup)result[0].ImmediateBaseObject;

            Assert.IsTrue(!string.IsNullOrEmpty(affinityGroupProperties.Name));
            Assert.IsTrue(!string.IsNullOrEmpty(affinityGroupProperties.Location));
            Assert.AreEqual<string>(TestConstants.AffinityGroupName, affinityGroupProperties.Name);
        }
    }
}
