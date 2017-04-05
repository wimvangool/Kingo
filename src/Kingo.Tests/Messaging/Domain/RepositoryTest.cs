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

        #region [====== GetByIdAsync ======]             

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public async Task GetByIdAsync_Throws_IfInNullState_And_AggregateDoesNotExistInDataStore()
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
                Assert.AreEqual($"Aggregate of type 'AggregateRootSpy' with Id '{command.Id}' was not found in the data store.", aggregateNotFoundException.Message);
                throw;
            }
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsExpectedInstance_IfInNullState_And_AggregateExistsInDataStore()
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
        public async Task GetByIdAsync_ReturnsExpectedInstance_IfInUnmodifiedState()
        {
            var command = DoSomethingCommand.WithRandomValues();
            var aggregate = CreateExistingAggregate(command.Id);
            var repository = CreateMemoryRepository(aggregate);            

            _processor.Implement<DoSomethingCommandHandler>().AsAsync<DoSomethingCommand>(async (message, context) =>
            {
                var aggregate1 = await repository.GetByIdAsync(command.Id);
                var aggregate2 = await repository.GetByIdAsync(command.Id);
                
                Assert.AreSame(aggregate1, aggregate2);
            });

            var outputStream = await _processor.HandleAsync(command);

            Assert.AreEqual(0, outputStream.Count);
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsExpectedInstance_IfInAddedState()
        {
            var command = DoSomethingCommand.WithRandomValues();
            var repository = CreateMemoryRepository();

            _processor.Implement<DoSomethingCommandHandler>().AsAsync<DoSomethingCommand>(async (message, context) =>
            {
                var aggregate1 = CreateNewAggregate(message.Id);

                Assert.IsTrue(await repository.AddAsync(aggregate1));

                var aggregate2 = await repository.GetByIdAsync(command.Id);                

                Assert.AreSame(aggregate1, aggregate2);
            });

            var outputStream = await _processor.HandleAsync(command);

            Assert.AreEqual(1, outputStream.Count);
            AssertEvent<AggregateRootSpyCreatedEvent>(outputStream, 0, @event =>
            {
                Assert.AreEqual(command.Id, @event.Id);
                Assert.AreEqual(1, @event.Version);                
            });

            repository.AssertRequiresFlushCountIs(1);
            repository.AssertFlushCountIs(1);

            var changeSet = repository.GetChangeSet(0);

            Assert.IsFalse(repository.RequiresFlush());
            Assert.AreEqual(1, changeSet.AggregatesToInsert.Count);
            Assert.AreEqual(0, changeSet.AggregatesToUpdate.Count);
            Assert.AreEqual(0, changeSet.AggregatesToDelete.Count);
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsExpectedInstance_IfInModifiedState()
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
                Assert.AreEqual(2, @event.Version);
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

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public async Task GetByIdAsync_Throws_IfInRemovedState()
        {
            var command = DoSomethingCommand.WithRandomValues();
            var aggregate = CreateExistingAggregate(command.Id);
            var repository = CreateMemoryRepository(aggregate);

            _processor.Implement<DoSomethingCommandHandler>().AsAsync<DoSomethingCommand>(async (message, context) =>
            {
                Assert.IsTrue(await repository.RemoveByIdAsync(message.Id));

                await repository.GetByIdAsync(message.Id);
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
                Assert.AreEqual($"Aggregate of type 'AggregateRootSpy' with Id '{command.Id}' could not be retrieved because it was removed from the data store in this session.", aggregateNotFoundException.Message);
                throw;
            }           
        }

        #endregion

        #region [====== AddAsync ======]

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task AddAsync_Throws_IfAggregateIsNull()
        {
            var command = DoSomethingCommand.WithRandomValues();            
            var repository = CreateMemoryRepository();

            _processor.Implement<DoSomethingCommandHandler>().AsAsync<DoSomethingCommand>(async (message, context) =>
            {
                await repository.AddAsync(null);
            });

            try
            {
                await _processor.HandleAsync(command);
            }
            catch (InternalServerErrorException exception)
            {                
                Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentNullException));               
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public async Task AddAsync_Throws_IfInNullState_And_AggregateAlreadyExistsInDataStore()
        {
            var command = DoSomethingCommand.WithRandomValues();
            var aggregate = CreateExistingAggregate(command.Id);
            var repository = CreateMemoryRepository(aggregate);

            _processor.Implement<DoSomethingCommandHandler>().AsAsync<DoSomethingCommand>(async (message, context) =>
            {
                await repository.AddAsync(aggregate);
            });

            try
            {
                await _processor.HandleAsync(command);
            }
            catch (BadRequestException exception)
            {
                var duplicateKeyException = exception.InnerException as DuplicateKeyException;

                Assert.IsNotNull(duplicateKeyException);
                Assert.AreEqual(command.Id, duplicateKeyException.AggregateId);
                Assert.AreEqual($"Cannot add aggregate of type 'AggregateRootSpy' to the repository because another aggregate with Id '{command.Id}' is already present in the data store.", duplicateKeyException.Message);
                throw;
            }
        }

        [TestMethod]
        public async Task AddAsync_ReturnsTrue_IfInNullState_And_AggregateDoesNotYetExistInDataStore()
        {
            var command = DoSomethingCommand.WithRandomValues();            
            var repository = CreateMemoryRepository();

            _processor.Implement<DoSomethingCommandHandler>().AsAsync<DoSomethingCommand>(async (message, context) =>
            {                
                Assert.IsTrue(await repository.AddAsync(CreateNewAggregate(message.Id)));
            });

            var outputStream = await _processor.HandleAsync(command);

            Assert.AreEqual(1, outputStream.Count);
            AssertEvent<AggregateRootSpyCreatedEvent>(outputStream, 0, @event =>
            {
                Assert.AreEqual(command.Id, @event.Id);
                Assert.AreEqual(1, @event.Version);
            });

            repository.AssertRequiresFlushCountIs(1);
            repository.AssertFlushCountIs(1);

            var changeSet = repository.GetChangeSet(0);

            Assert.AreEqual(1, changeSet.AggregatesToInsert.Count);
            Assert.AreEqual(0, changeSet.AggregatesToUpdate.Count);
            Assert.AreEqual(0, changeSet.AggregatesToDelete.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public async Task AddAsync_Throws_IfInUnmodifiedState_And_NewAggregateIsAdded()
        {
            var command = DoSomethingCommand.WithRandomValues();
            var aggregate = CreateExistingAggregate(command.Id);
            var repository = CreateMemoryRepository(aggregate);

            _processor.Implement<DoSomethingCommandHandler>().AsAsync<DoSomethingCommand>(async (message, context) =>
            {
                await repository.GetByIdAsync(message.Id);
                await repository.AddAsync(aggregate);
            });

            try
            {
                await _processor.HandleAsync(command);
            }
            catch (BadRequestException exception)
            {
                var duplicateKeyException = exception.InnerException as DuplicateKeyException;

                Assert.IsNotNull(duplicateKeyException);
                Assert.AreEqual(command.Id, duplicateKeyException.AggregateId);
                Assert.AreEqual($"Cannot add aggregate of type 'AggregateRootSpy' to the repository because another aggregate with Id '{command.Id}' is already present in the data store.", duplicateKeyException.Message);
                throw;
            }
        }

        [TestMethod]        
        public async Task AddAsync_ReturnsFalse_IfInUnmodifiedState_And_SameAggregatesIsAdded()
        {
            var command = DoSomethingCommand.WithRandomValues();
            var aggregate = CreateExistingAggregate(command.Id);
            var repository = CreateMemoryRepository(aggregate);

            _processor.Implement<DoSomethingCommandHandler>().AsAsync<DoSomethingCommand>(async (message, context) =>
            {
                var retrievedAggregate = await repository.GetByIdAsync(message.Id);

                Assert.IsFalse(await repository.AddAsync(retrievedAggregate));
            });

            var outputStream = await _processor.HandleAsync(command);

            Assert.AreEqual(0, outputStream.Count);

            repository.AssertRequiresFlushCountIs(0);
            repository.AssertFlushCountIs(0);
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public async Task AddAsync_Throws_IfInModifiedState_And_NewAggregateIsAdded()
        {
            var command = DoSomethingCommand.WithRandomValues();
            var aggregate = CreateExistingAggregate(command.Id);
            var repository = CreateMemoryRepository(aggregate);

            _processor.Implement<DoSomethingCommandHandler>().AsAsync<DoSomethingCommand>(async (message, context) =>
            {
                var retrievedAggregate = await repository.GetByIdAsync(message.Id);
                
                retrievedAggregate.ChangeValue(message.NewValue);

                await repository.AddAsync(aggregate);
            });

            try
            {
                await _processor.HandleAsync(command);
            }
            catch (BadRequestException exception)
            {
                var duplicateKeyException = exception.InnerException as DuplicateKeyException;

                Assert.IsNotNull(duplicateKeyException);
                Assert.AreEqual(command.Id, duplicateKeyException.AggregateId);
                Assert.AreEqual($"Cannot add aggregate of type 'AggregateRootSpy' to the repository because another aggregate with Id '{command.Id}' is already present in the data store.", duplicateKeyException.Message);
                throw;
            }
        }

        [TestMethod]
        public async Task AddAsync_ReturnsFalse_IfInModifiedState_And_SameAggregatesIsAdded()
        {
            var command = DoSomethingCommand.WithRandomValues();
            var aggregate = CreateExistingAggregate(command.Id);
            var repository = CreateMemoryRepository(aggregate);

            _processor.Implement<DoSomethingCommandHandler>().AsAsync<DoSomethingCommand>(async (message, context) =>
            {
                var retrievedAggregate = await repository.GetByIdAsync(message.Id);

                retrievedAggregate.ChangeValue(message.NewValue);

                Assert.IsFalse(await repository.AddAsync(retrievedAggregate));
            });

            var outputStream = await _processor.HandleAsync(command);

            Assert.AreEqual(1, outputStream.Count);
            AssertEvent<ValueChangedEvent>(outputStream, 0, @event =>
            {
                Assert.AreEqual(command.Id, @event.Id);
                Assert.AreEqual(2, @event.Version);
                Assert.AreEqual(command.NewValue, @event.NewValue);
            });

            repository.AssertRequiresFlushCountIs(1);
            repository.AssertFlushCountIs(1);

            var changeSet = repository.GetChangeSet(0);

            Assert.AreEqual(0, changeSet.AggregatesToInsert.Count);
            Assert.AreEqual(1, changeSet.AggregatesToUpdate.Count);
            Assert.AreEqual(0, changeSet.AggregatesToDelete.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public async Task AddAsync_Throws_IfInAddedState_And_NewAggregateIsAdded()
        {
            var command = DoSomethingCommand.WithRandomValues();
            var repository = CreateMemoryRepository();

            _processor.Implement<DoSomethingCommandHandler>().AsAsync<DoSomethingCommand>(async (message, context) =>
            {                
                Assert.IsTrue(await repository.AddAsync(CreateNewAggregate(message.Id)));

                await repository.AddAsync(CreateNewAggregate(message.Id));
            });

            try
            {
                await _processor.HandleAsync(command);
            }
            catch (BadRequestException exception)
            {
                var duplicateKeyException = exception.InnerException as DuplicateKeyException;

                Assert.IsNotNull(duplicateKeyException);
                Assert.AreEqual(command.Id, duplicateKeyException.AggregateId);
                Assert.AreEqual($"Cannot add aggregate of type 'AggregateRootSpy' to the repository because another aggregate with Id '{command.Id}' is already present in the data store.", duplicateKeyException.Message);
                throw;
            }
        }

        [TestMethod]
        public async Task AddAsync_ReturnsFalse_IfInAddedState_And_SameAggregateIsAdded()
        {
            var command = DoSomethingCommand.WithRandomValues();
            var repository = CreateMemoryRepository();

            _processor.Implement<DoSomethingCommandHandler>().AsAsync<DoSomethingCommand>(async (message, context) =>
            {
                var aggregate = CreateNewAggregate(message.Id);

                Assert.IsTrue(await repository.AddAsync(aggregate));
                Assert.IsFalse(await repository.AddAsync(aggregate));
            });

            var outputStream = await _processor.HandleAsync(command);

            Assert.AreEqual(1, outputStream.Count);
            AssertEvent<AggregateRootSpyCreatedEvent>(outputStream, 0, @event =>
            {
                Assert.AreEqual(command.Id, @event.Id);
                Assert.AreEqual(1, @event.Version);
            });

            repository.AssertRequiresFlushCountIs(1);
            repository.AssertFlushCountIs(1);

            var changeSet = repository.GetChangeSet(0);

            Assert.AreEqual(1, changeSet.AggregatesToInsert.Count);
            Assert.AreEqual(0, changeSet.AggregatesToUpdate.Count);
            Assert.AreEqual(0, changeSet.AggregatesToDelete.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public async Task AddAsync_Throws_IfInRemovedState()
        {
            var command = DoSomethingCommand.WithRandomValues();
            var aggregate = CreateExistingAggregate(command.Id);
            var repository = CreateMemoryRepository(aggregate);

            _processor.Implement<DoSomethingCommandHandler>().AsAsync<DoSomethingCommand>(async (message, context) =>
            {
                Assert.IsTrue(await repository.RemoveByIdAsync(message.Id));

                await repository.AddAsync(aggregate);
            });

            try
            {
                await _processor.HandleAsync(command);
            }
            catch (BadRequestException exception)
            {
                var duplicateKeyException = exception.InnerException as DuplicateKeyException;

                Assert.IsNotNull(duplicateKeyException);
                Assert.AreEqual(command.Id, duplicateKeyException.AggregateId);
                Assert.AreEqual($"Cannot add aggregate of type 'AggregateRootSpy' to the repository because another aggregate with Id '{command.Id}' is already present in the data store.", duplicateKeyException.Message);
                throw;
            }
        }

        #endregion

        #region [====== RemoveByIdAsync ======]

        [TestMethod]
        public async Task RemoveByIdAsync_ReturnsFalse_IfInNullState_And_AggregateDoesNotExistInRepository()
        {
            var command = DoSomethingCommand.WithRandomValues();
            var repository = CreateMemoryRepository();

            _processor.Implement<DoSomethingCommandHandler>().AsAsync<DoSomethingCommand>(async (message, context) =>
            {
                Assert.IsFalse(await repository.RemoveByIdAsync(message.Id));                
            });

            var outputStream = await _processor.HandleAsync(command);

            Assert.AreEqual(0, outputStream.Count);            

            repository.AssertRequiresFlushCountIs(0);
            repository.AssertFlushCountIs(0);            
        }

        [TestMethod]
        public async Task RemoveByIdAsync_ReturnsTrue_IfInNullState_And_AggregateExistsInRepository()
        {
            var command = DoSomethingCommand.WithRandomValues();
            var aggregate = CreateExistingAggregate(command.Id);
            var repository = CreateMemoryRepository(aggregate);

            _processor.Implement<DoSomethingCommandHandler>().AsAsync<DoSomethingCommand>(async (message, context) =>
            {
                Assert.IsTrue(await repository.RemoveByIdAsync(message.Id));
            });

            var outputStream = await _processor.HandleAsync(command);

            Assert.AreEqual(1, outputStream.Count);
            AssertEvent<AggregateRootSpyRemovedEvent>(outputStream, 0, @event =>
            {
                Assert.AreEqual(command.Id, @event.Id);
                Assert.AreEqual(2, @event.Version);
            });

            repository.AssertRequiresFlushCountIs(1);
            repository.AssertFlushCountIs(1);

            var changeSet = repository.GetChangeSet(0);

            Assert.AreEqual(0, changeSet.AggregatesToInsert.Count);
            Assert.AreEqual(1, changeSet.AggregatesToUpdate.Count);
            Assert.AreEqual(1, changeSet.AggregatesToDelete.Count);
        }

        [TestMethod]
        public async Task RemoveByIdAsync_ReturnsTrue_IfInUnmodifiedState()
        {
            var command = DoSomethingCommand.WithRandomValues();
            var aggregate = CreateExistingAggregate(command.Id);
            var repository = CreateMemoryRepository(aggregate);

            _processor.Implement<DoSomethingCommandHandler>().AsAsync<DoSomethingCommand>(async (message, context) =>
            {
                IAggregateRoot<Guid> retrievedAggregate = await repository.GetByIdAsync(message.Id);

                Assert.IsTrue(await repository.RemoveByIdAsync(retrievedAggregate.Id));
            });

            var outputStream = await _processor.HandleAsync(command);

            Assert.AreEqual(1, outputStream.Count);
            AssertEvent<AggregateRootSpyRemovedEvent>(outputStream, 0, @event =>
            {
                Assert.AreEqual(command.Id, @event.Id);
                Assert.AreEqual(2, @event.Version);
            });

            repository.AssertRequiresFlushCountIs(1);
            repository.AssertFlushCountIs(1);

            var changeSet = repository.GetChangeSet(0);

            Assert.AreEqual(0, changeSet.AggregatesToInsert.Count);
            Assert.AreEqual(1, changeSet.AggregatesToUpdate.Count);
            Assert.AreEqual(1, changeSet.AggregatesToDelete.Count);
        }

        [TestMethod]
        public async Task RemoveByIdAsync_ReturnsTrue_IfInAddedState()
        {
            var command = DoSomethingCommand.WithRandomValues();            
            var repository = CreateMemoryRepository();

            _processor.Implement<DoSomethingCommandHandler>().AsAsync<DoSomethingCommand>(async (message, context) =>
            {
                Assert.IsTrue(await repository.AddAsync(CreateNewAggregate(message.Id)));
                Assert.IsTrue(await repository.RemoveByIdAsync(message.Id));
            });

            var outputStream = await _processor.HandleAsync(command);

            Assert.AreEqual(2, outputStream.Count);
            AssertEvent<AggregateRootSpyCreatedEvent>(outputStream, 0, @event =>
            {
                Assert.AreEqual(command.Id, @event.Id);
                Assert.AreEqual(1, @event.Version);
            });
            AssertEvent<AggregateRootSpyRemovedEvent>(outputStream, 1, @event =>
            {
                Assert.AreEqual(command.Id, @event.Id);
                Assert.AreEqual(2, @event.Version);
            });

            repository.AssertRequiresFlushCountIs(1);
            repository.AssertFlushCountIs(1);

            var changeSet = repository.GetChangeSet(0);

            Assert.AreEqual(1, changeSet.AggregatesToInsert.Count);
            Assert.AreEqual(0, changeSet.AggregatesToUpdate.Count);
            Assert.AreEqual(1, changeSet.AggregatesToDelete.Count);
        }

        [TestMethod]
        public async Task RemoveByIdAsync_ReturnsTrue_IfInModifiedState()
        {
            var command = DoSomethingCommand.WithRandomValues();
            var aggregate = CreateExistingAggregate(command.Id);
            var repository = CreateMemoryRepository(aggregate);

            _processor.Implement<DoSomethingCommandHandler>().AsAsync<DoSomethingCommand>(async (message, context) =>
            {
                var retrievedAggregate = await repository.GetByIdAsync(message.Id);

                retrievedAggregate.ChangeValue(message.NewValue);

                Assert.IsTrue(await repository.RemoveByIdAsync(message.Id));
            });

            var outputStream = await _processor.HandleAsync(command);

            Assert.AreEqual(2, outputStream.Count);
            AssertEvent<ValueChangedEvent>(outputStream, 0, @event =>
            {
                Assert.AreEqual(command.Id, @event.Id);
                Assert.AreEqual(2, @event.Version);
                Assert.AreEqual(command.NewValue, @event.NewValue);
            });
            AssertEvent<AggregateRootSpyRemovedEvent>(outputStream, 1, @event =>
            {
                Assert.AreEqual(command.Id, @event.Id);
                Assert.AreEqual(3, @event.Version);
            });

            repository.AssertRequiresFlushCountIs(1);
            repository.AssertFlushCountIs(1);

            var changeSet = repository.GetChangeSet(0);

            Assert.AreEqual(0, changeSet.AggregatesToInsert.Count);
            Assert.AreEqual(1, changeSet.AggregatesToUpdate.Count);
            Assert.AreEqual(1, changeSet.AggregatesToDelete.Count);
        }

        [TestMethod]
        public async Task RemoveByIdAsync_ReturnsFalse_IfInRemovedState()
        {
            var command = DoSomethingCommand.WithRandomValues();
            var aggregate = CreateExistingAggregate(command.Id);
            var repository = CreateMemoryRepository(aggregate);

            _processor.Implement<DoSomethingCommandHandler>().AsAsync<DoSomethingCommand>(async (message, context) =>
            {                
                Assert.IsTrue(await repository.RemoveByIdAsync(message.Id));
                Assert.IsFalse(await repository.RemoveByIdAsync(message.Id));
            });

            var outputStream = await _processor.HandleAsync(command);

            Assert.AreEqual(1, outputStream.Count);            
            AssertEvent<AggregateRootSpyRemovedEvent>(outputStream, 0, @event =>
            {
                Assert.AreEqual(command.Id, @event.Id);
                Assert.AreEqual(2, @event.Version);
            });

            repository.AssertRequiresFlushCountIs(1);
            repository.AssertFlushCountIs(1);

            var changeSet = repository.GetChangeSet(0);

            Assert.AreEqual(0, changeSet.AggregatesToInsert.Count);
            Assert.AreEqual(1, changeSet.AggregatesToUpdate.Count);
            Assert.AreEqual(1, changeSet.AggregatesToDelete.Count);
        }

        #endregion

        private static AggregateRootSpy CreateNewAggregate(Guid id) =>
            new AggregateRootWithoutEventHandlers(id);

        private static AggregateRootSpy CreateExistingAggregate(Guid id) =>
            new AggregateRootWithoutEventHandlers(new SnapshotMock(false) { Id = id, Version = 1 });

        private static MemoryRepositorySpy<Guid, AggregateRootSpy> CreateMemoryRepository(params AggregateRootSpy[] existingAggregates) =>
            new MemoryRepositorySpy<Guid, AggregateRootSpy>(existingAggregates);

        private static void AssertEvent<TEvent>(IMessageStream stream, int index, Action<TEvent> assert) =>
            assert.Invoke((TEvent) stream[index]);
    }
}
