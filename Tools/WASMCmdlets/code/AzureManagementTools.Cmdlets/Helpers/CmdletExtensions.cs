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

namespace Microsoft.Samples.AzureManagementTools.PowerShell.Helpers
{
    using System.Globalization;
    using System.Management.Automation;

    public static class CmdletExtensions
    {
        public static string ResolvePath(this PSCmdlet cmdlet, string path)
        {
            var result = cmdlet.SessionState.Path.GetResolvedPSPathFromPSPath(path);
            string fullPath = string.Empty;

            if (result != null && result.Count > 0)
            {
                fullPath = result[0].Path;
            }

            return fullPath;
        }

        public static void WriteVerbose(this PSCmdlet cmdlet, string format, params object[] args)
        {
            var text = string.Format(CultureInfo.InvariantCulture, format, args);
            cmdlet.WriteVerbose(text);
        }
    }
}
