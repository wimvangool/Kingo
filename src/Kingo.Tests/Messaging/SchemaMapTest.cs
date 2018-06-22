using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class SchemaMapTest
    {
        #region [====== DataContracts ======]

        [DataContract]
        private sealed class DataContractWithDefaultValues { }

        [DataContract(Name = "CustomName/v1")]
        private sealed class DataContractWithNameSetExplicitly { }

        [DataContract(Namespace = "http://www.github.com/wimvangool/kingo/", Name = "CustomName/v1")]
        private sealed class DataContractWithAllValuesSetExplicitly { }

        #endregion

        private SchemaMap _map;

        [TestInitialize]
        public void Setup()
        {
            _map = new SchemaMap();
        }

        #region [====== AddDataContracts ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddDataContracts_Throws_IfTypesIsNull()
        {
            AddDataContracts(null);
        }

        [TestMethod]
        public void AddDataContracts_DoesNothing_IfTypeDoesNotHaveDataContractAttribute()
        {
            Assert.AreEqual(0, AddDataContracts(typeof(object)).Count);
        }

        [TestMethod]
        public void AddDataContracts_AddsTypeMapping_IfTypeHasDataContractAttributeWithDefaultValues()
        {
            var type = typeof(DataContractWithDefaultValues);
            var map = AddDataContracts(type);

            Assert.AreEqual(1, map.Count);
            Assert.AreEqual(nameof(DataContractWithDefaultValues), map.GetTypeId(type));
        }

        [TestMethod]
        public void AddDataContracts_AddsTypeMapping_IfTypeHasDataContractAttributeWithNameSetExplicitly()
        {
            var type = typeof(DataContractWithNameSetExplicitly);
            var map = AddDataContracts(type);

            Assert.AreEqual(1, map.Count);
            Assert.AreEqual("CustomName/v1", map.GetTypeId(type));
        }

        [TestMethod]
        public void AddDataContracts_AddsTypeMapping_IfTypeHasDataContractAttributeWithAllValuesSetExplicitly()
        {
            var type = typeof(DataContractWithAllValuesSetExplicitly);
            var map = AddDataContracts(type);

            Assert.AreEqual(1, map.Count);
            Assert.AreEqual("http://www.github.com/wimvangool/kingo/CustomName/v1", map.GetTypeId(type));
        }

        private SchemaMap AddDataContracts(params Type[] types) =>
            _map.AddDataContracts(types);

        #endregion

        #region [====== Add ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_Throws_IfTypeIdIsNull()
        {
            _map.Add(null, typeof(object));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_Throws_IfTypeIsNull()
        {
            _map.Add(string.Empty, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Add_Throws_IfTypeIdIsAlreadyMapped_And_TypeIdsAreSame()
        {
            _map.Add(string.Empty, typeof(object));

            try
            {
                _map.Add(string.Empty, typeof(string));
            }
            catch (ArgumentException exception)
            {
                Assert.IsTrue(exception.Message.Contains("Cannot add mapping for type-id '' because it has already been mapped."));
                throw;
            }            
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Add_Throws_IfTypeIdIsAlreadyMapped_And_TypeIdsDifferOnlyByCase()
        {
            _map.Add("a", typeof(object));

            try
            {
                _map.Add("A", typeof(string));
            }
            catch (ArgumentException exception)
            {
                Assert.IsTrue(exception.Message.Contains("Cannot add mapping for type-id 'A' because it has already been mapped."));
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Add_Throws_IfTypeIsAlreadyMapped()
        {
            _map.Add(string.Empty, typeof(object));

            try
            {
                _map.Add(" ", typeof(object));
            }
            catch (ArgumentException exception)
            {
                Assert.IsTrue(exception.Message.Contains("Cannot add mapping for type 'Object' because it has already been mapped."));
                throw;
            }
        }

        #endregion

        #region [====== GetType ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetType_Throws_IfTypeIdIsNull()
        {
            _map.GetType(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetType_Throws_IfTypeIsNotFound()
        {
            try
            {
                _map.GetType(string.Empty);
            }
            catch (ArgumentException exception)
            {
                Assert.IsTrue(exception.Message.Contains("Type that corresponds to type-id '' not found."));
                throw;
            }            
        }

        [TestMethod]
        public void GetType_ReturnsExpectedType_IfTypeIsFound()
        {
            _map.Add(string.Empty, typeof(object));

            Assert.AreEqual(typeof(object), _map.GetType(string.Empty));
        }

        #endregion

        #region [====== GetTypeId ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetTypeId_Throws_IfTypeIsNull()
        {
            _map.GetTypeId(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetTypeId_Throws_IfTypeIdIsNotFound()
        {
            try
            {
                _map.GetTypeId(typeof(object));
            }
            catch (ArgumentException exception)
            {
                Assert.IsTrue(exception.Message.Contains("Type-id that corresponds to type 'Object' not found."));
                throw;
            }
        }

        [TestMethod]
        public void GetTypeId_ReturnsExpectedTypeId_IfTypeIdIsFound()
        {
            _map.Add(string.Empty, typeof(object));

            Assert.AreEqual(string.Empty, _map.GetTypeId(typeof(object)));
        }

        #endregion
    }
}
