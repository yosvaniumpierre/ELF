namespace Avanade.BootStrapper.Web.Unit.Tests
{
    using System;

    using Avanade.BootStrapper.Web.Framework;

    using NUnit.Framework;

    [TestFixture]
    public class TestExceptionFactory
    {
        #region Methods

        [Test]
        public void TestRetrieveCodeId()
        {
            var exception = new Exception("Whatever");
            CustomAppException customAppException = ExceptionFactory.Wrap(exception);

            var codeId = ExceptionFactory.RetrieveExceptionCodeId(customAppException);
            Assert.AreEqual(-1, codeId);
        }

        #endregion Methods
    }
}