using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Domain
{
    [TestClass]
    public sealed class MemoryRepositoryBehaviorTest
    {
        private readonly MicroServiceBusStub _serviceBus;
        private readonly MicroProcessor _processor;        

        public MemoryRepositoryBehaviorTest()
        {
            _serviceBus = new MicroServiceBusStub();
            _processor = new MicroProcessor(null, null, _serviceBus);
        }

        #region [====== Constructor ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Throws_IfSerializationStrategyIsNull()
        {
            new MemoryRepository<Guid, int, Number>(null);
        }

        #endregion

        #region [====== GetByIdOrNullAsync ======]

        [TestMethod]
        public async Task GetByIdOrNullAsync_ReturnsNull_IfAggregateIsNotFound()
        {
            var command = AddValueCommand.Random();
            var repository = await CreateRepositoryAsync();

            await _processor.HandleAsync(command, async (message, context) =>
            {
                Assert.IsNull(await repository.GetByIdOrNullAsync(message.NumberId));
            });

            _serviceBus.AssertEventCountIs(0);
        }

        [TestMethod]
        public async Task GetByIdOrNullAsync_ReturnsAggregate_IfAggregateIsFound()
        {
            var command = AddValueCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);

            await _processor.HandleAsync(command, async (message, context) =>
            {
                Assert.IsNotNull(await repository.GetByIdOrNullAsync(message.NumberId));
            });

            _serviceBus.AssertEventCountIs(0);
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

            await _processor.HandleAsync(command, async (message, context) =>
            {
                (await repository.GetByIdAsync(message.NumberId)).Add(message.Value);
            });

            AssertValueAddedEvent(command);
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsExpectedInstance_IfInUnmodifiedState()
        {
            var command = AddValueCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);

            await _processor.HandleAsync(command, async (message, context) =>
            {
                var number1 = await repository.GetByIdAsync(message.NumberId);
                var number2 = await repository.GetByIdAsync(message.NumberId);

                Assert.AreSame(number1, number2);
            });

            _serviceBus.AssertEventCountIs(0);
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsExpectedInstance_IfInAddedState()
        {
            var command = CreateNumberCommand.Random();
            var repository = await CreateRepositoryAsync();

            await _processor.HandleAsync(command, async (message, context) =>
            {
                var number1 = CreateNewNumber(message, context);

                Assert.IsTrue(await repository.AddAsync(number1));

                var number2 = await repository.GetByIdAsync(message.NumberId);

                Assert.AreSame(number1, number2);
            });

            AssertNumberCreatedEvent(command);
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsExpectedInstance_IfInModifiedState()
        {
            var command = AddValueCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);

            await _processor.HandleAsync(command, async (message, context) =>
            {
                var number1 = await repository.GetByIdAsync(message.NumberId);

                number1.Add(message.Value);

                var number2 = await repository.GetByIdAsync(message.NumberId);

                Assert.AreSame(number1, number2);
            });

            _serviceBus.AssertEventCountIs(1);
            _serviceBus.AssertEvent<ValueAddedEvent>(0, @event =>
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

            await _processor.HandleAsync(command, async (message, context) =>
            {
                Assert.IsTrue(await repository.AddAsync(CreateNewNumber(message, context)));
            });
            
            AssertNumberCreatedEvent(command);
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

            await _processor.HandleAsync(command, async (message, context) =>
            {
                var number = await repository.GetByIdAsync(message.NumberId);

                Assert.IsFalse(await repository.AddAsync(number));
            });

            _serviceBus.AssertEventCountIs(0);
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

            await _processor.HandleAsync(command, async (message, context) =>
            {
                var number = await repository.GetByIdAsync(message.NumberId);

                number.Add(message.Value);

                Assert.IsFalse(await repository.AddAsync(number));
            });

            AssertValueAddedEvent(command);
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

            await _processor.HandleAsync(command, async (message, context) =>
            {
                var number = CreateNewNumber(message, context);

                Assert.IsTrue(await repository.AddAsync(number));
                Assert.IsFalse(await repository.AddAsync(number));
            });

            AssertNumberCreatedEvent(command);
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

        private void AssertNumberCreatedEvent(CreateNumberCommand command)
        {
            _serviceBus.AssertEventCountIs(1);
            _serviceBus.AssertEvent<NumberCreatedEvent>(0, @event =>
            {
                Assert.AreEqual(command.NumberId, @event.NumberId);
                Assert.AreEqual(1, @event.NumberVersion);
                Assert.AreEqual(command.Value, @event.Value);
            });
        }

        private void AssertValueAddedEvent(AddValueCommand command)
        {
            _serviceBus.AssertEventCountIs(1);
            _serviceBus.AssertEvent<ValueAddedEvent>(0, @event =>
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

            await _processor.HandleAsync(command, async (message, context) =>
            {
                Assert.IsFalse(await repository.RemoveAsync(null));
            });

            _serviceBus.AssertEventCountIs(0);
        }

        [TestMethod]
        public async Task RemoveAsync_ReturnsFalse_IfInNullState_And_AggregateIsNotFound()
        {
            var command = DeleteNumberCommand.Random();
            var repository = await CreateRepositoryAsync();

            await _processor.HandleAsync(command, async (message, context) =>
            {
                Assert.IsFalse(await repository.RemoveAsync(CreateNewNumber(command.NumberId)));
            });

            _serviceBus.AssertEventCountIs(0);
        }

        [TestMethod]
        public async Task RemoveAsync_ReturnsFalse_IfInNullState_And_AggregateIsFound()
        {
            var command = DeleteNumberCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);

            await _processor.HandleAsync(command, async (message, context) =>
            {
                Assert.IsFalse(await repository.RemoveAsync(CreateNewNumber(command.NumberId)));
            });

            _serviceBus.AssertEventCountIs(0);
        }

        [TestMethod]
        public async Task RemoveAsync_ReturnsFalse_IfInUnmodifiedState_And_AggregateIsNotTheRetrievedInstance()
        {
            var command = DeleteNumberCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);

            await _processor.HandleAsync(command, async (message, context) =>
            {
                Assert.IsNotNull(await repository.GetByIdAsync(message.NumberId));
                Assert.IsFalse(await repository.RemoveAsync(CreateNewNumber(command.NumberId)));
            });

            _serviceBus.AssertEventCountIs(0);
        }

        [TestMethod]
        public async Task RemoveAsync_ReturnsTrue_IfInUnmodifiedState_And_AggregateIsTheRetrievedInstance()
        {
            var command = DeleteNumberCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);

            await _processor.HandleAsync(command, async (message, context) =>
            {
                var number = await repository.GetByIdAsync(message.NumberId);

                Assert.IsTrue(await repository.RemoveAsync(number));
            });

            AssertNumberDeletedEvent(command);
        }

        [TestMethod]
        public async Task RemoveAsync_ReturnsFalse_IfInModifiedState_And_AggregateIsNotTheRetrievedInstance()
        {
            var command = AddValueCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);

            await _processor.HandleAsync(command, async (message, context) =>
            {
                var number = await repository.GetByIdAsync(message.NumberId);
                number.Add(message.Value);
                Assert.IsFalse(await repository.RemoveAsync(CreateNewNumber(command.NumberId)));
            });

            AssertValueAddedEvent(command);
        }

        [TestMethod]
        public async Task RemoveAsync_ReturnsTrue_IfInModifiedState_And_AggregateIsTheRetrievedInstance()
        {
            var command = AddValueCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);

            await _processor.HandleAsync(command, async (message, context) =>
            {
                var number = await repository.GetByIdAsync(message.NumberId);
                number.Add(message.Value);
                Assert.IsTrue(await repository.RemoveAsync(number));
            });

            _serviceBus.AssertEventCountIs(2);
            _serviceBus.AssertEvent<ValueAddedEvent>(0, @event =>
            {
                Assert.AreEqual(command.NumberId, @event.NumberId);
                Assert.AreEqual(2, @event.NumberVersion);
                Assert.AreEqual(command.Value, @event.Value);
            });
            _serviceBus.AssertEvent<NumberDeletedEvent>(1, @event =>
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

            await _processor.HandleAsync(command, async (message, context) =>
            {
                var number = await repository.GetByIdAsync(message.NumberId);
                
                Assert.IsTrue(await repository.RemoveAsync(number));
                Assert.IsFalse(await repository.RemoveAsync(CreateNewNumber(command.NumberId)));
            });

            AssertNumberDeletedEvent(command);
        }

        [TestMethod]
        public async Task RemoveAsync_ReturnsFalse_IfInRemovedState_And_AggregateIsTheRetrievedInstance()
        {
            var command = DeleteNumberCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);

            await _processor.HandleAsync(command, async (message, context) =>
            {
                var number = await repository.GetByIdAsync(message.NumberId);

                Assert.IsTrue(await repository.RemoveAsync(number));
                Assert.IsFalse(await repository.RemoveAsync(number));
            });

            AssertNumberDeletedEvent(command);
        }

        #endregion

        #region [====== RemoveByIdAsync ======]

        [TestMethod]
        public async Task RemoveByIdAsync_ReturnsFalse_IfInNullState_And_AggregateDoesNotExistInRepository()
        {
            var command = DeleteNumberCommand.Random();
            var repository = await CreateRepositoryAsync();

            await _processor.HandleAsync(command, async (message, context) =>
            {
                Assert.IsFalse(await repository.RemoveByIdAsync(message.NumberId));
            });

            _serviceBus.AssertEventCountIs(0);
        }

        [TestMethod]
        public async Task RemoveByIdAsync_ReturnsTrue_IfInNullState_And_AggregateExistsInRepository()
        {
            var command = DeleteNumberCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);

            await _processor.HandleAsync(command, async (message, context) =>
            {
                Assert.IsTrue(await repository.RemoveByIdAsync(message.NumberId));
            });

            AssertNumberDeletedEvent(command);
        }

        [TestMethod]
        public async Task RemoveByIdAsync_ReturnsTrue_IfInUnmodifiedState()
        {
            var command = DeleteNumberCommand.Random();
            var repository = await CreateRepositoryAsync(command.NumberId);

            await _processor.HandleAsync(command, async (message, context) =>
            {
                Assert.IsNotNull(await repository.GetByIdAsync(message.NumberId));
                Assert.IsTrue(await repository.RemoveByIdAsync(message.NumberId));
            });

            AssertNumberDeletedEvent(command);
        }

        [TestMethod]
        public async Task RemoveByIdAsync_ReturnsTrue_IfInAddedState()
        {
            var command = CreateNumberCommand.Random();
            var repository = await CreateRepositoryAsync();

            await _processor.HandleAsync(command, async (message, context) =>
            {
                Assert.IsTrue(await repository.AddAsync(CreateNewNumber(message, context)));
                Assert.IsTrue(await repository.RemoveByIdAsync(message.NumberId));
            });

            _serviceBus.AssertEventCountIs(2);
            _serviceBus.AssertEvent<NumberCreatedEvent>(0, @event =>
            {
                Assert.AreEqual(command.NumberId, @event.NumberId);
                Assert.AreEqual(1, @event.NumberVersion);
                Assert.AreEqual(command.Value, @event.Value);
            });
            _serviceBus.AssertEvent<NumberDeletedEvent>(1, @event =>
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

            await _processor.HandleAsync(command, async (message, context) =>
            {
                var number = await repository.GetByIdAsync(message.NumberId);

                number.Add(message.Value);

                Assert.IsTrue(await repository.RemoveByIdAsync(message.NumberId));
            });

            _serviceBus.AssertEventCountIs(2);
            _serviceBus.AssertEvent<ValueAddedEvent>(0, @event =>
            {
                Assert.AreEqual(command.NumberId, @event.NumberId);
                Assert.AreEqual(2, @event.NumberVersion);
                Assert.AreEqual(command.Value, @event.Value);
            });
            _serviceBus.AssertEvent<NumberDeletedEvent>(1, @event =>
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

            await _processor.HandleAsync(command, async (message, context) =>
            {
                Assert.IsTrue(await repository.RemoveByIdAsync(message.NumberId));
                Assert.IsFalse(await repository.RemoveByIdAsync(message.NumberId));
            });

            AssertNumberDeletedEvent(command);
        }

        private void AssertNumberDeletedEvent(DeleteNumberCommand command)
        {
            _serviceBus.AssertEventCountIs(1);
            _serviceBus.AssertEvent<NumberDeletedEvent>(0, @event =>
            {
                Assert.AreEqual(command.NumberId, @event.NumberId);
                Assert.AreEqual(2, @event.NumberVersion);
            });
        }

        #endregion

        private static async Task<IRepository<Guid, Number>> CreateRepositoryAsync(params Guid[] numberIds)
        {
            var repository = new MemoryRepository<Guid, int, Number>(SerializationStrategy.UseSnapshots());

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
