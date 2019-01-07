using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Domain
{
    [TestClass]
    public sealed class MemoryRepositoryUsingEventsOnlyTest : MemoryRepositoryUsingEventsTest
    {
        #region [====== Read Methods ======]

        [TestMethod]
        [ExpectedException(typeof(CouldNotRestoreAggregateException))]
        public async Task GetByIdAsync_Throws_IfDataSetHasOnlySnapshot()
        {
            var numberId = Guid.NewGuid();

            Repository.Add(numberId, new AggregateDataSet(new NumberUsingEvents.Snapshot(numberId, 1, 0, false)));

            await Repository.GetByIdAsync(numberId);
        }

        #endregion

        #region [====== Write Methods ======]

        [TestMethod]
        public async Task AddAsync_FlushesEventsToInsert()
        {
            var command = CreateNumberCommand.Random();

            await Processor.HandleAsync(command, async (message, context) =>
            {
                var number = CreateNumber(message.NumberId, command.Value, context.EventBus);
                number.Add(command.Value);

                Assert.IsTrue(await Repository.AddAsync(number));
            });

            Repository.AssertChangeSet(0, changeSet =>
            {
                Assert.AreEqual(1, changeSet.AggregatesToInsert.Count);
                AssertEventsOnly(changeSet.AggregatesToInsert[0], command.NumberId, 1, 2);

                Assert.AreEqual(0, changeSet.AggregatesToUpdate.Count);
                Assert.AreEqual(0, changeSet.AggregatesToDelete.Count);
            });
        }

        [TestMethod]
        public async Task GetByIdAsyncAndModification_FlushesEventsToUpdate()
        {
            var command = AddValueCommand.Random();
            await Repository.AddAsync(CreateNumber(command.NumberId));

            await Processor.HandleAsync(command, async (message, context) =>
            {
                var number = await Repository.GetByIdAsync(message.NumberId);
                number.Add(message.Value);
                number.Add(message.Value);
            });

            Repository.AssertChangeSet(1, changeSet =>
            {
                Assert.AreEqual(0, changeSet.AggregatesToInsert.Count);
                Assert.AreEqual(1, changeSet.AggregatesToUpdate.Count);
                AssertEventsOnly(changeSet.AggregatesToUpdate[0], command.NumberId, 2, 3);

                Assert.AreEqual(0, changeSet.AggregatesToDelete.Count);
            });
        }

        [TestMethod]
        public async Task RemoveAsync_FlushesEventsToUpdate_IfDeleteIsSoftDelete()
        {
            var command = AddValueCommand.Random();
            await Repository.AddAsync(CreateNumber(command.NumberId));

            await Processor.HandleAsync(command, async (message, context) =>
            {
                var number = await Repository.GetByIdAsync(message.NumberId);
                number.EnableSoftDelete = true;
                number.Add(command.Value);
                number.Add(command.Value);

                Assert.IsTrue(await Repository.RemoveAsync(number));
            });

            Repository.AssertChangeSet(1, changeSet =>
            {
                Assert.AreEqual(0, changeSet.AggregatesToInsert.Count);
                Assert.AreEqual(1, changeSet.AggregatesToUpdate.Count);
                AssertEventsOnly(changeSet.AggregatesToUpdate[0], command.NumberId, 2, 3, 4);

                Assert.AreEqual(0, changeSet.AggregatesToDelete.Count);
            });
        }            

        #endregion

        protected override MemoryRepositorySerializationStub CreateRepository() =>
            new MemoryRepositorySerializationStub(SerializationStrategy.UseEvents());
    }
}
