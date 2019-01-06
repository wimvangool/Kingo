using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Domain
{
    [TestClass]
    public sealed class MemoryRepositoryUsingSnapshotsTest : MemoryRepositorySerializationTest
    {
        #region [====== Snapshots ======]

        private sealed class SnapshotOfIncorrectType : SnapshotOrEvent<Guid, long>
        {
            public SnapshotOfIncorrectType(Guid id)
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

        private sealed class SnapshotThatCannotRestoreAggregate : SnapshotOrEvent<Guid, int>
        {
            public SnapshotThatCannotRestoreAggregate(Guid id)
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
        }

        private sealed class SnapshotThatRestoresAggregateOfWrongType : SnapshotOrEvent<Guid, int>
        {
            public SnapshotThatRestoresAggregateOfWrongType(Guid id)
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

        private sealed class VeryOldSnapshot : ISnapshotOrEvent
        {
            private readonly Guid _numberId;

            public VeryOldSnapshot(Guid numberId)
            {
                _numberId = numberId;
            }

            public ISnapshotOrEvent UpdateToNextVersion() =>
                new NumberUsingSnapshots.Snapshot(_numberId, 1, 0, false);
        }

        #endregion

        #region [====== Read Methods ======]

        [TestMethod]
        [ExpectedException(typeof(CouldNotRestoreAggregateException))]
        public async Task GetByIdAsync_Throws_IfDataSetHasOnlyEvents()
        {
            var numberId = Guid.NewGuid();

            Repository.Add(numberId, new AggregateDataSet(null, new ISnapshotOrEvent[]
            {
                new NumberUsingSnapshots.CreatedEvent(numberId, 0)
            }));

            await Repository.GetByIdAsync(numberId);
        }

        [TestMethod]
        [ExpectedException(typeof(CouldNotRestoreAggregateException))]
        public async Task GetByIdAsync_Throws_IfSnapshotIsNotOfValidType()
        {
            var numberId = Guid.NewGuid();

            Repository.Add(numberId, new AggregateDataSet(new SnapshotOfIncorrectType(numberId)));

            await Repository.GetByIdAsync(numberId);
        }

        [TestMethod]
        [ExpectedException(typeof(CouldNotRestoreAggregateException))]
        public async Task GetByIdAsync_Throws_IfSnapshotCannotRestoreAggregate()
        {
            var numberId = Guid.NewGuid();

            Repository.Add(numberId, new AggregateDataSet(new SnapshotThatCannotRestoreAggregate(numberId)));

            await Repository.GetByIdAsync(numberId);
        }

        [TestMethod]
        [ExpectedException(typeof(CouldNotRestoreAggregateException))]
        public async Task GetByIdAsync_Throws_IfSnapshotRestoresAggregateOfWrongType()
        {
            var numberId = Guid.NewGuid();

            Repository.Add(numberId, new AggregateDataSet(new SnapshotThatRestoresAggregateOfWrongType(numberId)));

            await Repository.GetByIdAsync(numberId);
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsAggregate_IfSnapshotIsLatestVersion_And_SnapshotCanRestoreAggregate()
        {
            var numberId = Guid.NewGuid();

            Repository.Add(numberId, new AggregateDataSet(new NumberUsingSnapshots.Snapshot(numberId, 1, 0, false)));

            Assert.IsNotNull(await Repository.GetByIdAsync(numberId));
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsAggregate_IfSnapshotIsNotLatestVersion_But_LatestVersionCanRestoreAggregate()
        {
            var numberId = Guid.NewGuid();

            Repository.Add(numberId, new AggregateDataSet(new VeryOldSnapshot(numberId)));

            Assert.IsNotNull(await Repository.GetByIdAsync(numberId));
        }

        #endregion

        #region [====== Write Methods ======]

        

        #endregion

        protected override MemoryRepositorySerializationStub CreateRepository() =>
            new MemoryRepositorySerializationStub(SerializationStrategy.UseSnapshots());

        protected override Number RandomNumber(int value = 0, IEventBus eventBus = null) =>
            NumberUsingSnapshots.CreateNumber(Guid.NewGuid(), value, eventBus);
    }
}
