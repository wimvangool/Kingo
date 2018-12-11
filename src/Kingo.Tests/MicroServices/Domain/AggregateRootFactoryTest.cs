using System;
using JetBrains.Annotations;
using Kingo.Clocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Domain
{
    [TestClass]
    public sealed class AggregateRootFactoryTest
    {
        private abstract class AbstractType { }

        private sealed class AggregateRootWithoutConstructors { }

        private sealed class AggregateRootWithConstructors
        {
            [UsedImplicitly]
            public AggregateRootWithConstructors(Snapshot snapshot)
            {
                Value = snapshot.Value;
            }

            [UsedImplicitly]
            internal AggregateRootWithConstructors(Event @event, bool isNewAggregate)
            {
                Value = @event.Value;
                IsNewAggregate = isNewAggregate;
            }            

            public int Value
            {
                get;
            }

            public bool IsNewAggregate
            {
                get;
            }
        }

        private sealed class Snapshot
        {
            public Snapshot(int value)
            {
                Value = value;
            }

            public int Value
            {
                get;
            }
        }

        private sealed class Event
        {
            public Event(int value)
            {
                Value = value;
            }

            public int Value
            {
                get;
            }
        }

        #region [====== Snapshots ======]



        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RestoreAggregateFromSnapshot_Throws_IfAggregateTypeIsNoClass()
        {
            AggregateRootFactory.RestoreAggregateFromSnapshot(typeof(int), new object());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RestoreAggregateFromSnapshot_Throws_IfAggregateTypeIsAbstract()
        {
            AggregateRootFactory.RestoreAggregateFromSnapshot(typeof(AbstractType), new object());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RestoreAggregateFromSnapshot_Throws_IfAggregateTypeDoesNotHaveAppropriateConstructor()
        {
            try
            {
                AggregateRootFactory.RestoreAggregateFromSnapshot(typeof(AggregateRootWithoutConstructors), new object());
            }
            catch (ArgumentException exception)
            {
                Assert.IsTrue(exception.Message.Contains($"Cannot restore aggregate of type '{nameof(AggregateRootWithoutConstructors)}': required constructor '{nameof(AggregateRootWithoutConstructors)}(Object)' not found."));
                throw;
            }
        }

        [TestMethod]
        public void RestoreAggregateFromSnapshot_RestoresAggregate_IfAggregateTypeHasTheAppropriateConstructor()
        {
            var snapshot = new Snapshot(Clock.Current.UtcTime().Milliseconds);
            var aggregateRoot = (AggregateRootWithConstructors) AggregateRootFactory.RestoreAggregateFromSnapshot(typeof(AggregateRootWithConstructors), snapshot);

            Assert.AreEqual(snapshot.Value, aggregateRoot.Value);
            Assert.IsFalse(aggregateRoot.IsNewAggregate);
        }

        #endregion

        #region [====== Events ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RestoreAggregateFromEvent_Throws_IfAggregateTypeIsNoClass()
        {
            AggregateRootFactory.RestoreAggregateFromEvent(typeof(int), new object());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RestoreAggregateFromEvent_Throws_IfAggregateTypeIsAbstract()
        {
            AggregateRootFactory.RestoreAggregateFromEvent(typeof(AbstractType), new object());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RestoreAggregateFromEvent_Throws_IfAggregateTypeDoesNotHaveAppropriateConstructor()
        {
            try
            {
                AggregateRootFactory.RestoreAggregateFromEvent(typeof(AggregateRootWithoutConstructors), new object());
            }
            catch (ArgumentException exception)
            {
                Assert.IsTrue(exception.Message.Contains($"Cannot restore aggregate of type '{nameof(AggregateRootWithoutConstructors)}': required constructor '{nameof(AggregateRootWithoutConstructors)}(Object, Boolean)' not found."));
                throw;
            }
        }

        [TestMethod]
        public void RestoreAggregateFromEvent_RestoresAggregate_IfAggregateTypeHasTheAppropriateConstructor()
        {
            var @event = new Event(Clock.Current.UtcTime().Milliseconds);
            var aggregateRoot = (AggregateRootWithConstructors) AggregateRootFactory.RestoreAggregateFromEvent(typeof(AggregateRootWithConstructors), @event);

            Assert.AreEqual(@event.Value, aggregateRoot.Value);
            Assert.IsFalse(aggregateRoot.IsNewAggregate);
        }

        #endregion
    }
}
