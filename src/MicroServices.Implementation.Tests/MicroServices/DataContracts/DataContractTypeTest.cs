using System;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.DataContracts
{
    [TestClass]
    public sealed class DataContractTypeTest
    {
        #region [====== Parse ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Parse_Throws_IfContentTypeIsNull()
        {
            DataContractContentType.Parse(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_Throws_IfContentTypeIsEmpty()
        {
            DataContractContentType.Parse(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parse_Throws_IfContentTypeIsRelativeUri()
        {
            DataContractContentType.Parse("SomeName");
        }

        [TestMethod]
        public void Parse_ReturnsExpectedType_IfContentTypeIsAbsoluteUri()
        {
            var type = DataContractContentType.Parse("https://some-type");

            Assert.AreEqual("https://some-type/", type.ToString());
        }

        [TestMethod]
        public void Parse_ReturnsEqualTypes_IfContentTypesOnlyDifferInCasing()
        {
            var typeA = DataContractContentType.Parse("https://some-type");
            var typeB = DataContractContentType.Parse("https://Some-Type");

            Assert.AreEqual(typeA, typeB);
        }

        #endregion

        #region [====== FromAttribute ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FromAttribute_Throws_IfAttributeIsNull()
        {
            DataContractContentType.FromAttribute(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FromAttribute_Throws_IfAttributeNameIsNull()
        {
            DataContractContentType.FromAttribute(new DataContractAttribute());
        }

        #endregion

        #region [====== FromName ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FromName_Throws_IfNameIsNull()
        {
            DataContractContentType.FromName(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FromName_Throws_IfNameIsAbsoluteUri()
        {
            DataContractContentType.FromName("https://SomeName");
        }

        [TestMethod]
        public void FromName_ReturnsExpectedType_IfNameIsRelativeUri()
        {
            var type = DataContractContentType.FromName("SomeName");

            Assert.AreEqual("http://schemas.datacontract.org/2004/07/somename", type.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FromName_Throws_IfNamespaceIsRelativeUri()
        {
            DataContractContentType.FromName("SomeName", "SomeNamespace");
        }

        [TestMethod]
        public void FromName_ReturnsExpectedType_IfNamespaceIsAbsoluteUri()
        {
            var type = DataContractContentType.FromName("SomeName", "https://mynamespace/");

            Assert.AreEqual("https://mynamespace/somename", type.ToString());
        }

        #endregion
    }
}
