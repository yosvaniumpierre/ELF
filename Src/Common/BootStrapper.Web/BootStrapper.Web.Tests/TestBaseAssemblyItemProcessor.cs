namespace Avanade.BootStrapper.Web.Unit.Tests
{
    using Azure;
    using Azure.Processor;

    using NUnit.Framework;

    class FakeAssemblyItemProcessor : BaseAssemblyItemProcessor
    {
        #region Methods

        public override bool Process(AssemblyItem assemblyItem)
        {
            return IsMatch(assemblyItem.Name, "*.Fake.dll");
        }

        #endregion Methods
    }

    [TestFixture]
    class TestBaseAssemblyItemProcessor
    {
        #region Methods

        [Test]
        public void TestAssemblyNameMatch()
        {
            var assemblyItemProcessor = new FakeAssemblyItemProcessor();
            Assert.IsTrue(assemblyItemProcessor.Process(new AssemblyItem ("Test.Fake.dll", null)));
            Assert.IsFalse(assemblyItemProcessor.Process(new AssemblyItem("Test.Plugin.dll", null)));

            Assert.IsTrue(assemblyItemProcessor.Process(new AssemblyItem("Test.fake.dll", null)));
            Assert.IsFalse(assemblyItemProcessor.Process(new AssemblyItem("Test.PLUGIN.dll", null)));
        }

        #endregion Methods
    }
}