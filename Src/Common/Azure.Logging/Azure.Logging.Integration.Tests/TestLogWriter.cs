namespace Avanade.Azure.Logging.Integration.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class TestLogWriter
    {
        #region Methods

        [Test]
        public void TestLogEntry()
        {
            var logWriter = LogWriterFactory.CreateAzureWriter();
            logWriter.Log("2010-11-11", "DEBUG", "Integration testing conducted!", "Logger", "StackTrace");
        }

        #endregion Methods
    }
}