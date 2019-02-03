using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Domain
{
    [TestClass]
    public abstract class MemoryRepositoryUsingEventsTest : MemoryRepositorySerializationTest
    {
        #region [====== Events ======]

        private sealed class EventOfIncorrectType : SnapshotOrEvent<Guid, long>
        {
            public EventOfIncorrectType(Guid id)
            {
                Id = id;
                Version = 1;
            }

            protected override Guid Id
            {
                get;
            }

            protected override long Version
            {
                get;
            }
        }

        private sealed class EventThatCannotRestoreAggregate : SnapshotOrEvent<Guid, int>
        {
            public EventThatCannotRestoreAggregate(Guid id, int version = 1)
            {
                Id = id;
                Version = version;
            }

            protected override Guid Id
            {
                get;
            }

            protected override int Version
            {
                get;
            }
        }

        private sealed class EventThatRestoresAggregateOfWrongType : SnapshotOrEvent<Guid, int>
        {
            public EventThatRestoresAggregateOfWrongType(Guid id)
            {
                Id = id;
                Version = 1;
            }

            protected override Guid Id
            {
                get;
            }

            protected override int Version
            {
                get;
            }

            protected override IAggregateRoot<Guid, int> RestoreAggregate(IEventBus eventBus = null) =>
                new AggregateOfWrongType(this);
        }

        private sealed class VeryOldEvent : ISnapshotOrEvent
        {
            private readonly Guid _numberId;

            public VeryOldEvent(Guid numberId)
            {
                _numberId = numberId;
            }

            public ISnapshotOrEvent UpdateToNextVersion() =>
                new NumberUsingEvents.CreatedEvent(_numberId, 1);
        }

        #endregion

        #region [====== Read Methods ======]        

        [TestMethod]
        [ExpectedException(typeof(CouldNotRestoreAggregateException))]
        public async Task GetByIdAsync_Throws_IfEventIsNotOfValidType()
        {
            var numberId = Guid.NewGuid();

            Repository.Add(numberId, CreateDataSet(new EventOfIncorrectType(numberId)));

            await Repository.GetByIdAsync(numberId);
        }

        [TestMethod]
        [ExpectedException(typeof(CouldNotRestoreAggregateException))]
        public async Task GetByIdAsync_Throws_IfEventCannotRestoreAggregate()
        {
            var numberId = Guid.NewGuid();

            Repository.Add(numberId, CreateDataSet(new EventThatCannotRestoreAggregate(numberId)));

            await Repository.GetByIdAsync(numberId);
        }

        [TestMethod]
        [ExpectedException(typeof(CouldNotRestoreAggregateException))]
        public async Task GetByIdAsync_Throws_IfEventRestoresAggregateOfWrongType()
        {
            var numberId = Guid.NewGuid();

            Repository.Add(numberId, CreateDataSet(new EventThatRestoresAggregateOfWrongType(numberId)));

            await Repository.GetByIdAsync(numberId);
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsAggregate_IfEventIsLatestVersion_And_EventCanRestoreAggregate()
        {
            var numberId = Guid.NewGuid();

            Repository.Add(numberId, CreateDataSet(new NumberUsingEvents.CreatedEvent(numberId, 1)));

            Assert.IsNotNull(await Repository.GetByIdAsync(numberId));
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsAggregate_IfEventIsNotLatestVersion_But_LatestVersionCanRestoreAggregate()
        {
            var numberId = Guid.NewGuid();

            Repository.Add(numberId, CreateDataSet(new VeryOldEvent(numberId)));

            Assert.IsNotNull(await Repository.GetByIdAsync(numberId));
        }

        [TestMethod]
        [ExpectedException(typeof(CouldNotRestoreAggregateException))]
        public async Task GetByIdAsync_Throws_IfSomeEventsAreNotRecognizedByAggregate()
        {
            var numberId = Guid.NewGuid();

            Repository.Add(numberId, CreateDataSet(            
                new NumberUsingSnapshots.CreatedEvent(numberId, 0), 
                new ValueAddedEvent(numberId, 2, 3),
                new EventThatCannotRestoreAggregate(numberId, 3)));

            await Repository.GetByIdAsync(numberId);
        }

        [TestMethod]
        [ExpectedException(typeof(CouldNotRestoreAggregateException))]
        public async Task GetByIdAsync_Throws_IfSomeEventsSpecifyInvalidAggregateId()
        {
            var numberId = Guid.NewGuid();

            Repository.Add(numberId, CreateDataSet(
                new NumberUsingSnapshots.CreatedEvent(numberId, 0),
                new ValueAddedEvent(Guid.NewGuid(), 2, 3)));

            await Repository.GetByIdAsync(numberId);
        }

        [TestMethod]        
        public async Task GetByIdAsync_ReturnsAggregate_IfDataSetContainsEventsInOrder()
        {
            var numberId = Guid.NewGuid();

            Repository.Add(numberId, CreateDataSet(
                new VeryOldEvent(numberId),
                new ValueAddedEvent(numberId, 2, 3)));

            Assert.IsNotNull(await Repository.GetByIdAsync(numberId));
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsAggregate_IfDataSetContainsEventsOutOfOrder()
        {
            var numberId = Guid.NewGuid();

            Repository.Add(numberId, CreateDataSet(
                new ValueAddedEvent(numberId, 2, 3),
                new VeryOldEvent(numberId)));

            Assert.IsNotNull(await Repository.GetByIdAsync(numberId));
        }

        #endregion       

        private static AggregateReadSet CreateDataSet(params ISnapshotOrEvent[] events) =>
            new AggregateReadSet(null, events);        

        protected override Number CreateNumber(Guid numberId, int value = 0, IEventBus eventBus = null) =>
            NumberUsingEvents.CreateNumber(numberId, value, eventBus);
    }
}
