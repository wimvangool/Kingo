using System;
using Kingo.Clocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Domain
{
    [TestClass]
    public sealed class AggregateEventTest
    {
        #region [====== Events ======]

        private sealed class EventWithNoAttributes : AggregateEvent<Guid, int> { }

        private sealed class EventWithTooManyAttributes : AggregateEvent<Guid, int>
        {
            [AggregateId]
            public Guid Id1
            {
                get;
                set;
            }

            [AggregateId]
            public Guid Id2
            {
                get;
                set;
            }

            [AggregateVersion]
            public int Version1
            {
                get;
                set;
            }

            [AggregateVersion]
            public int Version2
            {
                get;
                set;
            }
        }

        private sealed class EventWithAttributesOnIndexer : AggregateEvent<Guid, int>
        {
            [AggregateId]
            public Guid this[Guid id]
            {
                get { return id; }
                set { }
            }
                
            [AggregateVersion]
            public int this[int version]
            {
                get { return version; }
                set { }
            }
        }

        private sealed class EventWithWrongPropertyTypes : AggregateEvent<Guid, int>
        {
            [AggregateId]
            public object Id
            {
                get;
                set;
            }

            [AggregateVersion]
            public object Version
            {
                get;
                set;
            }
        }

        private sealed class EventWithNoGetters : AggregateEvent<Guid, int>
        {
            [AggregateId]
            public Guid Id
            {
                set { }
            }

            [AggregateVersion]
            public int Version
            {
                set { }
            }
        }

        private sealed class EventWithNoSetters : AggregateEvent<Guid, int>
        {
            [AggregateId]
            public Guid Id
            {
                get;
            }

            [AggregateVersion]
            public int Version
            {
                get;
            }
        }

        private sealed class EventWithPublicGettersAndSetters : AggregateEvent<Guid, int>
        {
            [AggregateId]
            public Guid Id
            {
                get;
                set;
            }

            [AggregateVersion]
            public int Version
            {
                get;
                set;
            }
        }

        private sealed class EventWithPublicGettersAndPrivateSetters : AggregateEvent<Guid, int>
        {
            public EventWithPublicGettersAndPrivateSetters(Guid id, int version)
            {
                Id = id;
                Version = version;
            }

            [AggregateId]
            public Guid Id
            {
                get;
                private set;
            }

            [AggregateVersion]
            public int Version
            {
                get;
                private set;
            }
        }

        #endregion

        #region [====== GetAggregateId ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetAggregateId_Throws_IfNoPropertyHasAggregateIdAttribute()
        {
            try
            {
                GetAggregateId(new EventWithNoAttributes());
            }
            catch (InvalidOperationException exception)
            {
                Assert.IsTrue(exception.Message.StartsWith("Missing declaration of attribute 'AggregateIdAttribute' on a property in class 'EventWithNoAttributes'."));
                Assert.IsTrue(exception.Message.EndsWith("Please make sure exactly one public property that has a getter and a setter is decorated with this attribute."));
                throw;
            }            
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetAggregateId_Throws_IfMultiplePropertiesHaveAggregateIdAttribute()
        {
            try
            {
                GetAggregateId(new EventWithTooManyAttributes());
            }
            catch (InvalidOperationException exception)
            {
                Assert.IsTrue(exception.Message.StartsWith("Illegal use of attribute 'AggregateIdAttribute' in class 'EventWithTooManyAttributes': multiple properties (Id1, Id2) have been decorated with the attribute where only one is allowed."));
                Assert.IsTrue(exception.Message.EndsWith("Please make sure exactly one public property that has a getter and a setter is decorated with this attribute."));
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetAggregateId_Throws_IfPropertyIsIndexer()
        {
            try
            {
                GetAggregateId(new EventWithAttributesOnIndexer());
            }
            catch (InvalidOperationException exception)
            {
                Assert.IsTrue(exception.Message.StartsWith("Illegal use of attribute 'AggregateIdAttribute' in class 'EventWithAttributesOnIndexer': cannot declare this attribute on an indexer (Item)."));
                Assert.IsTrue(exception.Message.EndsWith("Please make sure exactly one public property that has a getter and a setter is decorated with this attribute."));
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetAggregateId_Throws_IfPropertyIsOfWrongType()
        {
            try
            {
                GetAggregateId(new EventWithWrongPropertyTypes());
            }
            catch (InvalidOperationException exception)
            {
                Assert.IsTrue(exception.Message.StartsWith("Illegal use of attribute 'AggregateIdAttribute' in class 'EventWithWrongPropertyTypes': property 'Id' must be of type 'Guid'."));
                Assert.IsTrue(exception.Message.EndsWith("Please make sure exactly one public property that has a getter and a setter is decorated with this attribute."));
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetAggregateId_Throws_IfPropertyDoesNotHaveGetter()
        {
            try
            {
                GetAggregateId(new EventWithNoGetters());
            }
            catch (InvalidOperationException exception)
            {
                Assert.IsTrue(exception.Message.StartsWith("Illegal use of attribute 'AggregateIdAttribute' in class 'EventWithNoGetters': property 'Id' must have a getter."));
                Assert.IsTrue(exception.Message.EndsWith("Please make sure exactly one public property that has a getter and a setter is decorated with this attribute."));
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetAggregateId_Throws_IfPropertyDoesNotHaveSetter()
        {
            try
            {
                GetAggregateId(new EventWithNoSetters());
            }
            catch (InvalidOperationException exception)
            {
                Assert.IsTrue(exception.Message.StartsWith("Illegal use of attribute 'AggregateIdAttribute' in class 'EventWithNoSetters': property 'Id' must have a setter."));
                Assert.IsTrue(exception.Message.EndsWith("Please make sure exactly one public property that has a getter and a setter is decorated with this attribute."));
                throw;
            }
        }      
        
        [TestMethod]
        public void GetAggregateId_ReturnsExpectedValue_IfAttributeIsPlacedOnPropertyWithPublicGetterAndSetter()
        {
            var @event = new EventWithPublicGettersAndSetters()
            {
                Id = Guid.NewGuid()
            };

            Assert.AreEqual(@event.Id, GetAggregateId(@event));
        }

        [TestMethod]
        public void GetAggregateId_ReturnsExpectedValue_IfAttributeIsPlacedOnPropertyWithPublicGetterAndPrivateSetter()
        {
            var @event = new EventWithPublicGettersAndPrivateSetters(Guid.NewGuid(), 0);

            Assert.AreEqual(@event.Id, GetAggregateId(@event));
        }

        #endregion

        #region [====== SetAggregateId ======]

        [TestMethod]
        public void SetAggregateId_SetsExpectedValue_IfAttributeIsPlacedOnPropertyWithPublicGetterAndSetter()
        {
            var @event = new EventWithPublicGettersAndSetters();
            var value = Guid.NewGuid();

            SetAggregateId(@event, value);

            Assert.AreEqual(value, @event.Id);
        }

        [TestMethod]
        public void SetAggregateId_SetsExpectedValue_IfAttributeIsPlacedOnPropertyWithPublicGetterAndPrivateSetter()
        {
            var @event = new EventWithPublicGettersAndPrivateSetters(Guid.Empty, 0);
            var value = Guid.NewGuid();

            SetAggregateId(@event, value);

            Assert.AreEqual(value, @event.Id);
        }

        #endregion

        #region [====== GetAggregateVersion ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetAggregateVersion_Throws_IfNoPropertyHasAggregateVersionAttribute()
        {
            try
            {
                GetAggregateVersion(new EventWithNoAttributes());
            }
            catch (InvalidOperationException exception)
            {
                Assert.IsTrue(exception.Message.StartsWith("Missing declaration of attribute 'AggregateVersionAttribute' on a property in class 'EventWithNoAttributes'."));
                Assert.IsTrue(exception.Message.EndsWith("Please make sure exactly one public property that has a getter and a setter is decorated with this attribute."));
                throw;
            }            
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetAggregateVersion_Throws_IfMultiplePropertiesHaveAggregateVersionAttribute()
        {
            try
            {
                GetAggregateVersion(new EventWithTooManyAttributes());
            }
            catch (InvalidOperationException exception)
            {
                Assert.IsTrue(exception.Message.StartsWith("Illegal use of attribute 'AggregateVersionAttribute' in class 'EventWithTooManyAttributes': multiple properties (Version1, Version2) have been decorated with the attribute where only one is allowed."));
                Assert.IsTrue(exception.Message.EndsWith("Please make sure exactly one public property that has a getter and a setter is decorated with this attribute."));
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetAggregateVersion_Throws_IfPropertyIsIndexer()
        {
            try
            {
                GetAggregateVersion(new EventWithAttributesOnIndexer());
            }
            catch (InvalidOperationException exception)
            {
                Assert.IsTrue(exception.Message.StartsWith("Illegal use of attribute 'AggregateVersionAttribute' in class 'EventWithAttributesOnIndexer': cannot declare this attribute on an indexer (Item)."));
                Assert.IsTrue(exception.Message.EndsWith("Please make sure exactly one public property that has a getter and a setter is decorated with this attribute."));
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetAggregateVersion_Throws_IfPropertyIsOfWrongType()
        {
            try
            {
                GetAggregateVersion(new EventWithWrongPropertyTypes());
            }
            catch (InvalidOperationException exception)
            {
                Assert.IsTrue(exception.Message.StartsWith("Illegal use of attribute 'AggregateVersionAttribute' in class 'EventWithWrongPropertyTypes': property 'Version' must be of type 'Int32'."));
                Assert.IsTrue(exception.Message.EndsWith("Please make sure exactly one public property that has a getter and a setter is decorated with this attribute."));
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetAggregateVersion_Throws_IfPropertyDoesNotHaveGetter()
        {
            try
            {
                GetAggregateVersion(new EventWithNoGetters());
            }
            catch (InvalidOperationException exception)
            {
                Assert.IsTrue(exception.Message.StartsWith("Illegal use of attribute 'AggregateVersionAttribute' in class 'EventWithNoGetters': property 'Version' must have a getter."));
                Assert.IsTrue(exception.Message.EndsWith("Please make sure exactly one public property that has a getter and a setter is decorated with this attribute."));
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetAggregateVersion_Throws_IfPropertyDoesNotHaveSetter()
        {
            try
            {
                GetAggregateVersion(new EventWithNoSetters());
            }
            catch (InvalidOperationException exception)
            {
                Assert.IsTrue(exception.Message.StartsWith("Illegal use of attribute 'AggregateVersionAttribute' in class 'EventWithNoSetters': property 'Version' must have a setter."));
                Assert.IsTrue(exception.Message.EndsWith("Please make sure exactly one public property that has a getter and a setter is decorated with this attribute."));
                throw;
            }
        }

        [TestMethod]
        public void GetAggregateVersion_ReturnsExpectedValue_IfAttributeIsPlacedOnPropertyWithPublicGetterAndSetter()
        {
            var @event = new EventWithPublicGettersAndSetters()
            {
                Version = Clock.Current.UtcDateAndTime().Millisecond
            };

            Assert.AreEqual(@event.Version, GetAggregateVersion(@event));
        }

        [TestMethod]
        public void GetAggregateVersion_ReturnsExpectedValue_IfAttributeIsPlacedOnPropertyWithPublicGetterAndPrivateSetter()
        {
            var @event = new EventWithPublicGettersAndPrivateSetters(Guid.Empty, Clock.Current.UtcDateAndTime().Millisecond);

            Assert.AreEqual(@event.Version, GetAggregateVersion(@event));
        }

        #endregion

        #region [====== SetAggregateVersion ======]

        [TestMethod]
        public void SetAggregateVersion_SetsExpectedValue_IfAttributeIsPlacedOnPropertyWithPublicGetterAndSetter()
        {
            var @event = new EventWithPublicGettersAndSetters();
            var value = Clock.Current.UtcDateAndTime().Millisecond;

            SetAggregateVersion(@event, value);

            Assert.AreEqual(value, @event.Version);
        }

        [TestMethod]
        public void SetAggregateVersion_SetsExpectedValue_IfAttributeIsPlacedOnPropertyWithPublicGetterAndPrivateSetter()
        {
            var @event = new EventWithPublicGettersAndPrivateSetters(Guid.Empty, 0);
            var value = Clock.Current.UtcDateAndTime().Millisecond;

            SetAggregateVersion(@event, value);

            Assert.AreEqual(value, @event.Version);
        }

        #endregion

        #region [====== ToString ======]

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfPropertiesAreNotCorrectlyDefined()
        {
            Assert.AreEqual("EventWithNoAttributes (?, ?)", new EventWithNoAttributes().ToString());
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfPropertiesAreCorrectlyDefined()
        {
            Assert.AreEqual("EventWithPublicGettersAndSetters (Id, Version)", new EventWithPublicGettersAndSetters().ToString());
        }

        #endregion

        private static Guid GetAggregateId(IAggregateEvent<Guid, int> aggregateEvent) =>
            aggregateEvent.AggregateId;

        private static void SetAggregateId(IAggregateEvent<Guid, int> aggregateEvent, Guid value) =>
            aggregateEvent.AggregateId = value;

        private static int GetAggregateVersion(IAggregateEvent<Guid, int> aggregateEvent) =>
            aggregateEvent.AggregateVersion;

        private static void SetAggregateVersion(IAggregateEvent<Guid, int> aggregateEvent, int version) =>
            aggregateEvent.AggregateVersion = version;
    }
}
