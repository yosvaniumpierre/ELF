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

namespace Microsoft.Samples.AzureManagementTools.PowerShell.Diagnostics
{
    using System.Collections.Generic;
    using System.Management.Automation;

    /// <summary>
    /// Lists the set of roles which have successfully started at least one diagnostic monitor.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "DiagnosticAwareRoles")]
    public class GetDiagnosticAwareRolesCommand : DiagnosticsCmdletBase
    {
        public GetDiagnosticAwareRolesCommand()
        {
        }

        public IEnumerable<string> GetDiagnosticAwareRolesProcess()
        {
            return this.DiagnosticManager.GetRoleNames();
        }

        protected override void ProcessRecord()
        {
            var roles = this.GetDiagnosticAwareRolesProcess();

            this.WriteObject(roles, true);
        }
    }
}