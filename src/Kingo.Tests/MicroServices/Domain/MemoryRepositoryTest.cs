using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kingo.Clocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Domain
{
    [TestClass]
    public sealed class MemoryRepositoryTest
    {
        private readonly MicroServiceBusStub _serviceBus;
        private readonly MicroProcessor _processor;        

        public MemoryRepositoryTest()
        {
            _serviceBus = new MicroServiceBusStub();
            _processor = new MicroProcessor(null, null, _serviceBus);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Throws_IfSerializationStrategyIsNull()
        {
            new MemoryRepository<Guid, int, Number>(null);
        }

        #region [====== GetByIdAsync ======]        

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public async Task GetByIdAsync_Throws_IfInNullState_And_AggregateDoesNotExistInDataStore()
        {
            var repository = await CreateRepositoryAsync();  
            var command = new AddValueCommand(Guid.Empty, 0);            

            try
            {
                await _processor.HandleAsync(command, async (message, context) =>
                {
                    await repository.GetByIdAsync(message.NumberId);
                });
            }
            catch (BadRequestException exception)
            {
                var aggregateNotFoundException = exception.InnerException as AggregateNotFoundException;

                Assert.IsNotNull(aggregateNotFoundException);                
                Assert.AreEqual($"Aggregate of type 'Number' with Id '{Guid.Empty}' was not found in the data store.", aggregateNotFoundException.Message);
                throw;
            }
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsExpectedInstance_IfInNullState_And_AggregateExistsInDataStore()
        {
            var numberId = Guid.NewGuid();
            var repository = await CreateRepositoryAsync(numberId);
            var command = new AddValueCommand(numberId, DateTimeOffset.UtcNow.Second);

            await _processor.HandleAsync(command, async (message, context) =>
            {
                (await repository.GetByIdAsync(message.NumberId)).Add(message.Value);
            });

            _serviceBus.AssertEventCountIs(1);
            _serviceBus.AssertEvent<ValueAddedEvent>(0, @event =>
            {
                Assert.AreEqual(numberId, @event.NumberId);
                Assert.AreEqual(2, @event.NumberVersion);
                Assert.AreEqual(command.Value, @event.Value);
            });
        }     

        #endregion

        //[TestMethod]
        //public async Task GetByIdAsync_RestoresAggregateBySnapshot_IfStrategyIsEventsAndSnapshots_And_AggregateWasInsertedButNotUpdated()
        //{
        //    var aggregateCreatedEvent = new AggregateEventSourcedAggregateCreatedAggregateEvent();
        //    var aggregate = new EventSourcedAggregate(aggregateCreatedEvent, true);
        //    var newValue = Clock.Current.UtcDateAndTime().Millisecond + 1;

        //    aggregate.ChangeValue(newValue);
        //    aggregate.AssertHandleCountIs(1);

        //    await _processor.HandleAsync(new object(), async (message, context) =>
        //    {
        //        Assert.IsTrue(await _repository.AddAsync(aggregate));
        //    });

        //    Assert.AreEqual(1, _repository.Count);

        //    await _processor.HandleAsync(new object(), async (message, context) =>
        //    {
        //        var restoredAggregate = await _repository.GetByIdAsync(aggregateCreatedEvent.Id);

        //        Assert.AreNotSame(aggregate, restoredAggregate);

        //        restoredAggregate.AssertValueIs(newValue);
        //        restoredAggregate.AssertHandleCountIs(0);                
        //    });

        //    Assert.AreEqual(1, _repository.Count);
        //}

        //[TestMethod]
        //public async Task GetByIdAsync_RestoresAggregateBySnapshotAndEvents_IfBehaviorIsStoreEvents_And_AggregateWasInsertedAndUpdatedSeveralTimes()
        //{
        //    var aggregateCreatedEvent = new AggregateEventSourcedAggregateCreatedAggregateEvent();
        //    var aggregate = new EventSourcedAggregate(aggregateCreatedEvent, true);
        //    var newValue1 = Clock.Current.UtcDateAndTime().Millisecond + 1;
        //    var newValue2 = newValue1 * 2;
        //    var newValue3 = newValue2 * 2;

        //    aggregate.ChangeValue(newValue1);
        //    aggregate.AssertHandleCountIs(1);

        //    await _processor.HandleAsync(new object(), async (message, context) =>
        //    {
        //        Assert.IsTrue(await _repository.AddAsync(aggregate));
        //    });

        //    Assert.AreEqual(1, _repository.Count);

        //    await _processor.HandleAsync(new object(), async (message, context) =>
        //    {
        //        var restoredAggregate = await _repository.GetByIdAsync(aggregateCreatedEvent.Id);

        //        restoredAggregate.ChangeValue(newValue2);
        //        restoredAggregate.ChangeValue(newValue3);
        //        restoredAggregate.AssertHandleCountIs(2);
        //    });

        //    Assert.AreEqual(1, _repository.Count);

        //    await _processor.HandleAsync(new object(), async (message, context) =>
        //    {
        //        var restoredAggregate = await _repository.GetByIdAsync(aggregateCreatedEvent.Id);

        //        Assert.AreNotSame(aggregate, restoredAggregate);

        //        restoredAggregate.AssertValueIs(newValue3);
        //        restoredAggregate.AssertHandleCountIs(0);
        //    });
        //}               

        private static async Task<IRepository<Guid, Number>> CreateRepositoryAsync(params Guid[] numberIds)
        {
            var repository = new MemoryRepository<Guid, int, Number>(SerializationStrategy.UseSnapshots());

            foreach (var numberId in numberIds)
            {
                await repository.AddAsync(CreateNewNumber(numberId));
            }
            return repository;
        }

        private static Number CreateNewNumber(Guid numberId) =>
            NumberUsingSnapshots.CreateNumber(numberId, 0);
    }
}
