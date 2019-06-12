using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class CommandExecutionTest : MicroProcessorTest
    {
        #region [====== Null Parameters ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ExecuteAsync_Throws_IfCommandHandlerIsNull()
        {
            await CreateProcessor().ExecuteAsync(null, new object());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ExecuteAsync_Throws_IfCommandHandlerActionIsNull()
        {
            await CreateProcessor().ExecuteAsync(null as Action<object, MessageHandlerOperationContext>, new object());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ExecuteAsync_Throws_IfCommandHandlerFuncIsNull()
        {
            await CreateProcessor().ExecuteAsync(null as Func<object, MessageHandlerOperationContext, Task>, new object());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ExecuteAsync_Throws_IfCommandIsNull()
        {
            await CreateProcessor().ExecuteAsync<object>((message, context) => { }, null);
        }

        #endregion

        #region [====== Return Value ======]

        [TestMethod]
        public async Task ExecuteAsync_ReturnsNoEvents_IfCommandHandlerPublishesNoEvents()
        {
            var result = await CreateProcessor().ExecuteAsync((message, context) => { }, new object());

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.MessageHandlerCount);
            Assert.AreEqual(0, result.Events.Count);
        }

        [TestMethod]
        public async Task ExecuteAsync_ReturnsExpectedEvent_IfCommandHandlerPublishesOneEvent()
        {
            var command = new object();
            var result = await CreateProcessor().ExecuteAsync((message, context) =>
            {
                context.EventBus.Publish(message);
            }, command);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.MessageHandlerCount);
            Assert.AreEqual(1, result.Events.Count);
            Assert.AreSame(command, result.Events[0]);
        }

        [TestMethod]
        public async Task ExecuteAsync_ReturnsExpectedEvents_IfCommandHandlerPublishesOneEvent_And_EventHandlerPublishesAnotherEvent()
        {
            var command = new object();
            var eventA = string.Empty;
            var eventB = new object();

            // Handles Event A.
            ProcessorBuilder.Components.AddMessageHandler<string>((message, context) =>
            {
                context.EventBus.Publish(eventB);
            }, false, true);

            var result = await CreateProcessor().ExecuteAsync((message, context) =>
            {
                context.EventBus.Publish(eventA);
            }, command);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.MessageHandlerCount);
            Assert.AreEqual(2, result.Events.Count);
            Assert.AreSame(eventA, result.Events[0]);
            Assert.AreSame(eventB, result.Events[1]);
        }

        [TestMethod]
        public async Task ExecuteAsync_ReturnsExpectedEvents_IfCommandHandlerPublishesOneEvent_And_EventHandlersPublishMoreEvents()
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
            }, false, true);

            // Handles Event B.
            ProcessorBuilder.Components.AddMessageHandler<int>((message, context) =>
            {
                context.EventBus.Publish(eventC);
            }, false, true);

            // Handles Event B.
            ProcessorBuilder.Components.AddMessageHandler<int>((message, context) =>
            {
                context.EventBus.Publish(eventD);
            }, false, true);

            var result = await CreateProcessor().ExecuteAsync((message, context) =>
            {
                context.EventBus.Publish(eventA);
            }, command);

            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.MessageHandlerCount);
            Assert.AreEqual(4, result.Events.Count);
            Assert.AreSame(eventA, result.Events[0]);
            Assert.AreEqual(eventB, result.Events[1]);
            Assert.AreSame(eventC, result.Events[2]);
            Assert.AreSame(eventD, result.Events[3]);
        }

        #endregion

        #region [====== Context Verification ======]

        [TestMethod]
        public async Task ExecuteAsync_ProducesExpectedStackTrace_IfCommandHandlerPublishesOneEvent_And_EventHandlersPublishMoreEvents()
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
            }, false, true);

            // Handles Event B.
            ProcessorBuilder.Components.AddMessageHandler<int>((message, context) =>
            {
                AssertStackTrace(context.StackTrace, 3, message, MessageKind.Event);
                AssertEventBus(context.EventBus, 0);

                context.EventBus.Publish(eventC);

                AssertEventBus(context.EventBus, 1);
            }, false, true);

            // Handles Event B.
            ProcessorBuilder.Components.AddMessageHandler<int>((message, context) =>
            {
                AssertStackTrace(context.StackTrace, 3, message, MessageKind.Event);
                AssertEventBus(context.EventBus, 0);

                context.EventBus.Publish(eventD);

                AssertEventBus(context.EventBus, 1);
            }, false, true);

            await CreateProcessor().ExecuteAsync((message, context) =>
            {
                AssertStackTrace(context.StackTrace, 1, message, MessageKind.Command);
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

        private static void AssertOperation(IAsyncMethodOperation operation, MicroProcessorOperationKinds operationKind, object message, MessageKind messageKind)
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
        public async Task ExecuteAsync_Throws_IfUnitOfWorkModeIsInvalid()
        {
            ProcessorBuilder.UnitOfWorkMode = (UnitOfWorkMode) (-1);

            await CreateProcessor().ExecuteAsync((message, context) => { }, new object());
        }

        [TestMethod]
        public async Task ExecuteAsync_DoesNotFlushResourceManager_IfUnitOfWorkModeIsDisabled_And_ResourceManagerDoesNotRequireFlush()
        {
            ProcessorBuilder.UnitOfWorkMode = UnitOfWorkMode.Disabled;

            var resourceManager = new UnitOfWorkResourceManagerSpy(false);

            await CreateProcessor().ExecuteAsync(async (message, context) =>
            {
                await context.UnitOfWork.EnlistAsync(resourceManager);

                resourceManager.AssertRequiresFlushCountIs(1);
                resourceManager.AssertFlushCountIs(0);
            }, new object());

            resourceManager.AssertRequiresFlushCountIs(1);
            resourceManager.AssertFlushCountIs(0);
        }

        [TestMethod]
        public async Task ExecuteAsync_FlushesResourceManagerOnEnlistment_IfUnitOfWorkIsDisabled_And_ResourceManagerRequiresFlush()
        {
            ProcessorBuilder.UnitOfWorkMode = UnitOfWorkMode.Disabled;

            var resourceManager = new UnitOfWorkResourceManagerSpy(true);

            await CreateProcessor().ExecuteAsync(async (message, context) =>
            {
                await context.UnitOfWork.EnlistAsync(resourceManager);

                resourceManager.AssertRequiresFlushCountIs(1);
                resourceManager.AssertFlushCountIs(1);
            }, new object());

            resourceManager.AssertRequiresFlushCountIs(1);
            resourceManager.AssertFlushCountIs(1);
        }

        [TestMethod]
        public async Task ExecuteAsync_FlushesResourceManagersSynchronously_IfUnitOfWorkIsSingleThreaded_And_SomeResourceManagersRequireFlush()
        {
            ProcessorBuilder.UnitOfWorkMode = UnitOfWorkMode.SingleThreaded;

            var resourceManagerA = new UnitOfWorkResourceManagerSpy(true);
            var resourceManagerB = new UnitOfWorkResourceManagerSpy(true);
            var resourceManagerC = new UnitOfWorkResourceManagerSpy(false);

            await CreateProcessor().ExecuteAsync(async (message, context) =>
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
        public async Task ExecuteAsync_FlushesResourceManagersSynchronously_IfUnitOfWorkIsSingleThreaded_And_SomeResourceManagerThrows()
        {
            ProcessorBuilder.UnitOfWorkMode = UnitOfWorkMode.SingleThreaded;

            var exceptionToThrow = new InternalServerErrorException();
            var resourceManagerA = new UnitOfWorkResourceManagerSpy(true);
            var resourceManagerB = new UnitOfWorkResourceManagerSpy(true, exceptionToThrow);
            var resourceManagerC = new UnitOfWorkResourceManagerSpy(true);

            try
            {
                await CreateProcessor().ExecuteAsync(async (message, context) =>
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
        public async Task ExecuteAsync_FlushesResourceManagersAsynchronously_IfUnitOfWorkIsMultiThreaded_And_SomeResourceManagersRequireFlush()
        {
            ProcessorBuilder.UnitOfWorkMode = UnitOfWorkMode.MultiThreaded;

            var resourceManagerA = new UnitOfWorkResourceManagerSpy(true);
            var resourceManagerB = new UnitOfWorkResourceManagerSpy(true);
            var resourceManagerC = new UnitOfWorkResourceManagerSpy(false);

            await CreateProcessor().ExecuteAsync(async (message, context) =>
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
        public async Task ExecuteAsync_FlushesResourceManagersAsynchronously_IfUnitOfWorkIsSingleThreaded_And_SomeResourceManagerThrows()
        {
            ProcessorBuilder.UnitOfWorkMode = UnitOfWorkMode.MultiThreaded;

            var exceptionToThrow = new InternalServerErrorException();
            var resourceManagerA = new UnitOfWorkResourceManagerSpy(true);
            var resourceManagerB = new UnitOfWorkResourceManagerSpy(true, exceptionToThrow);
            var resourceManagerC = new UnitOfWorkResourceManagerSpy(true);

            try
            {
                await CreateProcessor().ExecuteAsync(async (message, context) =>
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



        #endregion
    }
}
