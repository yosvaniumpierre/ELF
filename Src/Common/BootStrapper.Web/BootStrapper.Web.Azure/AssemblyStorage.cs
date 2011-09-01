namespace Avanade.BootStrapper.Web.Azure
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;

    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.ServiceRuntime;
    using Microsoft.WindowsAzure.StorageClient;

    using NLog;

    public class AzureContext : IDisposable
    {
        #region Fields

        private readonly HttpContext oldHttpContext;
        private readonly bool restoreOldHttpContext;

        #endregion Fields

        #region Constructors

        public AzureContext(bool forceSettingContextToNull = false)
        {
            if (forceSettingContextToNull)
            {
                oldHttpContext = HttpContext.Current;
                HttpContext.Current = null;
                restoreOldHttpContext = true;
            }
            else
            {
                try
                {
                    HttpResponse response = HttpContext.Current.Response;
                }
                catch (HttpException)
                {
                    oldHttpContext = HttpContext.Current;
                    HttpContext.Current = null;
                    restoreOldHttpContext = true;
                }
            }
        }

        ~AzureContext()
        {
            Dispose(false);
        }

        #endregion Constructors

        #region Methods

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (restoreOldHttpContext)
                {
                    HttpContext.Current = oldHttpContext;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion Methods
    }

    internal static class AssemblyStorage
    {
        #region Fields

        private const string ConnectionStringKey = "AssemblyBlobConnectionString";
        private const string ContainerReference = "assembly";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion Fields

        #region Methods

        internal static IList<AssemblyItem> Extract()
        {
            var list = new List<AssemblyItem>();

            try
            {
                // After several unsuccessful attempts to get the application to work in Azure, the following link helped.
                // http://toolheaven.net/post/Azure-and-Response-is-not-available-in-this-context.aspx
                using (new AzureContext())
                {
                    var connectionString = RoleEnvironment.GetConfigurationSettingValue(ConnectionStringKey);
                    var account = CloudStorageAccount.Parse(connectionString);
                    //take the assemblies from Blob Storage
                    CloudBlobContainer cbc = account.CreateCloudBlobClient().GetContainerReference(ContainerReference);

                    Logger.Info(
                        "Extracting assemblies using key ({0}) for connection string ({1}) and Container reference ({2}).",
                        ConnectionStringKey, connectionString, ContainerReference);

                    var assemblyBlobs = cbc.ListBlobs().ToList();

                    Logger.Info("Number of blobs retrieved: {0}", assemblyBlobs.Count);

                    foreach (var listBlobItem in assemblyBlobs)
                    {
                        var uri = listBlobItem.Uri.AbsoluteUri;
                        var assemblyName = Path.GetFileName(uri);
                        if (Logger.IsDebugEnabled)
                        {
                            Logger.Debug("Assembly found: Name-{0}, Uri-{1}", assemblyName, uri);
                        }
                        byte[] byteStream = cbc.GetBlobReference(uri).DownloadByteArray();
                        list.Add(new AssemblyItem(assemblyName, byteStream));
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Unexpected error encountered during assembly extraction from blob storage!", exception);
            }

            return list;
        }

        #endregion Methods
    }
}