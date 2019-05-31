using System;
using System.Threading.Tasks;
using Kingo.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Domain
{
    [TestClass]
    public sealed class MemoryRepositoryBehaviorTest
    {        
        private readonly MicroProcessor _processor;        

        public MemoryRepositoryBehaviorTest()
        {            
            _processor = new MicroProcessor();
        }        

        #region [====== GetByIdOrNullAsync ======]

        [TestMethod]
        public async Task GetByIdOrNullAsync_ReturnsNull_IfAggregateIsNotFound()
        {
            var command = AddValueCommand.Random();
            var repository = await CreateRepositoryAsync();

            var result = await _processor.HandleAsync(command, async (message, context) =>
            {
                Assert.IsNull(await repository.GetByIdOrNullAsync(message.NumberId));
            });

            Assert.AreEqual(0, result.Events.Count);            
        }

        [TestMethod]
        public async Task GetByIdOrNullAsync_ReturnsAggregate_IfAggregateIsFound()
        {
            var command = AddValueCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);

            var result = await _processor.HandleAsync(command, async (message, context) =>
            {
                Assert.IsNotNull(await repository.GetByIdOrNullAsync(message.NumberId));
            });

            Assert.AreEqual(0, result.Events.Count);
        }

        #endregion

        #region [====== GetByIdAsync ======]        

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public async Task GetByIdAsync_Throws_IfInNullState_And_AggregateDoesNotExistInDataStore()
        {
            var command = AddValueCommand.Random();
            var repository = await CreateRepositoryAsync();              

            try
            {
                await _processor.HandleAsync(command, async (message, context) =>
                {
                    await repository.GetByIdAsync(message.NumberId);
                });
            }
            catch (BadRequestException exception)
            {
                AssertNotFoundException(exception, command.NumberId);
                throw;
            }
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsExpectedInstance_IfInNullState_And_AggregateExistsInDataStore()
        {
            var command = AddValueCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);            

            var result = await _processor.HandleAsync(command, async (message, context) =>
            {
                (await repository.GetByIdAsync(message.NumberId)).Add(message.Value);
            });

            AssertValueAddedEvent(result.Events, command);
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsExpectedInstance_IfInUnmodifiedState()
        {
            var command = AddValueCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);

            var result = await _processor.HandleAsync(command, async (message, context) =>
            {
                var number1 = await repository.GetByIdAsync(message.NumberId);
                var number2 = await repository.GetByIdAsync(message.NumberId);

                Assert.AreSame(number1, number2);
            });

            Assert.AreEqual(0, result.Events.Count);
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsExpectedInstance_IfInAddedState()
        {
            var command = CreateNumberCommand.Random();
            var repository = await CreateRepositoryAsync();

            var result = await _processor.HandleAsync(command, async (message, context) =>
            {
                var number1 = CreateNewNumber(message, context);

                Assert.IsTrue(await repository.AddAsync(number1));

                var number2 = await repository.GetByIdAsync(message.NumberId);

                Assert.AreSame(number1, number2);
            });

            AssertNumberCreatedEvent(result.Events, command);
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsExpectedInstance_IfInModifiedState()
        {
            var command = AddValueCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);

            var result = await _processor.HandleAsync(command, async (message, context) =>
            {
                var number1 = await repository.GetByIdAsync(message.NumberId);

                number1.Add(message.Value);

                var number2 = await repository.GetByIdAsync(message.NumberId);

                Assert.AreSame(number1, number2);
            });

            result.Events.AssertCountIs(1);
            result.Events.AssertElement<ValueAddedEvent>(0, @event =>
            {
                Assert.AreEqual(command.NumberId, @event.NumberId);
                Assert.AreEqual(2, @event.NumberVersion);
                Assert.AreEqual(command.Value, @event.Value);
            });
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public async Task GetByIdAsync_Throws_IfInRemovedState()
        {
            var command = AddValueCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);            

            try
            {
                await _processor.HandleAsync(command, async (message, context) =>
                {
                    Assert.IsTrue(await repository.RemoveByIdAsync(message.NumberId));

                    await repository.GetByIdAsync(message.NumberId);
                });
            }
            catch (BadRequestException exception)
            {
                AssertNotFoundException(exception, command.NumberId);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public async Task GetByIdAsync_Throws_IfAggregateHasBeenSoftDeleted()
        {
            var command = DeleteNumberCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);

            await _processor.HandleAsync(command, async (message, context) =>
            {
                var number = await repository.GetByIdAsync(command.NumberId);
                number.EnableSoftDelete = true;

                Assert.IsTrue(await repository.RemoveAsync(number));
            });

            try
            {
                await _processor.HandleAsync(command, async (message, context) =>
                {
                    await repository.GetByIdAsync(message.NumberId);
                });
            }
            catch (BadRequestException exception)
            {
                AssertNotFoundException(exception, command.NumberId);
                throw;
            }            
        }

        private static void AssertNotFoundException(Exception exception, object aggregateId)
        {
            var aggregateNotFoundException = exception.InnerException as AggregateNotFoundException;

            Assert.IsNotNull(aggregateNotFoundException);
            Assert.AreEqual(aggregateId, aggregateNotFoundException.AggregateId);
            Assert.AreEqual($"Aggregate of type 'Number' with Id '{aggregateId}' was not found.", aggregateNotFoundException.Message);
        }

        #endregion

        #region [====== AddAsync ======]

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task AddAsync_Throws_IfAggregateIsNull()
        {
            var repository = await CreateRepositoryAsync();            

            try
            {
                await _processor.HandleAsync(CreateNumberCommand.Random(), async (message, context) =>
                {
                    await repository.AddAsync(null);
                });
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
            var command = CreateNumberCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);                        

            try
            {
                await _processor.HandleAsync(command, async (message, context) =>
                {
                    await repository.AddAsync(CreateNewNumber(message, context));
                });
            }
            catch (BadRequestException exception)
            {
                AssertDuplicateKeyException(exception, command.NumberId);
                throw;
            }
        }

        [TestMethod]
        public async Task AddAsync_ReturnsTrue_IfInNullState_And_AggregateDoesNotYetExistInDataStore()
        {
            var command = CreateNumberCommand.Random();
            var repository = await CreateRepositoryAsync();

            var result = await _processor.HandleAsync(command, async (message, context) =>
            {
                Assert.IsTrue(await repository.AddAsync(CreateNewNumber(message, context)));
            });
            
            AssertNumberCreatedEvent(result.Events, command);
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public async Task AddAsync_Throws_IfInUnmodifiedState_And_NewAggregateIsAdded()
        {
            var command = CreateNumberCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);            

            try
            {
                await _processor.HandleAsync(command, async (message, context) =>
                {
                    Assert.IsNotNull(await repository.GetByIdAsync(message.NumberId));
                    await repository.AddAsync(CreateNewNumber(message, context));
                });
            }
            catch (BadRequestException exception)
            {
                AssertDuplicateKeyException(exception, command.NumberId);
                throw;
            }
        }

        [TestMethod]
        public async Task AddAsync_ReturnsFalse_IfInUnmodifiedState_And_SameAggregatesIsAdded()
        {
            var command = CreateNumberCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);

            var result = await _processor.HandleAsync(command, async (message, context) =>
            {
                var number = await repository.GetByIdAsync(message.NumberId);

                Assert.IsFalse(await repository.AddAsync(number));
            });

            result.Events.AssertCountIs(0);
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public async Task AddAsync_Throws_IfInModifiedState_And_NewAggregateIsAdded()
        {
            var command = AddValueCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);            

            try
            {
                await _processor.HandleAsync(command, async (message, context) =>
                {
                    var number = await repository.GetByIdAsync(message.NumberId);

                    number.Add(message.Value);

                    await repository.AddAsync(CreateNewNumber(message.NumberId, message.Value, context.EventBus));
                });
            }
            catch (BadRequestException exception)
            {
                AssertDuplicateKeyException(exception, command.NumberId);
                throw;
            }
        }

        [TestMethod]
        public async Task AddAsync_ReturnsFalse_IfInModifiedState_And_SameAggregatesIsAdded()
        {
            var command = AddValueCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);

            var result = await _processor.HandleAsync(command, async (message, context) =>
            {
                var number = await repository.GetByIdAsync(message.NumberId);

                number.Add(message.Value);

                Assert.IsFalse(await repository.AddAsync(number));
            });

            AssertValueAddedEvent(result.Events, command);
        }        

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public async Task AddAsync_Throws_IfInAddedState_And_NewAggregateIsAdded()
        {
            var command = CreateNumberCommand.Random();
            var repository = await CreateRepositoryAsync();            

            try
            {
                await _processor.HandleAsync(command, async (message, context) =>
                {
                    Assert.IsTrue(await repository.AddAsync(CreateNewNumber(message, context)));

                    await repository.AddAsync(CreateNewNumber(message, context));
                });
            }
            catch (BadRequestException exception)
            {
                AssertDuplicateKeyException(exception, command.NumberId);
                throw;
            }
        }

        [TestMethod]
        public async Task AddAsync_ReturnsFalse_IfInAddedState_And_SameAggregateIsAdded()
        {
            var command = CreateNumberCommand.Random();
            var repository = await CreateRepositoryAsync();

            var result = await _processor.HandleAsync(command, async (message, context) =>
            {
                var number = CreateNewNumber(message, context);

                Assert.IsTrue(await repository.AddAsync(number));
                Assert.IsFalse(await repository.AddAsync(number));
            });

            AssertNumberCreatedEvent(result.Events, command);
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public async Task AddAsync_Throws_IfInRemovedState()
        {
            var command = CreateNumberCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);           

            try
            {
                await _processor.HandleAsync(command, async (message, context) =>
                {
                    Assert.IsTrue(await repository.RemoveByIdAsync(message.NumberId));

                    await repository.AddAsync(CreateNewNumber(message, context));
                });
            }
            catch (BadRequestException exception)
            {
                AssertDuplicateKeyException(exception, command.NumberId);
                throw;
            }
        }

        private static void AssertNumberCreatedEvent(MessageStream stream, CreateNumberCommand command)
        {
            stream.AssertCountIs(1);
            stream.AssertElement<NumberCreatedEvent>(0, @event =>
            {
                Assert.AreEqual(command.NumberId, @event.NumberId);
                Assert.AreEqual(1, @event.NumberVersion);
                Assert.AreEqual(command.Value, @event.Value);
            });
        }

        private static void AssertValueAddedEvent(MessageStream stream, AddValueCommand command)
        {
            stream.AssertCountIs(1);
            stream.AssertElement<ValueAddedEvent>(0, @event =>
            {
                Assert.AreEqual(command.NumberId, @event.NumberId);
                Assert.AreEqual(2, @event.NumberVersion);
                Assert.AreEqual(command.Value, @event.Value);
            });
        }

        private static void AssertDuplicateKeyException(Exception exception, object aggregateId)
        {
            var duplicateKeyException = exception.InnerException as DuplicateKeyException;

            Assert.IsNotNull(duplicateKeyException);
            Assert.AreEqual(aggregateId, duplicateKeyException.AggregateId);
            Assert.AreEqual($"Cannot add aggregate of type 'Number' to the repository because another aggregate with Id '{aggregateId}' is already present in the data store.", duplicateKeyException.Message);
        }

        #endregion

        #region [====== RemoveAsync ======]

        [TestMethod]
        public async Task RemoveAsync_ReturnsFalse_IfAggregateIsNull()
        {
            var command = DeleteNumberCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);

            var result = await _processor.HandleAsync(command, async (message, context) =>
            {
                Assert.IsFalse(await repository.RemoveAsync(null));
            });

            result.Events.AssertCountIs(0);
        }

        [TestMethod]
        public async Task RemoveAsync_ReturnsFalse_IfInNullState_And_AggregateIsNotFound()
        {
            var command = DeleteNumberCommand.Random();
            var repository = await CreateRepositoryAsync();

            var result = await _processor.HandleAsync(command, async (message, context) =>
            {
                Assert.IsFalse(await repository.RemoveAsync(CreateNewNumber(command.NumberId)));
            });

            result.Events.AssertCountIs(0);
        }

        [TestMethod]
        public async Task RemoveAsync_ReturnsFalse_IfInNullState_And_AggregateIsFound()
        {
            var command = DeleteNumberCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);

            var result = await _processor.HandleAsync(command, async (message, context) =>
            {
                Assert.IsFalse(await repository.RemoveAsync(CreateNewNumber(command.NumberId)));
            });

            result.Events.AssertCountIs(0);
        }

        [TestMethod]
        public async Task RemoveAsync_ReturnsFalse_IfInUnmodifiedState_And_AggregateIsNotTheRetrievedInstance()
        {
            var command = DeleteNumberCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);

            var result = await _processor.HandleAsync(command, async (message, context) =>
            {
                Assert.IsNotNull(await repository.GetByIdAsync(message.NumberId));
                Assert.IsFalse(await repository.RemoveAsync(CreateNewNumber(command.NumberId)));
            });

            result.Events.AssertCountIs(0);
        }

        [TestMethod]
        public async Task RemoveAsync_ReturnsTrue_IfInUnmodifiedState_And_AggregateIsTheRetrievedInstance()
        {
            var command = DeleteNumberCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);

            var result = await _processor.HandleAsync(command, async (message, context) =>
            {
                var number = await repository.GetByIdAsync(message.NumberId);

                Assert.IsTrue(await repository.RemoveAsync(number));
            });

            AssertNumberDeletedEvent(result.Events, command);
        }

        [TestMethod]
        public async Task RemoveAsync_ReturnsFalse_IfInModifiedState_And_AggregateIsNotTheRetrievedInstance()
        {
            var command = AddValueCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);

            var result = await _processor.HandleAsync(command, async (message, context) =>
            {
                var number = await repository.GetByIdAsync(message.NumberId);
                number.Add(message.Value);
                Assert.IsFalse(await repository.RemoveAsync(CreateNewNumber(command.NumberId)));
            });

            AssertValueAddedEvent(result.Events, command);
        }

        [TestMethod]
        public async Task RemoveAsync_ReturnsTrue_IfInModifiedState_And_AggregateIsTheRetrievedInstance()
        {
            var command = AddValueCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);

            var result = await _processor.HandleAsync(command, async (message, context) =>
            {
                var number = await repository.GetByIdAsync(message.NumberId);
                number.Add(message.Value);
                Assert.IsTrue(await repository.RemoveAsync(number));
            });

            result.Events.AssertCountIs(2);
            result.Events.AssertElement<ValueAddedEvent>(0, @event =>
            {
                Assert.AreEqual(command.NumberId, @event.NumberId);
                Assert.AreEqual(2, @event.NumberVersion);
                Assert.AreEqual(command.Value, @event.Value);
            });
            result.Events.AssertElement<NumberDeletedEvent>(1, @event =>
            {
                Assert.AreEqual(command.NumberId, @event.NumberId);
                Assert.AreEqual(3, @event.NumberVersion);
            });
        }

        [TestMethod]
        public async Task RemoveAsync_ReturnsFalse_IfInRemovedState_And_AggregateIsNotTheRetrievedInstance()
        {
            var command = DeleteNumberCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);

            var result = await _processor.HandleAsync(command, async (message, context) =>
            {
                var number = await repository.GetByIdAsync(message.NumberId);
                
                Assert.IsTrue(await repository.RemoveAsync(number));
                Assert.IsFalse(await repository.RemoveAsync(CreateNewNumber(command.NumberId)));
            });

            AssertNumberDeletedEvent(result.Events, command);
        }

        [TestMethod]
        public async Task RemoveAsync_ReturnsFalse_IfInRemovedState_And_AggregateIsTheRetrievedInstance()
        {
            var command = DeleteNumberCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);

            var result = await _processor.HandleAsync(command, async (message, context) =>
            {
                var number = await repository.GetByIdAsync(message.NumberId);

                Assert.IsTrue(await repository.RemoveAsync(number));
                Assert.IsFalse(await repository.RemoveAsync(number));
            });

            AssertNumberDeletedEvent(result.Events, command);
        }

        #endregion

        #region [====== RemoveByIdAsync ======]

        [TestMethod]
        public async Task RemoveByIdAsync_ReturnsFalse_IfInNullState_And_AggregateDoesNotExistInRepository()
        {
            var command = DeleteNumberCommand.Random();
            var repository = await CreateRepositoryAsync();

            var result = await _processor.HandleAsync(command, async (message, context) =>
            {
                Assert.IsFalse(await repository.RemoveByIdAsync(message.NumberId));
            });

            result.Events.AssertCountIs(0);
        }

        [TestMethod]
        public async Task RemoveByIdAsync_ReturnsTrue_IfInNullState_And_AggregateExistsInRepository()
        {
            var command = DeleteNumberCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);

            var result = await _processor.HandleAsync(command, async (message, context) =>
            {
                Assert.IsTrue(await repository.RemoveByIdAsync(message.NumberId));
            });

            AssertNumberDeletedEvent(result.Events, command);
        }

        [TestMethod]
        public async Task RemoveByIdAsync_ReturnsTrue_IfInUnmodifiedState()
        {
            var command = DeleteNumberCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);

            var result = await _processor.HandleAsync(command, async (message, context) =>
            {
                Assert.IsNotNull(await repository.GetByIdAsync(message.NumberId));
                Assert.IsTrue(await repository.RemoveByIdAsync(message.NumberId));
            });

            AssertNumberDeletedEvent(result.Events, command);
        }

        [TestMethod]
        public async Task RemoveByIdAsync_ReturnsTrue_IfInAddedState()
        {
            var command = CreateNumberCommand.Random();
            var repository = await CreateRepositoryAsync();

            var result = await _processor.HandleAsync(command, async (message, context) =>
            {
                Assert.IsTrue(await repository.AddAsync(CreateNewNumber(message, context)));
                Assert.IsTrue(await repository.RemoveByIdAsync(message.NumberId));
            });

            result.Events.AssertCountIs(2);
            result.Events.AssertElement<NumberCreatedEvent>(0, @event =>
            {
                Assert.AreEqual(command.NumberId, @event.NumberId);
                Assert.AreEqual(1, @event.NumberVersion);
                Assert.AreEqual(command.Value, @event.Value);
            });
            result.Events.AssertElement<NumberDeletedEvent>(1, @event =>
            {
                Assert.AreEqual(command.NumberId, @event.NumberId);
                Assert.AreEqual(2, @event.NumberVersion);                
            });
        }

        [TestMethod]
        public async Task RemoveByIdAsync_ReturnsTrue_IfInModifiedState()
        {
            var command = AddValueCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);

            var result = await _processor.HandleAsync(command, async (message, context) =>
            {
                var number = await repository.GetByIdAsync(message.NumberId);

                number.Add(message.Value);

                Assert.IsTrue(await repository.RemoveByIdAsync(message.NumberId));
            });

            result.Events.AssertCountIs(2);
            result.Events.AssertElement<ValueAddedEvent>(0, @event =>
            {
                Assert.AreEqual(command.NumberId, @event.NumberId);
                Assert.AreEqual(2, @event.NumberVersion);
                Assert.AreEqual(command.Value, @event.Value);
            });
            result.Events.AssertElement<NumberDeletedEvent>(1, @event =>
            {
                Assert.AreEqual(command.NumberId, @event.NumberId);
                Assert.AreEqual(3, @event.NumberVersion);
            });
        }

        [TestMethod]
        public async Task RemoveByIdAsync_ReturnsFalse_IfInRemovedState()
        {
            var command = DeleteNumberCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);

            var result = await _processor.HandleAsync(command, async (message, context) =>
            {
                Assert.IsTrue(await repository.RemoveByIdAsync(message.NumberId));
                Assert.IsFalse(await repository.RemoveByIdAsync(message.NumberId));
            });

            AssertNumberDeletedEvent(result.Events, command);
        }

        private void AssertNumberDeletedEvent(MessageStream stream, DeleteNumberCommand command)
        {
            stream.AssertCountIs(1);
            stream.AssertElement<NumberDeletedEvent>(0, @event =>
            {
                Assert.AreEqual(command.NumberId, @event.NumberId);
                Assert.AreEqual(2, @event.NumberVersion);
            });
        }

        #endregion

        private static async Task<IRepository<Guid, Number>> CreateRepositoryAsync(params Guid[] numberIds)
        {
            var repository = new MemoryRepository<Guid, int, NumberSnapshot, Number>(SerializationStrategy.UseSnapshots());

            foreach (var numberId in numberIds)
            {
                await repository.AddAsync(CreateNewNumber(numberId));
            }
            return repository;
        }

        private static Number CreateNewNumber(CreateNumberCommand command, MessageHandlerContext context) =>
            CreateNewNumber(command.NumberId, command.Value, context.EventBus);

        private static Number CreateNewNumber(Guid numberId, int value = 0, IEventBus eventBus = null) =>
            NumberUsingSnapshots.CreateNumber(numberId, value, eventBus);
    }
}
