namespace Avanade.Platform.Services.Web.Unit.Tests.Configuration
{
    using System.Collections.Generic;
    using System.IO;

    using Avanade.Platform.Services.Web.Configuration;

    using NLog;

    using NUnit.Framework;

    /// <summary>
    /// Tests run for the reinitialisation of the component configuration values.
    /// </summary>
    [TestFixture]
    public class ReinitialisationTest
    {
        #region Fields

        private const string ConfigFolder = @"\ConfigFolder";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private DefaultConfigurationImpl configurationImpl;
        private string destFileName;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            configurationImpl = new DefaultConfigurationImpl();
        }

        /// <summary>
        /// Tears down the unit test harness.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            if (!string.IsNullOrEmpty(destFileName))
            {
                Logger.Debug("Deleting config file: {0}", destFileName);
                File.Delete(destFileName);
            }
        }

        /// <summary>
        /// Tests the reinitialisation.
        /// </summary>
        [Test]
        public void TestReinit()
        {
            Assert.IsTrue(configurationImpl.Reinitialise());
        }

        /// <summary>
        /// Tests the reinit with new external config file.
        /// Scenario:
        /// <li>
        /// <ol>VerifySection that the configuration is not available.</ol>
        /// <ol>Copy external scenario file.</ol>
        /// <ol>Reinitialisation commenced.</ol>
        /// <ol>Check for latest value.</ol>
        /// </li>
        /// </summary>
        [Test]
        public void TestReinitWithNewExternalConfigFile()
        {
            // Step 1
            Assert.Throws<KeyNotFoundException>(() => configurationImpl.Get("Greeting").GetValueAsString());
            Assert.Throws<KeyNotFoundException>(() => configurationImpl.Get("Test", "Recipe").GetValueAsString());

            // Step 2
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
                    destFileName = Path.Combine(targetPath, fileName);

                    // To copy a file to another location and
                    // overwrite the destination file if it already exists.
                    File.Copy(sourceFile, destFileName, true);
                }

                Logger.Debug("Copied config file to the location: {0}", destFileName);
            }

            // Step 3
            configurationImpl.Reinitialise();

            // Step 4
            Assert.AreEqual("Hello", configurationImpl.Get("Greeting").GetValueAsString());
            Assert.AreEqual("Cheese Cake", configurationImpl.Get("Test", "Recipe").GetValueAsString());
        }

        #endregion Methods
    }
}