using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public abstract class MessageHandlerOperationTest : MicroProcessorTest<MicroProcessor>
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
            Assert.AreEqual(0, result.Output.Count);
        }

        [TestMethod]
        public async Task HandleMessageAsync_ReturnsExpectedEvent_IfMessageHandlerPublishesOneEvent()
        {
            var command = new object();
            var result = await HandleMessageAsync((message, context) =>
            {
                context.MessageBus.Publish(message);
            }, command);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.MessageHandlerCount);
            Assert.AreEqual(1, result.Output.Count);
            Assert.AreSame(command, result.Output[0].Content);
        }

        [TestMethod]
        public async Task HandleMessageAsync_ReturnsExpectedEvents_IfMessageHandlerPublishesOneEvent_And_EventHandlerPublishesAnotherEvent()
        {
            var command = new object();
            var eventA = string.Empty;
            var eventB = new object();

            // Handles Event A.
            Processor.ConfigureMessageHandlers(messageHandlers =>
            {
                messageHandlers.AddInstance<string>((message, context) =>
                {
                    context.MessageBus.Publish(eventB);
                });
            });

            var result = await HandleMessageAsync((message, context) =>
            {
                context.MessageBus.Publish(eventA);
            }, command);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.MessageHandlerCount);
            Assert.AreEqual(2, result.Output.Count);
            Assert.AreSame(eventA, result.Output[0].Content);
            Assert.AreSame(eventB, result.Output[1].Content);
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
            Processor.ConfigureMessageHandlers(messageHandlers =>
            {
                messageHandlers.AddInstance<string>((message, context) =>
                {
                    context.MessageBus.Publish(eventB);
                });

                // Handles Event B.
                messageHandlers.AddInstance<int>((message, context) =>
                {
                    context.MessageBus.Publish(eventC);
                });

                // Handles Event B.
                messageHandlers.AddInstance<int>((message, context) =>
                {
                    context.MessageBus.Publish(eventD);
                });
            });

            var result = await HandleMessageAsync((message, context) =>
            {
                context.MessageBus.Publish(eventA);
            }, command);

            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.MessageHandlerCount);
            Assert.AreEqual(4, result.Output.Count);
            Assert.AreSame(eventA, result.Output[0].Content);
            Assert.AreEqual(eventB, result.Output[1].Content);
            Assert.AreSame(eventC, result.Output[2].Content);
            Assert.AreSame(eventD, result.Output[3].Content);
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
            Processor.ConfigureMessageHandlers(messageHandlers =>
            {
                messageHandlers.AddInstance<string>((message, context) =>
                {
                    AssertStackTrace(context.StackTrace, 2, message, MessageKind.Event);
                    AssertEventBus(context.MessageBus, 0);

                    context.MessageBus.Publish(eventB);

                    AssertEventBus(context.MessageBus, 1);
                });

                // Handles Event B.
                messageHandlers.AddInstance<int>((message, context) =>
                {
                    AssertStackTrace(context.StackTrace, 3, message, MessageKind.Event);
                    AssertEventBus(context.MessageBus, 0);

                    context.MessageBus.Publish(eventC);

                    AssertEventBus(context.MessageBus, 1);
                });

                // Handles Event B.
                messageHandlers.AddInstance<int>((message, context) =>
                {
                    AssertStackTrace(context.StackTrace, 3, message, MessageKind.Event);
                    AssertEventBus(context.MessageBus, 0);

                    context.MessageBus.Publish(eventD);

                    AssertEventBus(context.MessageBus, 1);
                });
            });

            await HandleMessageAsync((message, context) =>
            {
                AssertStackTrace(context.StackTrace, 1, message, MessageKind);
                AssertEventBus(context.MessageBus, 0);

                context.MessageBus.Publish(eventA);

                AssertEventBus(context.MessageBus, 1);
            }, command);
        }

        private static void AssertStackTrace(IAsyncMethodOperationStackTrace stackTrace, int count, object message, MessageKind messageKind)
        {
            Assert.AreEqual(count, stackTrace.Count);

            if (count == 1)
            {
                AssertOperation(stackTrace.CurrentOperation, MicroProcessorOperationKind.RootOperation, message, messageKind);
            }
            else
            {
                AssertOperation(stackTrace.CurrentOperation, MicroProcessorOperationKind.BranchOperation, message, messageKind);
            }
        }

        private static void AssertOperation(IMicroProcessorOperation operation, MicroProcessorOperationKind operationKind, object message, MessageKind messageKind)
        {
            Assert.IsNotNull(operation);
            Assert.AreEqual(MicroProcessorOperationType.MessageHandlerOperation, operation.Type);
            Assert.AreEqual(message, operation.Message.Content);
            Assert.AreEqual(messageKind, operation.Message.Kind);
        }

        private static void AssertEventBus(IReadOnlyCollection<object> eventBus, int count) =>
            Assert.AreEqual(count, eventBus.Count);

        #endregion

        #region [====== UnitOfWork ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public async Task HandleMessageAsync_Throws_IfUnitOfWorkModeIsInvalid()
        {
            Processor.ConfigureSettings(settings =>
            {
                settings.UnitOfWorkMode = (UnitOfWorkMode) (-1);
            });

            await HandleMessageAsync((message, context) => { }, new object());
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task HandleMessageAsync_ThrowsOnEnlist_IfUnitOfWorkIsDisabled()
        {
            Processor.ConfigureSettings(settings =>
            {
                settings.UnitOfWorkMode = UnitOfWorkMode.Disabled;
            });

            var changeTracker = new ChangeTrackerSpy(true);

            try
            {
                await HandleMessageAsync((message, context) =>
                {
                    context.UnitOfWork.Enlist(changeTracker);

                }, new object());
            }
            catch (InternalServerErrorException exception)
            {
                Assert.IsInstanceOfType(exception.InnerException, typeof(InvalidOperationException));
                throw;
            }
            finally
            {
                changeTracker.AssertRequiresFlushCountIs(0);
                changeTracker.AssertFlushCountIs(0);
            }
        }

        [TestMethod]
        public async Task HandleMessageAsync_DoesNothingOnFlushAsync_IfUnitOfWorkModeIsDisabled()
        {
            Processor.ConfigureSettings(settings =>
            {
                settings.UnitOfWorkMode = UnitOfWorkMode.Disabled;
            });

            await HandleMessageAsync(async (message, context) =>
            {
                await context.UnitOfWork.SaveChangesAsync();

            }, new object());
        }

        [TestMethod]
        public async Task HandleMessageAsync_FlushesChangeTrackersSynchronously_IfUnitOfWorkIsSingleThreaded_And_SomeChangeTrackersRequireFlush()
        {
            Processor.ConfigureSettings(settings =>
            {
                settings.UnitOfWorkMode = UnitOfWorkMode.SingleThreaded;
            });

            var changeTrackerA = new ChangeTrackerSpy(true);
            var changeTrackerB = new ChangeTrackerSpy(true);
            var changeTrackerC = new ChangeTrackerSpy(false);

            await HandleMessageAsync((message, context) =>
            {
                context.UnitOfWork.Enlist(changeTrackerA);
                context.UnitOfWork.Enlist(changeTrackerB);
                context.UnitOfWork.Enlist(changeTrackerC);

                changeTrackerA.AssertRequiresFlushCountIs(0);
                changeTrackerA.AssertFlushCountIs(0);

                changeTrackerB.AssertRequiresFlushCountIs(0);
                changeTrackerB.AssertFlushCountIs(0);

                changeTrackerC.AssertRequiresFlushCountIs(0);
                changeTrackerC.AssertFlushCountIs(0);
            }, new object());

            changeTrackerA.AssertRequiresFlushCountIs(1);
            changeTrackerA.AssertFlushCountIs(1);

            changeTrackerB.AssertRequiresFlushCountIs(1);
            changeTrackerB.AssertFlushCountIs(1);

            changeTrackerC.AssertRequiresFlushCountIs(1);
            changeTrackerC.AssertFlushCountIs(0);
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task HandleMessageAsync_FlushesChangeTrackersSynchronously_IfUnitOfWorkIsSingleThreaded_And_SomeChangeTrackerThrows()
        {
            Processor.ConfigureSettings(settings =>
            {
                settings.UnitOfWorkMode = UnitOfWorkMode.SingleThreaded;
            });

            var exceptionToThrow = new InternalServerErrorException(MicroProcessorOperationStackTrace.Empty);
            var changeTrackerA = new ChangeTrackerSpy(true);
            var changeTrackerB = new ChangeTrackerSpy(true, exceptionToThrow);
            var changeTrackerC = new ChangeTrackerSpy(true);

            try
            {
                await HandleMessageAsync((message, context) =>
                {
                    context.UnitOfWork.Enlist(changeTrackerA);
                    context.UnitOfWork.Enlist(changeTrackerB);
                    context.UnitOfWork.Enlist(changeTrackerC);

                    changeTrackerA.AssertRequiresFlushCountIs(0);
                    changeTrackerA.AssertFlushCountIs(0);

                    changeTrackerB.AssertRequiresFlushCountIs(0);
                    changeTrackerB.AssertFlushCountIs(0);

                    changeTrackerC.AssertRequiresFlushCountIs(0);
                    changeTrackerC.AssertFlushCountIs(0);
                }, new object());
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(exceptionToThrow, exception);
                throw;
            }
            finally
            {
                // All change trackers required a flush, but since the second tracker
                // throws an exception, the third tracker is never flushed, and all
                // changes are undone.
                changeTrackerA.AssertRequiresFlushCountIs(1);
                changeTrackerA.AssertFlushCountIs(1);
                changeTrackerA.AssertUndoCountIs(1);

                changeTrackerB.AssertRequiresFlushCountIs(1);
                changeTrackerB.AssertFlushCountIs(1);
                changeTrackerB.AssertUndoCountIs(1);

                changeTrackerC.AssertRequiresFlushCountIs(0);
                changeTrackerC.AssertFlushCountIs(0);
                changeTrackerC.AssertUndoCountIs(1);
            }
        }

        [TestMethod]
        public async Task HandleMessageAsync_FlushesChangeTrackersAsynchronously_IfUnitOfWorkIsMultiThreaded_And_SomeChangeTrackersRequireFlush()
        {
            Processor.ConfigureSettings(settings =>
            {
                settings.UnitOfWorkMode = UnitOfWorkMode.MultiThreaded;
            });

            var changeTrackerA = new ChangeTrackerSpy(true);
            var changeTrackerB = new ChangeTrackerSpy(true);
            var changeTrackerC = new ChangeTrackerSpy(false);

            await HandleMessageAsync((message, context) =>
            {
                context.UnitOfWork.Enlist(changeTrackerA);
                context.UnitOfWork.Enlist(changeTrackerB);
                context.UnitOfWork.Enlist(changeTrackerC);

                changeTrackerA.AssertRequiresFlushCountIs(0);
                changeTrackerA.AssertFlushCountIs(0);

                changeTrackerB.AssertRequiresFlushCountIs(0);
                changeTrackerB.AssertFlushCountIs(0);

                changeTrackerC.AssertRequiresFlushCountIs(0);
                changeTrackerC.AssertFlushCountIs(0);
            }, new object());

            changeTrackerA.AssertRequiresFlushCountIs(1);
            changeTrackerA.AssertFlushCountIs(1);
            changeTrackerA.AssertUndoCountIs(0);

            changeTrackerB.AssertRequiresFlushCountIs(1);
            changeTrackerB.AssertFlushCountIs(1);
            changeTrackerB.AssertUndoCountIs(0);

            changeTrackerC.AssertRequiresFlushCountIs(1);
            changeTrackerC.AssertFlushCountIs(0);
            changeTrackerC.AssertUndoCountIs(0);
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task HandleMessageAsync_FlushesChangeTrackersAsynchronously_IfUnitOfWorkIsSingleThreaded_And_SomeChangeTrackerThrows()
        {
            Processor.ConfigureSettings(settings =>
            {
                settings.UnitOfWorkMode = UnitOfWorkMode.MultiThreaded;
            });

            var exceptionToThrow = new InternalServerErrorException(MicroProcessorOperationStackTrace.Empty);
            var changeTrackerA = new ChangeTrackerSpy(true);
            var changeTrackerB = new ChangeTrackerSpy(true, exceptionToThrow);
            var changeTrackerC = new ChangeTrackerSpy(true);

            try
            {
                await HandleMessageAsync((message, context) =>
                {
                    context.UnitOfWork.Enlist(changeTrackerA);
                    context.UnitOfWork.Enlist(changeTrackerB);
                    context.UnitOfWork.Enlist(changeTrackerC);

                    changeTrackerA.AssertRequiresFlushCountIs(0);
                    changeTrackerA.AssertFlushCountIs(0);

                    changeTrackerB.AssertRequiresFlushCountIs(0);
                    changeTrackerB.AssertFlushCountIs(0);

                    changeTrackerC.AssertRequiresFlushCountIs(0);
                    changeTrackerC.AssertFlushCountIs(0);
                }, new object());
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(exceptionToThrow, exception);
                Assert.AreEqual(0, exception.OperationStackTrace.Count);
                throw;
            }
            finally
            {
                // All resource managers required a flush, and since all managers
                // manage a different resource and are flushed asynchronously,
                // the third manager is flushed even though the second one throws an exception.
                changeTrackerA.AssertRequiresFlushCountIs(1);
                changeTrackerA.AssertFlushCountIs(1);

                changeTrackerB.AssertRequiresFlushCountIs(1);
                changeTrackerB.AssertFlushCountIs(1);

                changeTrackerC.AssertRequiresFlushCountIs(1);
                changeTrackerC.AssertFlushCountIs(1);
            }
        }

        #endregion

        #region [====== Exception Handling ======]

        [TestMethod]
        [ExpectedException(typeof(GatewayTimeoutException))]
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
            catch (GatewayTimeoutException exception)
            {
                Assert.IsInstanceOfType(exception.InnerException, typeof(OperationCanceledException));
                Assert.AreEqual(1, exception.OperationStackTrace.Count);
                Assert.IsInstanceOfType(exception.OperationStackTrace.RootOperation.Message.Content, typeof(object));
                Assert.IsInstanceOfType(exception.OperationStackTrace.CurrentOperation.Message.Content, typeof(object));
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
                Assert.AreEqual(1, exception.OperationStackTrace.Count);
                Assert.IsInstanceOfType(exception.OperationStackTrace.RootOperation.Message.Content, typeof(object));
                Assert.IsInstanceOfType(exception.OperationStackTrace.CurrentOperation.Message.Content, typeof(object));
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
            var exceptionToThrow = new InternalServerErrorException(MicroProcessorOperationStackTrace.Empty);

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
                Assert.AreEqual(0, exception.OperationStackTrace.Count);
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
                Assert.AreEqual(1, exception.OperationStackTrace.Count);
                Assert.IsInstanceOfType(exception.OperationStackTrace.RootOperation.Message.Content, typeof(object));
                Assert.IsInstanceOfType(exception.OperationStackTrace.CurrentOperation.Message.Content, typeof(object));
                throw;
            }
        }

        #endregion

        #region [====== Invocation of Registered EventHandlers ======]

        private abstract class MessageHandlerBase : IMessageHandler<int>, IMessageHandler<object>
        {
            [MessageBusEndpoint(MessageBusTypes.All)]
            public Task HandleAsync(int message, MessageHandlerOperationContext context)
            {
                try
                {
                    return HandleMessageAsync(message, context);
                }
                finally
                {
                    context.MessageBus.Publish(string.Empty);
                }
            }

            [MessageBusEndpoint(MessageBusTypes.All)]
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
                Assert.AreEqual(message, operation.Message.Content);
                AssertMethod(typeof(TMessage), operation.Method);
                AssertMessageHandlerType(GetType(), operation.Method.ComponentType);
            }

            private static void AssertMethod(Type messageType, IAsyncMethod method)
            {
                Assert.AreSame(messageType, method.MessageParameterInfo.ParameterType);
                Assert.AreSame(typeof(MessageHandlerOperationContext), method.ContextParameterInfo.ParameterType);
            }

            private static void AssertMessageHandlerType(Type expectedType, Type actualType)
            {
                Assert.IsNotNull(actualType);
                Assert.AreSame(expectedType, actualType);
            }

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
            Processor.ConfigureMessageHandlers(messageHandlers =>
            {
                messageHandlers.Add<TransientMessageHandler>();
            });

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
            Processor.ConfigureMessageHandlers(messageHandlers =>
            {
                messageHandlers.Add<ScopedMessageHandler>();
            });

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
            Processor.ConfigureMessageHandlers(messageHandlers =>
            {
                messageHandlers.Add<SingletonMessageHandler>();
            });

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
            Processor.ConfigureMessageHandlers(messageHandlers =>
            {
                messageHandlers.AddInstance(new TransientMessageHandler());
            });

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
        public async Task HandleMessageAsync_InvokesInstanceOverType_IfInstanceIsRegisteredBeforeType()
        {
            Processor.ConfigureMessageHandlers(messageHandlers =>
            {
                messageHandlers.Add<TransientMessageHandler>();
                messageHandlers.AddInstance(new TransientMessageHandler());
            });

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
        public async Task HandleMessageAsync_InvokesInstanceOverType_IfInstanceIsRegisteredAfterType()
        {
            Processor.ConfigureMessageHandlers(messageHandlers =>
            {
                messageHandlers.Add<TransientMessageHandler>();
                messageHandlers.AddInstance(new TransientMessageHandler());
            });

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

        private Task HandleMessageAsync<TMessageHandler>(IMicroProcessor processor) where TMessageHandler : IMessageHandler<int> =>
            HandleMessageAsync(processor, processor.ServiceProvider.GetRequiredService<TMessageHandler>());

        private async Task HandleMessageAsync(IMicroProcessor processor, IMessageHandler<int> messageHandler) =>
            AssertMessageHandlerResult(await HandleMessageAsync(processor, messageHandler, DateTimeOffset.UtcNow.Second));

        private static void AssertMessageHandlerResult(IMessageHandlerOperationResult result)
        {
            Assert.AreEqual(2, result.MessageHandlerCount);
            Assert.AreEqual(1, result.Output.Count);
        }

        private static void AssertInstanceCount(IMicroProcessor processor, int count) =>
            processor.ServiceProvider.GetRequiredService<IInstanceCollector>().AssertInstanceCountIs(count);

        #endregion

        #region [====== HandleMessageAsync ======]

        protected abstract MessageKind MessageKind
        {
            get;
        }

        protected Task<MessageHandlerOperationResult<TMessage>> HandleMessageAsync<TMessage>(Action<TMessage, MessageHandlerOperationContext> messageHandler, TMessage message, CancellationToken? token = null) =>
            HandleMessageAsync(CreateProcessor(), messageHandler, message, token);

        protected Task<MessageHandlerOperationResult<TMessage>> HandleMessageAsync<TMessage>(Func<TMessage, MessageHandlerOperationContext, Task> messageHandler, TMessage message, CancellationToken? token = null) =>
            HandleMessageAsync(CreateProcessor(), messageHandler, message, token);

        protected Task<MessageHandlerOperationResult<TMessage>> HandleMessageAsync<TMessage>(IMessageHandler<TMessage> messageHandler, TMessage message, CancellationToken? token = null) =>
            HandleMessageAsync(CreateProcessor(), messageHandler, message, token);

        protected abstract Task<MessageHandlerOperationResult<TMessage>> HandleMessageAsync<TMessage>(IMicroProcessor processor, Action<TMessage, MessageHandlerOperationContext> messageHandler, TMessage message, CancellationToken? token = null);

        protected abstract Task<MessageHandlerOperationResult<TMessage>> HandleMessageAsync<TMessage>(IMicroProcessor processor, Func<TMessage, MessageHandlerOperationContext, Task> messageHandler, TMessage message, CancellationToken? token = null);

        protected abstract Task<MessageHandlerOperationResult<TMessage>> HandleMessageAsync<TMessage>(IMicroProcessor processor, IMessageHandler<TMessage> messageHandler, TMessage message, CancellationToken? token = null);

        #endregion
    }
}
