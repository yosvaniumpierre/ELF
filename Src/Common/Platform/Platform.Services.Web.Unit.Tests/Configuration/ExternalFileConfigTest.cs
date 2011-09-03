using Avanade.Platform.Services.Web.Configuration;

namespace Avanade.Platform.Services.Web.Unit.Tests.Configuration
{
    using System.IO;

    using NUnit.Framework;

    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class ExternalFileConfigTest
    {
        #region Fields

        private const string ConfigFolder = @"\ConfigFolder";

        private IConfiguration configuration;
        private string destConfigFile;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            const string fileName = "CrimsonLogic.Common.Configuration.ini";

            FileInfo fileInfo = ConfigFileLocator.GetFromAppConfigLocation(fileName);
            Assert.NotNull(fileInfo);

            //Copy the ini file to the subfolder
            {
                string sourcePath = fileInfo.DirectoryName;
                string targetPath = sourcePath + ConfigFolder;
                // To copy a folder's contents to a new location:
                // Create a new target folder, if necessary.
                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                }

                // Use Path class to manipulate file and directory paths.
                {
                    string sourceFile = Path.Combine(sourcePath, fileName);
                    destConfigFile = Path.Combine(targetPath, fileName);

                    // To copy a file to another location and
                    // overwrite the destination file if it already exists.
                    File.Copy(sourceFile, destConfigFile, true);
                }
            }

            configuration = new DefaultConfigurationImpl("TestData.ini");
        }

        /// <summary>
        /// Tears down this instance.
        /// </summary>
        [TearDown]
        public void Teardown()
        {
            File.Delete(destConfigFile);
        }

        /// <summary>
        /// Tests the external config file.
        /// </summary>
        [Test]
        public void TestExternalConfigFile()
        {
            Assert.AreEqual("Hello", configuration.Get("Greeting").GetValueAsString());
            Assert.AreEqual("Earth", configuration.Get("Planets", "World").GetValueAsString());
            Assert.AreEqual(true, configuration.Get("Test", "Success").GetValueAsBool());
            Assert.AreEqual("Cheese Cake", configuration.Get("Test", "Recipe").GetValueAsString());
        }

        #endregion Methods
    }
}