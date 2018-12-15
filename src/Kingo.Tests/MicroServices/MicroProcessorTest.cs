using System;
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

        [MessageHandler(ServiceLifetime.Transient, MicroProcessorOperationTypes.OutputMessage)]
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
        private readonly MicroProcessorPipelineFactoryBuilder _pipeline;

        public MicroProcessorTest()
        {
            _messageHandlers = new SimpleMessageHandlerFactoryBuilder();
            _pipeline = new MicroProcessorPipelineFactoryBuilder();
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
            _messageHandlers.Register<ObjectHandler>();
            _messageHandlers.Register<StringHandler>();

            Assert.AreEqual(2, await CreateProcessor().HandleAsync(string.Empty));
        }

        [TestMethod]
        public async Task HandleAsync_ReturnsExpectedInvocationCount_IfOneHandlerTriggersAnotherHandler()
        {
            _messageHandlers.Register<ObjectHandler>();

            Assert.AreEqual(2, await CreateProcessor().HandleAsync(string.Empty, (message, context) =>
            {
                context.EventBus.Publish(message);
            }));
        }

        [TestMethod]
        public async Task HandleAsync_ReturnsExpectedInvocationCount_IfOneHandlerIsInvokedTwiceForSameMessage()
        {
            _messageHandlers.Register<ObjectAndStringHandler>();

            Assert.AreEqual(2, await CreateProcessor().HandleAsync(string.Empty));
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
            _messageHandlers.Register<MessageHandlerExceptionThrower>();

            var exception = await Assert.ThrowsExceptionAsync<InternalServerErrorException>(() => CreateProcessor().HandleAsync(new SomeCommand(), (command, context) =>
            {
                context.EventBus.Publish(command);
            }));

            Assert.IsInstanceOfType(exception.InnerException, typeof(MessageHandlerException));
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
            _messageHandlers.Register<MessageHandlerExceptionThrower>();

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

        #region [====== ExecuteAsync_1 - Basic Behavior ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ExecuteAsync_1_Throws_IfQueryIsNull()
        {
            await CreateProcessor().ExecuteAsync<object>(null);
        }

        #endregion

        #region [====== ExecuteAsync_2 - Basic Behavior ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ExecuteAsync_2_Throws_IfQueryIsNull()
        {
            await CreateProcessor().ExecuteAsync<object, object>(new object(), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ExecuteAsync_2_Throws_IfMessageIsNull()
        {
            await CreateProcessor().ExecuteAsync<object, object>(null, (message, context) => null);
        }

        #endregion

        private MicroProcessor CreateProcessor() =>
            new MicroProcessor(_messageHandlers.Build(), _pipeline.Build(), null);
    }
}
