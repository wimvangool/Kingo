using System;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class DataContractTypeTest
    {
        #region [====== Parse ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Parse_Throws_IfContentTypeIsNull()
        {
            DataContractType.Parse(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_Throws_IfContentTypeIsEmpty()
        {
            DataContractType.Parse(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_Throws_IfContentTypeIsRelativeUri()
        {
            DataContractType.Parse("SomeName");
        }

        [TestMethod]
        public void Parse_ReturnsExpectedType_IfContentTypeIsAbsoluteUri()
        {
            var type = DataContractType.Parse("https://some-type");

            Assert.AreEqual("https://some-type/", type.ToString());
        }

        [TestMethod]
        public void Parse_ReturnsEqualTypes_IfContentTypesOnlyDifferInCasing()
        {
            var typeA = DataContractType.Parse("https://some-type");
            var typeB = DataContractType.Parse("https://Some-Type");

            Assert.AreEqual(typeA, typeB);
        }

        #endregion

        #region [====== FromAttribute ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FromAttribute_Throws_IfAttributeIsNull()
        {
            DataContractType.FromAttribute(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FromAttribute_Throws_IfAttributeNameIsNull()
        {
            DataContractType.FromAttribute(new DataContractAttribute());
        }

        #endregion

        #region [====== FromName ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FromName_Throws_IfNameIsNull()
        {
            DataContractType.FromName(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FromName_Throws_IfNameIsAbsoluteUri()
        {
            DataContractType.FromName("https://SomeName");
        }

        [TestMethod]
        public void FromName_ReturnsExpectedType_IfNameIsRelativeUri()
        {
            var type = DataContractType.FromName("SomeName");

            Assert.AreEqual("https://schemas.kingo.net/2020/05/somename", type.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FromName_Throws_IfNamespaceIsRelativeUri()
        {
            DataContractType.FromName("SomeName", "SomeNamespace");
        }

        [TestMethod]
        public void FromName_ReturnsExpectedType_IfNamespaceIsAbsoluteUri()
        {
            var type = DataContractType.FromName("SomeName", "https://mynamespace/");

            Assert.AreEqual("https://mynamespace/somename", type.ToString());
        }

        #endregion
    }
}
