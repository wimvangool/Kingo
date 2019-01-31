using System;
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

            Repository.Add(numberId, new AggregateReadSet(new ISnapshotOrEvent[]
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

            Repository.Add(numberId, new AggregateReadSet(new SnapshotOfIncorrectType(numberId)));

            await Repository.GetByIdAsync(numberId);
        }

        [TestMethod]
        [ExpectedException(typeof(CouldNotRestoreAggregateException))]
        public async Task GetByIdAsync_Throws_IfSnapshotCannotRestoreAggregate()
        {
            var numberId = Guid.NewGuid();

            Repository.Add(numberId, new AggregateReadSet(new SnapshotThatCannotRestoreAggregate(numberId)));

            await Repository.GetByIdAsync(numberId);
        }

        [TestMethod]
        [ExpectedException(typeof(CouldNotRestoreAggregateException))]
        public async Task GetByIdAsync_Throws_IfSnapshotRestoresAggregateOfWrongType()
        {
            var numberId = Guid.NewGuid();

            Repository.Add(numberId, new AggregateReadSet(new SnapshotThatRestoresAggregateOfWrongType(numberId)));

            await Repository.GetByIdAsync(numberId);
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsAggregate_IfSnapshotIsLatestVersion_And_SnapshotCanRestoreAggregate()
        {
            var numberId = Guid.NewGuid();

            Repository.Add(numberId, new AggregateReadSet(new NumberUsingSnapshots.Snapshot(numberId, 1, 0, false)));

            Assert.IsNotNull(await Repository.GetByIdAsync(numberId));
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsAggregate_IfSnapshotIsNotLatestVersion_But_LatestVersionCanRestoreAggregate()
        {
            var numberId = Guid.NewGuid();

            Repository.Add(numberId, new AggregateReadSet(new VeryOldSnapshot(numberId)));

            Assert.IsNotNull(await Repository.GetByIdAsync(numberId));
        }

        #endregion

        #region [====== Write Methods ======]

        [TestMethod]
        public async Task AddAsync_FlushesSnapshotToInsert()
        {
            var command = CreateNumberCommand.Random();

            await Processor.HandleAsync(command, async (message, context) =>
            {
                Assert.IsTrue(await Repository.AddAsync(CreateNumber(message.NumberId, command.Value, context.EventBus)));
            });

            Repository.AssertChangeSet(0, changeSet =>
            {
                Assert.AreEqual(1, changeSet.AggregatesToInsert.Count);
                AssertSnapshotOnly(changeSet.AggregatesToInsert[0], command.NumberId, 1);

                Assert.AreEqual(0, changeSet.AggregatesToUpdate.Count);
                Assert.AreEqual(0, changeSet.AggregatesToDelete.Count);
            });
        }

        [TestMethod]
        public async Task GetByIdAsyncAndModification_FlushesSnapshotToUpdate()
        {
            var command = AddValueCommand.Random();
            await Repository.AddAsync(CreateNumber(command.NumberId));

            await Processor.HandleAsync(command, async (message, context) =>
            {
                var number = await Repository.GetByIdAsync(message.NumberId);
                number.Add(message.Value);
            });

            Repository.AssertChangeSet(1, changeSet =>
            {
                Assert.AreEqual(0, changeSet.AggregatesToInsert.Count);                
                Assert.AreEqual(1, changeSet.AggregatesToUpdate.Count);
                AssertSnapshotOnly(changeSet.AggregatesToUpdate[0], command.NumberId, 2);

                Assert.AreEqual(0, changeSet.AggregatesToDelete.Count);
            });
        }

        [TestMethod]
        public async Task RemoveAsync_FlushesSnapshotToUpdate_IfDeleteIsSoftDelete()
        {
            var command = DeleteNumberCommand.Random();            
            await Repository.AddAsync(CreateNumber(command.NumberId));

            await Processor.HandleAsync(command, async (message, context) =>
            {
                var number = await Repository.GetByIdAsync(message.NumberId);
                number.EnableSoftDelete = true;

                Assert.IsTrue(await Repository.RemoveAsync(number));                
            });

            Repository.AssertChangeSet(1, changeSet =>
            {
                Assert.AreEqual(0, changeSet.AggregatesToInsert.Count);
                Assert.AreEqual(1, changeSet.AggregatesToUpdate.Count);
                AssertSnapshotOnly(changeSet.AggregatesToUpdate[0], command.NumberId, 2);

                Assert.AreEqual(0, changeSet.AggregatesToDelete.Count);
            });
        }            

        #endregion

        protected override MemoryRepositorySerializationStub CreateRepository() =>
            new MemoryRepositorySerializationStub(SerializationStrategy.UseSnapshots());

        protected override Number CreateNumber(Guid numberId, int value = 0, IEventBus eventBus = null) =>
            NumberUsingSnapshots.CreateNumber(numberId, value, eventBus);
    }
}
