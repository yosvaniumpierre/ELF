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

    public class CloudUploadBlockBlob : Task
    {
        #region Fields

        private readonly IAzureBlobClientFactory blobClientWrapper;
        private readonly IFileManager fileManager;
        private readonly ITaskLogger logger;

        #endregion Fields

        #region Constructors

        [CoverageExclude(Reason.Humble)]
        public CloudUploadBlockBlob()
            : this(new AzureBlobClientFactory(), null, new FileManager())
        {
        }

        [CoverageExclude(Reason.Test)]
        public CloudUploadBlockBlob(IAzureBlobClientFactory blobClientWrapper, ITaskLogger taskLogger, IFileManager fileManager)
        {
            if (taskLogger == null)
                taskLogger = new LoggingHelperWrapper(this);

            logger = taskLogger;
            this.fileManager = fileManager;
            this.blobClientWrapper = blobClientWrapper;

            StorageClientTimeoutInMinutes = 30;
            ParallelOptionsThreadCount = 1;
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
        public string Endpoint
        {
            get; set;
        }

        public int ParallelOptionsThreadCount
        {
            get; set;
        }

        [Required]
        public ITaskItem[] SourceFiles
        {
            get; set;
        }

        [Required]
        public string StorageAccountKey
        {
            get; set;
        }

        [Required]
        public string StorageAccountName
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
            const string msg = "Creating cloud storage client with Endpoint: {0}, StorageAccountKey: {1}, StorageAccountName: {2}";
            logger.LogMessage(msg, Endpoint, StorageAccountKey, StorageAccountName);

            var endpoint = new Uri(Endpoint);
            var credentials = new StorageCredentialsAccountAndKey(StorageAccountName, StorageAccountKey);
            var blobClient = new CloudBlobClient(endpoint, credentials)
            {
                Timeout = new TimeSpan(0, StorageClientTimeoutInMinutes, 0),
                ParallelOperationThreadCount = ParallelOptionsThreadCount
            };

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