namespace Windows.Azure.Msbuild.Test
{
    using System.Collections;
    using System.IO;
    using System.Linq;
    using System.Text;

    using AzureTools;

    using Microsoft.Build.Framework;
    using Microsoft.WindowsAzure.StorageClient;

    using Ninject;
    using Ninject.MockingKernel.RhinoMock;

    using NUnit.Framework;

    using Rhino.Mocks;

    [TestFixture]
    public class UploadBlockBlobTest
    {
        #region Fields

        private string accountKey;
        private string accountName;
        private IAzureBlob blob;
        private IAzureBlobClient blobClient;
        private IAzureBlobContainer blobContainer;
        private IFileManager fileManager;
        private RhinoMocksMockingKernel kernel;
        private ITaskLogger logger;
        private IAzureBlobClientFactory storageFactory;
        private MemoryStream stream;
        private CloudUploadBlockBlob task;
        private string uri;

        #endregion Fields

        #region Methods

        [TestCase("http://test.com", "accountName", "accountKey")]
        [TestCase("http://test123test.com", "accountName2", "accountKe3")]
        public void Execute_CreatesCloudStorageClient(string uri, string accountName, string accountKey)
        {
            storageFactory.ClearBehavior();
            blobClient.ClearBehavior();

            storageFactory.Expect(it => it.Create(Arg<CloudBlobClient>.Is.NotNull)).Return(blobClient);
            blobClient.Stub(it => it.GetContainerReference(task.ContainerName)).Return(blobContainer);
            blobContainer.Stub(it => it.CreateIfNotExists()).Return(true);

            task.Endpoint = uri;
            task.StorageAccountName = accountName;
            task.StorageAccountKey = accountKey;
            task.Execute();

            storageFactory.VerifyAllExpectations();
        }

        [SetUp]
        public void Setup()
        {
            kernel = new RhinoMocksMockingKernel();
            blobContainer = kernel.Get<IAzureBlobContainer>();
            logger = kernel.Get<ITaskLogger>();
            storageFactory = kernel.Get<IAzureBlobClientFactory>();
            blobClient = kernel.Get<IAzureBlobClient>();
            blob = kernel.Get<IAzureBlob>();
            fileManager = kernel.Get<IFileManager>();

            stream = new MemoryStream(Encoding.ASCII.GetBytes("this is a test stream \r\n this is a test stream"));

            blob.Stub(it => it.DeleteIfExists()).Return(true).Repeat.AtLeastOnce();
            storageFactory.Stub(it => it.Create(Arg<CloudBlobClient>.Is.NotNull)).Return(blobClient);
            blobClient.Stub(it => it.GetContainerReference(Arg<string>.Is.NotNull)).Return(blobContainer);
            blobContainer.Stub(it => it.GetBlockBlobReference(Arg<string>.Is.NotNull)).Repeat.AtLeastOnce().Return(blob);
            fileManager.Stub(it => it.GetFile(Arg<string>.Is.NotNull)).Return(stream);

            task = kernel.Get<CloudUploadBlockBlob>();

            accountName = "accountName";
            accountKey = "accountKey";

            uri = "http://testuri.com";
            task.Endpoint = uri;
            task.StorageAccountName = accountName;
            task.StorageAccountKey = accountKey;
            task.ContainerName = "someContainer";
            task.SourceFiles = new ITaskItem[] { };
            task.DestinationFiles = new ITaskItem[] { };
        }

        [TestCase("container1")]
        [TestCase("container2")]
        public void Task_Creates_TargetContainer(string containerName)
        {
            blobClient.ClearBehavior();
            blobContainer.ClearBehavior();

            task.ContainerName = containerName;
            storageFactory.Stub(it => it.Create(Arg<CloudBlobClient>.Is.NotNull)).Return(blobClient);
            blobClient.Expect(it => it.GetContainerReference(task.ContainerName)).Return(blobContainer);
            blobContainer.Expect(it => it.CreateIfNotExists()).Return(true);

            task.Execute();

            blobClient.VerifyAllExpectations();
            blobContainer.VerifyAllExpectations();
        }

        [TestCaseSource("GetBlobReferenceDataSource")]
        public void Task_Deletes_ExistingBlobs(ITaskItem[] sourceFiles, ITaskItem[] destinationFiles)
        {
            task.SourceFiles = sourceFiles;
            task.DestinationFiles = destinationFiles;

            blob.Expect(it => it.DeleteIfExists()).Return(true).Repeat.Times(task.DestinationFiles.Count());
            storageFactory.Stub(it => it.Create(Arg<CloudBlobClient>.Is.NotNull)).Return(blobClient);
            blobClient.Stub(it => it.GetContainerReference(Arg<string>.Is.NotNull)).Return(blobContainer);
            blobContainer.Stub(it => it.GetBlobReference(Arg<string>.Is.NotNull)).Repeat.AtLeastOnce().Return(blob);

            task.Execute();

            blob.AssertWasCalled(it => it.DeleteIfExists(), opt => opt.Repeat.Times(task.DestinationFiles.Count()));
        }

        [TestCaseSource("GetBlobReferenceDataSource")]
        public void Task_Gets_BlockBlobReference(ITaskItem[] sourceFiles, ITaskItem[] destinationFiles)
        {
            storageFactory.Stub(it => it.Create(Arg<CloudBlobClient>.Is.NotNull)).Return(blobClient);
            blobClient.Stub(it => it.GetContainerReference(Arg<string>.Is.NotNull)).Return(blobContainer);
            blobContainer.Expect(it => it.GetBlockBlobReference(Arg<string>.Is.NotNull)).Repeat.AtLeastOnce().Return(blob);

            task.SourceFiles = sourceFiles;
            task.DestinationFiles = destinationFiles;

            // act
            task.Execute();

            // assert
            blobContainer.AssertWasCalled(it => it.GetBlockBlobReference(Arg<string>.Is.NotNull), opt => opt.Repeat.Times(task.SourceFiles.Count()));
        }

        [TestCaseSource("GetBlobReferenceDataSource")]
        public void Task_ReadsFile_FromStream(ITaskItem[] sourceFiles, ITaskItem[] destinationFiles)
        {
            task.SourceFiles = sourceFiles;
            task.DestinationFiles = destinationFiles;

            fileManager.ClearBehavior();
            fileManager.Expect(it => it.GetFile(Arg<string>.Is.NotNull)).Return(stream);

            task.Execute();

            fileManager.VerifyAllExpectations();
        }

        [TestCaseSource("GetBlobReferenceDataSource")]
        public void Task_Uploads_Files(ITaskItem[] sourceFiles, ITaskItem[] destinationFiles)
        {
            task.SourceFiles = sourceFiles;
            task.DestinationFiles = destinationFiles;

            task.Execute();

            blob.AssertWasCalled(it => it.UploadFromStream(Arg<Stream>.Is.NotNull), opt => opt.Repeat.Times(task.DestinationFiles.Count()));
        }

        private static IEnumerable GetBlobReferenceDataSource()
        {
            var kernel = new RhinoMocksMockingKernel();
            var sourceItem1 = kernel.Get<ITaskItem>();
            sourceItem1.Stub(it => it.ItemSpec).Return("file1.txt");
            var sourceItem2 = kernel.Get<ITaskItem>();
            sourceItem2.Stub(it => it.ItemSpec).Return("file2.txt");
            var sourceFiles = new[] { sourceItem1, sourceItem2 };

            var destItem1 = kernel.Get<ITaskItem>();
            destItem1.Stub(it => it.ItemSpec).Return("prd\file1.txt");
            var destItem2 = kernel.Get<ITaskItem>();
            destItem2.Stub(it => it.ItemSpec).Return("prd\file2.txt");
            var destinationFiles = new ITaskItem[] { destItem1, destItem2 };

            yield return new TestCaseData(sourceFiles, destinationFiles);
        }

        #endregion Methods
    }
}