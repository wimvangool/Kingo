using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Domain
{
    [TestClass]
    public abstract class MemoryRepositorySerializationTest
    {
        #region [====== Aggregates ======]

        protected sealed class AggregateOfWrongType : AggregateRoot<Guid, int>
        {
            public AggregateOfWrongType(ISnapshotOrEvent<Guid, int> snapshotOrEvent) :
                base(null, snapshotOrEvent, false) { }

            protected override int NextVersion() =>
                Version + 1;
        }

        #endregion

        #region [====== Setup ======]

        [TestInitialize]
        public virtual void Setup()
        {
            Processor = new MicroProcessor();
            Repository = CreateRepository();
        }

        protected MicroProcessor Processor
        {
            get;
            private set;
        }

        protected MemoryRepositorySerializationStub Repository
        {
            get;
            private set;
        }        

        protected abstract MemoryRepositorySerializationStub CreateRepository();

        #endregion

        #region [====== Read Methods ======]

        [TestMethod]
        [ExpectedException(typeof(CouldNotRestoreAggregateException))]
        public async Task GetByIdAsync_Throws_IfSelectedDataSetIsEmpty()
        {
            var numberId = Guid.NewGuid();

            Repository.Add(numberId, new AggregateDataSet());

            await Repository.GetByIdAsync(numberId);
        }        

        #endregion

        #region [====== Write Methods ======]

        [TestMethod]
        public async Task RemoveAsync_FlushesKeyToDelete_IfDeleteIsHardDelete()
        {
            var command = AddValueCommand.Random();
            await Repository.AddAsync(CreateNumber(command.NumberId));

            await Processor.HandleAsync(command, async (message, context) =>
            {
                var number = await Repository.GetByIdAsync(message.NumberId);
                number.Add(command.Value);
                number.Add(command.Value);

                Assert.IsTrue(await Repository.RemoveAsync(number));
            });

            Repository.AssertChangeSet(1, changeSet =>
            {
                Assert.AreEqual(0, changeSet.AggregatesToInsert.Count);
                Assert.AreEqual(0, changeSet.AggregatesToUpdate.Count);
                Assert.AreEqual(1, changeSet.AggregatesToDelete.Count);
                Assert.AreEqual(command.NumberId, changeSet.AggregatesToDelete[0]);
            });
        }

        protected abstract Number CreateNumber(Guid numberId, int value = 0, IEventBus eventBus = null);

        protected static void AssertSnapshotOnly(AggregateDataSet<Guid, int> dataSet, Guid numberId, int version)
        {
            AssertSnapshot(dataSet, numberId, version);
            Assert.AreEqual(0, dataSet.Events.Count);
        }

        protected static void AssertSnapshot(AggregateDataSet<Guid, int> dataSet, Guid numberId, int version)
        {
            Assert.IsNotNull(dataSet.Snapshot);
            Assert.AreEqual(numberId, dataSet.Snapshot.Id);
            Assert.AreEqual(version, dataSet.Snapshot.Version);
        }

        protected static void AssertEventsOnly(AggregateDataSet<Guid, int> dataSet, Guid numberId, params int[] versions)
        {
            Assert.IsNull(dataSet.Snapshot);
            AssertEvents(dataSet, numberId, versions);
        }

        protected static void AssertEvents(AggregateDataSet<Guid, int> dataSet, Guid numberId, params int[] versions)
        {
            Assert.AreEqual(versions.Length, dataSet.Events.Count);

            for (var index = 0; index < versions.Length; index++)
            {
                AssertEvent(dataSet.Events[index], numberId, versions[index]);
            }
        }

        private static void AssertEvent(ISnapshotOrEvent<Guid, int> @event, Guid numberId, int version)
        {
            Assert.AreEqual(numberId, @event.Id);
            Assert.AreEqual(version, @event.Version);
        }

        #endregion
    }
}
