using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kingo.MicroServices.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class MicroProcessorTest
    {
        #region [====== MessageHandlers ======]

        [MessageHandler(ServiceLifetime.Transient, MicroProcessorOperationTypes.All)]
        private sealed class ObjectHandler : IMessageHandler<object>
        {
            public Task HandleAsync(object message, MessageHandlerContext context) =>
                Task.CompletedTask;
        }

        private sealed class StringHandler : IMessageHandler<string>
        {
            public Task HandleAsync(string message, MessageHandlerContext context) =>
                Task.CompletedTask;
        }

        private sealed class ObjectAndStringHandler : IMessageHandler<object>, IMessageHandler<string>
        {
            public Task HandleAsync(object message, MessageHandlerContext context) =>
                Task.CompletedTask;

            public Task HandleAsync(string message, MessageHandlerContext context) =>
                Task.CompletedTask;
        }

        [MessageHandler(ServiceLifetime.Transient, MicroProcessorOperationTypes.OutputMessageHandler)]
        private sealed class MessageHandlerExceptionThrower : IMessageHandler<object>
        {
            public Task HandleAsync(object message, MessageHandlerContext context) =>
                throw new MessageHandlerException();
        }                   

        #endregion

        #region [====== Messages ======]

        private sealed class SomeCommand { }

        #endregion

        private readonly MessageHandlerFactoryBuilder _messageHandlers;        
        private readonly MicroServiceBusStub _serviceBus;

        public MicroProcessorTest()
        {
            _messageHandlers = new SimpleMessageHandlerFactoryBuilder();            
            _serviceBus = new MicroServiceBusStub();
        }

        #region [====== HandleAsync - MessageHandler Invocation ======]        

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandleAsync_Throws_IfMessageIsNull()
        {
            await CreateProcessor().HandleAsync<object>(null);
        }

        [TestMethod]
        public async Task HandleAsync_ReturnsZero_IfNoMessageHandlersWereInvoked()
        {
            Assert.AreEqual(0, await CreateProcessor().HandleAsync(new object()));
        }

        [TestMethod]
        public async Task HandleAsync_ReturnsOne_IfMessageHandlerIsExplicitlySpecified()
        {
            Assert.AreEqual(1, await CreateProcessor().HandleAsync(new object(), (message, context) => {}));
        }

        [TestMethod]
        public async Task HandleAsync_ReturnsExpectedInvocationCount_IfMultipleHandlersAreResolvedForMessage()
        {
            _messageHandlers.RegisterType<ObjectHandler>();
            _messageHandlers.RegisterType<StringHandler>();

            Assert.AreEqual(2, await CreateProcessor().HandleAsync(string.Empty));
        }

        [TestMethod]
        public async Task HandleAsync_ReturnsExpectedInvocationCount_IfOneHandlerTriggersAnotherHandler()
        {
            _messageHandlers.RegisterType<ObjectHandler>();

            Assert.AreEqual(2, await CreateProcessor().HandleAsync(string.Empty, (message, context) =>
            {
                context.EventBus.Publish(message);
            }));
        }

        [TestMethod]
        public async Task HandleAsync_ReturnsExpectedInvocationCount_IfOneHandlerIsInvokedTwiceForSameMessage()
        {
            _messageHandlers.Register(new ObjectAndStringHandler());
            _messageHandlers.RegisterType<ObjectAndStringHandler>();

            Assert.AreEqual(4, await CreateProcessor().HandleAsync(string.Empty));
        }

        [TestMethod]
        public async Task HandleAsync_HandlesMessageInsideDependencyContext()
        {
            Assert.IsNull(DependencyContext.Current);

            Assert.AreEqual(1, await CreateProcessor().HandleAsync(new object(), (message, context) =>
            {
                Assert.IsNotNull(DependencyContext.Current);
            }));

            Assert.IsNull(DependencyContext.Current);
        }

        #endregion        

        #region [====== HandleAsync - Exception Handling ======]

        [TestMethod]        
        public async Task HandleAsync_ThrowsBadRequestException_IfInputMessageHandlerThrowsMessageHandlerException_And_InputMessageIsCommand()
        {
            var exception = await Assert.ThrowsExceptionAsync<BadRequestException>(() => CreateProcessor().HandleAsync(new SomeCommand(), (command, context) =>
            {
                throw new MessageHandlerException();
            }));

            Assert.IsInstanceOfType(exception.InnerException, typeof(MessageHandlerException));
        }

        [TestMethod]
        public async Task HandleAsync_ThrowsBadRequestException_IfInputMessageHandlerThrowsBadRequestException_And_InputMessageIsCommand()
        {
            var exception = await Assert.ThrowsExceptionAsync<BadRequestException>(() => CreateProcessor().HandleAsync(new SomeCommand(), (command, context) =>
            {
                throw new BadRequestException();
            }));

            Assert.IsNull(exception.InnerException);
        }

        [TestMethod]
        public async Task HandleAsync_ThrowsInternalServerErrorException_IfInputMessageHandlerThrowsInvalidOperationException_And_InputMessageIsCommand()
        {
            var exception = await Assert.ThrowsExceptionAsync<InternalServerErrorException>(() => CreateProcessor().HandleAsync(new SomeCommand(), (command, context) =>
            {
                throw new InvalidOperationException();
            }));

            Assert.IsInstanceOfType(exception.InnerException, typeof(InvalidOperationException));
        }

        [TestMethod]
        public async Task HandleAsync_ThrowsInternalServerErrorException_IfOutputMessageHandlerThrowsMessageHandlerException_And_InputMessageIsCommand()
        {
            _messageHandlers.RegisterType<MessageHandlerExceptionThrower>();

            var exception = await Assert.ThrowsExceptionAsync<InternalServerErrorException>(() => CreateProcessor().HandleAsync(new SomeCommand(), (command, context) =>
            {
                context.EventBus.Publish(command);
            }));

            Assert.IsInstanceOfType(exception.InnerException, typeof(MessageHandlerException));
        }

        [TestMethod]
        public async Task HandleAsync_ThrowsInternalServerErrorException_IfInputMessageHandlerThrowsBadRequestException_And_InputMessageIsCommand()
        {
            var exception = await Assert.ThrowsExceptionAsync<InternalServerErrorException>(() => CreateProcessor().HandleAsync(new object(), (message, context) =>
            {
                throw new BadRequestException();
            }));

            Assert.IsInstanceOfType(exception.InnerException, typeof(BadRequestException));
        }

        [TestMethod]
        public async Task HandleAsync_ThrowsInternalServerErrorException_IfInputMessageHandlerThrowsMessageHandlerException_And_InputMessageIsEvent()
        {            
            var exception = await Assert.ThrowsExceptionAsync<InternalServerErrorException>(() => CreateProcessor().HandleAsync(new object(), (message, context) =>
            {
                throw new MessageHandlerException();
            }));

            Assert.IsInstanceOfType(exception.InnerException, typeof(MessageHandlerException));
        }

        [TestMethod]
        public async Task HandleAsync_ThrowsInternalServerErrorException_IfInputMessageHandlerThrowsMicroProcessorException_And_InputMessageIsEvent()
        {
            var exception = await Assert.ThrowsExceptionAsync<InternalServerErrorException>(() => CreateProcessor().HandleAsync(new object(), (message, context) =>
            {
                throw new InternalServerErrorException();
            }));

            Assert.IsNull(exception.InnerException);
        }

        [TestMethod]
        public async Task HandleAsync_ThrowsInternalServerErrorException_IfInputMessageHandlerThrowsInvalidOperationException_And_InputMessageIsEvent()
        {
            var exception = await Assert.ThrowsExceptionAsync<InternalServerErrorException>(() => CreateProcessor().HandleAsync(new object(), (message, context) =>
            {
                throw new InvalidOperationException();
            }));

            Assert.IsInstanceOfType(exception.InnerException, typeof(InvalidOperationException));
        }

        [TestMethod]
        public async Task HandleAsync_ThrowsInternalServerErrorException_IfOutputMessageHandlerThrowsMessageHandlerException_And_InputMessageIsEvent()
        {
            _messageHandlers.RegisterType<MessageHandlerExceptionThrower>();

            var exception = await Assert.ThrowsExceptionAsync<InternalServerErrorException>(() => CreateProcessor().HandleAsync(new object(), (message, context) =>
            {
                context.EventBus.Publish(message);
            }));

            Assert.IsInstanceOfType(exception.InnerException, typeof(MessageHandlerException));
        }

        [TestMethod]
        public async Task HandleAsync_ThrowsOperationCancelledException_IfInputMessageHandlerCancelsTheOperation()
        {
            var tokenSource = new CancellationTokenSource();            

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() => CreateProcessor().HandleAsync(new object(), (message, context) =>
            {                
                tokenSource.Cancel();
            }, tokenSource.Token));
        }       

        #endregion

        #region [====== HandleAsync - UnitOfWork ======]

        [TestMethod]
        public async Task HandleAsync_DoesNotFlushResourceManager_IfResourceManagerDoesNotNeedToBeFlushed()
        {
            var resourceManager = new UnitOfWorkResourceManagerSpy(false);

            await CreateProcessor().HandleAsync(new object(), async (message, context) =>
            {
                await context.UnitOfWork.EnlistAsync(resourceManager);
            });

            resourceManager.AssertRequiresFlushCountIs(1);
            resourceManager.AssertFlushCountIs(0);
        }

        [TestMethod]
        public async Task HandleAsync_DoesNotFlushResourceManager_IfResourceManagerNeedsToBeFlushed_But_SomeExceptionIsThrownByMessageHandler()
        {
            var resourceManager = new UnitOfWorkResourceManagerSpy(true);

            await Assert.ThrowsExceptionAsync<InternalServerErrorException>(() => CreateProcessor().HandleAsync(new object(), async (message, context) =>
            {
                await context.UnitOfWork.EnlistAsync(resourceManager);
                throw new InvalidOperationException();
            }));

            resourceManager.AssertRequiresFlushCountIs(0);
            resourceManager.AssertFlushCountIs(0);
        }

        [TestMethod]
        public async Task HandleAsync_FlushesResourceManager_IfResourceManagerNeedsToBeFlushed()
        {
            var resourceManager = new UnitOfWorkResourceManagerSpy(true);

            await CreateProcessor().HandleAsync(new object(), async (message, context) =>
            {
                await context.UnitOfWork.EnlistAsync(resourceManager);
            });

            resourceManager.AssertRequiresFlushCountIs(1);
            resourceManager.AssertFlushCountIs(1);
        }

        [TestMethod]
        public async Task HandleAsync_FlushesResourceManagerOnlyOnce_IfResourceManagerNeedsToBeFlushed_But_IsEnlistedMoreThanOnce()
        {
            var resourceManager = new UnitOfWorkResourceManagerSpy(true);

            await CreateProcessor().HandleAsync(new object(), async (message, context) =>
            {
                await context.UnitOfWork.EnlistAsync(resourceManager);
                await context.UnitOfWork.EnlistAsync(resourceManager);
            });

            resourceManager.AssertRequiresFlushCountIs(1);
            resourceManager.AssertFlushCountIs(1);
        }

        [TestMethod]
        public async Task HandleAsync_FlushesOnlyThoseResourceManagersThatNeedToBeFlushed_IfMultipleResourceManagersAreEnlisted()
        {
            var resourceManagerA = new UnitOfWorkResourceManagerSpy(true);
            var resourceManagerB = new UnitOfWorkResourceManagerSpy(false);

            await CreateProcessor().HandleAsync(new object(), async (message, context) =>
            {
                await context.UnitOfWork.EnlistAsync(resourceManagerA);
                await context.UnitOfWork.EnlistAsync(resourceManagerB);
            });

            resourceManagerA.AssertRequiresFlushCountIs(1);
            resourceManagerA.AssertFlushCountIs(1);

            resourceManagerB.AssertRequiresFlushCountIs(1);
            resourceManagerB.AssertFlushCountIs(0);
        }

        [TestMethod]
        public async Task HandleAsync_FlushesResourceManagerOnlyOnce_IfUnitOfWorkIsFlushedManually_And_ResourceManagerIsNotEnlistedAfterThat()
        {
            var resourceManager = new UnitOfWorkResourceManagerSpy(true);

            await CreateProcessor().HandleAsync(new object(), async (message, context) =>
            {
                await context.UnitOfWork.EnlistAsync(resourceManager);
                await context.UnitOfWork.FlushAsync();
            });

            resourceManager.AssertRequiresFlushCountIs(1);
            resourceManager.AssertFlushCountIs(1);
        }

        [TestMethod]
        public async Task HandleAsync_FlushesResourceManagerTwice_IfUnitOfWorkIsFlushedManually_And_ResourceManagerIsReenlistedAfterThat()
        {
            var resourceManager = new UnitOfWorkResourceManagerSpy(true);

            await CreateProcessor().HandleAsync(new object(), async (message, context) =>
            {
                await context.UnitOfWork.EnlistAsync(resourceManager);
                await context.UnitOfWork.FlushAsync();
                await context.UnitOfWork.EnlistAsync(resourceManager);
            });

            resourceManager.AssertRequiresFlushCountIs(2);
            resourceManager.AssertFlushCountIs(2);
        }

        [TestMethod]
        public async Task HandleAsync_FlushesBothResourceManagersInParrallel_IfResourceManagersHaveDifferentResourceIds()
        {            
            var resourceManagerA = new UnitOfWorkResourceManagerSpy(true, new object());                       
            var resourceManagerB = new UnitOfWorkResourceManagerSpy(true, new object());            

            await CreateProcessor().HandleAsync(new object(), async (message, context) =>
            {
                await context.UnitOfWork.EnlistAsync(resourceManagerA);
                await context.UnitOfWork.EnlistAsync(resourceManagerB);                
            });

            resourceManagerA.AssertRequiresFlushCountIs(1);
            resourceManagerA.AssertFlushCountIs(1);

            resourceManagerB.AssertRequiresFlushCountIs(1);
            resourceManagerB.AssertFlushCountIs(1);            
        }

        [TestMethod]
        public async Task HandleAsync_ThrowsConflictException_IfFlushAsyncThrowsConcurrencyException_And_MessageIsCommand()
        {
            var resourceManager = new UnitOfWorkResourceManagerSpy(true, new ConcurrencyException());

            var exception = await Assert.ThrowsExceptionAsync<ConflictException>(() => CreateProcessor().HandleAsync(new SomeCommand(), async (command, context) =>
            {
                await context.UnitOfWork.EnlistAsync(resourceManager);                
            }));

            resourceManager.AssertRequiresFlushCountIs(1);
            resourceManager.AssertFlushCountIs(1);

            Assert.IsInstanceOfType(exception.InnerException, typeof(ConcurrencyException));
        }

        [TestMethod]
        public async Task HandleAsync_ThrowsInternalServerErrorException_IfFlushAsyncThrowsConcurrencyException_And_MessageIsEvent()
        {
            var resourceManager = new UnitOfWorkResourceManagerSpy(true, new ConcurrencyException());

            var exception = await Assert.ThrowsExceptionAsync<InternalServerErrorException>(() => CreateProcessor().HandleAsync(new object(), async (message, context) =>
            {
                await context.UnitOfWork.EnlistAsync(resourceManager);
            }));

            resourceManager.AssertRequiresFlushCountIs(1);
            resourceManager.AssertFlushCountIs(1);

            Assert.IsInstanceOfType(exception.InnerException, typeof(ConcurrencyException));
        }

        #endregion

        #region [====== HandleAsync - Event Publication ======]

        [TestMethod]
        public async Task HandleAsync_PublishesNoEvents_IfNoHandlerIsInvoked()
        {
            await CreateProcessor().HandleAsync(new object());

            Assert.AreEqual(0, _serviceBus.Count);
        }

        [TestMethod]
        public async Task HandleAsync_PublishesExpectedEvent_IfExplicitHandlerPublishesEvent()
        {
            var @event = new object();

            await CreateProcessor().HandleAsync(new object(), (message, context) =>
            {
                context.EventBus.Publish(@event);
            });

            Assert.AreEqual(1, _serviceBus.Count);
            Assert.AreSame(@event, _serviceBus[0]);
        }

        [TestMethod]
        public async Task HandleAsync_PublishesExpectedEvents_IfSeveralHandlersPublishEvents()
        {
            var eventA = string.Empty;
            var eventB = new object();

            _messageHandlers.Register<string>((message, context) =>
            {
                context.EventBus.Publish(eventB);
            });

            await CreateProcessor().HandleAsync(new object(), (message, context) =>
            {
                context.EventBus.Publish(eventA);
            });

            Assert.AreEqual(2, _serviceBus.Count);
            Assert.AreSame(eventA, _serviceBus[0]);
            Assert.AreSame(eventB, _serviceBus[1]);
        }

        [TestMethod]
        public async Task HandleAsync_PublishesNoEvents_IfSeveralHandlersPublishEvents_But_ExceptionIsThrown()
        {
            var eventA = string.Empty;
            var eventB = new object();

            _messageHandlers.Register<string>((message, context) =>
            {
                context.EventBus.Publish(eventB);
            });

            await Assert.ThrowsExceptionAsync<InternalServerErrorException>(() => CreateProcessor().HandleAsync(new object(), (message, context) =>
            {
                context.EventBus.Publish(eventA);
                throw new InvalidOperationException();
            }));

            Assert.AreEqual(0, _serviceBus.Count);            
        }

        #endregion

        #region [====== HandleAsync - Operation ======]

        [TestMethod]
        public async Task HandleAsync_IsInvokedWithinExpectedContext_IfInputMessageHandlerIsInvoked()
        {
            var command = new object();

            await CreateProcessor().HandleAsync(command, (message, context) =>
            {
                Assert.AreSame(command, message);
                Assert.AreSame(command, context.Operation.Message);
                Assert.AreEqual(MicroProcessorOperationTypes.InputMessageHandler, context.Operation.Type);
                Assert.AreEqual(1, context.Operation.StackTrace().Count());
            });
        }

        [TestMethod]
        public async Task HandleAsync_IsInvokedWithinExpectedContext_IfOutputMessageHandlerIsInvoked_Depth1()
        {
            var @event = new object();

            _messageHandlers.Register<object>((message, context) =>
            {
                Assert.AreSame(@event, message);
                Assert.AreSame(@event, context.Operation.Message);
                Assert.AreEqual(MicroProcessorOperationTypes.OutputMessageHandler, context.Operation.Type);
                Assert.AreEqual(2, context.Operation.StackTrace().Count());
            });

            await CreateProcessor().HandleAsync(new object(), (message, context) =>
            {
                context.EventBus.Publish(@event);
            });
        }

        [TestMethod]
        public async Task HandleAsync_IsInvokedWithinExpectedContext_IfOutputMessageHandlerIsInvoked_Depth2()
        {
            var @event = string.Empty;

            _messageHandlers.Register<int>((message, context) =>
            {
                context.EventBus.Publish(@event);
            });

            _messageHandlers.Register<string>((message, context) =>
            {
                Assert.AreSame(@event, message);
                Assert.AreSame(@event, context.Operation.Message);
                Assert.AreEqual(MicroProcessorOperationTypes.OutputMessageHandler, context.Operation.Type);
                Assert.AreEqual(3, context.Operation.StackTrace().Count());
            });

            Assert.AreEqual(3, await CreateProcessor().HandleAsync(new object(), (message, context) =>
            {
                context.EventBus.Publish(DateTimeOffset.UtcNow.Second);
            }));
        }

        #endregion        

        #region [====== ExecuteAsync1 ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ExecuteAsync1_Throws_IfQueryIsNull()
        {
            await CreateProcessor().ExecuteAsync<object>(null);
        }

        [TestMethod]
        public async Task ExecuteAsync1_ReturnsNull_IfQueryReturnsNull()
        {
            Assert.IsNull(await CreateProcessor().ExecuteAsync(context => null as object));
        }

        [TestMethod]
        public async Task ExecuteAsync1_ReturnsResponse_IfQueryReturnsResponse()
        {
            var response = new object();

            Assert.AreSame(await CreateProcessor().ExecuteAsync(context => response), response);
        }

        [TestMethod]
        public async Task ExecuteAsync1_ThrowsBadRequestException_IfQueryThrowsBadRequestException()
        {
            var exceptionToThrow = new BadRequestException();
            var exception = await Assert.ThrowsExceptionAsync<BadRequestException>(() => CreateProcessor().ExecuteAsync<object>(context =>
            {
                throw exceptionToThrow;
            }));

            Assert.AreSame(exceptionToThrow, exception);
        }

        [TestMethod]
        public async Task ExecuteAsync1_ThrowsInternalServerErrorException_IfQueryThrowsInternalServerErrorException()
        {
            var exceptionToThrow = new InternalServerErrorException();
            var exception = await Assert.ThrowsExceptionAsync<InternalServerErrorException>(() => CreateProcessor().ExecuteAsync<object>(context =>
            {
                throw exceptionToThrow;
            }));

            Assert.AreSame(exceptionToThrow, exception);
        }

        [TestMethod]
        public async Task ExecuteAsync1_ThrowsInternalServerErrorException_IfQueryThrowsInvalidOperationException()
        {
            var exceptionToThrow = new InvalidOperationException();
            var exception = await Assert.ThrowsExceptionAsync<InternalServerErrorException>(() => CreateProcessor().ExecuteAsync<object>(context =>
            {
                throw exceptionToThrow;
            }));

            Assert.AreSame(exceptionToThrow, exception.InnerException);
        }

        [TestMethod]
        public async Task ExecuteAsync1_IsInvokedWithinExpectedContext()
        {            
            await CreateProcessor().ExecuteAsync(context =>
            {                
                Assert.IsNull(context.Operation.Message);
                Assert.AreEqual(MicroProcessorOperationTypes.Query, context.Operation.Type);
                Assert.AreEqual(1, context.Operation.StackTrace().Count());
                return new object();
            });
        }

        [TestMethod]
        public async Task ExecuteAsync1_ExecutesQueryInsideDependencyContext()
        {
            Assert.IsNull(DependencyContext.Current);

            await CreateProcessor().ExecuteAsync(context =>
            {
                Assert.IsNotNull(DependencyContext.Current);
                return new object();
            });

            Assert.IsNull(DependencyContext.Current);
        }

        #endregion

        #region [====== ExecuteAsync2 ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ExecuteAsync2_Throws_IfQueryIsNull()
        {
            await CreateProcessor().ExecuteAsync<object, object>(new object(), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ExecuteAsync2_Throws_IfMessageIsNull()
        {
            await CreateProcessor().ExecuteAsync<object, object>(null, (message, context) => null);
        }

        [TestMethod]
        public async Task ExecuteAsync2_ReturnsNull_IfQueryReturnsNull()
        {
            Assert.IsNull(await CreateProcessor().ExecuteAsync(new object(), (message, context) => null as object));
        }

        [TestMethod]
        public async Task ExecuteAsync2_ReturnsResponse_IfQueryReturnsResponse()
        {
            var response = new object();

            Assert.AreSame(await CreateProcessor().ExecuteAsync(new object(), (message, context) => response), response);
        }

        [TestMethod]
        public async Task ExecuteAsync2_ThrowsBadRequestException_IfQueryThrowsBadRequestException()
        {
            var exceptionToThrow = new BadRequestException();
            var exception = await Assert.ThrowsExceptionAsync<BadRequestException>(() => CreateProcessor().ExecuteAsync<object, object>(new object(), (message, context) =>
            {
                throw exceptionToThrow;
            }));

            Assert.AreSame(exceptionToThrow, exception);
        }

        [TestMethod]
        public async Task ExecuteAsync2_ThrowsInternalServerErrorException_IfQueryThrowsInternalServerErrorException()
        {
            var exceptionToThrow = new InternalServerErrorException();
            var exception = await Assert.ThrowsExceptionAsync<InternalServerErrorException>(() => CreateProcessor().ExecuteAsync<object, object>(new object(), (message, context) =>
            {
                throw exceptionToThrow;
            }));

            Assert.AreSame(exceptionToThrow, exception);
        }

        [TestMethod]
        public async Task ExecuteAsync2_ThrowsInternalServerErrorException_IfQueryThrowsInvalidOperationException()
        {
            var exceptionToThrow = new InvalidOperationException();
            var exception = await Assert.ThrowsExceptionAsync<InternalServerErrorException>(() => CreateProcessor().ExecuteAsync<object, object>(new object(), (message, context) =>
            {
                throw exceptionToThrow;
            }));

            Assert.AreSame(exceptionToThrow, exception.InnerException);
        }

        [TestMethod]
        public async Task ExecuteAsync2_IsInvokedWithinExpectedContext()
        {
            var request = new object();

            await CreateProcessor().ExecuteAsync(request, (message, context) =>
            {
                Assert.AreSame(request, message);
                Assert.AreSame(request, context.Operation.Message);
                Assert.AreEqual(MicroProcessorOperationTypes.Query, context.Operation.Type);
                Assert.AreEqual(1, context.Operation.StackTrace().Count());
                return new object();
            });
        }

        [TestMethod]
        public async Task ExecuteAsync2_ExecutesQueryInsideDependencyContext()
        {
            Assert.IsNull(DependencyContext.Current);

            await CreateProcessor().ExecuteAsync(new object(), (request, context) =>
            {
                Assert.IsNotNull(DependencyContext.Current);
                return new object();
            });

            Assert.IsNull(DependencyContext.Current);
        }

        #endregion

        private MicroProcessor CreateProcessor() =>
            new MicroProcessor(_serviceBus, _messageHandlers.Build(), null);
    }
}
