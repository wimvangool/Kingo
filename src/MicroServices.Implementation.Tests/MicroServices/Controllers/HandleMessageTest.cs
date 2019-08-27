using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Controllers
{
    [TestClass]
    public abstract class HandleMessageTest : MicroProcessorTest<MicroProcessor>
    {        
        #region [====== Null Parameters ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandleMessageAsync_Throws_IfMessageHandlerIsNull()
        {
            await HandleMessageAsync(null as IMessageHandler<object>, new object());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandleMessageAsync_Throws_IfMessageHandlerActionIsNull()
        {
            await HandleMessageAsync(null as Action<object, MessageHandlerOperationContext>, new object());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandleMessageAsync_Throws_IfMessageHandlerFuncIsNull()
        {
            await HandleMessageAsync(null as Func<object, MessageHandlerOperationContext, Task>, new object());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task HandleMessageAsync_Throws_IfCommandIsNull()
        {
            await HandleMessageAsync<object>((message, context) => { }, null);
        }

        #endregion

        #region [====== Return Value ======]

        [TestMethod]
        public async Task HandleMessageAsync_ReturnsNoEvents_IfMessageHandlerPublishesNoEvents()
        {
            var result = await HandleMessageAsync((message, context) => { }, new object());

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.MessageHandlerCount);
            Assert.AreEqual(0, result.Events.Count);
        }

        [TestMethod]
        public async Task HandleMessageAsync_ReturnsExpectedEvent_IfMessageHandlerPublishesOneEvent()
        {
            var command = new object();
            var result = await HandleMessageAsync((message, context) =>
            {
                context.EventBus.Publish(message);
            }, command);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.MessageHandlerCount);
            Assert.AreEqual(1, result.Events.Count);
            Assert.AreSame(command, result.Events[0].Instance);
        }

        [TestMethod]
        public async Task HandleMessageAsync_ReturnsExpectedEvents_IfMessageHandlerPublishesOneEvent_And_EventHandlerPublishesAnotherEvent()
        {
            var command = new object();
            var eventA = string.Empty;
            var eventB = new object();

            // Handles Event A.
            ProcessorBuilder.Components.AddMessageHandler<string>((message, context) =>
            {
                context.EventBus.Publish(eventB);
            });

            var result = await HandleMessageAsync((message, context) =>
            {
                context.EventBus.Publish(eventA);
            }, command);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.MessageHandlerCount);
            Assert.AreEqual(2, result.Events.Count);
            Assert.AreSame(eventA, result.Events[0].Instance);
            Assert.AreSame(eventB, result.Events[1].Instance);
        }

        [TestMethod]
        public async Task HandleMessageAsync_ReturnsExpectedEvents_IfMessageHandlerPublishesOneEvent_And_EventHandlersPublishMoreEvents()
        {
            var command = new object();
            var eventA = string.Empty;
            var eventB = DateTimeOffset.UtcNow.Second;
            var eventC = new object();
            var eventD = new object();

            // Handles Event A.
            ProcessorBuilder.Components.AddMessageHandler<string>((message, context) =>
            {
                context.EventBus.Publish(eventB);
            });

            // Handles Event B.
            ProcessorBuilder.Components.AddMessageHandler<int>((message, context) =>
            {
                context.EventBus.Publish(eventC);
            });

            // Handles Event B.
            ProcessorBuilder.Components.AddMessageHandler<int>((message, context) =>
            {
                context.EventBus.Publish(eventD);
            });

            var result = await HandleMessageAsync((message, context) =>
            {
                context.EventBus.Publish(eventA);
            }, command);

            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.MessageHandlerCount);
            Assert.AreEqual(4, result.Events.Count);
            Assert.AreSame(eventA, result.Events[0].Instance);
            Assert.AreEqual(eventB, result.Events[1].Instance);
            Assert.AreSame(eventC, result.Events[2].Instance);
            Assert.AreSame(eventD, result.Events[3].Instance);
        }

        #endregion

        #region [====== Context Verification ======]

        [TestMethod]
        public async Task HandleMessageAsync_ProducesExpectedStackTrace_IfMessageHandlerPublishesOneEvent_And_EventHandlersPublishMoreEvents()
        {
            var command = new object();
            var eventA = string.Empty;
            var eventB = DateTimeOffset.UtcNow.Second;
            var eventC = new object();
            var eventD = new object();

            // Handles Event A.
            ProcessorBuilder.Components.AddMessageHandler<string>((message, context) =>
            {
                AssertStackTrace(context.StackTrace, 2, message, MessageKind.Event);
                AssertEventBus(context.EventBus, 0);

                context.EventBus.Publish(eventB);

                AssertEventBus(context.EventBus, 1);
            });

            // Handles Event B.
            ProcessorBuilder.Components.AddMessageHandler<int>((message, context) =>
            {
                AssertStackTrace(context.StackTrace, 3, message, MessageKind.Event);
                AssertEventBus(context.EventBus, 0);

                context.EventBus.Publish(eventC);

                AssertEventBus(context.EventBus, 1);
            });

            // Handles Event B.
            ProcessorBuilder.Components.AddMessageHandler<int>((message, context) =>
            {
                AssertStackTrace(context.StackTrace, 3, message, MessageKind.Event);
                AssertEventBus(context.EventBus, 0);

                context.EventBus.Publish(eventD);

                AssertEventBus(context.EventBus, 1);
            });

            await HandleMessageAsync((message, context) =>
            {
                AssertStackTrace(context.StackTrace, 1, message, MessageKind);
                AssertEventBus(context.EventBus, 0);

                context.EventBus.Publish(eventA);

                AssertEventBus(context.EventBus, 1);
            }, command);
        }

        private static void AssertStackTrace(IAsyncMethodOperationStackTrace stackTrace, int count, object message, MessageKind messageKind)
        {
            Assert.AreEqual(count, stackTrace.Count);

            if (count == 1)
            {
                AssertOperation(stackTrace.CurrentOperation, MicroProcessorOperationKinds.RootOperation, message, messageKind);
            }
            else
            {
                AssertOperation(stackTrace.CurrentOperation, MicroProcessorOperationKinds.BranchOperation, message, messageKind);
            }
        }

        private static void AssertOperation(IMicroProcessorOperation operation, MicroProcessorOperationKinds operationKind, object message, MessageKind messageKind)
        {
            Assert.IsNotNull(operation);
            Assert.AreEqual(MicroProcessorOperationType.MessageHandlerOperation, operation.Type);
            Assert.AreEqual(message, operation.Message.Instance);
            Assert.AreEqual(messageKind, operation.Message.Kind);
        }

        private static void AssertEventBus(IReadOnlyCollection<object> eventBus, int count) =>
            Assert.AreEqual(count, eventBus.Count);

        #endregion

        #region [====== UnitOfWork ======]

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task HandleMessageAsync_Throws_IfUnitOfWorkModeIsInvalid()
        {
            ProcessorBuilder.UnitOfWorkMode = (UnitOfWorkMode)(-1);

            await HandleMessageAsync((message, context) => { }, new object());
        }

        [TestMethod]
        public async Task HandleMessageAsync_DoesNotFlushResourceManager_IfUnitOfWorkModeIsDisabled_And_ResourceManagerDoesNotRequireFlush()
        {
            ProcessorBuilder.UnitOfWorkMode = UnitOfWorkMode.Disabled;

            var resourceManager = new UnitOfWorkResourceManagerSpy(false);

            await HandleMessageAsync(async (message, context) =>
            {
                await context.UnitOfWork.EnlistAsync(resourceManager);

                resourceManager.AssertRequiresFlushCountIs(1);
                resourceManager.AssertFlushCountIs(0);
            }, new object());

            resourceManager.AssertRequiresFlushCountIs(1);
            resourceManager.AssertFlushCountIs(0);
        }

        [TestMethod]
        public async Task HandleMessageAsync_FlushesResourceManagerOnEnlistment_IfUnitOfWorkIsDisabled_And_ResourceManagerRequiresFlush()
        {
            ProcessorBuilder.UnitOfWorkMode = UnitOfWorkMode.Disabled;

            var resourceManager = new UnitOfWorkResourceManagerSpy(true);

            await HandleMessageAsync(async (message, context) =>
            {
                await context.UnitOfWork.EnlistAsync(resourceManager);

                resourceManager.AssertRequiresFlushCountIs(1);
                resourceManager.AssertFlushCountIs(1);
            }, new object());

            resourceManager.AssertRequiresFlushCountIs(1);
            resourceManager.AssertFlushCountIs(1);
        }

        [TestMethod]
        public async Task HandleMessageAsync_FlushesResourceManagersSynchronously_IfUnitOfWorkIsSingleThreaded_And_SomeResourceManagersRequireFlush()
        {
            ProcessorBuilder.UnitOfWorkMode = UnitOfWorkMode.SingleThreaded;

            var resourceManagerA = new UnitOfWorkResourceManagerSpy(true);
            var resourceManagerB = new UnitOfWorkResourceManagerSpy(true);
            var resourceManagerC = new UnitOfWorkResourceManagerSpy(false);

            await HandleMessageAsync(async (message, context) =>
            {
                await context.UnitOfWork.EnlistAsync(resourceManagerA);
                await context.UnitOfWork.EnlistAsync(resourceManagerB);
                await context.UnitOfWork.EnlistAsync(resourceManagerC);

                resourceManagerA.AssertRequiresFlushCountIs(0);
                resourceManagerA.AssertFlushCountIs(0);

                resourceManagerB.AssertRequiresFlushCountIs(0);
                resourceManagerB.AssertFlushCountIs(0);

                resourceManagerC.AssertRequiresFlushCountIs(0);
                resourceManagerC.AssertFlushCountIs(0);
            }, new object());

            resourceManagerA.AssertRequiresFlushCountIs(1);
            resourceManagerA.AssertFlushCountIs(1);

            resourceManagerB.AssertRequiresFlushCountIs(1);
            resourceManagerB.AssertFlushCountIs(1);

            resourceManagerC.AssertRequiresFlushCountIs(1);
            resourceManagerC.AssertFlushCountIs(0);
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task HandleMessageAsync_FlushesResourceManagersSynchronously_IfUnitOfWorkIsSingleThreaded_And_SomeResourceManagerThrows()
        {
            ProcessorBuilder.UnitOfWorkMode = UnitOfWorkMode.SingleThreaded;

            var exceptionToThrow = new InternalServerErrorException();
            var resourceManagerA = new UnitOfWorkResourceManagerSpy(true);
            var resourceManagerB = new UnitOfWorkResourceManagerSpy(true, exceptionToThrow);
            var resourceManagerC = new UnitOfWorkResourceManagerSpy(true);

            try
            {
                await HandleMessageAsync(async (message, context) =>
                {
                    await context.UnitOfWork.EnlistAsync(resourceManagerA);
                    await context.UnitOfWork.EnlistAsync(resourceManagerB);
                    await context.UnitOfWork.EnlistAsync(resourceManagerC);

                    resourceManagerA.AssertRequiresFlushCountIs(0);
                    resourceManagerA.AssertFlushCountIs(0);

                    resourceManagerB.AssertRequiresFlushCountIs(0);
                    resourceManagerB.AssertFlushCountIs(0);

                    resourceManagerC.AssertRequiresFlushCountIs(0);
                    resourceManagerC.AssertFlushCountIs(0);
                }, new object());
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(exceptionToThrow, exception);
                throw;
            }
            finally
            {
                // All resource managers required a flush, but since the second resource manager
                // throws an exception, the third resource manager is never flushed.
                resourceManagerA.AssertRequiresFlushCountIs(1);
                resourceManagerA.AssertFlushCountIs(1);

                resourceManagerB.AssertRequiresFlushCountIs(1);
                resourceManagerB.AssertFlushCountIs(1);

                resourceManagerC.AssertRequiresFlushCountIs(0);
                resourceManagerC.AssertFlushCountIs(0);
            }
        }

        [TestMethod]
        public async Task HandleMessageAsync_FlushesResourceManagersAsynchronously_IfUnitOfWorkIsMultiThreaded_And_SomeResourceManagersRequireFlush()
        {
            ProcessorBuilder.UnitOfWorkMode = UnitOfWorkMode.MultiThreaded;

            var resourceManagerA = new UnitOfWorkResourceManagerSpy(true);
            var resourceManagerB = new UnitOfWorkResourceManagerSpy(true);
            var resourceManagerC = new UnitOfWorkResourceManagerSpy(false);

            await HandleMessageAsync(async (message, context) =>
            {
                await context.UnitOfWork.EnlistAsync(resourceManagerA);
                await context.UnitOfWork.EnlistAsync(resourceManagerB);
                await context.UnitOfWork.EnlistAsync(resourceManagerC);

                resourceManagerA.AssertRequiresFlushCountIs(0);
                resourceManagerA.AssertFlushCountIs(0);

                resourceManagerB.AssertRequiresFlushCountIs(0);
                resourceManagerB.AssertFlushCountIs(0);

                resourceManagerC.AssertRequiresFlushCountIs(0);
                resourceManagerC.AssertFlushCountIs(0);
            }, new object());

            resourceManagerA.AssertRequiresFlushCountIs(1);
            resourceManagerA.AssertFlushCountIs(1);

            resourceManagerB.AssertRequiresFlushCountIs(1);
            resourceManagerB.AssertFlushCountIs(1);

            resourceManagerC.AssertRequiresFlushCountIs(1);
            resourceManagerC.AssertFlushCountIs(0);
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task HandleMessageAsync_FlushesResourceManagersAsynchronously_IfUnitOfWorkIsSingleThreaded_And_SomeResourceManagerThrows()
        {
            ProcessorBuilder.UnitOfWorkMode = UnitOfWorkMode.MultiThreaded;

            var exceptionToThrow = new InternalServerErrorException();
            var resourceManagerA = new UnitOfWorkResourceManagerSpy(true);
            var resourceManagerB = new UnitOfWorkResourceManagerSpy(true, exceptionToThrow);
            var resourceManagerC = new UnitOfWorkResourceManagerSpy(true);

            try
            {
                await HandleMessageAsync(async (message, context) =>
                {
                    await context.UnitOfWork.EnlistAsync(resourceManagerA);
                    await context.UnitOfWork.EnlistAsync(resourceManagerB);
                    await context.UnitOfWork.EnlistAsync(resourceManagerC);

                    resourceManagerA.AssertRequiresFlushCountIs(0);
                    resourceManagerA.AssertFlushCountIs(0);

                    resourceManagerB.AssertRequiresFlushCountIs(0);
                    resourceManagerB.AssertFlushCountIs(0);

                    resourceManagerC.AssertRequiresFlushCountIs(0);
                    resourceManagerC.AssertFlushCountIs(0);
                }, new object());
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(exceptionToThrow, exception);
                throw;
            }
            finally
            {
                // All resource managers required a flush, and since all managers
                // manage a different resource and are flushed asynchronously,
                // the third manager is flushed even though the second one throws an exception.
                resourceManagerA.AssertRequiresFlushCountIs(1);
                resourceManagerA.AssertFlushCountIs(1);

                resourceManagerB.AssertRequiresFlushCountIs(1);
                resourceManagerB.AssertFlushCountIs(1);

                resourceManagerC.AssertRequiresFlushCountIs(1);
                resourceManagerC.AssertFlushCountIs(1);
            }
        }

        #endregion

        #region [====== Exception Handling ======]

        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException), AllowDerivedTypes = true)]
        public async Task HandleMessageAsync_ThrowsExpectedException_IfOperationIsCancelledWithTheSpecifiedToken()
        {
            var tokenSource = new CancellationTokenSource();

            try
            {
                await HandleMessageAsync((message, context) =>
                {
                    tokenSource.Cancel();
                }, new object(), tokenSource.Token);
            }
            catch (OperationCanceledException exception)
            {
                Assert.AreEqual(tokenSource.Token, exception.CancellationToken);
                throw;
            }
            finally
            {
                tokenSource.Dispose();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task HandleMessageAsync_ThrowsExpectedException_IfOperationIsCancelledWithSomeOtherToken()
        {
            var exceptionToThrow = new OperationCanceledException();
            var tokenSource = new CancellationTokenSource();

            try
            {
                await HandleMessageAsync((message, context) =>
                {
                    throw exceptionToThrow;
                }, new object(), tokenSource.Token);
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(exceptionToThrow, exception.InnerException);
                throw;
            }
            finally
            {
                tokenSource.Dispose();
            }
        }

        [TestMethod]
        public abstract Task HandleMessageAsync_ThrowsExpectedException_IfOperationThrowsMessageHandlerOperationException();

        [TestMethod]
        public abstract Task HandleMessageAsync_ThrowsExpectedException_IfOperationThrowsBadRequestException();

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task HandleMessageAsync_ThrowsExpectedException_IfOperationThrowsInternalServerErrorException()
        {
            var exceptionToThrow = new InternalServerErrorException();

            try
            {
                await HandleMessageAsync((message, context) =>
                {
                    throw exceptionToThrow;
                }, new object());
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(exceptionToThrow, exception);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task HandleMessageAsync_ThrowsExpectedException_IfOperationThrowsRandomException()
        {
            var exceptionToThrow = new Exception();

            try
            {
                await HandleMessageAsync((message, context) =>
                {
                    throw exceptionToThrow;
                }, new object());
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(exceptionToThrow, exception.InnerException);
                throw;
            }
        }

        #endregion

        #region [====== Invocation of Registered EventHandlers ======]

        [MessageHandler(HandlesExternalMessages = true, HandlesInternalMessages = true)]
        private abstract class MessageHandlerBase : IMessageHandler<int>, IMessageHandler<object>
        {
            public Task HandleAsync(int message, MessageHandlerOperationContext context)
            {
                try
                {
                    return HandleMessageAsync(message, context);
                }
                finally
                {
                    context.EventBus.Publish(string.Empty);
                }
            }

            public Task HandleAsync(object message, MessageHandlerOperationContext context)
            {
                try
                {
                    return HandleMessageAsync(message, context);
                }
                finally
                {
                    Assert.AreSame(typeof(string), message.GetType());
                }
            }

            private Task HandleMessageAsync<TMessage>(TMessage message, MessageHandlerOperationContext context)
            {
                AssertContext(message, context);
                Instances(context).Add(this);
                return Task.CompletedTask;
            }

            private void AssertContext<TMessage>(TMessage message, MessageHandlerOperationContext context) =>
                AssertOperation(message, context.StackTrace.CurrentOperation);

            private void AssertOperation<TMessage>(TMessage message, IAsyncMethodOperation operation)
            {
                Assert.AreEqual(message, operation.Message.Instance);
                AssertMethod(typeof(TMessage), operation.Method);
                AssertMessageHandler(GetType(), operation.Method.Component as MessageHandler);
            }

            private static void AssertMethod(Type messageType, IAsyncMethod method)
            {
                Assert.AreSame(messageType, method.MessageParameter.Type);
                Assert.AreSame(typeof(MessageHandlerOperationContext), method.ContextParameter.Type);
            }

            private static void AssertMessageHandler(Type messageHandlerType, MessageHandler messageHandler)
            {
                Assert.IsNotNull(messageHandler);
                Assert.AreSame(messageHandlerType, messageHandler.Type);
                Assert.AreEqual(2, messageHandler.Interfaces.Count);
                AssertImplements<IMessageHandler<int>>(messageHandler);
                AssertImplements<IMessageHandler<object>>(messageHandler);
            }

            private static void AssertImplements<TInterface>(MessageHandler messageHandler) =>
                Assert.IsTrue(messageHandler.Interfaces.Any(@interface => @interface.Type == typeof(TInterface)));

            private static IInstanceCollector Instances(MicroProcessorOperationContext context) =>
                context.ServiceProvider.GetRequiredService<IInstanceCollector>();
        }

        private sealed class TransientMessageHandler : MessageHandlerBase { }

        [MicroProcessorComponent(ServiceLifetime.Scoped)]
        private sealed class ScopedMessageHandler : MessageHandlerBase { }

        [MicroProcessorComponent(ServiceLifetime.Singleton)]
        private sealed class SingletonMessageHandler : MessageHandlerBase { }

        [TestMethod]
        public async Task HandleMessageAsync_CreatesNewInstanceForEveryInvocation_IfMessageHandlerHasTransientLifetime()
        {
            ProcessorBuilder.Components.AddMessageHandler<TransientMessageHandler>();

            var processor = CreateProcessor();

            using (processor.ServiceProvider.CreateScope())
            {
                await HandleMessageAsync<TransientMessageHandler>(processor);
            }
            AssertInstanceCount(processor, 2);

            using (processor.ServiceProvider.CreateScope())
            {
                await HandleMessageAsync<TransientMessageHandler>(processor);
            }
            AssertInstanceCount(processor, 4);
        }

        [TestMethod]
        public async Task HandleMessageAsync_CreatesNewInstancePerScope_IfMessageHandlerHasScopedLifetime()
        {
            ProcessorBuilder.Components.AddMessageHandler<ScopedMessageHandler>();

            var processor = CreateProcessor();

            using (processor.ServiceProvider.CreateScope())
            {
                await HandleMessageAsync<ScopedMessageHandler>(processor);
            }
            AssertInstanceCount(processor, 1);

            using (processor.ServiceProvider.CreateScope())
            {
                await HandleMessageAsync<ScopedMessageHandler>(processor);
            }
            AssertInstanceCount(processor, 2);
        }

        [TestMethod]
        public async Task HandleMessageAsync_CreatesOnlyOneInstanceEver_IfMessageHandlerHasSingletonLifetime()
        {
            ProcessorBuilder.Components.AddMessageHandler<SingletonMessageHandler>();

            var processor = CreateProcessor();

            using (processor.ServiceProvider.CreateScope())
            {
                await HandleMessageAsync<SingletonMessageHandler>(processor);
            }
            AssertInstanceCount(processor, 1);

            using (processor.ServiceProvider.CreateScope())
            {
                await HandleMessageAsync<SingletonMessageHandler>(processor);
            }
            AssertInstanceCount(processor, 1);
        }

        [TestMethod]
        public async Task HandleMessageAsync_UsesOnlyOneInstanceEver_IfMessageHandlerWasRegisteredAsInstance()
        {
            ProcessorBuilder.Components.AddMessageHandler(new TransientMessageHandler());

            var processor = CreateProcessor();

            using (processor.ServiceProvider.CreateScope())
            {
                await HandleMessageAsync<TransientMessageHandler>(processor);
            }
            AssertInstanceCount(processor, 1);

            using (processor.ServiceProvider.CreateScope())
            {
                await HandleMessageAsync<TransientMessageHandler>(processor);
            }
            AssertInstanceCount(processor, 1);
        }

        [TestMethod]
        public async Task HandleMessageAsync_InvokesInstanceOvertType_IfInstanceIsRegisteredBeforeType()
        {
            ProcessorBuilder.Components.AddMessageHandler<TransientMessageHandler>();
            ProcessorBuilder.Components.AddMessageHandler(new TransientMessageHandler());

            var processor = CreateProcessor();

            using (processor.ServiceProvider.CreateScope())
            {
                await HandleMessageAsync<TransientMessageHandler>(processor);
            }
            AssertInstanceCount(processor, 1);

            using (processor.ServiceProvider.CreateScope())
            {
                await HandleMessageAsync<TransientMessageHandler>(processor);
            }
            AssertInstanceCount(processor, 1);
        }

        [TestMethod]
        public async Task HandleMessageAsync_InvokesInstanceOvertType_IfInstanceIsRegisteredAfterType()
        {
            ProcessorBuilder.Components.AddMessageHandler<TransientMessageHandler>();
            ProcessorBuilder.Components.AddMessageHandler(new TransientMessageHandler());

            var processor = CreateProcessor();

            using (processor.ServiceProvider.CreateScope())
            {
                await HandleMessageAsync<TransientMessageHandler>(processor);
            }
            AssertInstanceCount(processor, 1);

            using (processor.ServiceProvider.CreateScope())
            {
                await HandleMessageAsync<TransientMessageHandler>(processor);
            }
            AssertInstanceCount(processor, 1);
        }

        [TestMethod]
        public async Task HandleMessageAsync_InvokesActionOnce_IfDelegateIsRegisteredTwice_And_ConfigurationIsEqual()
        {
            Action<int, MessageHandlerOperationContext> messageHandler = (message, context) =>
            {
                context.EventBus.Publish(string.Empty);
            };

            ProcessorBuilder.Components.AddMessageHandler(messageHandler);
            ProcessorBuilder.Components.AddMessageHandler(messageHandler);

            var result = await HandleMessageAsync((message, context) =>
            {
                context.EventBus.Publish(DateTimeOffset.UtcNow.Second);
            }, new object());

            Assert.AreEqual(2, result.MessageHandlerCount);
            Assert.AreEqual(2, result.Events.Count);
        }

        [TestMethod]
        public async Task HandleMessageAsync_InvokesFuncOnce_IfDelegateIsRegisteredTwice_And_ConfigurationIsEqual()
        {
            Func<int, MessageHandlerOperationContext, Task> messageHandler = (message, context) =>
            {
                context.EventBus.Publish(string.Empty);
                return Task.CompletedTask;
            };

            ProcessorBuilder.Components.AddMessageHandler(messageHandler);
            ProcessorBuilder.Components.AddMessageHandler(messageHandler);

            var result = await HandleMessageAsync((message, context) =>
            {
                context.EventBus.Publish(DateTimeOffset.UtcNow.Second);
            }, new object());

            Assert.AreEqual(2, result.MessageHandlerCount);
            Assert.AreEqual(2, result.Events.Count);
        }

        private Task HandleMessageAsync<TMessageHandler>(IMicroProcessor processor) where TMessageHandler : IMessageHandler<int> =>
            HandleMessageAsync(processor, processor.ServiceProvider.GetRequiredService<TMessageHandler>());

        private async Task HandleMessageAsync(IMicroProcessor processor, IMessageHandler<int> messageHandler) =>
            AssertMessageHandlerResult(await HandleMessageAsync<int>(processor, messageHandler, DateTimeOffset.UtcNow.Second));

        private static void AssertMessageHandlerResult(MessageHandlerOperationResult result)
        {
            Assert.AreEqual(2, result.MessageHandlerCount);
            Assert.AreEqual(1, result.Events.Count);
        }

        private static void AssertInstanceCount(IMicroProcessor processor, int count) =>
            processor.ServiceProvider.GetRequiredService<IInstanceCollector>().AssertInstanceCountIs(count);

        #endregion

        #region [====== HandleMessageAsync ======]

        protected abstract MessageKind MessageKind
        {
            get;
        }

        protected Task<MessageHandlerOperationResult> HandleMessageAsync<TMessage>(Action<TMessage, MessageHandlerOperationContext> messageHandler, TMessage message, CancellationToken? token = null) =>
            HandleMessageAsync(CreateProcessor(), messageHandler, message, token);

        protected Task<MessageHandlerOperationResult> HandleMessageAsync<TMessage>(Func<TMessage, MessageHandlerOperationContext, Task> messageHandler, TMessage message, CancellationToken? token = null) =>
            HandleMessageAsync(CreateProcessor(), messageHandler, message, token);

        protected Task<MessageHandlerOperationResult> HandleMessageAsync<TMessage>(IMessageHandler<TMessage> messageHandler, TMessage message, CancellationToken? token = null) =>
            HandleMessageAsync(CreateProcessor(), messageHandler, message, token);

        protected abstract Task<MessageHandlerOperationResult> HandleMessageAsync<TMessage>(IMicroProcessor processor, Action<TMessage, MessageHandlerOperationContext> messageHandler, TMessage message, CancellationToken? token = null);

        protected abstract Task<MessageHandlerOperationResult> HandleMessageAsync<TMessage>(IMicroProcessor processor, Func<TMessage, MessageHandlerOperationContext, Task> messageHandler, TMessage message, CancellationToken? token = null);

        protected abstract Task<MessageHandlerOperationResult> HandleMessageAsync<TMessage>(IMicroProcessor processor, IMessageHandler<TMessage> messageHandler, TMessage message, CancellationToken? token = null);

        #endregion
    }
}
