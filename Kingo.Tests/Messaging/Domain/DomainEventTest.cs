using System;
using Kingo.Clocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Domain
{
    [TestClass]
    public sealed class DomainEventTest
    {
        #region [====== DomainEvents ======]

        private sealed class MissingMembersEvent : DomainEvent<MissingMembersEvent, Guid, int> { }  
      
        private sealed class IndexerWithAttributeEvent : DomainEvent<IndexerWithAttributeEvent, Guid, int>
        {
            [AggregateMember(AggregateMemberType.Key)]
            internal Guid this[int index]
            {
                get { throw new NotImplementedException(); }
            }

            [AggregateMember(AggregateMemberType.Version)]
            internal int this[Guid index]
            {
                get { throw new NotImplementedException(); }
            }
        }

        private sealed class MultipleExplicitMembersEvent : DomainEvent<MultipleExplicitMembersEvent, Guid, int>
        {
            [AggregateMember(AggregateMemberType.Key)]
            internal readonly Guid Id;

            [AggregateMember(AggregateMemberType.Key)]
            internal readonly Guid Key;

            [AggregateMember(AggregateMemberType.Version)]
            internal readonly int OneVersion;

            [AggregateMember(AggregateMemberType.Version)]
            internal readonly int TwoVersion;
        }

        private sealed class MultipleImplicitMembersEvent : DomainEvent<MultipleImplicitMembersEvent, Guid, int>
        {
            internal readonly Guid Id;
            internal readonly Guid Key;

            internal readonly int OneVersion;
            internal readonly int TwoVersion;
        }

        private sealed class IncompatibleExplicitMemberTypeEvent : DomainEvent<IncompatibleExplicitMemberTypeEvent, Guid, int>
        {
            [AggregateMember(AggregateMemberType.Key)]
            internal readonly string Id;

            [AggregateMember(AggregateMemberType.Version)]
            internal readonly string Version;
        }

        private sealed class IncompatibleImplicitMemberTypeEvent : DomainEvent<IncompatibleImplicitMemberTypeEvent, Guid, int>
        {            
            internal readonly string Id;            
            internal readonly string Version;
        }

        private sealed class CorrectImplicitMembersEvent : DomainEvent<CorrectImplicitMembersEvent, Guid, int>
        {
            internal readonly Guid Id;            
            internal readonly int Version;   
         
            internal CorrectImplicitMembersEvent(Guid id, int version)
            {
                Id = id;
                Version = version;
            }
        }

        private sealed class CorrectExplicitMembersEvent : DomainEvent<CorrectExplicitMembersEvent, Guid, int>
        {
            [AggregateMember(AggregateMemberType.Key)]
            internal readonly Guid Id;
            internal readonly Guid DummyKey;

            [AggregateMember(AggregateMemberType.Version)]
            internal readonly int Version;
            internal readonly int DummyVersion;

            internal CorrectExplicitMembersEvent(Guid id, int version)
            {
                Id = DummyKey = id;
                Version = DummyVersion = version;
            }
        }

        #endregion

        #region [====== Key ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Key_Throws_IfNoSuitedKeyMemberWasFound()
        {
            IVersionedObject<Guid, int> domainEvent = new MissingMembersEvent();

            try
            {
                Assert.Fail("Key was found unexpectedly: {0}.", domainEvent.Key);
            }
            catch (Exception exception)
            {
                Assert.AreEqual("Could not resolve member of type 'Key' on event of type 'MissingMembersEvent'. Consider decorating the appropriate member with the AggregateMemberAttribute.", exception.Message);
                throw;
            }            
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Key_Throws_IfIndexerWasDecoratedWithKeyAttribute()
        {
            IVersionedObject<Guid, int> domainEvent = new IndexerWithAttributeEvent();

            try
            {
                Assert.Fail("Key was found unexpectedly: {0}.", domainEvent.Key);
            }
            catch (Exception exception)
            {
                Assert.AreEqual("The AggregateMemberAttribute declared on property 'IndexerWithAttributeEvent.Item' cannot be declared on indexers.", exception.Message);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Key_Throws_IfMultipleExplicitlySuitedKeyMembersWereFound()
        {
            IVersionedObject<Guid, int> domainEvent = new MultipleExplicitMembersEvent();

            try
            {
                Assert.Fail("Key was found unexpectedly: {0}.", domainEvent.Key);
            }
            catch (Exception exception)
            {
                Assert.AreEqual("Could not decide on a member of type 'Key' on event of type 'MultipleExplicitMembersEvent' because multiple members were decorated with this attribute.", exception.Message);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Key_Throws_IfExplicitKeyMemberIsOfIncompatibleType()
        {
            IVersionedObject<Guid, int> domainEvent = new IncompatibleExplicitMemberTypeEvent();

            try
            {
                Assert.Fail("Key was found unexpectedly: {0}.", domainEvent.Key);
            }
            catch (Exception exception)
            {
                Assert.AreEqual("Value of member 'IncompatibleExplicitMemberTypeEvent.Id' of type 'System.String' could not be converted to an instance of type 'System.Guid'.", exception.Message);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Key_Throws_IfImplicitKeyMemberIsOfIncompatibleType()
        {
            IVersionedObject<Guid, int> domainEvent = new IncompatibleImplicitMemberTypeEvent();

            try
            {
                Assert.Fail("Key was found unexpectedly: {0}.", domainEvent.Key);
            }
            catch (Exception exception)
            {
                Assert.AreEqual("Value of member 'IncompatibleImplicitMemberTypeEvent.Id' of type 'System.String' could not be converted to an instance of type 'System.Guid'.", exception.Message);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Key_Throws_IfMultipleImplicitlySuitedKeyMembersWereFound()
        {
            IVersionedObject<Guid, int> domainEvent = new MultipleImplicitMembersEvent();

            try
            {
                Assert.Fail("Key was found unexpectedly: {0}.", domainEvent.Key);
            }
            catch (Exception exception)
            {
                Assert.AreEqual("Could not decide on a member of type 'Key' on event of type 'MultipleImplicitMembersEvent' because multiple candidate members were found. Consider decorating the appropriate member with the AggregateMemberAttribute.", exception.Message);
                throw;
            }            
        }                

        [TestMethod]
        public void Key_ReturnsExpectedValue_IfImplicitKeyMemberWasFound()
        {
            var key = Guid.NewGuid();
            var version = Clock.Current.LocalDateAndTime().Millisecond;
            IVersionedObject<Guid, int> domainEvent = new CorrectImplicitMembersEvent(key, version);

            Assert.AreEqual(key, domainEvent.Key);
        }

        [TestMethod]
        public void Key_ReturnsExpectedValue_IfExplicitKeyMemberWasFound()
        {
            var key = Guid.NewGuid();
            var version = Clock.Current.LocalDateAndTime().Millisecond;
            IVersionedObject<Guid, int> domainEvent = new CorrectExplicitMembersEvent(key, version);

            Assert.AreEqual(key, domainEvent.Key);
        }

        #endregion

        #region [====== Version ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Version_Throws_IfNoSuitedVersionMemberWasFound()
        {
            IVersionedObject<Guid, int> domainEvent = new MissingMembersEvent();

            try
            {
                Assert.Fail("Version was found unexpectedly: {0}.", domainEvent.Version);
            }
            catch (Exception exception)
            {
                Assert.AreEqual("Could not resolve member of type 'Version' on event of type 'MissingMembersEvent'. Consider decorating the appropriate member with the AggregateMemberAttribute.", exception.Message);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Version_Throws_IfIndexerWasDecoratedWithVersionAttribute()
        {
            IVersionedObject<Guid, int> domainEvent = new IndexerWithAttributeEvent();

            try
            {
                Assert.Fail("Version was found unexpectedly: {0}.", domainEvent.Version);
            }
            catch (Exception exception)
            {
                Assert.AreEqual("The AggregateMemberAttribute declared on property 'IndexerWithAttributeEvent.Item' cannot be declared on indexers.", exception.Message);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Version_Throws_IfMultipleExplicitlySuitedVersionMembersWereFound()
        {
            IVersionedObject<Guid, int> domainEvent = new MultipleExplicitMembersEvent();

            try
            {
                Assert.Fail("Version was found unexpectedly: {0}.", domainEvent.Version);
            }
            catch (Exception exception)
            {
                Assert.AreEqual("Could not decide on a member of type 'Version' on event of type 'MultipleExplicitMembersEvent' because multiple members were decorated with this attribute.", exception.Message);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Version_Throws_IfExplicitVersionMemberIsOfIncompatibleType()
        {
            IVersionedObject<Guid, int> domainEvent = new IncompatibleExplicitMemberTypeEvent();

            try
            {
                Assert.Fail("Version was found unexpectedly: {0}.", domainEvent.Version);
            }
            catch (Exception exception)
            {
                Assert.AreEqual("Value of member 'IncompatibleExplicitMemberTypeEvent.Version' of type 'System.String' could not be converted to an instance of type 'System.Int32'.", exception.Message);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Version_Throws_IfImplicitVersionMemberIsOfIncompatibleType()
        {
            IVersionedObject<Guid, int> domainEvent = new IncompatibleImplicitMemberTypeEvent();

            try
            {
                Assert.Fail("Version was found unexpectedly: {0}.", domainEvent.Version);
            }
            catch (Exception exception)
            {
                Assert.AreEqual("Value of member 'IncompatibleImplicitMemberTypeEvent.Version' of type 'System.String' could not be converted to an instance of type 'System.Int32'.", exception.Message);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Version_Throws_IfMultipleImplicitlySuitedVersionMembersWereFound()
        {
            IVersionedObject<Guid, int> domainEvent = new MultipleImplicitMembersEvent();

            try
            {
                Assert.Fail("Version was found unexpectedly: {0}.", domainEvent.Version);
            }
            catch (Exception exception)
            {
                Assert.AreEqual("Could not decide on a member of type 'Version' on event of type 'MultipleImplicitMembersEvent' because multiple candidate members were found. Consider decorating the appropriate member with the AggregateMemberAttribute.", exception.Message);
                throw;
            }
        }

        [TestMethod]
        public void Version_ReturnsExpectedValue_IfImplicitVersionMemberWasFound()
        {
            var key = Guid.NewGuid();
            var version = Clock.Current.LocalDateAndTime().Millisecond;
            IVersionedObject<Guid, int> domainEvent = new CorrectImplicitMembersEvent(key, version);

            Assert.AreEqual(version, domainEvent.Version);
        }

        [TestMethod]
        public void Version_ReturnsExpectedValue_IfExplicitVersionMemberWasFound()
        {
            var key = Guid.NewGuid();
            var version = Clock.Current.LocalDateAndTime().Millisecond;
            IVersionedObject<Guid, int> domainEvent = new CorrectExplicitMembersEvent(key, version);

            Assert.AreEqual(version, domainEvent.Version);
        }

        #endregion
    }
}
