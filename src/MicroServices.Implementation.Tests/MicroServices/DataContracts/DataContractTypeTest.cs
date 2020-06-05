using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Kingo.Serialization;
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

        #region [====== FromType ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FromType_Throws_IfTypeIsNull()
        {
            DataContractContentType.FromType(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FromType_Throws_IfTypeIsInterface()
        {
            DataContractContentType.FromType(typeof(IDisposable));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FromType_Throws_IfTypeIsAbstractClass()
        {
            DataContractContentType.FromType(typeof(Disposable));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FromType_Throws_IfTypeIsGenericTypeDefinition()
        {
            DataContractContentType.FromType(typeof(List<>));
        }

        [TestMethod]
        public void FromType_ReturnsExpectedContentType_IfTypeIsObject()
        {
            FromType_ReturnsExpectedContentType_IsTypeIsSupportedSystemType<object>();
        }

        [TestMethod]
        public void FromType_ReturnsExpectedContentType_IfTypeIsInt32()
        {
            FromType_ReturnsExpectedContentType_IsTypeIsSupportedSystemType<int>();
        }

        [TestMethod]
        public void FromType_ReturnsExpectedContentType_IfTypeIsGenericList()
        {
            FromType_ReturnsExpectedContentType_IsTypeIsSupportedSystemType<List<int>>();
        }

        [TestMethod]
        public void FromType_ReturnsExpectedContentType_IfTypeIsGenericDictionary()
        {
            FromType_ReturnsExpectedContentType_IsTypeIsSupportedSystemType<Dictionary<int, Disposable>>();
        }

        private static void FromType_ReturnsExpectedContentType_IsTypeIsSupportedSystemType<TType>()
        {
            var contentType = DataContractContentType.FromType(typeof(TType));

            Assert.IsTrue(contentType.ToString().StartsWith("http://schemas.datacontract.org/2004/07/"));
            Assert.IsTrue(contentType.IsSystemType(out var type));
            Assert.AreSame(typeof(TType), type);
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

            Assert.AreEqual("http://schemas.datacontract.org/2004/07/SomeName", type.ToString());
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

            Assert.AreEqual("https://mynamespace/SomeName", type.ToString());
        }

        #endregion
    }
}
