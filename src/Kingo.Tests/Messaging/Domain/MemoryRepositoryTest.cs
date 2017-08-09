using System;
using System.Threading.Tasks;
using Kingo.Clocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Domain
{
    [TestClass]
    public sealed class MemoryRepositoryTest
    {
        #region [====== EventSourcedAggregate ======]

        private sealed class EventSourcedAggregate : AggregateRoot<Guid, int>
        {            
            private int _value;

            public EventSourcedAggregate(AggregateEventSourcedAggregateCreatedAggregateEvent aggregateEvent)
                : base(aggregateEvent) { }

            public EventSourcedAggregate(EventSourcedAggregateSnapshot snapshot)
                : base(snapshot)
            {
                _value = snapshot.Value;
            }

            protected override int NextVersion() =>
                Version + 1;

            protected override ISnapshot<Guid, int> TakeSnapshot() =>
                new EventSourcedAggregateSnapshot(Id, Version, _value);

            protected override bool ApplyEventsToSelf =>
                true;

            public void ChangeValue(int newValue)
            {
                if (_value == newValue)
                {
                    return;
                }
                Publish(new AggregateEventSourcedAggregateValueChangedAggregateEvent(Id, Version, newValue));
            }

            public void AssertValueIs(int value)
            {
                Assert.AreEqual(value, _value);
            }

            #region [====== EventHandlers ======]

            private int _handleCount;

            protected override EventHandlerCollection RegisterEventHandlers(EventHandlerCollection eventHandlers)
            {
                return eventHandlers
                    .Register<AggregateEventSourcedAggregateValueChangedAggregateEvent>(Handle);
            }

            private void Handle(AggregateEventSourcedAggregateValueChangedAggregateEvent aggregateEvent)
            {
                _handleCount++;
                _value = aggregateEvent.NewValue;
            }

            public void AssertHandleCountIs(int count)
            {
                Assert.AreEqual(count, _handleCount);
            }

            #endregion
        }

        private sealed class AggregateEventSourcedAggregateCreatedAggregateEvent : AggregateEvent<Guid, int>
        {
            public AggregateEventSourcedAggregateCreatedAggregateEvent()
            {
                Id = Guid.NewGuid();
                Version = 1;
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

        private sealed class EventSourcedAggregateSnapshot : Snapshot<Guid, int>
        {            
            public EventSourcedAggregateSnapshot(Guid id, int version, int value)
            {
                Id = id;
                Version = version;
                Value = value;
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

            public int Value
            {
                get;
                private set;
            }

            protected override IAggregateRoot RestoreAggregate() =>
                new EventSourcedAggregate(this);
        }

        private sealed class AggregateEventSourcedAggregateValueChangedAggregateEvent : AggregateEvent<Guid, int>
        {
            public AggregateEventSourcedAggregateValueChangedAggregateEvent(Guid id, int version, int newValue)
            {
                Id = id;
                Version = version;
                NewValue = newValue;
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

            public int NewValue
            {
                get;
                private set;
            }
        }

        #endregion

        private MicroProcessor _processor;
        private MemoryRepository<Guid, EventSourcedAggregate> _repository;

        [TestInitialize]
        public void Setup()
        {
            _processor = new MicroProcessor();
            _repository = new MemoryRepository<Guid, EventSourcedAggregate>(MemoryRepositoryBehavior.StoreEvents);
        }        
            
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_Throws_IfBehaviorIsNotValid()
        {
            try
            {
                new MemoryRepository<Guid, AggregateRootSpy>((MemoryRepositoryBehavior) (-1));
            }
            catch (ArgumentOutOfRangeException exception)
            {
                Assert.IsTrue(exception.Message.StartsWith("Invalid behavior specified: '-1'. Please choose one of the following values: <StoreSnapshots, StoreEvents>."));
                throw;
            }
        }

        [TestMethod]
        public async Task GetByIdAsync_RestoresAggregateBySnapshot_IfBehaviorIsStoreEvents_And_AggregateWasInsertedButNotUpdated()
        {
            var aggregateCreatedEvent = new AggregateEventSourcedAggregateCreatedAggregateEvent();
            var aggregate = new EventSourcedAggregate(aggregateCreatedEvent);
            var newValue = Clock.Current.UtcDateAndTime().Millisecond + 1;

            aggregate.ChangeValue(newValue);
            aggregate.AssertHandleCountIs(1);

            await _processor.HandleAsync(new object(), async (message, context) =>
            {
                Assert.IsTrue(await _repository.AddAsync(aggregate));
            });

            Assert.AreEqual(1, _repository.Count);

            await _processor.HandleAsync(new object(), async (message, context) =>
            {
                var restoredAggregate = await _repository.GetByIdAsync(aggregateCreatedEvent.Id);

                Assert.AreNotSame(aggregate, restoredAggregate);

                restoredAggregate.AssertValueIs(newValue);
                restoredAggregate.AssertHandleCountIs(0);                
            });

            Assert.AreEqual(1, _repository.Count);
        }

        [TestMethod]
        public async Task GetByIdAsync_RestoresAggregateBySnapshotAndEvents_IfBehaviorIsStoreEvents_And_AggregateWasInsertedAndUpdatedSeveralTimes()
        {
            var aggregateCreatedEvent = new AggregateEventSourcedAggregateCreatedAggregateEvent();
            var aggregate = new EventSourcedAggregate(aggregateCreatedEvent);
            var newValue1 = Clock.Current.UtcDateAndTime().Millisecond + 1;
            var newValue2 = newValue1 * 2;
            var newValue3 = newValue2 * 2;

            aggregate.ChangeValue(newValue1);
            aggregate.AssertHandleCountIs(1);

            await _processor.HandleAsync(new object(), async (message, context) =>
            {
                Assert.IsTrue(await _repository.AddAsync(aggregate));
            });

            Assert.AreEqual(1, _repository.Count);

            await _processor.HandleAsync(new object(), async (message, context) =>
            {
                var restoredAggregate = await _repository.GetByIdAsync(aggregateCreatedEvent.Id);

                restoredAggregate.ChangeValue(newValue2);
                restoredAggregate.ChangeValue(newValue3);
            });

            Assert.AreEqual(1, _repository.Count);

            await _processor.HandleAsync(new object(), async (message, context) =>
            {
                var restoredAggregate = await _repository.GetByIdAsync(aggregateCreatedEvent.Id);

                Assert.AreNotSame(aggregate, restoredAggregate);

                restoredAggregate.AssertValueIs(newValue3);
                restoredAggregate.AssertHandleCountIs(2);
            });
        }
    }
}
