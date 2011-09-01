namespace Windows.Azure.Msbuild
{
    using System;
    using System.IO;

    using AzureTools;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;

    using Properties;

    public class DevUploadBlockBlob : Task
    {
        #region Fields

        private readonly IAzureBlobClientFactory blobClientWrapper;
        private readonly IFileManager fileManager;
        private readonly ITaskLogger logger;

        #endregion Fields

        #region Constructors

        [CoverageExclude(Reason.Humble)]
        public DevUploadBlockBlob()
            : this(new AzureBlobClientFactory(), null, new FileManager())
        {
        }

        [CoverageExclude(Reason.Test)]
        public DevUploadBlockBlob(IAzureBlobClientFactory blobClientWrapper, ITaskLogger taskLogger, IFileManager fileManager)
        {
            if (taskLogger == null)
                taskLogger = new LoggingHelperWrapper(this);

            logger = taskLogger;
            this.fileManager = fileManager;
            this.blobClientWrapper = blobClientWrapper;

            StorageClientTimeoutInMinutes = 30;
        }

        #endregion Constructors

        #region Properties

        public bool CleanContainer
        {
            get; set;
        }

        [Required]
        public string ContainerName
        {
            get; set;
        }

        [Required]
        public ITaskItem[] DestinationFiles
        {
            get; set;
        }

        [Required]
        public ITaskItem[] SourceFiles
        {
            get; set;
        }

        public int StorageClientTimeoutInMinutes
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public override bool Execute()
        {
            const string msg = "Creating cloud storage client for the local dev blob storage";
            logger.LogMessage(msg);

            var cloudStorageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            var blobClient = cloudStorageAccount.CreateCloudBlobClient();
            blobClient.Timeout = new TimeSpan(0, StorageClientTimeoutInMinutes, 0);

            var client = blobClientWrapper.Create(blobClient);
            var container = client.GetContainerReference(ContainerName);
            if (container.CreateIfNotExists())
            {
                logger.LogMessage(Resources.Msg_ContainerCreated, ContainerName);
            }
            else if (CleanContainer)
            {
                container.Cleanup();
            }

            for (int i = 0; i < SourceFiles.Length; i++)
            {
                var pathToSource = SourceFiles[i].ItemSpec;
                var pathToDest = (DestinationFiles != null) ? DestinationFiles[i].ItemSpec : Path.GetFileName(pathToSource);

                var blob = container.GetBlockBlobReference(pathToDest);
                if (blob.DeleteIfExists())
                {
                    logger.LogMessage(Resources.Msg_DeletedBlob, pathToDest);
                }

                using (var stream = fileManager.GetFile(pathToSource))
                {
                    blob.UploadFromStream(stream);
                    logger.LogMessage(Resources.Msg_UploadedBlob, pathToSource);
                }
            }

            return true;
        }

        #endregion Methods
    }
}