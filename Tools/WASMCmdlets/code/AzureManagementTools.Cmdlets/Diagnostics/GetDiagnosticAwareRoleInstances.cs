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
    /// Returns a list of IDs of active role instances that have a diagnostic monitor running.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "DiagnosticAwareRoleInstances")]
    public class GetDiagnosticAwareRoleInstancesCommand : DiagnosticsCmdletBase
    {
        public GetDiagnosticAwareRoleInstancesCommand()
        {
        }

        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The name of the role.")]
        [ValidateNotNullOrEmpty]
        public string RoleName { get; set; }

        public IEnumerable<string> GetDiagnosticAwareRoleInstancesProcess()
        {
            return this.DiagnosticManager.GetRoleInstanceIdsForRole(this.RoleName);
        }

        protected override void ProcessRecord()
        {
            var roleInstances = this.GetDiagnosticAwareRoleInstancesProcess();

            this.WriteObject(roleInstances, true);
        }
    }
}