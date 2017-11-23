using System;
using System.Linq;
using Kingo.Clocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Domain
{
    [TestClass]
    public sealed class AggregateRootTest
    {
        #region [====== Constructor ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Throws_IfEventIsNull()
        {
            new AggregateRootWithoutEventHandlers(null as AggregateRootSpyCreatedAggregateEvent);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Throws_IfSnapshotIsNull()
        {
            new AggregateRootWithoutEventHandlers(null as SnapshotMock);
        }

        #endregion       

        #region [====== ChangeValue(int) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ChangeValue_Throws_IfNextVersionIsImplementedIncorrectlyAnd_AggregateHasNoEventHandlers()
        {
            var aggregate = new AggregateRootWithoutEventHandlers(true);

            aggregate.ChangeValue(RandomValue());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ChangeValue_Throws_IfNextVersionIsImplementedIncorrectlyAnd_AggregateHasEventHandlers()
        {
            var aggregate = new AggregateRootWithEventHandlers(true);

            aggregate.ChangeValue(RandomValue());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ChangeValue_Throws_IfSameEventHandlerIsRegisteredMoreThanOnce()
        {
            var aggregate = new AggregateRootWithEventHandlers(false, true);

            try
            {
                aggregate.ChangeValue(RandomValue());
            }
            catch (ArgumentException exception)
            {
                Assert.AreEqual("Another handler for event of type 'ValueChangedEvent' has already been added to this collection.", exception.Message);
                throw;
            }
        }

        [TestMethod]
        public void ChangeValue_PublishesExpectedEvent_IfNextVersionIsImplementedCorrectly_And_AggregateHasNoEventHandlers()
        {
            var aggregate = new AggregateRootWithoutEventHandlers();
            var newValue = RandomValue();
            IAggregateRoot<Guid> aggregateRoot = aggregate;
            EventPublishedEventArgs<ValueChangedEvent> eventArgs = null;

            aggregateRoot.EventPublished += (s, e) => eventArgs = e as EventPublishedEventArgs<ValueChangedEvent>;
            aggregate.ChangeValue(newValue);

            Assert.IsNotNull(eventArgs);
            Assert.AreEqual(aggregateRoot.Id, eventArgs.Event.Id);
            Assert.AreEqual(2, eventArgs.Event.Version);
            Assert.AreEqual(newValue, eventArgs.Event.NewValue);

            Assert.IsTrue(aggregateRoot.HasPendingEvents);
            Assert.AreSame(eventArgs.Event, aggregateRoot.FlushEvents().ElementAt(1));
        }

        [TestMethod]
        public void ChangeValue_PublishesExpectedEvent_IfNextVersionIsImplementedCorrectly_And_AggregateHasEventHandlers()
        {
            var aggregate = new AggregateRootWithEventHandlers();
            var newValue = RandomValue();
            IAggregateRoot<Guid> aggregateRoot = aggregate;
            EventPublishedEventArgs<ValueChangedEvent> eventArgs = null;

            aggregateRoot.EventPublished += (s, e) => eventArgs = e as EventPublishedEventArgs<ValueChangedEvent>;
            aggregate.ChangeValue(newValue);

            Assert.IsNotNull(eventArgs);
            Assert.AreEqual(aggregateRoot.Id, eventArgs.Event.Id);
            Assert.AreEqual(2, eventArgs.Event.Version);
            Assert.AreEqual(newValue, eventArgs.Event.NewValue);

            Assert.IsTrue(aggregateRoot.HasPendingEvents);
            Assert.AreSame(eventArgs.Event, aggregateRoot.FlushEvents().ElementAt(1));
        }

        [TestMethod]
        public void ChangeValue_DoesNotAddEventToInternalEventBuffer_IfEventIsHandledByEventPublishedHandler()
        {
            var aggregate = new AggregateRootWithEventHandlers(new SnapshotMock(true));
            var newValue = RandomValue();
            IAggregateRoot<Guid> aggregateRoot = aggregate;
            EventPublishedEventArgs<ValueChangedEvent> eventArgs = null;

            aggregateRoot.EventPublished += (s, e) =>
            {
                eventArgs = e as EventPublishedEventArgs<ValueChangedEvent>;
                eventArgs.WriteEventTo(new EventStreamImplementation());

                Assert.IsTrue(eventArgs.HasBeenPublished);
            };
            aggregate.ChangeValue(newValue);

            Assert.IsNotNull(eventArgs);
            Assert.AreEqual(aggregateRoot.Id, eventArgs.Event.Id);
            Assert.AreEqual(1, eventArgs.Event.Version);
            Assert.AreEqual(newValue, eventArgs.Event.NewValue);

            Assert.IsFalse(aggregateRoot.HasPendingEvents);
            Assert.AreEqual(0, aggregateRoot.FlushEvents().Count());
        }

        #endregion

        #region [====== ToString() ======]

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfAggregateHasNoPendingEvents()
        {
            IAggregateRoot<Guid> aggregateRoot = new AggregateRootWithoutEventHandlers(new SnapshotMock(false));            

            Assert.AreEqual($"AggregateRootWithoutEventHandlers [Id = {aggregateRoot.Id}, Version = 0, Events = 0]", aggregateRoot.ToString());
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfAggregateHasPendingEvents()
        {
            var aggregate = new AggregateRootWithoutEventHandlers();
            IAggregateRoot<Guid> aggregateRoot = aggregate;           

            Assert.AreEqual($"AggregateRootWithoutEventHandlers [Id = {aggregateRoot.Id}, Version = 1, Events = 1]", aggregateRoot.ToString());
        }

        #endregion

        #region [====== TakeSnapshot(), RestoreAggregate() & LoadFromHistory() ======]

        [TestMethod]
        public void TakeSnapshot_ReturnsSnapshotThatCanRestoreAggregate_IfAggregateHasNoEventHandlers()
        {
            var aggregate = new AggregateRootWithoutEventHandlers();
            var newValue = RandomValue();

            IAggregateRoot<Guid> aggregateRoot = aggregate;
            aggregate.ChangeValue(newValue);

            var aggregateRootSnapshot = aggregateRoot.TakeSnapshot();
            var snapshot = aggregateRootSnapshot as SnapshotMock;

            Assert.IsNotNull(snapshot);
            Assert.AreEqual(aggregateRoot.Id, snapshot.Id);
            Assert.AreEqual(2, snapshot.Version);
            Assert.AreEqual(newValue, snapshot.Value);
            
            var restoredAggregate = aggregateRootSnapshot.RestoreAggregate() as AggregateRootWithoutEventHandlers;
            IAggregateRoot<Guid> restoredAggregateRoot = restoredAggregate;

            Assert.IsNotNull(restoredAggregate);
            Assert.AreNotSame(aggregate, restoredAggregate);
            Assert.AreEqual(snapshot.Id, restoredAggregateRoot.Id);

            restoredAggregate.AssertValueIs(newValue);
        }

        [TestMethod]
        public void TakeSnapshot_ReturnsSnapshotThatCanRestoreAggregate_IfAggregateHasEventHandlers()
        {
            var aggregate = new AggregateRootWithEventHandlers();
            var newValue = RandomValue();

            IAggregateRoot<Guid> aggregateRoot = aggregate;
            aggregate.ChangeValue(newValue);

            var aggregateRootSnapshot = aggregateRoot.TakeSnapshot();
            var snapshot = aggregateRootSnapshot as SnapshotMock;

            Assert.IsNotNull(snapshot);
            Assert.AreEqual(aggregateRoot.Id, snapshot.Id);
            Assert.AreEqual(2, snapshot.Version);
            Assert.AreEqual(newValue, snapshot.Value);

            var restoredAggregate = aggregateRootSnapshot.RestoreAggregate() as AggregateRootWithEventHandlers;
            IAggregateRoot<Guid> restoredAggregateRoot = restoredAggregate;

            Assert.IsNotNull(restoredAggregate);
            Assert.AreNotSame(aggregate, restoredAggregate);
            Assert.AreEqual(snapshot.Id, restoredAggregateRoot.Id);

            restoredAggregate.AssertValueIs(newValue);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LoadFromHistory_Throws_IfEventIdDoesNotMatchAggregateId_And_AggregateHasNoEventHandlers()
        {
            IAggregateRoot<Guid> aggregateRoot = new AggregateRootWithoutEventHandlers();
            var aggregateRootSnapshot = aggregateRoot.TakeSnapshot();

            try
            {
                aggregateRootSnapshot.RestoreAggregate(new IEvent[]
                {
                    new ValueChangedEvent
                    {
                        Version = 1
                    }
                });
            }
            catch (ArgumentException exception)
            {
                Assert.AreEqual($"Id '{Guid.Empty}' on event 'ValueChangedEvent' does not match the identifier of the aggregate it is being applied to ({aggregateRoot.Id}).", exception.Message);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LoadFromHistory_Throws_IfEventIdDoesNotMatchAggregateId_And_AggregateHasEventHandlers()
        {
            IAggregateRoot<Guid> aggregateRoot = new AggregateRootWithEventHandlers();
            var aggregateRootSnapshot = aggregateRoot.TakeSnapshot();

            try
            {
                aggregateRootSnapshot.RestoreAggregate(new IEvent[]
                {
                    new ValueChangedEvent
                    {
                        Version = 1
                    }
                });
            }
            catch (ArgumentException exception)
            {
                Assert.AreEqual($"Id '{Guid.Empty}' on event 'ValueChangedEvent' does not match the identifier of the aggregate it is being applied to ({aggregateRoot.Id}).", exception.Message);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void LoadFromHistory_Throws_IfEventVersionIsNotHigherThanVersion_And_AggregateHasNoEventHandlers()
        {
            IAggregateRoot<Guid> aggregateRoot = new AggregateRootWithEventHandlers();
            var aggregateRootSnapshot = aggregateRoot.TakeSnapshot();

            try
            {
                aggregateRootSnapshot.RestoreAggregate(new IEvent[]
                {
                    new ValueChangedEvent
                    {
                        Id = aggregateRoot.Id,
                        Version = 1
                    }
                });
            }
            catch (ArgumentOutOfRangeException exception)
            {
                Assert.IsTrue(exception.Message.StartsWith("The next version (1) must represent a newer version than the current version (1)."));                
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void LoadFromHistory_Throws_IfEventVersionIsNotHigherThanVersion_And_AggregateHasEventHandlers()
        {
            IAggregateRoot<Guid> aggregateRoot = new AggregateRootWithEventHandlers();
            var aggregateRootSnapshot = aggregateRoot.TakeSnapshot();

            try
            {
                aggregateRootSnapshot.RestoreAggregate(new IEvent[]
                {
                    new ValueChangedEvent
                    {
                        Id = aggregateRoot.Id,
                        Version = 1
                    }
                });
            }
            catch (ArgumentOutOfRangeException exception)
            {
                Assert.IsTrue(exception.Message.StartsWith("The next version (1) must represent a newer version than the current version (1)."));
                throw;
            }            
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LoadFromHistory_Throws_IfAggregateHasNoEventHandlerForSpecifiedEvent()
        {
            IAggregateRoot<Guid> aggregateRoot = new AggregateRootWithoutEventHandlers();
            var aggregateRootSnapshot = aggregateRoot.TakeSnapshot();

            try
            {
                aggregateRootSnapshot.RestoreAggregate(new IEvent[]
                {
                    new ValueChangedEvent
                    {
                        Id = aggregateRoot.Id,
                        Version = 2
                    }
                });
            }
            catch (ArgumentException exception)
            {
                Assert.AreEqual("Missing event handler for event of type 'ValueChangedEvent'.", exception.Message);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LoadFromHistory_Throws_IfSomeEventCouldNotBeCastToRequiredType()
        {
            IAggregateRoot<Guid> aggregateRoot = new AggregateRootWithEventHandlers();
            var aggregateRootSnapshot = aggregateRoot.TakeSnapshot();

            try
            {
                aggregateRootSnapshot.RestoreAggregate(new IEvent[]
                {
                    new ValueChangedEvent
                    {
                        Id = aggregateRoot.Id,
                        Version = 1
                    },
                    new UnsupportedEvent() 
                });
            }
            catch (ArgumentException exception)
            {
                Assert.AreEqual("Could not convert event of type 'UnsupportedEvent' to an instance of type 'IAggregateEvent<Guid, Int32>'. Please review the UpdateToLatestVersion() method of this event to ensure it returns the correct event type.", exception.Message);
                throw;
            }
        }

        [TestMethod]
        public void LoadFromHistory_AutomaticallyUpdatesAndOrdersAndAppliesAllEvents_IfAggregateHasEventHandlersForEveryEvent()
        {
            IAggregateRoot<Guid> aggregateRoot = new AggregateRootWithEventHandlers();
            var aggregateRootSnapshot = aggregateRoot.TakeSnapshot();

            var restoredAggregate = aggregateRootSnapshot.RestoreAggregate(new IEvent[]
            {
                new OldValueChangedEvent(aggregateRoot.Id, 3, 3),
                new ValueChangedEvent
                {
                    Id = aggregateRoot.Id,
                    Version = 4,
                    NewValue = 4
                },
                new OldValueChangedEvent(aggregateRoot.Id, 2, 2)
            }) as AggregateRootWithEventHandlers;

            IAggregateRoot<Guid> restoredAggregateRoot = restoredAggregate;

            Assert.IsNotNull(restoredAggregate);
            Assert.AreEqual(aggregateRoot.Id, restoredAggregateRoot.Id);

            restoredAggregate.AssertVersionIs(4);
            restoredAggregate.AssertValueIs(4);
        }

        #endregion

        #region [====== HasPendingEvents & FlushEvents() ======]

        [TestMethod]
        public void HasPendingEvents_IsFalse_IfAggregateHasJustBeenRestoredFromHistory()
        {
            IAggregateRoot<Guid> aggregateRoot = new AggregateRootWithEventHandlers();
            var aggregateRootSnapshot = aggregateRoot.TakeSnapshot();

            var restoredAggregate = aggregateRootSnapshot.RestoreAggregate(new IEvent[]
            {
                new OldValueChangedEvent(aggregateRoot.Id, 3, 3),
                new ValueChangedEvent
                {
                    Id = aggregateRoot.Id,
                    Version = 4,
                    NewValue = 4
                },
                new OldValueChangedEvent(aggregateRoot.Id, 2, 2)
            });

            Assert.IsFalse(restoredAggregate.HasPendingEvents);
            Assert.AreEqual(0, restoredAggregate.FlushEvents().Count());
        }

        [TestMethod]
        public void FlushEvents_ClearsTheInternalEventBuffer_IfAggregateHasNoEventHandlers()
        {
            var aggregate = new AggregateRootWithoutEventHandlers();
            var newValue = RandomValue();
            IAggregateRoot<Guid> aggregateRoot = aggregate;            
            
            aggregate.ChangeValue(newValue);            

            Assert.IsTrue(aggregateRoot.HasPendingEvents);
            Assert.AreEqual(2, aggregateRoot.FlushEvents().Count());
            Assert.IsFalse(aggregateRoot.HasPendingEvents);
            Assert.AreEqual(0, aggregateRoot.FlushEvents().Count());
        }

        #endregion

        private static int RandomValue() =>
            Clock.Current.UtcDateAndTime().Millisecond + 1;
    }
}
