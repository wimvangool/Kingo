using System;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Domain
{
    [TestClass]
    public sealed class TypeToContractMapTest
    {
        #region [====== Non-Events ======]

        private interface IEvent { }

        private abstract class AbstractEvent { }

        private sealed class GenericEvent<T> { }        

        #endregion

        #region [====== Events ======]

        private sealed class EventWithoutAttribute { }

        [DataContract]
        private sealed class Event { }

        [DataContract(Name = "Event")]
        private sealed class EventA { }

        [DataContract(Name = "Event")]
        private sealed class EventB { }

        [DataContract(Name = "Event", Namespace = @"http://kingo.samples/v1")]
        private sealed class EventC { }

        #endregion

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Throws_IfTypesToScanIsNull()
        {
            new TypeToContractMap(null);
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfNoTypesAreRegistered()
        {
            var map = new TypeToContractMap(new []
            {
                typeof(IEvent),
                typeof(AbstractEvent),
                typeof(GenericEvent<>)               
            });    

            Assert.AreEqual("0 mapping(s) registered.", map.ToString());
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfOneTypeIsRegistered()
        {
            var map = new TypeToContractMap(new[]
            {
                typeof(Event),
                typeof(Event)
            });

            Assert.AreEqual("1 mapping(s) registered.", map.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetContract_Throws_IfTypeIsNull()
        {
            var map = new TypeToContractMap(Enumerable.Empty<Type>());

            map.GetContract(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetContract_Throws_IfTypesToScanIsEmpty()
        {
            var map = new TypeToContractMap(Enumerable.Empty<Type>());

            map.GetContract(typeof(EventA));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetContract_Throws_IfTypeIsNotMappedToContract()
        {
            var map = new TypeToContractMap(new [] { typeof(EventA) });

            map.GetContract(typeof(EventB));
        }

        [TestMethod]
        public void GetContract_ReturnsExpectedContract_IfTypeIsMappedToContract_And_TypeDoesNotHaveDataContractAttribute()
        {
            var map = new TypeToContractMap(new[] { typeof(EventWithoutAttribute) });

            Assert.AreEqual("EventWithoutAttribute", map.GetContract(typeof(EventWithoutAttribute)));
        }

        [TestMethod]
        public void GetContract_ReturnsExpectedContract_IfTypeIsMappedToContract_And_NamespaceIsNotSet_And_NameIsNotSet()
        {
            var map = new TypeToContractMap(new[] { typeof(Event) });

            Assert.AreEqual("Event", map.GetContract(typeof(Event)));
        }

        [TestMethod]
        public void GetContract_ReturnsExpectedContract_IfTypeIsMappedToContract_And_NamespaceIsNotSet()
        {
            var map = new TypeToContractMap(new[] { typeof(EventA) });

            Assert.AreEqual("Event", map.GetContract(typeof(EventA)));
        }

        [TestMethod]
        public void GetContract_ReturnsExpectedContract_IfTypeIsMappedToContract_And_NamespaceIsSet()
        {
            var map = new TypeToContractMap(new[] { typeof(EventC) });

            Assert.AreEqual("http://kingo.samples/v1/Event", map.GetContract(typeof(EventC)));
        }

        [TestMethod]
        [ExpectedException(typeof(TypeToContractMapException))]
        public void GetContract_Throws_IfTwoTypesShareTheSameContract()
        {
            var map = new TypeToContractMap(new[] { typeof(EventA), typeof(EventB) });

            map.GetContract(typeof(EventA));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetType_Throws_IfContractIsNull()
        {
            var map = new TypeToContractMap(Enumerable.Empty<Type>());

            map.GetType(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetType_Throws_IfTypesToScanIsEmpty()
        {
            var map = new TypeToContractMap(Enumerable.Empty<Type>());

            map.GetType("Event");
        }

        [TestMethod]
        public void GetType_ReturnsType_IfContractIsMappedToType_And_TypeDoesNotHaveDataContractAttribute()
        {
            var map = new TypeToContractMap(new[] { typeof(EventWithoutAttribute) });

            Assert.AreEqual(typeof(EventWithoutAttribute), map.GetType("EventWithoutAttribute"));
        }

        [TestMethod]
        public void GetType_ReturnsType_IfContractIsMappedToType_And_NamespaceIsNotSet_And_NameIsNotSet()
        {
            var map = new TypeToContractMap(new[] { typeof(Event) });

            Assert.AreEqual(typeof(Event), map.GetType("Event"));
        }

        [TestMethod]
        public void GetType_ReturnsType_IfContractIsMappedToType_And_NamespaceIsNotSet()
        {
            var map = new TypeToContractMap(new[] { typeof(EventA) });

            Assert.AreEqual(typeof(EventA), map.GetType("Event"));
        }

        [TestMethod]
        public void GetType_ReturnsType_IfContractIsMappedToType_And_NamespaceIsSet()
        {
            var map = new TypeToContractMap(new[] { typeof(EventC) });

            Assert.AreEqual(typeof(EventC), map.GetType(@"http://kingo.samples/v1/Event"));
        }

        [TestMethod]
        [ExpectedException(typeof(TypeToContractMapException))]
        public void GetType_Throws_IfTwoTypesShareTheSameContract()
        {
            var map = new TypeToContractMap(new[] { typeof(EventA), typeof(EventB) });

            map.GetType("Event");
        }
    }
}
