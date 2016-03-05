using System;
using JetBrains.Annotations;
using Kingo.Clocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Domain
{
    [TestClass]
    public sealed class DomainEventTest
    {
#pragma warning disable
        #region [====== DomainEvents ======]

        private sealed class MissingAttributesEvent : DomainEvent<Guid, int> { }

        private sealed class MultipleAttributesEvent : DomainEvent<Guid, int>
        {
            [Key]
            public Guid KeyX;

            [Key, UsedImplicitly]
            public Guid KeyY
            {
                get;
                set;
            }

            [Version]
            public int VersionX;

            [Version, UsedImplicitly]
            public int VersionY
            {
                get;
                set;
            }
        }

        private sealed class IncompatibleMemberTypeEvent : DomainEvent<Guid, int>
        {
            [Key]
            public object Key;

            [Version]
            public VersionDummy Version;
        }

        private struct VersionDummy : IEquatable<VersionDummy>, IComparable<VersionDummy>
        {
            public bool Equals(VersionDummy other)
            {
                return true;
            }

            public int CompareTo(VersionDummy other)
            {
                return 0;
            }
        }

        private sealed class MatchingFieldTypeEvent : DomainEvent<Guid, int>
        {
            [Key]
            public readonly Guid Key;

            [Version]
            public readonly int Version;

            public MatchingFieldTypeEvent(Guid key, int version)
            {
                Key = key;
                Version = version;
            }
        }

        private sealed class MatchingPropertyTypeEvent : DomainEvent<Guid, int>
        {
            [Key]
            public Guid Key
            {
                get;
                private set;
            }

            [Version]
            public int Version
            {
                get;
                private set;
            }

            public MatchingPropertyTypeEvent(Guid key, int version)
            {
                Key = key;
                Version = version;
            }
        }

        #endregion
#pragma warning restore

        #region [====== Key ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Key_Throws_IfKeyAttributeIsMissing()
        {
            IHasKeyAndVersion<Guid, int> message = new MissingAttributesEvent();
   
            EnsureExceptionIsThrownWhenAccessing(message.Key);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Key_Throws_IfMultipleMembersAreMarkedAsKey()
        {
            IHasKeyAndVersion<Guid, int> message = new MultipleAttributesEvent();

            EnsureExceptionIsThrownWhenAccessing(message.Key);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Key_Throws_IfKeyIsOfIncompatibleType()
        {
            IHasKeyAndVersion<Guid, int> message = new IncompatibleMemberTypeEvent();

            EnsureExceptionIsThrownWhenAccessing(message.Key);
        }        

        [TestMethod]
        public void Key_ReturnsKey_IfMatchingTypedFieldIsMarkedAsKey()
        {
            var key = Guid.NewGuid();
            var version = Clock.Current.UtcDateAndTime().Millisecond;

            IHasKeyAndVersion<Guid, int> message = new MatchingFieldTypeEvent(key, version);

            Assert.AreEqual(key, message.Key);
        }

        [TestMethod]
        public void Key_ReturnsKey_IfMatchingTypedPropertyIsMarkedAsKey()
        {
            var key = Guid.NewGuid();
            var version = Clock.Current.UtcDateAndTime().Millisecond;

            IHasKeyAndVersion<Guid, int> message = new MatchingPropertyTypeEvent(key, version);

            Assert.AreEqual(key, message.Key);
        }

        #endregion

        #region [====== Version ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Version_Throws_IfVersionAttributeIsMissing()
        {
            IHasKeyAndVersion<Guid, int> message = new MissingAttributesEvent();

            EnsureExceptionIsThrownWhenAccessing(message.Version);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Version_Throws_IfMultipleMembersAreMarkedAsVersion()
        {
            IHasKeyAndVersion<Guid, int> message = new MultipleAttributesEvent();

            EnsureExceptionIsThrownWhenAccessing(message.Version);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Version_Throws_IfVersionIsOfIncompatibleType()
        {
            IHasKeyAndVersion<Guid, int> message = new IncompatibleMemberTypeEvent();

            EnsureExceptionIsThrownWhenAccessing(message.Version);
        }        

        [TestMethod]
        public void Version_ReturnsVersion_IfMatchingTypedFieldIsMarkedAsVersion()
        {
            var key = Guid.NewGuid();
            var version = Clock.Current.UtcDateAndTime().Millisecond;

            IHasKeyAndVersion<Guid, int> message = new MatchingFieldTypeEvent(key, version);

            Assert.AreEqual(version, message.Version);
        }

        [TestMethod]
        public void Version_ReturnsVersion_IfMatchingTypedPropertyIsMarkedAsVersion()
        {
            var key = Guid.NewGuid();
            var version = Clock.Current.UtcDateAndTime().Millisecond;

            IHasKeyAndVersion<Guid, int> message = new MatchingPropertyTypeEvent(key, version);

            Assert.AreEqual(version, message.Version);
        }

        #endregion

        private static void EnsureExceptionIsThrownWhenAccessing(object value)
        {
            Assert.Fail("Value should not have been retrieved: {0}.", value);
        }
    }
}
