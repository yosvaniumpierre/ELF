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

namespace Microsoft.Samples.AzureManagementTools.PowerShell.Tests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;

    public static class CmdletTestingHelper
    {
        private static RunspaceConfiguration runspaceConfig = RunspaceConfiguration.Create();

        public static Collection<PSObject> RunCmdlet(Dictionary<string, Type> cmdletsRegistration, string command)
        {
            // Register cmdlets
            runspaceConfig.Cmdlets.Reset();
            foreach (var cmdlet in cmdletsRegistration)
            {
                var cmdletName = cmdlet.Key;
                var cmdletType = cmdlet.Value;
                
                runspaceConfig.Cmdlets.Append(
                    new CmdletConfigurationEntry(
                        cmdletName,
                        cmdletType,
                        string.Empty));
            }

            using (Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfig))
            {
                runspace.Open();
                using (Pipeline pipeline = runspace.CreatePipeline(command))
                {
                    var result = pipeline.Invoke();

                    return result;
                }
            }
        }

        public static Collection<PSObject> RunCmdlet(string cmdletName, string cmdletParameters, Type cmdletImplementationType)
        {
            var command = string.Format(CultureInfo.InvariantCulture, "{0} {1}", cmdletName, cmdletParameters);

            runspaceConfig.Cmdlets.Reset();
            runspaceConfig.Cmdlets.Append(
                new CmdletConfigurationEntry(
                    cmdletName,
                    cmdletImplementationType,
                    string.Empty));

            using (Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfig))
            {
                runspace.Open();
                using (Pipeline pipeline = runspace.CreatePipeline(command))
                {
                    var result = pipeline.Invoke();

                    return result;
                }
            }
        }

        public static Collection<PSObject> RunCmdlet(Command command, Type cmdletImplementationType, params string[] scripts)
        {
            runspaceConfig.Cmdlets.Reset();
            runspaceConfig.Cmdlets.Append(
                new CmdletConfigurationEntry(
                    command.CommandText,
                    cmdletImplementationType,
                    string.Empty));

            using (Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfig))
            {
                runspace.Open();
                using (Pipeline pipeline = runspace.CreatePipeline())
                {
                    foreach (var script in scripts)
                    {
                        pipeline.Commands.AddScript(script);
                    }

                    pipeline.Commands.Add(command);

                    var result = pipeline.Invoke();

                    return result;
                }
            }
        }
    }
}