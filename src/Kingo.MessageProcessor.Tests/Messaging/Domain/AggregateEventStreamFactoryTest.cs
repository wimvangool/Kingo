using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Domain
{
    [TestClass]
    public sealed class AggregateEventStreamFactoryTest
    {
        #region [====== AggregateStubs ======]

        private sealed class AggregateStubA : EventStream<Guid, int>
        {
            private readonly Guid _id;
            private int _version;
            private int _x;
            private int _y;

            internal AggregateStubA(Guid id, int version = 0)
            {
                _id = id;
                _version = version;
            }

            public override Guid Id
            {
                get { return _id; }
            }

            protected override int Version
            {
                get { return _version; }
                set { _version = value; }
            }  

            public void AssertVersionIsEqualTo(int version)
            {
                Assert.AreEqual(version, _version);
            }

            public void AssertXIsEqualTo(int x)
            {
                Assert.AreEqual(x, _x);    
            }

            public void AssertYIsEqualTo(int y)
            {
                Assert.AreEqual(y, _y);
            }

            protected override void RegisterEventHandlers()
            {
                RegisterEventHandler<XIncrementedEvent>(Handle);
                RegisterEventHandler<YIncrementedEvent>(Handle);
            }

            private void Handle(XIncrementedEvent @event)
            {
                _x += @event.X;
            }

            private void Handle(YIncrementedEvent @event)
            {
                _y += @event.Y;
            }
        }

        private sealed class XIncrementedEvent : DomainEvent<Guid, int>
        {            
            internal XIncrementedEvent(Guid id, int version, int x)
            {
                Id = id;
                Version = version;
                X = x;
            }

            [Key]
            public Guid Id
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

            public int X
            {
                get;
                private set;
            }
        }

        private sealed class YIncrementedEvent : DomainEvent<Guid, int>
        {           
            internal YIncrementedEvent(Guid id, int version, int y)
            {
                Id = id;
                Version = version;
                Y = y;
            }

            [Key]
            public Guid Id
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

            public int Y
            {
                get;
                private set;
            }
        }

        private sealed class AggregateStubB : EventStream<Guid, int>
        {
            private readonly Guid _id;            

            internal AggregateStubB()
            {
                _id = Guid.NewGuid();                
            }

            public override Guid Id
            {
                get { return _id; }
            }

            protected override int Version
            {
                get;
                set;
            }

            protected override void RegisterEventHandlers() { }
        }

        #endregion

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Throws_IfEventsIsNull()
        {
            new EventStreamHistory<Guid, int, AggregateStubA>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RestoreAggregate_Throws_IfUseDefaultConstructorIsTrue_And_TypeDoesNotDeclareADefaultConstructor()
        {
            var factory = new EventStreamHistory<Guid, int, AggregateStubA>(_NoEvents);

            factory.RestoreAggregate();
        }        

        [TestMethod]
        public void RestoreAggregate_ReturnsAggregateInExpectedState_IfUseDefaultConstructorIsTrue_And_TypeHasDefaultConstructor()
        {
            var factory = new EventStreamHistory<Guid, int, AggregateStubB>(_NoEvents);
            var aggregate = factory.RestoreAggregate();

            Assert.IsNotNull(aggregate);
            Assert.AreNotEqual(Guid.Empty, aggregate.Id);
        }

        [TestMethod]
        public void RestoreAggregate_ReturnsAggregateInExpectedState_IfUseDefaultConstructorIsFalse()
        {
            var factory = new EventStreamHistory<Guid, int, AggregateStubA>(_NoEvents, false);
            var aggregate = factory.RestoreAggregate();

            Assert.IsNotNull(aggregate);
            Assert.AreEqual(Guid.Empty, aggregate.Id);
        }

        [TestMethod]
        public void RestoreAggregate_UsesSnapshotToRestoreAggregate_IfSnapshotIsSpecified()
        {
            var snapshot = new AggregateStubA(Guid.NewGuid());
            var factory = new EventStreamHistory<Guid, int, AggregateStubA>(_NoEvents, snapshot);
            var aggregate = factory.RestoreAggregate();

            Assert.IsNotNull(aggregate);
            Assert.AreEqual(snapshot.Id, aggregate.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RestoreAggregate_Throws_IfOneOrMoreEventsHaveNonMatchingKeys()
        {
            var events = new IDomainEvent<Guid, int>[] { new XIncrementedEvent(Guid.NewGuid(), 1, 1)  };
            var snapshot = new AggregateStubA(Guid.NewGuid());
            var factory = new EventStreamHistory<Guid, int, AggregateStubA>(events, snapshot);

            factory.RestoreAggregate();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RestoreAggregate_Throws_IfOneOrMoreEventsVersionIsEqualToSnapshotVersion()
        {
            var id = Guid.NewGuid();
            var events = new IDomainEvent<Guid, int>[] { new XIncrementedEvent(Guid.NewGuid(), 1, 1) };
            var snapshot = new AggregateStubA(id, 1);
            var factory = new EventStreamHistory<Guid, int, AggregateStubA>(events, snapshot);

            factory.RestoreAggregate();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RestoreAggregate_Throws_IfTwoOrMoreEventsHaveEqualVersion()
        {
            var id = Guid.NewGuid();
            var events = new IDomainEvent<Guid, int>[]
            {
                new XIncrementedEvent(id, 1, 1),
                new YIncrementedEvent(id, 1, 2)
            };
            var snapshot = new AggregateStubA(id);
            var factory = new EventStreamHistory<Guid, int, AggregateStubA>(events, snapshot);

            factory.RestoreAggregate();
        }

        [TestMethod]
        public void RestoreAggregate_ReturnsAggregateInExpectedState_IfEventsAreAllAppliedSuccesfully()
        {
            var id = Guid.NewGuid();
            var events = new IDomainEvent<Guid, int>[]
            {
                new XIncrementedEvent(id, 1, 1),
                new YIncrementedEvent(id, 2, 2),
                new YIncrementedEvent(id, 3, -3),
                new XIncrementedEvent(id, 4, 6),
                new XIncrementedEvent(id, 5, 10)
            };
            var snapshot = new AggregateStubA(id);
            var factory = new EventStreamHistory<Guid, int, AggregateStubA>(events, snapshot);
            var aggregate = factory.RestoreAggregate();

            Assert.IsNotNull(aggregate);

            aggregate.AssertVersionIsEqualTo(5);
            aggregate.AssertXIsEqualTo(17);
            aggregate.AssertYIsEqualTo(-1);
        }

        private static readonly IEnumerable<IDomainEvent<Guid, int>> _NoEvents = Enumerable.Empty<IDomainEvent<Guid, int>>();        
    }
}
