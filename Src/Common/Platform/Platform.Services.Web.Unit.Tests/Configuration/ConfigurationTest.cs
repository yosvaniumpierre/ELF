namespace Avanade.Platform.Services.Web.Unit.Tests.Configuration
{
    using System;
    using System.Collections.Generic;

    using Avanade.Platform.Services.Web.Configuration;

    using NUnit.Framework;

    /// <summary>
    /// Main test for the main features of the Sections component
    /// </summary>
    [TestFixture]
    public class ConfigurationTest
    {
        #region Fields

        private DefaultConfigurationImpl defaultConfigurationImpl;

        #endregion Fields

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        [SetUp]
        public void Init()
        {
            //Interface is not used because the Init method had to be run
            //This is intentional since this component was designed to be run from within NKernel
            defaultConfigurationImpl = new DefaultConfigurationImpl("TestData.ini");
        }

        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void TestInterfaceBoundary()
        {
            Assert.Throws<FormatException>(() => defaultConfigurationImpl.Get("Test2", "MixedTypes").GetValueAsIntArray());
            Assert.Throws<FormatException>(() => defaultConfigurationImpl.Get("Test", "Name").GetValueAsInt());
            Assert.Throws<KeyNotFoundException>(() => defaultConfigurationImpl.Get("NoSuchSection", "NoName").GetValueAsString());
            Assert.Throws<KeyNotFoundException>(() => defaultConfigurationImpl.Get("Test", "NoSuchKey").GetValueAsString());
            Assert.Throws<ArgumentNullException>(() => defaultConfigurationImpl.Get("null", "").GetValueAsString());
            Assert.Throws<ArgumentNullException>(() => defaultConfigurationImpl.Get("", "Lost").GetValueAsString());
        }

        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void TestInterfaceDecryption()
        {
            Assert.AreEqual("0SdMh8UQ0buU//sRCH1a+eI/DB2tVU5+rwSkBumQVZs=", defaultConfigurationImpl.Get("Test", "Password").RawValue);

            string decrypted = defaultConfigurationImpl.Get("Test", "Password").GetValueAsDecryptedString();
            Assert.AreEqual("@d#_!H4lO//?=", decrypted);

            decrypted = defaultConfigurationImpl.Get("Test", "ID").GetValueAsDecryptedString();
            Assert.AreEqual("@d#_!H4lO//?=", decrypted);
        }

        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void TestInterfaceGoodUseCase()
        {
            string title = defaultConfigurationImpl.Get("Test", "Title").GetValueAsString();
            Assert.AreEqual("Fun Day", title);

            string name = defaultConfigurationImpl.Get("Test", "Name").GetValueAsString();
            Assert.AreEqual("HelloWorld", name);

            string[] names = defaultConfigurationImpl.Get("Test", "Names").GetValueAsStringArray();
            Assert.AreEqual(4, names.Length);
            Assert.AreEqual("Larry", names[0]);
            Assert.AreEqual("Moe", names[1]);
            Assert.AreEqual("Mike", names[2]);
            Assert.AreEqual("Jim", names[3]);

            int width = defaultConfigurationImpl.Get("Test", "Width").GetValueAsInt();
            Assert.AreEqual(1280, width);

            int[] dimensions = defaultConfigurationImpl.Get("Test", "Dimensions").GetValueAsIntArray();
            Assert.AreEqual(3, dimensions.Length);
            Assert.AreEqual(720, dimensions[0]);
            Assert.AreEqual(480, dimensions[1]);
            Assert.AreEqual(200, dimensions[2]);

            bool success = defaultConfigurationImpl.Get("Test", "Success").GetValueAsBool();
            Assert.True(success);

            bool failure = defaultConfigurationImpl.Get("Test", "Failure").GetValueAsBool();
            Assert.False(failure);

            bool[] boolArray = defaultConfigurationImpl.Get("Test", "BoolArray").GetValueAsBoolArray();
            Assert.AreEqual(3, boolArray.Length);
            Assert.True(boolArray[0]);
            Assert.False(boolArray[1]);
            Assert.True(boolArray[2]);

            float weight = defaultConfigurationImpl.Get("Test", "Weight").GetValueAsFloat();
            Assert.AreEqual(2.22, weight, 0.001f);

            float[] weights = defaultConfigurationImpl.Get("Test", "Weights").GetValueAsFloatArray();
            Assert.AreEqual(2, weights.Length);
            Assert.AreEqual(1.11, weights[0], 0.001f);
            Assert.AreEqual(3, weights[1], 0.001f);

            string weightString = defaultConfigurationImpl.Get("Test", "Weight").GetValueAsString();
            Assert.AreEqual("2.22", weightString);
        }

        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void TestInterfaceMixedStringArray()
        {
            string[] names = defaultConfigurationImpl.Get("Test2", "BadNames").GetValueAsStringArray();
            Assert.AreEqual(3, names.Length);
            Assert.AreEqual("Bad", names[0]);
            Assert.AreEqual("Worse", names[1]);
            Assert.AreEqual("Badder", names[2]);
        }

        #endregion Methods
    }
}