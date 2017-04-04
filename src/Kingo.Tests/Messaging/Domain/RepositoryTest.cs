using System;
using System.Threading.Tasks;
using Kingo.Clocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Domain
{
    [TestClass]
    public sealed class RepositoryTest
    {        
        #region [====== DoSomething ======]

        private sealed class DoSomethingCommand
        {
            public DoSomethingCommand(Guid id, int? newValue = null)
            {
                Id = id;
                NewValue = newValue ?? RandomValue();
            }

            public Guid Id
            {
                get;
            }

            public int NewValue
            {
                get;
            }

            public static DoSomethingCommand WithRandomValues() =>
                new DoSomethingCommand(Guid.NewGuid(), RandomValue());

            private static int RandomValue() =>
                Clock.Current.UtcDateAndTime().Millisecond + 1;
        }

        private sealed class DoSomethingCommandHandler : IMessageHandler<DoSomethingCommand>
        {
            private readonly IMessageHandlerImplementation _implementation;                        

            public DoSomethingCommandHandler(IMessageHandlerImplementation implementation)
            {
                _implementation = implementation;
            }

            public Task HandleAsync(DoSomethingCommand message, IMicroProcessorContext context) =>
                _implementation.HandleAsync(message, context, typeof(DoSomethingCommandHandler));
        }

        #endregion
        
        private MicroProcessorSpy _processor;
        
        [TestInitialize]
        public void Setup()
        {            
            _processor = new MicroProcessorSpy();            
        }        

        #region [====== AddAsync ======]

        //[TestMethod]
        //[ExpectedException(typeof(InternalServerErrorException))]
        //public async Task AddAsync_Throws_IfAggregateIsNull()
        //{
        //    var command = new AddNullAggregateCommand();

        //    _processor.Register<AddNullAggregateCommandHandler>();

        //    try
        //    {
        //        await _processor.HandleAsync(command);
        //    }
        //    catch (InternalProcessorException exception)
        //    {
        //        Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentNullException));
        //        throw;
        //    }
        //}

        //[TestMethod]
        //public async Task AddAsync_AddsSpecifiedAggregateToTheRepository_IfAggregateDoesNotYetExist()
        //{
        //    var command = new AddNewInstanceCommand(Guid.NewGuid());

        //    _processor.Register<AddNewInstanceCommandHandler>();

        //    await _processor.HandleAsync(command);

        //    Assert.AreEqual(1, _repository.Count);
        //}

        #endregion

        #region [====== GetByIdAsync ======]             

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public async Task GetByIdAsync_Throws_IfAggregateDoesNotExistInDataStore()
        {
            var repository = CreateMemoryRepository();
            var command = DoSomethingCommand.WithRandomValues();                        

            _processor.Implement<DoSomethingCommandHandler>().AsAsync<DoSomethingCommand>(async (message, context) =>
            {
                await repository.GetByIdAsync(command.Id);
            });

            try
            {
                await _processor.HandleAsync(command);
            }
            catch (BadRequestException exception)
            {
                var aggregateNotFoundException = exception.InnerException as AggregateNotFoundException;

                Assert.IsNotNull(aggregateNotFoundException);
                Assert.AreEqual(command.Id, aggregateNotFoundException.AggregateId);
                Assert.AreEqual($"Instance of type 'AggregateRootSpy' with Id '{command.Id}' was not found.", aggregateNotFoundException.Message);
                throw;
            }
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsAggregate_IfAggregateExistsInDataStore()
        {
            var command = DoSomethingCommand.WithRandomValues();
            var aggregate = CreateExistingAggregate(command.Id);
            var repository = CreateMemoryRepository(aggregate);            

            _processor.Implement<DoSomethingCommandHandler>().AsAsync<DoSomethingCommand>(async (message, context) =>
            {
                var retrievedAggregate = await repository.GetByIdAsync(command.Id);

                Assert.IsNotNull(retrievedAggregate);
                Assert.AreNotSame(aggregate, retrievedAggregate);
            });

            var outputStream = await _processor.HandleAsync(command);

            Assert.AreEqual(0, outputStream.Count);
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsSameAggregateTwice_IfAggregateIsRetrievedTwiceInSameSession()
        {
            var command = DoSomethingCommand.WithRandomValues();
            var aggregate = CreateExistingAggregate(command.Id);
            var repository = CreateMemoryRepository(aggregate);            

            _processor.Implement<DoSomethingCommandHandler>().AsAsync<DoSomethingCommand>(async (message, context) =>
            {
                var retrievedAggregate1 = await repository.GetByIdAsync(command.Id);
                var retrievedAggregate2 = await repository.GetByIdAsync(command.Id);
                
                Assert.AreSame(retrievedAggregate1, retrievedAggregate2);
            });

            var outputStream = await _processor.HandleAsync(command);

            Assert.AreEqual(0, outputStream.Count);
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsSameAggregateTwice_IfAggregateIsChanged_And_AggregateIsRetrievedTwiceInSameSession()
        {
            var command = DoSomethingCommand.WithRandomValues();
            var aggregate = CreateExistingAggregate(command.Id);
            var repository = CreateMemoryRepository(aggregate);

            _processor.Implement<DoSomethingCommandHandler>().AsAsync<DoSomethingCommand>(async (message, context) =>
            {
                var retrievedAggregate1 = await repository.GetByIdAsync(command.Id);

                retrievedAggregate1.ChangeValue(message.NewValue);

                var retrievedAggregate2 = await repository.GetByIdAsync(command.Id);

                Assert.AreSame(retrievedAggregate1, retrievedAggregate2);                
            });

            var outputStream = await _processor.HandleAsync(command);

            Assert.AreEqual(1, outputStream.Count);
            AssertEvent<ValueChangedEvent>(outputStream, 0, @event =>
            {
                Assert.AreEqual(command.Id, @event.Id);
                Assert.AreEqual(1, @event.Version);
                Assert.AreEqual(command.NewValue, @event.NewValue);
            });            

            repository.AssertRequiresFlushCountIs(1);
            repository.AssertFlushCountIs(1);            

            var changeSet = repository.GetChangeSet(0);

            Assert.IsFalse(repository.RequiresFlush());
            Assert.AreEqual(0, changeSet.AggregatesToInsert.Count);
            Assert.AreEqual(1, changeSet.AggregatesToUpdate.Count);
            Assert.AreEqual(0, changeSet.AggregatesToDelete.Count);
        }

        #endregion

        private static AggregateRootSpy CreateExistingAggregate(Guid id) =>
            new AggregateRootWithoutEventHandlers(new SnapshotMock(false) { Id = id });

        private static MemoryRepositorySpy<Guid, AggregateRootSpy> CreateMemoryRepository(params AggregateRootSpy[] existingAggregates) =>
            new MemoryRepositorySpy<Guid, AggregateRootSpy>(existingAggregates);

        private static void AssertEvent<TEvent>(IMessageStream stream, int index, Action<TEvent> assert) =>
            assert.Invoke((TEvent) stream[index]);
    }
}
