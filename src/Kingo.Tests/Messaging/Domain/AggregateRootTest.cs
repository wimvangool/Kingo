using System;
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
            new AggregateRootWithoutEventHandlers(null, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Throws_IfSnapshotIsNull()
        {
            new AggregateRootWithoutEventHandlers(null);
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
            ValueChangedEvent @event = null;

            aggregateRoot.EventPublished += (s, e) => @event = e.Event as ValueChangedEvent;
            aggregate.ChangeValue(newValue);

            Assert.IsNotNull(@event);
            Assert.AreEqual(aggregateRoot.Id, @event.Id);
            Assert.AreEqual(2, @event.Version);
            Assert.AreEqual(newValue, @event.NewValue);

            var events = aggregateRoot.Commit();
            Assert.AreEqual(2, events.Count);
            Assert.AreSame(@event, events[1]);
        }

        [TestMethod]
        public void ChangeValue_PublishesExpectedEvent_IfNextVersionIsImplementedCorrectly_And_AggregateHasEventHandlers()
        {
            var aggregate = new AggregateRootWithEventHandlers();
            var newValue = RandomValue();
            IAggregateRoot<Guid> aggregateRoot = aggregate;
            ValueChangedEvent @event = null;

            aggregateRoot.EventPublished += (s, e) => @event = e.Event as ValueChangedEvent;
            aggregate.ChangeValue(newValue);

            Assert.IsNotNull(@event);
            Assert.AreEqual(aggregateRoot.Id, @event.Id);
            Assert.AreEqual(2, @event.Version);
            Assert.AreEqual(newValue, @event.NewValue);

            var events = aggregateRoot.Commit();
            Assert.AreEqual(2, events.Count);
            Assert.AreSame(@event, events[1]);
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

            var restoredAggregate = aggregateRootSnapshot.RestoreAggregate<AggregateRootWithoutEventHandlers>();
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

            var restoredAggregate = aggregateRootSnapshot.RestoreAggregate<AggregateRootWithEventHandlers>();
            IAggregateRoot<Guid> restoredAggregateRoot = restoredAggregate;

            Assert.IsNotNull(restoredAggregate);
            Assert.AreNotSame(aggregate, restoredAggregate);
            Assert.AreEqual(snapshot.Id, restoredAggregateRoot.Id);

            restoredAggregate.AssertValueIs(newValue);
        }

        [TestMethod]        
        public void LoadFromHistory_IgnoresEvent_IfEventIdDoesNotMatchAggregateId_And_AggregateHasNoEventHandlers()
        {
            IAggregateRoot<Guid> aggregateRoot = new AggregateRootWithoutEventHandlers();
            var aggregateRootSnapshot = aggregateRoot.TakeSnapshot();
            var aggregateDataSet = new AggregateDataSet<Guid>(aggregateRoot.Id, aggregateRootSnapshot, new IEvent[]
            {
                new ValueChangedEvent
                {
                    Version = 1
                }
            });

            Assert.IsNotNull(aggregateDataSet.RestoreAggregate<AggregateRootWithoutEventHandlers>());            
        }

        [TestMethod]        
        public void LoadFromHistory_IgnoresEvent_IfEventIdDoesNotMatchAggregateId_And_AggregateHasEventHandlers()
        {
            IAggregateRoot<Guid> aggregateRoot = new AggregateRootWithEventHandlers();
            var aggregateRootSnapshot = aggregateRoot.TakeSnapshot();
            var aggregateDataSet = new AggregateDataSet<Guid>(aggregateRoot.Id, aggregateRootSnapshot, new IEvent[]
            {
                new ValueChangedEvent
                {
                    Version = 1
                }
            });

            Assert.IsNotNull(aggregateDataSet.RestoreAggregate<AggregateRootWithEventHandlers>());
        }

        [TestMethod]        
        public void LoadFromHistory_IgnoresEvent_IfEventVersionIsNotHigherThanVersion_And_AggregateHasNoEventHandlers()
        {
            IAggregateRoot<Guid> aggregateRoot = new AggregateRootWithEventHandlers();
            var aggregateRootSnapshot = aggregateRoot.TakeSnapshot();
            var aggregateDataSet = new AggregateDataSet<Guid>(aggregateRoot.Id, aggregateRootSnapshot, new IEvent[]
            {
                new ValueChangedEvent
                {
                    Id = aggregateRoot.Id,
                    Version = 1
                }
            });

            Assert.IsNotNull(aggregateDataSet.RestoreAggregate<AggregateRootWithEventHandlers>());            
        }

        [TestMethod]        
        public void LoadFromHistory_IgnoresEvent_IfEventVersionIsNotHigherThanVersion_And_AggregateHasEventHandlers()
        {
            IAggregateRoot<Guid> aggregateRoot = new AggregateRootWithEventHandlers();
            var aggregateRootSnapshot = aggregateRoot.TakeSnapshot();
            var aggregateDataSet = new AggregateDataSet<Guid>(aggregateRoot.Id, aggregateRootSnapshot, new IEvent[]
            {
                new ValueChangedEvent
                {
                    Id = aggregateRoot.Id,
                    Version = 1
                }
            });

            Assert.IsNotNull(aggregateDataSet.RestoreAggregate<AggregateRootWithEventHandlers>());                      
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LoadFromHistory_Throws_IfAggregateHasNoEventHandlerForSpecifiedEvent()
        {
            IAggregateRoot<Guid> aggregateRoot = new AggregateRootWithoutEventHandlers();
            var aggregateRootSnapshot = aggregateRoot.TakeSnapshot();
            var aggregateDataSet = new AggregateDataSet<Guid>(aggregateRoot.Id, aggregateRootSnapshot, new IEvent[]
            {
                new ValueChangedEvent
                {
                    Id = aggregateRoot.Id,
                    Version = 2
                }
            });

            try
            {
                aggregateDataSet.RestoreAggregate<AggregateRootWithoutEventHandlers>();
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
            var aggregateDataSet = new AggregateDataSet<Guid>(aggregateRoot.Id, aggregateRootSnapshot, new IEvent[]
            {
                new ValueChangedEvent
                {
                    Id = aggregateRoot.Id,
                    Version = 1
                },
                new UnsupportedEvent()
            });

            try
            {
                aggregateDataSet.RestoreAggregate<AggregateRootWithEventHandlers>();
            }
            catch (ArgumentException exception)
            {
                Assert.AreEqual("Could not convert event of type 'UnsupportedEvent' to an instance of type 'IEvent<Guid, Int32>'. Please review the UpdateToLatestVersion() method of this event to ensure it returns the correct event type.", exception.Message);
                throw;
            }
        }

        [TestMethod]
        public void LoadFromHistory_AutomaticallyUpdatesAndOrdersAndAppliesAllEvents_IfAggregateHasEventHandlersForEveryEvent()
        {
            IAggregateRoot<Guid> aggregateRoot = new AggregateRootWithEventHandlers();
            var aggregateRootSnapshot = aggregateRoot.TakeSnapshot();
            var aggregateDataSet = new AggregateDataSet<Guid>(aggregateRoot.Id, aggregateRootSnapshot, new IEvent[]
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

            var restoredAggregate = aggregateDataSet.RestoreAggregate<AggregateRootWithEventHandlers>();

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
            var aggregateDataSet = new AggregateDataSet<Guid>(aggregateRoot.Id, aggregateRootSnapshot, new IEvent[]
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

            IAggregateRoot restoredAggregate = aggregateDataSet.RestoreAggregate<AggregateRootWithEventHandlers>();
            
            Assert.AreEqual(0, restoredAggregate.Commit().Count);            
        }

        [TestMethod]
        public void FlushEvents_ClearsTheInternalEventBuffer_IfAggregateHasNoEventHandlers()
        {
            var aggregate = new AggregateRootWithoutEventHandlers();
            var newValue = RandomValue();
            IAggregateRoot<Guid> aggregateRoot = aggregate;            
            
            aggregate.ChangeValue(newValue);            
            
            Assert.AreEqual(2, aggregateRoot.Commit().Count);            
            Assert.AreEqual(0, aggregateRoot.Commit().Count);
        }

        #endregion

        private static int RandomValue() =>
            Clock.Current.UtcDateAndTime().Millisecond + 1;
    }
}
