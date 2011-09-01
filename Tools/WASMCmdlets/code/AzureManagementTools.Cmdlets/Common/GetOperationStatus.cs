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

namespace Microsoft.Samples.AzureManagementTools.PowerShell.HostedServices
{
    using System;
    using System.Globalization;
    using System.Management.Automation;
    using System.ServiceModel;
    using System.Threading;
    using Microsoft.Samples.WindowsAzure.ServiceManagement;

    /// <summary>
    /// Shows status of specified operation id.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "OperationStatus")]
    public class GetOperationStatusCommand : CmdletBase
    {
        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Operation Id")]
        [ValidateNotNullOrEmpty]
        public string OperationId
        {
            get;
            set;
        }

        [Parameter(Position = 1, HelpMessage = "Wait while operation is 'in progress'")]
        public SwitchParameter WaitToComplete
        {
            get;
            set;
        }

        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();

                var operation = this.GetOperationStatusProcess();

                if (this.WaitToComplete.ToBool())
                {
                    var activityId = new Random().Next(1, 999999);
                    var progress = new ProgressRecord(activityId, "Please wait...", "Operation Status: " + operation.Status);

                    while (string.Compare(operation.Status, OperationState.Succeeded, StringComparison.OrdinalIgnoreCase) != 0 &&
                           string.Compare(operation.Status, OperationState.Failed, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        WriteProgress(progress);
                        Thread.Sleep(1 * 1000);
                        operation = this.GetOperationStatusProcess();
                    }
                }

                if (string.Compare(operation.Status, OperationState.Failed, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var errorMessage = string.Format(CultureInfo.InvariantCulture, "{0}: {1}", operation.Status, operation.Error.Message);
                    var exception = new Exception(errorMessage);
                    WriteError(new ErrorRecord(exception, string.Empty, ErrorCategory.CloseError, null));
                }
                else
                {
                    WriteObject(operation.Status);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }

        private Operation GetOperationStatusProcess()
        {
            Operation operation = null;

            try
            {
                operation = this.RetryCall(s => this.Channel.GetOperationStatus(s, this.OperationId));
            }
            catch (CommunicationException ex)
            {
                this.WriteErrorDetails(ex);
            }

            return operation;
        }
    }
}
