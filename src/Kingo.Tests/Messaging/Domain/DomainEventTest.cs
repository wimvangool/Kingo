using System;
using Kingo.Clocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Domain
{
    [TestClass]
    public sealed class DomainEventTest
    {
#pragma warning disable
        #region [====== DomainEvents ======]

        private sealed class MissingMembersEvent : DomainEvent<Guid, int> { }                

        private sealed class MultipleImplicitMembersEvent : DomainEvent<Guid, int>
        {
            internal readonly Guid Id;
            internal readonly Guid Key;

            internal readonly int OneVersion;
            internal readonly int TwoVersion;
        }       

        private sealed class IncompatibleImplicitMemberTypeEvent : DomainEvent<Guid, int>
        {            
            internal readonly string Id;            
            internal readonly string Version;
        }

        private sealed class CorrectImplicitMembersEvent : DomainEvent<Guid, int>
        {
            internal readonly Guid Id;            
            internal readonly int Version;   
         
            internal CorrectImplicitMembersEvent(Guid id, int version)
            {
                Id = id;
                Version = version;
            }
        }        

        #endregion

#pragma warning restore

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
                Assert.AreEqual("Could not resolve member of type 'Key' on event of type 'MissingMembersEvent'. Consider overriding the Key and Version properties.", exception.Message);
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
                Assert.AreEqual("Could not decide on a member of type 'Key' on event of type 'MultipleImplicitMembersEvent' because multiple candidate members were found. Consider overriding the Key and Version properties.", exception.Message);
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
                Assert.AreEqual("Could not resolve member of type 'Version' on event of type 'MissingMembersEvent'. Consider overriding the Key and Version properties.", exception.Message);
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
                Assert.AreEqual("Could not decide on a member of type 'Version' on event of type 'MultipleImplicitMembersEvent' because multiple candidate members were found. Consider overriding the Key and Version properties.", exception.Message);
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

        #endregion
    }
}
