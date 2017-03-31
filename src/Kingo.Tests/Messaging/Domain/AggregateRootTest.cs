using System;
using System.Linq;
using Kingo.Clocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Domain
{
    [TestClass]
    public sealed class AggregateRootTest
    {
        #region [====== Aggregates, Snapshots & Events ======]

        private sealed class Snapshot : Snapshot<Guid, int>
        {
            private readonly bool _aggregateHasEventHandlers;

            public Snapshot(bool aggregateHasEventHandlers)
            {
                _aggregateHasEventHandlers = aggregateHasEventHandlers;
            }

            public override Guid Id
            {
                get;
                set;
            }

            public override int Version
            {
                get;
                set;
            }

            public int Value
            {
                get;
                set;
            }

            protected override IAggregateRoot RestoreAggregate()
            {
                if (_aggregateHasEventHandlers)
                {
                    return new AggregateWithEventHandlers(this);
                }
                return new AggregateWithoutEventHandlers(this);
            }                
        }

        private sealed class OldValueChangedEvent : Event
        {
            private readonly Guid _id;
            private readonly int _version;
            private readonly int _value;

            public OldValueChangedEvent(Guid id, int version, int value)
            {
                _id = id;
                _version = version;
                _value = value;
            }

            protected override IEvent UpdateToLatestVersion()
            {
                return new ValueChangedEvent()
                {
                    Id = _id,
                    Version = _version,
                    NewValue = _value
                };
            }
        }

        private sealed class ValueChangedEvent : Event<Guid, int>
        {
            public override Guid Id
            {
                get;
                set;
            }

            public override int Version
            {
                get;
                set;
            }

            public int NewValue
            {
                get;
                set;
            }
        }
        
        private sealed class UnsupportedEvent : Event { }  
        
        private abstract class AggregateSpy : AggregateRoot<Guid, int>
        {
            private readonly bool _subtractOneForNextVersion;

            protected AggregateSpy(bool subtractOneForNextVersion) :
                base(Guid.NewGuid())
            {
                _subtractOneForNextVersion = subtractOneForNextVersion;
            }

            protected AggregateSpy(Snapshot snapshot) :
                base(snapshot)
            {
                Value = snapshot.Value;
            }

            protected override int NextVersion()
            {
                if (_subtractOneForNextVersion)
                {
                    return Version - 1;
                }
                return Version + 1;
            }

            protected int Value
            {
                get;
                set;
            }            

            public abstract void ChangeValue(int newValue);

            public void AssertVersionIs(int version)
            {
                Assert.AreEqual(version, Version);
            }

            public void AssertValueIs(int value)
            {
                Assert.AreEqual(value, Value);
            }
        }

        private sealed class AggregateWithoutEventHandlers : AggregateSpy
        {                        
            public AggregateWithoutEventHandlers(bool subtractOneForNextVersion = false)
                : base(subtractOneForNextVersion) { }

            public AggregateWithoutEventHandlers(Snapshot snapshot)
                : base(snapshot) { }

            protected override ISnapshot<Guid, int> TakeSnapshot()
            {
                return new Snapshot(false)
                {
                    Id = Id,
                    Version = Version,
                    Value = Value
                };
            }

            public override void ChangeValue(int newValue)
            {
                if (Value == newValue)
                {
                    return;
                }
                Value = newValue;

                Publish(new ValueChangedEvent()
                {
                    NewValue = newValue
                });
            }                          
        }

        private sealed class AggregateWithEventHandlers : AggregateSpy
        {
            private readonly bool _registerEventHandlerTwice;

            public AggregateWithEventHandlers(bool subtractOneForNextVersion = false, bool registerEventHandlerTwice = false)
                : base(subtractOneForNextVersion)
            {
                _registerEventHandlerTwice = registerEventHandlerTwice;
            }

            public AggregateWithEventHandlers(Snapshot snapshot)
                : base(snapshot) { }

            protected override ISnapshot<Guid, int> TakeSnapshot()
            {
                return new Snapshot(true)
                {
                    Id = Id,
                    Version = Version,
                    Value = Value
                };
            }

            protected override EventHandlerCollection RegisterEventHandlers(EventHandlerCollection eventHandlers)
            {
                eventHandlers = eventHandlers.Register<ValueChangedEvent>(OnValueChanged);

                if (_registerEventHandlerTwice)
                {
                    return eventHandlers.Register<ValueChangedEvent>(OnValueChanged);
                }
                return eventHandlers;
            }
                

            public override void ChangeValue(int newValue)
            {
                if (Value == newValue)
                {
                    return;
                }                
                Publish(new ValueChangedEvent()
                {
                    NewValue = newValue
                });
            }

            private void OnValueChanged(ValueChangedEvent @event)
            {
                Value = @event.NewValue;
            }
        }

        #endregion

        #region [====== Constructor ======]

        [TestMethod]
        public void Constructor_InitializesIdCorrectly_IfDefaultConstructorIsUsed()
        {
            IAggregateRoot<Guid> aggregate = new AggregateWithoutEventHandlers();

            Assert.AreNotEqual(Guid.Empty, aggregate.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Throws_IfSnapshotIsNull()
        {
            new AggregateWithoutEventHandlers(null);
        }

        #endregion       

        #region [====== ChangeValue(int) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ChangeValue_Throws_IfNextVersionIsImplementedIncorrectlyAnd_AggregateHasNoEventHandlers()
        {
            var aggregate = new AggregateWithoutEventHandlers(true);

            aggregate.ChangeValue(RandomValue());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ChangeValue_Throws_IfNextVersionIsImplementedIncorrectlyAnd_AggregateHasEventHandlers()
        {
            var aggregate = new AggregateWithEventHandlers(true);

            aggregate.ChangeValue(RandomValue());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ChangeValue_Throws_IfSameEventHandlerIsRegisteredMoreThanOnce()
        {
            var aggregate = new AggregateWithEventHandlers(false, true);

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
            var aggregate = new AggregateWithoutEventHandlers();
            var newValue = RandomValue();
            IAggregateRoot<Guid> aggregateRoot = aggregate;
            EventPublishedEventArgs<ValueChangedEvent> eventArgs = null;

            aggregateRoot.EventPublished += (s, e) => eventArgs = e as EventPublishedEventArgs<ValueChangedEvent>;
            aggregate.ChangeValue(newValue);

            Assert.IsNotNull(eventArgs);
            Assert.AreEqual(aggregateRoot.Id, eventArgs.Event.Id);
            Assert.AreEqual(1, eventArgs.Event.Version);
            Assert.AreEqual(newValue, eventArgs.Event.NewValue);

            Assert.IsTrue(aggregateRoot.HasPublishedEvents);
            Assert.AreSame(eventArgs.Event, aggregateRoot.FlushEvents().First());
        }

        [TestMethod]
        public void ChangeValue_PublishesExpectedEvent_IfNextVersionIsImplementedCorrectly_And_AggregateHasEventHandlers()
        {
            var aggregate = new AggregateWithEventHandlers();
            var newValue = RandomValue();
            IAggregateRoot<Guid> aggregateRoot = aggregate;
            EventPublishedEventArgs<ValueChangedEvent> eventArgs = null;

            aggregateRoot.EventPublished += (s, e) => eventArgs = e as EventPublishedEventArgs<ValueChangedEvent>;
            aggregate.ChangeValue(newValue);

            Assert.IsNotNull(eventArgs);
            Assert.AreEqual(aggregateRoot.Id, eventArgs.Event.Id);
            Assert.AreEqual(1, eventArgs.Event.Version);
            Assert.AreEqual(newValue, eventArgs.Event.NewValue);

            Assert.IsTrue(aggregateRoot.HasPublishedEvents);
            Assert.AreSame(eventArgs.Event, aggregateRoot.FlushEvents().First());
        }

        #endregion

        #region [====== ToString() ======]

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfAggregateHasNoPublishedEvents()
        {
            IAggregateRoot<Guid> aggregateRoot = new AggregateWithoutEventHandlers();            

            Assert.AreEqual($"AggregateWithoutEventHandlers [Id = {aggregateRoot.Id}, Version = 0, 0 pending event(s)]", aggregateRoot.ToString());
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfAggregateHasPublishedEvents()
        {
            var aggregate = new AggregateWithoutEventHandlers();
            IAggregateRoot<Guid> aggregateRoot = aggregate;

            aggregate.ChangeValue(RandomValue());

            Assert.AreEqual($"AggregateWithoutEventHandlers [Id = {aggregateRoot.Id}, Version = 1, 1 pending event(s)]", aggregateRoot.ToString());
        }

        #endregion

        #region [====== TakeSnapshot(), RestoreAggregate() & LoadFromHistory() ======]

        [TestMethod]
        public void TakeSnapshot_ReturnsSnapshotThatCanRestoreAggregate_IfAggregateHasNoEventHandlers()
        {
            var aggregate = new AggregateWithoutEventHandlers();
            var newValue = RandomValue();

            IAggregateRoot<Guid> aggregateRoot = aggregate;
            aggregate.ChangeValue(newValue);

            var aggregateRootSnapshot = aggregateRoot.TakeSnapshot();
            var snapshot = aggregateRootSnapshot as Snapshot;

            Assert.IsNotNull(snapshot);
            Assert.AreEqual(aggregateRoot.Id, snapshot.Id);
            Assert.AreEqual(1, snapshot.Version);
            Assert.AreEqual(newValue, snapshot.Value);
            
            var restoredAggregate = aggregateRootSnapshot.RestoreAggregate() as AggregateWithoutEventHandlers;
            IAggregateRoot<Guid> restoredAggregateRoot = restoredAggregate;

            Assert.IsNotNull(restoredAggregate);
            Assert.AreNotSame(aggregate, restoredAggregate);
            Assert.AreEqual(snapshot.Id, restoredAggregateRoot.Id);

            restoredAggregate.AssertValueIs(newValue);
        }

        [TestMethod]
        public void TakeSnapshot_ReturnsSnapshotThatCanRestoreAggregate_IfAggregateHasEventHandlers()
        {
            var aggregate = new AggregateWithEventHandlers();
            var newValue = RandomValue();

            IAggregateRoot<Guid> aggregateRoot = aggregate;
            aggregate.ChangeValue(newValue);

            var aggregateRootSnapshot = aggregateRoot.TakeSnapshot();
            var snapshot = aggregateRootSnapshot as Snapshot;

            Assert.IsNotNull(snapshot);
            Assert.AreEqual(aggregateRoot.Id, snapshot.Id);
            Assert.AreEqual(1, snapshot.Version);
            Assert.AreEqual(newValue, snapshot.Value);

            var restoredAggregate = aggregateRootSnapshot.RestoreAggregate() as AggregateWithEventHandlers;
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
            IAggregateRoot<Guid> aggregateRoot = new AggregateWithoutEventHandlers();
            var aggregateRootSnapshot = aggregateRoot.TakeSnapshot();

            try
            {
                aggregateRootSnapshot.RestoreAggregate(new IEvent[]
                {
                    new ValueChangedEvent()
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
            IAggregateRoot<Guid> aggregateRoot = new AggregateWithEventHandlers();
            var aggregateRootSnapshot = aggregateRoot.TakeSnapshot();

            try
            {
                aggregateRootSnapshot.RestoreAggregate(new IEvent[]
                {
                    new ValueChangedEvent()
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
        public void LoadFromHistory_Throws_IfEventVersionIsNotHigherThanAggregateVersion_And_AggregateHasNoEventHandlers()
        {
            IAggregateRoot<Guid> aggregateRoot = new AggregateWithoutEventHandlers();
            var aggregateRootSnapshot = aggregateRoot.TakeSnapshot();

            try
            {
                aggregateRootSnapshot.RestoreAggregate(new IEvent[]
                {
                    new ValueChangedEvent()
                    {
                        Id = aggregateRoot.Id,
                    }
                });
            }
            catch (ArgumentOutOfRangeException exception)
            {
                Assert.IsTrue(exception.Message.StartsWith("The next version (0) must represent a newer version than the current version (0)."));                
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void LoadFromHistory_Throws_IfEventVersionIsNotHigherThanAggregateVersion_And_AggregateHasEventHandlers()
        {
            IAggregateRoot<Guid> aggregateRoot = new AggregateWithEventHandlers();
            var aggregateRootSnapshot = aggregateRoot.TakeSnapshot();

            try
            {
                aggregateRootSnapshot.RestoreAggregate(new IEvent[]
                {
                    new ValueChangedEvent()
                    {
                        Id = aggregateRoot.Id
                    }
                });
            }
            catch (ArgumentOutOfRangeException exception)
            {
                Assert.IsTrue(exception.Message.StartsWith("The next version (0) must represent a newer version than the current version (0)."));
                throw;
            }            
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LoadFromHistory_Throws_IfAggregateHasNoEventHandlerForSpecifiedEvent()
        {
            IAggregateRoot<Guid> aggregateRoot = new AggregateWithoutEventHandlers();
            var aggregateRootSnapshot = aggregateRoot.TakeSnapshot();

            try
            {
                aggregateRootSnapshot.RestoreAggregate(new IEvent[]
                {
                    new ValueChangedEvent()
                    {
                        Id = aggregateRoot.Id,
                        Version = 1
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
            IAggregateRoot<Guid> aggregateRoot = new AggregateWithEventHandlers();
            var aggregateRootSnapshot = aggregateRoot.TakeSnapshot();

            try
            {
                aggregateRootSnapshot.RestoreAggregate(new IEvent[]
                {
                    new ValueChangedEvent()
                    {
                        Id = aggregateRoot.Id,
                        Version = 1
                    },
                    new UnsupportedEvent(), 
                });
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
            IAggregateRoot<Guid> aggregateRoot = new AggregateWithEventHandlers();
            var aggregateRootSnapshot = aggregateRoot.TakeSnapshot();

            var restoredAggregate = aggregateRootSnapshot.RestoreAggregate(new IEvent[]
            {
                new OldValueChangedEvent(aggregateRoot.Id, 2, 3),
                new ValueChangedEvent()
                {
                    Id = aggregateRoot.Id,
                    Version = 3,
                    NewValue = 4
                },
                new OldValueChangedEvent(aggregateRoot.Id, 1, 2),
            }) as AggregateWithEventHandlers;

            IAggregateRoot<Guid> restoredAggregateRoot = restoredAggregate;

            Assert.IsNotNull(restoredAggregate);
            Assert.AreEqual(aggregateRoot.Id, restoredAggregateRoot.Id);

            restoredAggregate.AssertVersionIs(3);
            restoredAggregate.AssertValueIs(4);
        }

        #endregion

        #region [====== HasPublishedEvents & FlushEvents() ======]

        [TestMethod]
        public void HasPublishedEvents_IsFalse_IfAggregateHasJustBeenRestoredFromHistory()
        {
            IAggregateRoot<Guid> aggregateRoot = new AggregateWithEventHandlers();
            var aggregateRootSnapshot = aggregateRoot.TakeSnapshot();

            var restoredAggregate = aggregateRootSnapshot.RestoreAggregate(new IEvent[]
            {
                new OldValueChangedEvent(aggregateRoot.Id, 2, 3),
                new ValueChangedEvent()
                {
                    Id = aggregateRoot.Id,
                    Version = 3,
                    NewValue = 4
                },
                new OldValueChangedEvent(aggregateRoot.Id, 1, 2),
            });

            Assert.IsFalse(restoredAggregate.HasPublishedEvents);
            Assert.AreEqual(0, restoredAggregate.FlushEvents().Count());
        }

        [TestMethod]
        public void FlushEvents_ClearsTheInternalEventBuffer_IfAggregateHasNoEventHandlers()
        {
            var aggregate = new AggregateWithoutEventHandlers();
            var newValue = RandomValue();
            IAggregateRoot<Guid> aggregateRoot = aggregate;            
            
            aggregate.ChangeValue(newValue);            

            Assert.IsTrue(aggregateRoot.HasPublishedEvents);
            Assert.AreEqual(1, aggregateRoot.FlushEvents().Count());
            Assert.IsFalse(aggregateRoot.HasPublishedEvents);
            Assert.AreEqual(0, aggregateRoot.FlushEvents().Count());
        }

        #endregion

        private static int RandomValue() =>
            Clock.Current.UtcDateAndTime().Millisecond + 1;
    }
}
