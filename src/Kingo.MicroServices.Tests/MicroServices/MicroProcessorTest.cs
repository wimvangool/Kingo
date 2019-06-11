namespace Kingo.MicroServices
{
    //[TestClass]
    //public sealed class MicroProcessorTest
    //{
    //    #region [====== MessageHandlers ======]

    //    [MessageHandlerConfiguration(ServiceLifetime.Transient, HandlesExternalMessages = true, HandlesInternalMessages = true)]
    //    private sealed class ObjectHandler : IMessageHandler<object>
    //    {
    //        public Task HandleAsync(object message, MessageHandlerOperationContext context) =>
    //            Task.CompletedTask;
    //    }

    //    private sealed class StringHandler : IMessageHandler<string>
    //    {
    //        public Task HandleAsync(string message, MessageHandlerOperationContext context) =>
    //            Task.CompletedTask;
    //    }

    //    private sealed class ObjectAndStringHandler : IMessageHandler<object>, IMessageHandler<string>
    //    {
    //        public Task HandleAsync(object message, MessageHandlerOperationContext context) =>
    //            Task.CompletedTask;

    //        public Task HandleAsync(string message, MessageHandlerOperationContext context) =>
    //            Task.CompletedTask;
    //    }

    //    [MessageHandlerConfiguration(ServiceLifetime.Transient, HandlesExternalMessages = false, HandlesInternalMessages = true)]
    //    private sealed class MessageHandlerExceptionThrower : IMessageHandler<object>
    //    {
    //        public Task HandleAsync(object message, MessageHandlerOperationContext context) =>
    //            throw new MessageHandlerOperationException();
    //    }                   

    //    #endregion

    //    #region [====== Messages ======]

    //    private sealed class SomeCommand { }

    //    #endregion

    //    private readonly MicroProcessorBuilder<MicroProcessor> _processor;                

    //    public MicroProcessorTest()
    //    {
    //        _processor = new MicroProcessorBuilder<MicroProcessor>();                        
    //    }

    //    #region [====== HandleAsync - MessageHandler Invocation ======]        

    //    [TestMethod]
    //    [ExpectedException(typeof(ArgumentNullException))]
    //    public async Task HandleAsync_Throws_IfMessageIsNull()
    //    {
    //        await CreateProcessor().HandleAsync<object>(null);
    //    }

    //    [TestMethod]
    //    public async Task HandleAsync_ReturnsMessageHandlerCountOfZero_IfNoMessageHandlersWereInvoked()
    //    {
    //        var result = await CreateProcessor().ExecuteAsync(new object());

    //        Assert.AreEqual(0, result.MessageHandlerCount);
    //    }

    //    [TestMethod]
    //    public async Task HandleAsync_ReturnsMessageHandlerCountOfOne_IfMessageHandlerIsExplicitlySpecified()
    //    {
    //        var result = await CreateProcessor().ExecuteAsync((message, context) => { }, new object());

    //        Assert.AreEqual(1, result.MessageHandlerCount);
    //    }

    //    [TestMethod]
    //    public async Task HandleAsync_ReturnsExpectedMessageHandlerCount_IfMultipleHandlersAreResolvedForMessage()
    //    {
    //        _processor.Components.AddMessageHandler<ObjectHandler>();
    //        _processor.Components.AddMessageHandler<StringHandler>();

    //        var result = await CreateProcessor().ExecuteAsync(string.Empty);

    //        Assert.AreEqual(2, result.MessageHandlerCount);
    //    }

    //    [TestMethod]
    //    public async Task HandleAsync_ReturnsExpectedMessageHandlerCount_IfOneHandlerTriggersAnotherHandler()
    //    {
    //        _processor.Components.AddMessageHandler<ObjectHandler>();

    //        var result = await CreateProcessor().ExecuteAsync(
    //            (message, context) =>
    //            {
    //                context.EventBus.Publish(message);
    //            },
    //            string.Empty);

    //        Assert.AreEqual(2, result.MessageHandlerCount);
    //    }

    //    [TestMethod]
    //    public async Task HandleAsync_ReturnsExpectedMessageHandlerCount_IfOneHandlerIsInvokedTwiceForSameMessage()
    //    {
    //        _processor.Components.AddMessageHandler(new ObjectAndStringHandler());
    //        _processor.Components.AddMessageHandler<ObjectAndStringHandler>();

    //        var result = await CreateProcessor().ExecuteAsync(string.Empty);

    //        Assert.AreEqual(4, result.MessageHandlerCount);
    //    }        

    //    #endregion        

    //    #region [====== HandleAsync - Exception Handling ======]

    //    [TestMethod]        
    //    public async Task HandleAsync_ThrowsBadRequestException_IfInputMessageHandlerThrowsMessageHandlerException_And_InputMessageIsCommand()
    //    {
    //        var exception = await Assert.ThrowsExceptionAsync<BadRequestException>(() => CreateProcessor().ExecuteAsync(
    //            (command, context) =>
    //            {
    //                throw new MessageHandlerOperationException();
    //            },
    //            new SomeCommand()));

    //        Assert.IsInstanceOfType(exception.InnerException, typeof(MessageHandlerOperationException));
    //    }

    //    [TestMethod]
    //    public async Task HandleAsync_ThrowsBadRequestException_IfInputMessageHandlerThrowsBadRequestException_And_InputMessageIsCommand()
    //    {
    //        var exception = await Assert.ThrowsExceptionAsync<BadRequestException>(() => CreateProcessor().ExecuteAsync(
    //            (command, context) =>
    //            {
    //                throw new BadRequestException();
    //            },
    //            new SomeCommand()));

    //        Assert.IsNull(exception.InnerException);
    //    }

    //    [TestMethod]
    //    public async Task HandleAsync_ThrowsInternalServerErrorException_IfInputMessageHandlerThrowsInvalidOperationException_And_InputMessageIsCommand()
    //    {
    //        var exception = await Assert.ThrowsExceptionAsync<InternalServerErrorException>(() => CreateProcessor().ExecuteAsync(
    //            (command, context) =>
    //            {
    //                throw new InvalidOperationException();
    //            },
    //            new SomeCommand()));

    //        Assert.IsInstanceOfType(exception.InnerException, typeof(InvalidOperationException));
    //    }

    //    [TestMethod]
    //    public async Task HandleAsync_ThrowsInternalServerErrorException_IfOutputMessageHandlerThrowsMessageHandlerException_And_InputMessageIsCommand()
    //    {
    //        _processor.Components.AddMessageHandler<MessageHandlerExceptionThrower>();

    //        var exception = await Assert.ThrowsExceptionAsync<InternalServerErrorException>(() => CreateProcessor().ExecuteAsync(
    //            (command, context) =>
    //            {
    //                context.EventBus.Publish(command);
    //            },
    //            new SomeCommand()));

    //        Assert.IsInstanceOfType(exception.InnerException, typeof(MessageHandlerOperationException));
    //    }

    //    [TestMethod]
    //    public async Task HandleAsync_ThrowsInternalServerErrorException_IfInputMessageHandlerThrowsBadRequestException_And_InputMessageIsCommand()
    //    {
    //        var exception = await Assert.ThrowsExceptionAsync<InternalServerErrorException>(() => CreateProcessor().ExecuteAsync(
    //            (message, context) =>
    //            {
    //                throw new BadRequestException();
    //            },
    //            new object()));

    //        Assert.IsInstanceOfType(exception.InnerException, typeof(BadRequestException));
    //    }

    //    [TestMethod]
    //    public async Task HandleAsync_ThrowsInternalServerErrorException_IfInputMessageHandlerThrowsMessageHandlerException_And_InputMessageIsEvent()
    //    {            
    //        var exception = await Assert.ThrowsExceptionAsync<InternalServerErrorException>(() => CreateProcessor().ExecuteAsync(
    //            (message, context) =>
    //            {
    //                throw new MessageHandlerOperationException();
    //            },
    //            new object()));

    //        Assert.IsInstanceOfType(exception.InnerException, typeof(MessageHandlerOperationException));
    //    }

    //    [TestMethod]
    //    public async Task HandleAsync_ThrowsInternalServerErrorException_IfInputMessageHandlerThrowsMicroProcessorException_And_InputMessageIsEvent()
    //    {
    //        var exception = await Assert.ThrowsExceptionAsync<InternalServerErrorException>(() => CreateProcessor().ExecuteAsync(
    //            (message, context) =>
    //            {
    //                throw new InternalServerErrorException();
    //            },
    //            new object()));

    //        Assert.IsNull(exception.InnerException);
    //    }

    //    [TestMethod]
    //    public async Task HandleAsync_ThrowsInternalServerErrorException_IfInputMessageHandlerThrowsInvalidOperationException_And_InputMessageIsEvent()
    //    {
    //        var exception = await Assert.ThrowsExceptionAsync<InternalServerErrorException>(() => CreateProcessor().ExecuteAsync(
    //            (message, context) =>
    //            {
    //                throw new InvalidOperationException();
    //            },
    //            new object()));

    //        Assert.IsInstanceOfType(exception.InnerException, typeof(InvalidOperationException));
    //    }

    //    [TestMethod]
    //    public async Task HandleAsync_ThrowsInternalServerErrorException_IfOutputMessageHandlerThrowsMessageHandlerException_And_InputMessageIsEvent()
    //    {
    //        _processor.Components.AddMessageHandler<MessageHandlerExceptionThrower>();

    //        var exception = await Assert.ThrowsExceptionAsync<InternalServerErrorException>(() => CreateProcessor().ExecuteAsync(
    //            (message, context) =>
    //            {
    //                context.EventBus.Publish(message);
    //            },
    //            new object()));

    //        Assert.IsInstanceOfType(exception.InnerException, typeof(MessageHandlerOperationException));
    //    }

    //    [TestMethod]
    //    public async Task HandleAsync_ThrowsOperationCancelledException_IfInputMessageHandlerCancelsTheOperation()
    //    {
    //        var tokenSource = new CancellationTokenSource();            

    //        await Assert.ThrowsExceptionAsync<OperationCanceledException>(() => CreateProcessor().ExecuteAsync(
    //            (message, context) =>
    //            {                
    //                tokenSource.Cancel();
    //            }, new object(),
    //            tokenSource.Token));
    //    }       

    //    #endregion

    //    #region [====== HandleAsync - UnitOfWork ======]

    //    [TestMethod]
    //    public async Task HandleAsync_DoesNotFlushResourceManager_IfResourceManagerDoesNotNeedToBeFlushed()
    //    {
    //        var resourceManager = new UnitOfWorkResourceManagerSpy(false);

    //        await CreateProcessor().ExecuteAsync(
    //            async (message, context) =>
    //            {
    //                await context.UnitOfWork.EnlistAsync(resourceManager);
    //            },
    //            new object());

    //        resourceManager.AssertRequiresFlushCountIs(1);
    //        resourceManager.AssertFlushCountIs(0);
    //    }

    //    [TestMethod]
    //    public async Task HandleAsync_DoesNotFlushResourceManager_IfResourceManagerNeedsToBeFlushed_But_SomeExceptionIsThrownByMessageHandler()
    //    {
    //        var resourceManager = new UnitOfWorkResourceManagerSpy(true);

    //        await Assert.ThrowsExceptionAsync<InternalServerErrorException>(() => CreateProcessor().ExecuteAsync(
    //            async (message, context) =>
    //            {
    //                await context.UnitOfWork.EnlistAsync(resourceManager);
    //                throw new InvalidOperationException();
    //            },
    //            new object()));

    //        resourceManager.AssertRequiresFlushCountIs(0);
    //        resourceManager.AssertFlushCountIs(0);
    //    }

    //    [TestMethod]
    //    public async Task HandleAsync_FlushesResourceManager_IfResourceManagerNeedsToBeFlushed()
    //    {
    //        var resourceManager = new UnitOfWorkResourceManagerSpy(true);

    //        await CreateProcessor().ExecuteAsync(
    //            async (message, context) =>
    //            {
    //                await context.UnitOfWork.EnlistAsync(resourceManager);
    //            },
    //            new object());

    //        resourceManager.AssertRequiresFlushCountIs(1);
    //        resourceManager.AssertFlushCountIs(1);
    //    }

    //    [TestMethod]
    //    public async Task HandleAsync_FlushesResourceManagerOnlyOnce_IfResourceManagerNeedsToBeFlushed_But_IsEnlistedMoreThanOnce()
    //    {
    //        var resourceManager = new UnitOfWorkResourceManagerSpy(true);

    //        await CreateProcessor().ExecuteAsync(
    //            async (message, context) =>
    //            {
    //                await context.UnitOfWork.EnlistAsync(resourceManager);
    //                await context.UnitOfWork.EnlistAsync(resourceManager);
    //            },
    //            new object());

    //        resourceManager.AssertRequiresFlushCountIs(1);
    //        resourceManager.AssertFlushCountIs(1);
    //    }

    //    [TestMethod]
    //    public async Task HandleAsync_FlushesOnlyThoseResourceManagersThatNeedToBeFlushed_IfMultipleResourceManagersAreEnlisted()
    //    {
    //        var resourceManagerA = new UnitOfWorkResourceManagerSpy(true);
    //        var resourceManagerB = new UnitOfWorkResourceManagerSpy(false);

    //        await CreateProcessor().ExecuteAsync(
    //            async (message, context) =>
    //            {
    //                await context.UnitOfWork.EnlistAsync(resourceManagerA);
    //                await context.UnitOfWork.EnlistAsync(resourceManagerB);
    //            },
    //            new object());

    //        resourceManagerA.AssertRequiresFlushCountIs(1);
    //        resourceManagerA.AssertFlushCountIs(1);

    //        resourceManagerB.AssertRequiresFlushCountIs(1);
    //        resourceManagerB.AssertFlushCountIs(0);
    //    }

    //    [TestMethod]
    //    public async Task HandleAsync_FlushesResourceManagerOnlyOnce_IfUnitOfWorkIsFlushedManually_And_ResourceManagerIsNotEnlistedAfterThat()
    //    {
    //        var resourceManager = new UnitOfWorkResourceManagerSpy(true);

    //        await CreateProcessor().ExecuteAsync(
    //            async (message, context) =>
    //            {
    //                await context.UnitOfWork.EnlistAsync(resourceManager);
    //                await context.UnitOfWork.FlushAsync();
    //            },
    //            new object());

    //        resourceManager.AssertRequiresFlushCountIs(1);
    //        resourceManager.AssertFlushCountIs(1);
    //    }

    //    [TestMethod]
    //    public async Task HandleAsync_FlushesResourceManagerTwice_IfUnitOfWorkIsFlushedManually_And_ResourceManagerIsReenlistedAfterThat()
    //    {
    //        var resourceManager = new UnitOfWorkResourceManagerSpy(true);

    //        await CreateProcessor().ExecuteAsync(
    //            async (message, context) =>
    //            {
    //                await context.UnitOfWork.EnlistAsync(resourceManager);
    //                await context.UnitOfWork.FlushAsync();
    //                await context.UnitOfWork.EnlistAsync(resourceManager);
    //            },
    //            new object());

    //        resourceManager.AssertRequiresFlushCountIs(2);
    //        resourceManager.AssertFlushCountIs(2);
    //    }

    //    [TestMethod]
    //    public async Task HandleAsync_FlushesBothResourceManagersInParrallel_IfResourceManagersHaveDifferentResourceIds()
    //    {            
    //        var resourceManagerA = new UnitOfWorkResourceManagerSpy(true, new object());                       
    //        var resourceManagerB = new UnitOfWorkResourceManagerSpy(true, new object());            

    //        await CreateProcessor().ExecuteAsync(
    //            async (message, context) =>
    //            {
    //                await context.UnitOfWork.EnlistAsync(resourceManagerA);
    //                await context.UnitOfWork.EnlistAsync(resourceManagerB);                
    //            },
    //            new object());

    //        resourceManagerA.AssertRequiresFlushCountIs(1);
    //        resourceManagerA.AssertFlushCountIs(1);

    //        resourceManagerB.AssertRequiresFlushCountIs(1);
    //        resourceManagerB.AssertFlushCountIs(1);            
    //    }

    //    [TestMethod]
    //    public async Task HandleAsync_ThrowsConflictException_IfFlushAsyncThrowsConcurrencyException_And_MessageIsCommand()
    //    {
    //        var resourceManager = new UnitOfWorkResourceManagerSpy(true, new ConcurrencyException());

    //        var exception = await Assert.ThrowsExceptionAsync<ConflictException>(() => CreateProcessor().ExecuteAsync(
    //            async (command, context) =>
    //            {
    //                await context.UnitOfWork.EnlistAsync(resourceManager);                
    //            },
    //            new SomeCommand()));

    //        resourceManager.AssertRequiresFlushCountIs(1);
    //        resourceManager.AssertFlushCountIs(1);

    //        Assert.IsInstanceOfType(exception.InnerException, typeof(ConcurrencyException));
    //    }

    //    [TestMethod]
    //    public async Task HandleAsync_ThrowsInternalServerErrorException_IfFlushAsyncThrowsConcurrencyException_And_MessageIsEvent()
    //    {
    //        var resourceManager = new UnitOfWorkResourceManagerSpy(true, new ConcurrencyException());

    //        var exception = await Assert.ThrowsExceptionAsync<InternalServerErrorException>(() => CreateProcessor().ExecuteAsync(
    //            async (message, context) =>
    //            {
    //                await context.UnitOfWork.EnlistAsync(resourceManager);
    //            },
    //            new object()));

    //        resourceManager.AssertRequiresFlushCountIs(1);
    //        resourceManager.AssertFlushCountIs(1);

    //        Assert.IsInstanceOfType(exception.InnerException, typeof(ConcurrencyException));
    //    }

    //    #endregion

    //    #region [====== HandleAsync - Event Publication ======]

    //    [TestMethod]
    //    public async Task HandleAsync_PublishesNoEvents_IfNoHandlerIsInvoked()
    //    {
    //        var result = await CreateProcessor().ExecuteAsync(new object());

    //        Assert.AreEqual(0, result.MessageHandlerCount);
    //        Assert.AreEqual(0, result.Events.Count);
    //    }

    //    [TestMethod]
    //    public async Task HandleAsync_PublishesExpectedEvent_IfExplicitHandlerPublishesEvent()
    //    {
    //        var @event = new object();

    //        var result = await CreateProcessor().ExecuteAsync(
    //            (message, context) =>
    //            {
    //                context.EventBus.Publish(@event);
    //            },
    //            new object());

    //        Assert.AreEqual(1, result.Events.Count);
    //        Assert.AreSame(@event, result.Events[0]);
    //    }

    //    [TestMethod]
    //    public async Task HandleAsync_PublishesExpectedEvents_IfSeveralHandlersPublishEvents()
    //    {
    //        var eventA = string.Empty;
    //        var eventB = new object();

    //        _processor.Components.AddMessageHandler<string>((message, context) =>
    //        {
    //            context.EventBus.Publish(eventB);
    //        }, MicroProcessorOperationType.OutputStream);

    //        var result = await CreateProcessor().ExecuteAsync(
    //            (message, context) =>
    //            {
    //                context.EventBus.Publish(eventA);
    //            },
    //            new object());

    //        Assert.AreEqual(2, result.Events.Count);
    //        Assert.AreSame(eventA, result.Events[0]);
    //        Assert.AreSame(eventB, result.Events[1]);
    //    }

    //    [TestMethod]
    //    public async Task HandleAsync_PublishesNoEvents_IfSeveralHandlersPublishEvents_But_ExceptionIsThrown()
    //    {
    //        var eventA = string.Empty;
    //        var eventB = new object();

    //        _processor.Components.AddMessageHandler<string>((message, context) =>
    //        {
    //            context.EventBus.Publish(eventB);
    //        });

    //        await Assert.ThrowsExceptionAsync<InternalServerErrorException>(() => CreateProcessor().ExecuteAsync(
    //            (message, context) =>
    //            {
    //                context.EventBus.Publish(eventA);
    //                throw new InvalidOperationException();
    //            },
    //            new object()));            
    //    }

    //    #endregion

    //    #region [====== HandleAsync - Operation ======]

    //    [TestMethod]
    //    public async Task HandleAsync_IsInvokedWithinExpectedContext_IfInputMessageHandlerIsInvoked()
    //    {
    //        var command = new object();

    //        await CreateProcessor().ExecuteAsync(
    //            (message, context) =>
    //            {
    //                Assert.AreSame(command, message);
    //                Assert.AreSame(command, context.Operation.Message);
    //                Assert.AreEqual(MicroProcessorOperationType.InputStream, context.Operation.Type);
    //                Assert.AreEqual(1, context.Operation.StackTrace().Count());
    //            },
    //            command);
    //    }

    //    [TestMethod]
    //    public async Task HandleAsync_IsInvokedWithinExpectedContext_IfOutputMessageHandlerIsInvoked_Depth1()
    //    {
    //        var @event = new object();

    //        _processor.Components.AddMessageHandler<object>((message, context) =>
    //        {
    //            Assert.AreSame(@event, message);
    //            Assert.AreSame(@event, context.Operation.Message);
    //            Assert.AreEqual(MicroProcessorOperationType.OutputStream, context.Operation.Type);
    //            Assert.AreEqual(2, context.Operation.StackTrace().Count());
    //        });

    //        await CreateProcessor().ExecuteAsync(
    //            (message, context) =>
    //            {
    //                context.EventBus.Publish(@event);
    //            },
    //            new object());
    //    }

    //    [TestMethod]
    //    public async Task HandleAsync_IsInvokedWithinExpectedContext_IfOutputMessageHandlerIsInvoked_Depth2()
    //    {
    //        var @event = string.Empty;

    //        _processor.Components.AddMessageHandler<int>((message, context) =>
    //        {
    //            context.EventBus.Publish(@event);
    //        }, MicroProcessorOperationType.OutputStream);

    //        _processor.Components.AddMessageHandler<string>((message, context) =>
    //        {
    //            Assert.AreSame(@event, message);
    //            Assert.AreSame(@event, context.Operation.Message);
    //            Assert.AreEqual(MicroProcessorOperationType.OutputStream, context.Operation.Type);
    //            Assert.AreEqual(3, context.Operation.StackTrace().Count());
    //        }, MicroProcessorOperationType.OutputStream);

    //        var result = await CreateProcessor().ExecuteAsync(
    //            (message, context) =>
    //            {
    //                context.EventBus.Publish(DateTimeOffset.UtcNow.Second);
    //            },
    //            new object());

    //        Assert.AreEqual(3, result.MessageHandlerCount);
    //    }

    //    #endregion        

    //    #region [====== ExecuteAsync1 ======]

    //    [TestMethod]
    //    [ExpectedException(typeof(ArgumentNullException))]
    //    public async Task ExecuteAsync1_Throws_IfQueryIsNull()
    //    {
    //        await CreateProcessor().ExecuteAsync<object>(null);
    //    }

    //    [TestMethod]
    //    public async Task ExecuteAsync1_ReturnsResultWithNullValue_IfQueryReturnsNull()
    //    {
    //        var result = await CreateProcessor().ExecuteAsync(context => null as object);

    //        Assert.IsNotNull(result);
    //        Assert.IsNull(result.Response);
    //    }

    //    [TestMethod]
    //    public async Task ExecuteAsync1_ReturnsResultWithResponse_IfQueryReturnsResponse()
    //    {
    //        var response = new object();
    //        var result = await CreateProcessor().ExecuteAsync(context => response);

    //        Assert.AreSame(response, result.Response);
    //    }

    //    [TestMethod]
    //    public async Task ExecuteAsync1_ThrowsBadRequestException_IfQueryThrowsBadRequestException()
    //    {
    //        var exceptionToThrow = new BadRequestException();
    //        var exception = await Assert.ThrowsExceptionAsync<BadRequestException>(() => CreateProcessor().ExecuteAsync<object>(context =>
    //        {
    //            throw exceptionToThrow;
    //        }));

    //        Assert.AreSame(exceptionToThrow, exception);
    //    }

    //    [TestMethod]
    //    public async Task ExecuteAsync1_ThrowsInternalServerErrorException_IfQueryThrowsInternalServerErrorException()
    //    {
    //        var exceptionToThrow = new InternalServerErrorException();
    //        var exception = await Assert.ThrowsExceptionAsync<InternalServerErrorException>(() => CreateProcessor().ExecuteAsync<object>(context =>
    //        {
    //            throw exceptionToThrow;
    //        }));

    //        Assert.AreSame(exceptionToThrow, exception);
    //    }

    //    [TestMethod]
    //    public async Task ExecuteAsync1_ThrowsInternalServerErrorException_IfQueryThrowsInvalidOperationException()
    //    {
    //        var exceptionToThrow = new InvalidOperationException();
    //        var exception = await Assert.ThrowsExceptionAsync<InternalServerErrorException>(() => CreateProcessor().ExecuteAsync<object>(context =>
    //        {
    //            throw exceptionToThrow;
    //        }));

    //        Assert.AreSame(exceptionToThrow, exception.InnerException);
    //    }

    //    [TestMethod]
    //    public async Task ExecuteAsync1_IsInvokedWithinExpectedContext()
    //    {            
    //        await CreateProcessor().ExecuteAsync(context =>
    //        {                
    //            Assert.IsNull(context.Operation.Message);
    //            Assert.AreEqual(MicroProcessorOperationType.Query, context.Operation.Type);
    //            Assert.AreEqual(1, context.Operation.StackTrace().Count());
    //            return new object();
    //        });
    //    }

    //    #endregion

    //    #region [====== ExecuteAsync2 ======]

    //    [TestMethod]
    //    [ExpectedException(typeof(ArgumentNullException))]
    //    public async Task ExecuteAsync2_Throws_IfQueryIsNull()
    //    {
    //        await CreateProcessor().ExecuteAsync<object, object>(null, new object());
    //    }

    //    [TestMethod]
    //    [ExpectedException(typeof(ArgumentNullException))]
    //    public async Task ExecuteAsync2_Throws_IfMessageIsNull()
    //    {
    //        await CreateProcessor().ExecuteAsync<object, object>((message, context) => null, null);
    //    }

    //    [TestMethod]
    //    public async Task ExecuteAsync2_ReturnsResultWithNullValue_IfQueryReturnsNull()
    //    {
    //        var result = await CreateProcessor().ExecuteAsync((message, context) => null as object, new object());

    //        Assert.IsNotNull(result);
    //        Assert.IsNull(result.Response);
    //    }

    //    [TestMethod]
    //    public async Task ExecuteAsync2_ReturnsResultWithResponse_IfQueryReturnsResponse()
    //    {
    //        var response = new object();
    //        var result = await CreateProcessor().ExecuteAsync((message, context) => response, new object());

    //        Assert.AreSame(response, result.Response);
    //    }

    //    [TestMethod]
    //    public async Task ExecuteAsync2_ThrowsBadRequestException_IfQueryThrowsBadRequestException()
    //    {
    //        var exceptionToThrow = new BadRequestException();
    //        var exception = await Assert.ThrowsExceptionAsync<BadRequestException>(() => CreateProcessor().ExecuteAsync<object, object>(
    //            (message, context) =>
    //            {
    //                throw exceptionToThrow;
    //            },
    //            new object()));

    //        Assert.AreSame(exceptionToThrow, exception);
    //    }

    //    [TestMethod]
    //    public async Task ExecuteAsync2_ThrowsInternalServerErrorException_IfQueryThrowsInternalServerErrorException()
    //    {
    //        var exceptionToThrow = new InternalServerErrorException();
    //        var exception = await Assert.ThrowsExceptionAsync<InternalServerErrorException>(() => CreateProcessor().ExecuteAsync<object, object>(
    //            (message, context) =>
    //            {
    //                throw exceptionToThrow;
    //            },
    //            new object()));

    //        Assert.AreSame(exceptionToThrow, exception);
    //    }

    //    [TestMethod]
    //    public async Task ExecuteAsync2_ThrowsInternalServerErrorException_IfQueryThrowsInvalidOperationException()
    //    {
    //        var exceptionToThrow = new InvalidOperationException();
    //        var exception = await Assert.ThrowsExceptionAsync<InternalServerErrorException>(() => CreateProcessor().ExecuteAsync<object, object>(
    //            (message, context) =>
    //            {
    //                throw exceptionToThrow;
    //            },
    //            new object()));

    //        Assert.AreSame(exceptionToThrow, exception.InnerException);
    //    }

    //    [TestMethod]
    //    public async Task ExecuteAsync2_IsInvokedWithinExpectedContext()
    //    {
    //        var request = new object();

    //        await CreateProcessor().ExecuteAsync(
    //            (message, context) =>
    //            {
    //                Assert.AreSame(request, message);
    //                Assert.AreSame(request, context.Operation.Message);
    //                Assert.AreEqual(MicroProcessorOperationType.Query, context.Operation.Type);
    //                Assert.AreEqual(1, context.Operation.StackTrace().Count());
    //                return new object();
    //            },
    //            request);
    //    }

    //    #endregion

    //    private IMicroProcessor CreateProcessor() =>
    //        _processor.BuildServiceCollection().BuildServiceProvider().GetRequiredService<IMicroProcessor>();
    //}
}
