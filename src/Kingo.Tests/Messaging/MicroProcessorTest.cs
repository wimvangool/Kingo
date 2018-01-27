using System;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Messaging.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class MicroProcessorTest
    {
        private MicroProcessorSpy _processor;

        [TestInitialize]
        public void Setup()
        {
            _processor = new MicroProcessorSpy();
        }

        #region [====== Commands & Events (Input/Output Tests) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Handle_Throws_IfMessageIsNull()
        {
            _processor.Handle(null as object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HandleStream_Throws_IfStreamIsNull()
        {
            _processor.HandleStream(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HandleAsync_Throws_IfMessageIsNull()
        {
            _processor.HandleAsync(null as object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HandleStreamAsync_Throws_IfStreamIsNull()
        {            
            _processor.HandleStreamAsync(null);
        }

        [TestMethod]
        public async Task HandleStreamAsync_ReturnsEmptyStream_IfStreamIsEmpty() =>
             AssertIsEmpty(await _processor.HandleStreamAsync(MessageStream.Empty));

        [TestMethod]
        public async Task HandleStreamAsync_ReturnsEmptyStream_IfStreamContainsOneMessage_But_NoHandlerForMessageIsRegistered() =>
             AssertIsEmpty(await _processor.HandleAsync(new object()));

        [TestMethod]
        public async Task HandleStreamAsync_ReturnsExpectedStream_IfStreamContainsOneMessage()
        {
            var messageIn = new object();
            var messageOut = new object();
            
            AssertStream(await _processor.HandleAsync(messageIn, (message, context) =>
            {
                Assert.AreSame(messageIn, message);

                context.OutputStream.Publish(messageOut);

            }), messageOut);           
        }

        [TestMethod]
        public async Task HandleStreamAsync_ReturnsExpectedStream_IfStreamContainsMultipleMessages()
        {
            var messageInA = new object();
            var messageOutA = new object();

            var messageInB = new object();
            var messageOutB = new object();

            var streamIn = MessageStream.CreateStream(messageInA, (message, context) =>
            {
                Assert.AreSame(messageInA, message);

                context.OutputStream.Publish(messageOutA);

            }).Append(messageInB, (message, context) =>
            {
                Assert.AreSame(messageInB, message);

                context.OutputStream.Publish(messageOutB);
            });
            
            AssertStream(await _processor.HandleStreamAsync(streamIn), messageOutA, messageOutB);            
        }

        #endregion

        #region [====== Commands & Events (Registered Handler Invocation Tests) ======]

        private sealed class SomeCommand { }

        private sealed class EventA { }

        private sealed class EventB { }

        private sealed class EventC { }

        [MessageHandler(InstanceLifetime.PerUnitOfWork, MessageSources.InputStream | MessageSources.OutputStream)]
        private sealed class SomeCommandHandler : IMessageHandler<SomeCommand>
        {
            private readonly IMessageHandlerImplementation _implementation;

            public SomeCommandHandler(IMessageHandlerImplementation implementation)
            {
                _implementation = implementation;
            }

            public Task HandleAsync(SomeCommand message, IMicroProcessorContext context) =>
                _implementation.HandleAsync(message, context, GetType());
        }

        [MessageHandler(InstanceLifetime.PerUnitOfWork, MessageSources.All)]
        private sealed class EventHandlerAB : IMessageHandler<EventA>, IMessageHandler<EventB>
        {
            private readonly IMessageHandlerImplementation _implementation;

            public EventHandlerAB(IMessageHandlerImplementation implementation)
            {
                _implementation = implementation;
            }

            public Task HandleAsync(EventA message, IMicroProcessorContext context) =>
                _implementation.HandleAsync(message, context, GetType());

            public Task HandleAsync(EventB message, IMicroProcessorContext context) =>
                _implementation.HandleAsync(message, context, GetType());
        }

        [MessageHandler(InstanceLifetime.PerUnitOfWork, MessageSources.All)]
        private sealed class EventHandlerC : IMessageHandler<EventC>
        {
            private readonly IMessageHandlerImplementation _implementation;

            public EventHandlerC(IMessageHandlerImplementation implementation)
            {
                _implementation = implementation;
            }

            public Task HandleAsync(EventC message, IMicroProcessorContext context) =>
                _implementation.HandleAsync(message, context, GetType());
        }

        [MessageHandler(InstanceLifetime.PerUnitOfWork, MessageSources.All)]
        private sealed class ObjectHandler : IMessageHandler<object>, IMessageHandler<string>
        {
            private readonly IMessageHandlerImplementation _implementation;

            public ObjectHandler(IMessageHandlerImplementation implementation)
            {
                _implementation = implementation;
            }

            public Task HandleAsync(object message, IMicroProcessorContext context) =>
                _implementation.HandleAsync(message, context, GetType());

            public Task HandleAsync(string message, IMicroProcessorContext context) =>
                _implementation.HandleAsync(message, context, GetType());
        }

        [MessageHandler(InstanceLifetime.PerUnitOfWork, MessageSources.InputStream)]
        private sealed class ExternalMessageHandler : IMessageHandler<EventA>
        {
            private readonly IMessageHandlerImplementation _implementation;

            public ExternalMessageHandler(IMessageHandlerImplementation implementation)
            {
                _implementation = implementation;
            }

            public Task HandleAsync(EventA message, IMicroProcessorContext context) =>
                _implementation.HandleAsync(message, context, GetType());
        }

        [MessageHandler(InstanceLifetime.PerUnitOfWork, MessageSources.OutputStream)]
        private sealed class InternalMessageHandler : IMessageHandler<EventB>
        {
            private readonly IMessageHandlerImplementation _implementation;

            public InternalMessageHandler(IMessageHandlerImplementation implementation)
            {
                _implementation = implementation;
            }

            public Task HandleAsync(EventB message, IMicroProcessorContext context) =>
                _implementation.HandleAsync(message, context, GetType());
        }

        [MessageHandler(InstanceLifetime.PerUnitOfWork, MessageSources.MetadataStream)]
        private sealed class MetadataEventHandlerAB : IMessageHandler<EventA>, IMessageHandler<EventB>
        {
            private readonly IMessageHandlerImplementation _implementation;

            public MetadataEventHandlerAB(IMessageHandlerImplementation implementation)
            {
                _implementation = implementation;
            }

            public Task HandleAsync(EventA message, IMicroProcessorContext context) =>
                _implementation.HandleAsync(message, context, GetType());

            public Task HandleAsync(EventB message, IMicroProcessorContext context) =>
                _implementation.HandleAsync(message, context, GetType());
        }

        [TestMethod]
        public async Task HandleAsyncStream_WillInvokeHandlers_IfAllHandlersAreRegistered_And_StreamContainsOneCommand()
        {
            var someCommand = new SomeCommand(); 
            var eventA = new EventA();
            var eventB = new EventB();   
            var eventC = new EventC();
            
            var unitOfWork = new UnitOfWorkSpy(true);        

            _processor.Implement<SomeCommandHandler>().As<SomeCommand>((message, context) =>
            {                
                AssertMessageStack(context.Messages, message, someCommand);

                context.UnitOfWork.Enlist(unitOfWork);
                context.OutputStream.Publish(eventA);
                context.OutputStream.Publish(eventB);
            });

            _processor.Implement<EventHandlerAB>().As<EventA>((message, context) =>
            {                
                AssertMessageStack(context.Messages, message, someCommand, eventA);

                context.UnitOfWork.Enlist(unitOfWork);
                context.OutputStream.Publish(eventC);
            });

            _processor.Implement<EventHandlerC>().As<EventC>((message, context) =>
            {
                AssertMessageStack(context.Messages, message, someCommand, eventA, eventC);               
            });

            _processor.Implement<EventHandlerAB>().As<EventB>((message, context) =>
            {
                AssertMessageStack(context.Messages, message, someCommand, eventB);               
            });

            AssertStream(await _processor.HandleAsync(someCommand), eventA, eventC, eventB);            

            unitOfWork.AssertRequiresFlushCountIs(1);
            unitOfWork.AssertFlushCountIs(1);
        }

        [TestMethod]
        public async Task HandleStreamAsync_WillInvokeSameHandlerAgainAndAgain_IfHandlerHandlesAnyTypeOfMessage()
        {
            var someCommand = new SomeCommand();
            var eventA = new EventA();
            var eventB = new EventB();
            var eventC = new EventC();

            _processor.Implement<ObjectHandler>().As<SomeCommand>((message, context) =>
            {
                AssertMessageStack(context.Messages, message, someCommand);

                context.OutputStream.Publish(eventA);
            });

            _processor.Implement<ObjectHandler>().As<EventA>((message, context) =>
            {
                AssertMessageStack(context.Messages, message, someCommand, eventA);

                context.OutputStream.Publish(eventB);
            });

            _processor.Implement<ObjectHandler>().As<EventB>((message, context) =>
            {
                AssertMessageStack(context.Messages, message, someCommand, eventA, eventB);

                context.OutputStream.Publish(eventC);
            });

            _processor.Implement<ObjectHandler>().As<EventC>((message, context) =>
            {
                AssertMessageStack(context.Messages, message, someCommand, eventA, eventB, eventC);                
            });

            AssertStream(await _processor.HandleAsync(someCommand), eventA, eventB, eventC);
        }

        [TestMethod]
        public async Task HandleStreamAsync_WillInvokeCorrectOverloadsOfHandler_IfHandlerContainsMultipleTargetOverloadsForMessage()
        {
            var emptyString = string.Empty;

            _processor.Implement<ObjectHandler>().As<object>((message, context) =>
            {
                AssertMessageStack(context.Messages, message, emptyString);                
            });

            _processor.Implement<ObjectHandler>().As<string>((message, context) =>
            {
                AssertMessageStack(context.Messages, message, emptyString);
            });

            AssertIsEmpty(await _processor.HandleAsync(emptyString));
        }

        [TestMethod]
        public async Task HandleStreamAsync_WillNotInvokeHandler_IfMessageIsExternal_And_HandlerOnlyAcceptsInternalMessages()
        {
            _processor.Register<InternalMessageHandler>();

            AssertIsEmpty(await _processor.HandleAsync(new EventB()));
        }

        [TestMethod]
        public async Task HandleStreamAsync_WillNotInvokeHandler_IfMessageIsInternal_And_HandlerOnlyAcceptsExternalMessages()
        {
            var someCommand = new SomeCommand();
            var eventA = new EventA();

            _processor.Implement<SomeCommandHandler>().As<SomeCommand>((message, context) =>
            {
                AssertMessageStack(context.Messages, message, someCommand);

                context.OutputStream.Publish(eventA);
            });

            _processor.Register<ExternalMessageHandler>();

            AssertStream(await _processor.HandleAsync(someCommand), eventA);
        }

        [TestMethod]
        public async Task HandleStreamAsync_WillInvokeRegisteredMetadataMessageHandler_IfFirstHandlerPublishesMetadataMessages()
        {
            var someCommand = new SomeCommand();
            var eventA = new EventA();
            var unitOfWork = new UnitOfWorkSpy(false);

            _processor.Implement<SomeCommandHandler>().As<SomeCommand>((message, context) =>
            {
                AssertMessageStack(context.Messages, message, someCommand);

                context.UnitOfWork.Enlist(unitOfWork);
                context.MetadataStream.Publish(eventA);
            });

            _processor.Implement<MetadataEventHandlerAB>().As<EventA>((message, context) =>
            {
                AssertMessageStack(context.Messages, message, someCommand, eventA);

                context.UnitOfWork.Enlist(unitOfWork);
            });

            AssertIsEmpty(await _processor.HandleAsync(someCommand));

            unitOfWork.AssertRequiresFlushCountIs(2);
            unitOfWork.AssertFlushCountIs(0);
        }

        [TestMethod]
        public async Task HandleStreamAsync_WillInvokeRegisteredMetadataMessageHandler_IfMetadataHandlerPublishesMetadataMessagesItself()
        {
            var someCommand = new SomeCommand();
            var eventA = new EventA();
            var eventB = new EventB();
            var unitOfWork = new UnitOfWorkSpy(false);

            _processor.Implement<SomeCommandHandler>().As<SomeCommand>((message, context) =>
            {
                AssertMessageStack(context.Messages, message, someCommand);

                context.UnitOfWork.Enlist(unitOfWork);
                context.MetadataStream.Publish(eventA);
            });

            _processor.Implement<MetadataEventHandlerAB>().As<EventA>((message, context) =>
            {
                AssertMessageStack(context.Messages, message, someCommand, eventA);

                context.UnitOfWork.Enlist(unitOfWork);
                context.MetadataStream.Publish(eventB);
            });

            _processor.Implement<MetadataEventHandlerAB>().As<EventB>((message, context) =>
            {
                AssertMessageStack(context.Messages, message, someCommand, eventA, eventB);

                context.UnitOfWork.Enlist(unitOfWork);                
            });

            AssertIsEmpty(await _processor.HandleAsync(someCommand));

            unitOfWork.AssertRequiresFlushCountIs(2);
            unitOfWork.AssertFlushCountIs(0);
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public void HandleStreamAsync_Throws_IfMetadataHandlerAttemptsToPublishOutputMessage()
        {
            var someCommand = new SomeCommand();
            var eventA = new EventA();            

            _processor.Implement<SomeCommandHandler>().As<SomeCommand>((message, context) =>
            {
                AssertMessageStack(context.Messages, message, someCommand);
                
                context.MetadataStream.Publish(eventA);
            });

            _processor.Implement<MetadataEventHandlerAB>().As<EventA>((message, context) =>
            {
                AssertMessageStack(context.Messages, message, someCommand, eventA);
                
                context.OutputStream.Publish(new object());
            });

            try
            {
                _processor.Handle(someCommand);
            }
            catch (InternalServerErrorException exception)
            {
                Assert.IsInstanceOfType(exception.InnerException, typeof(InvalidOperationException));
                throw;
            }            
        }

        #endregion

        #region [====== Commands & Events (Exception Management Tests) ======]

        private sealed class SomeInternalProcessorException : InternalProcessorException { }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public void HandleStream_ThrowsBadRequestException_IfHandlerThrowsInternalProcessorException_And_FirstMessageIsCommand()
        {
            var someCommand = new SomeCommand();
            var internalException = new SomeInternalProcessorException();
            var unitOfWork = new UnitOfWorkSpy(true);

            _processor.Implement<SomeCommandHandler>().As<SomeCommand>((message, context) =>
            {
                context.UnitOfWork.Enlist(unitOfWork);

                throw internalException;
            });

            try
            {
                _processor.Handle(someCommand);
            }
            catch (BadRequestException exception)
            {                
                Assert.AreSame(someCommand, exception.FailedMessage);
                Assert.AreSame(internalException, exception.InnerException);
                throw;
            }
            finally
            {
                unitOfWork.AssertRequiresFlushCountIs(0);
                unitOfWork.AssertFlushCountIs(0);
            }
        }

        [TestMethod]        
        [ExpectedException(typeof(BadRequestException))]        
        public async Task HandleStreamAsync_ThrowsBadRequestException_IfHandlerThrowsInternalProcessorException_And_FirstMessageIsCommand()
        {
            var someCommand = new SomeCommand();
            var internalException = new SomeInternalProcessorException();
            var unitOfWork = new UnitOfWorkSpy(true);

            _processor.Implement<SomeCommandHandler>().As<SomeCommand>((message, context) =>
            {
                context.UnitOfWork.Enlist(unitOfWork);

                throw internalException;
            });

            try
            {
                await _processor.HandleAsync(someCommand);
            }
            catch (BadRequestException exception)
            {                                                                
                Assert.AreSame(someCommand, exception.FailedMessage);
                Assert.AreSame(internalException, exception.InnerException);
                throw;
            }
            finally
            {
                unitOfWork.AssertRequiresFlushCountIs(0);
                unitOfWork.AssertFlushCountIs(0);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public async Task HandleStreamAsync_ThrowsBadRequestException_IfHandlerThrowsInternalProcessorException_And_SecondMessageIsCommand()
        {
            var eventA = new EventA();
            var someCommand = new SomeCommand();
            var inputStream = MessageStream.CreateStream(eventA).Append(someCommand);

            var internalException = new SomeInternalProcessorException();
            var unitOfWork = new UnitOfWorkSpy(true);

            _processor.Implement<EventHandlerAB>().As<EventA>((message, context) => {});

            _processor.Implement<SomeCommandHandler>().As<SomeCommand>((message, context) =>
            {
                context.UnitOfWork.Enlist(unitOfWork);

                throw internalException;
            });

            try
            {
                await _processor.HandleStreamAsync(inputStream);
            }
            catch (BadRequestException exception)
            {
                Assert.AreSame(someCommand, exception.FailedMessage);
                Assert.AreSame(internalException, exception.InnerException);
                throw;
            }
            finally
            {
                unitOfWork.AssertRequiresFlushCountIs(0);
                unitOfWork.AssertFlushCountIs(0);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public async Task HandleStreamAsync_ThrowsBadRequestException_IfAsyncHandlerThrowsInternalProcessorException_And_FirstMessageIsCommand()
        {
            var someCommand = new SomeCommand();
            var internalException = new SomeInternalProcessorException();
            var unitOfWork = new UnitOfWorkSpy(true);

            _processor.Implement<SomeCommandHandler>().AsAsync<SomeCommand>(async (message, context) =>
            {                
                await context.UnitOfWork.EnlistAsync(unitOfWork);

                throw internalException;
            });

            try
            {
                await _processor.HandleAsync(someCommand);
            }
            catch (BadRequestException exception)
            {                                
                Assert.AreSame(someCommand, exception.FailedMessage);
                Assert.AreSame(internalException, exception.InnerException);
                throw;
            }
            finally
            {
                unitOfWork.AssertRequiresFlushCountIs(0);
                unitOfWork.AssertFlushCountIs(0);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task HandleStreamAsync_ThrowsInternalServerErrorException_IfAsyncHandlerThrowsRandomException_And_FirstMessageIsCommand()
        {
            var someCommand = new SomeCommand();
            var randomException = new Exception();
            var unitOfWork = new UnitOfWorkSpy(true);

            _processor.Implement<SomeCommandHandler>().AsAsync<SomeCommand>(async (message, context) =>
            {
                await context.UnitOfWork.EnlistAsync(unitOfWork);

                throw randomException;
            });

            try
            {
                await _processor.HandleAsync(someCommand);
            }
            catch (BadRequestException exception)
            {
                Assert.AreSame(someCommand, exception.FailedMessage);
                Assert.AreSame(randomException, exception.InnerException);
                throw;
            }
            finally
            {
                unitOfWork.AssertRequiresFlushCountIs(0);
                unitOfWork.AssertFlushCountIs(0);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public void HandleStream_ThrowsInternalServerErrorException_IfHandlerThrowsInternalProcessorException_And_FirstMessageIsEvent()
        {
            var eventA = new EventA();
            var internalException = new SomeInternalProcessorException();
            var unitOfWork = new UnitOfWorkSpy(true);

            _processor.Implement<EventHandlerAB>().As<EventA>((message, context) =>
            {
                context.UnitOfWork.Enlist(unitOfWork);

                throw internalException;
            });

            try
            {
                _processor.Handle(eventA);
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(eventA, exception.FailedMessage);
                Assert.AreSame(internalException, exception.InnerException);
                throw;
            }
            finally
            {
                unitOfWork.AssertRequiresFlushCountIs(0);
                unitOfWork.AssertFlushCountIs(0);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task HandleStreamAsync_ThrowsInternalServerErrorException_IfHandlerThrowsInternalProcessorException_And_FirstMessageIsEvent()
        {
            var eventA = new EventA();
            var internalException = new SomeInternalProcessorException();
            var unitOfWork = new UnitOfWorkSpy(true);

            _processor.Implement<EventHandlerAB>().As<EventA>((message, context) =>
            {
                context.UnitOfWork.Enlist(unitOfWork);

                throw internalException;
            });

            try
            {
                await _processor.HandleAsync(eventA);
            }
            catch (InternalServerErrorException exception)
            {                                
                Assert.AreSame(eventA, exception.FailedMessage);
                Assert.AreSame(internalException, exception.InnerException);
                throw;
            }
            finally
            {
                unitOfWork.AssertRequiresFlushCountIs(0);
                unitOfWork.AssertFlushCountIs(0);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task HandleStreamAsync_ThrowsInternalServerErrorException_IfHandlerThrowsInternalProcessorException_And_ChildMessageIsEvent()
        {            
            var someCommand = new SomeCommand();
            var eventA = new EventA();
            var internalException = new SomeInternalProcessorException();
            var unitOfWork = new UnitOfWorkSpy(true);

            _processor.Implement<SomeCommandHandler>().As<SomeCommand>((message, context) =>
            {
                context.UnitOfWork.Enlist(unitOfWork);
                context.OutputStream.Publish(eventA);
                
            });

            _processor.Implement<EventHandlerAB>().As<EventA>((message, context) =>
            {
                context.UnitOfWork.Enlist(unitOfWork);

                throw internalException;
            });            

            try
            {
                await _processor.HandleAsync(someCommand);
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(eventA, exception.FailedMessage);
                Assert.AreSame(internalException, exception.InnerException);
                throw;
            }
            finally
            {
                unitOfWork.AssertRequiresFlushCountIs(0);
                unitOfWork.AssertFlushCountIs(0);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task HandleStreamAsync_ThrowsInternalServerErrorException_IfHandlerThrowsInternalProcessorException_And_ChildMessageIsCommand()
        {
            var eventA = new EventA();
            var someCommand = new SomeCommand();
            var internalException = new SomeInternalProcessorException();
            var unitOfWork = new UnitOfWorkSpy(true);

            _processor.Implement<EventHandlerAB>().As<EventA>((message, context) =>
            {
                context.UnitOfWork.Enlist(unitOfWork);

                context.OutputStream.Publish(someCommand);
            });

            _processor.Implement<SomeCommandHandler>().As<SomeCommand>((message, context) =>
            {
                context.UnitOfWork.Enlist(unitOfWork);

                throw internalException;
            });

            try
            {
                await _processor.HandleAsync(eventA);
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(someCommand, exception.FailedMessage);
                Assert.AreSame(internalException, exception.InnerException);
                throw;
            }
            finally
            {
                unitOfWork.AssertRequiresFlushCountIs(0);
                unitOfWork.AssertFlushCountIs(0);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task HandleStreamAsync_ThrowsInternalServerErrorException_IfMetadataHandlerThrowsInternalProcessorException()
        {
            var someCommand = new SomeCommand();
            var eventA = new EventA();
            var internalException = new SomeInternalProcessorException();
            var unitOfWorkA = new UnitOfWorkSpy(true);
            var unitOfWorkB = new UnitOfWorkSpy(true);

            _processor.Implement<SomeCommandHandler>().As<SomeCommand>((message, context) =>
            {
                context.UnitOfWork.Enlist(unitOfWorkA);
                context.MetadataStream.Publish(eventA);
            });

            _processor.Implement<MetadataEventHandlerAB>().As<EventA>((message, context) =>
            {
                context.UnitOfWork.Enlist(unitOfWorkB);

                throw internalException;
            });

            try
            {
                await _processor.HandleAsync(someCommand);
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(eventA, exception.FailedMessage);
                Assert.AreSame(internalException, exception.InnerException);
                throw;
            }
            finally
            {
                unitOfWorkA.AssertRequiresFlushCountIs(0);
                unitOfWorkA.AssertFlushCountIs(0);

                unitOfWorkB.AssertRequiresFlushCountIs(0);
                unitOfWorkB.AssertFlushCountIs(0);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task HandleStreamAsync_ThrowsInternalServerErrorException_IfMetadataHandlerThrowsRandomException()
        {
            var someCommand = new SomeCommand();
            var eventA = new EventA();
            var randomException = new Exception();
            var unitOfWorkA = new UnitOfWorkSpy(true);
            var unitOfWorkB = new UnitOfWorkSpy(true);

            _processor.Implement<SomeCommandHandler>().As<SomeCommand>((message, context) =>
            {
                context.UnitOfWork.Enlist(unitOfWorkA);
                context.MetadataStream.Publish(eventA);
            });

            _processor.Implement<MetadataEventHandlerAB>().As<EventA>((message, context) =>
            {
                context.UnitOfWork.Enlist(unitOfWorkB);

                throw randomException;
            });

            try
            {
                await _processor.HandleAsync(someCommand);
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(eventA, exception.FailedMessage);
                Assert.AreSame(randomException, exception.InnerException);
                throw;
            }
            finally
            {
                unitOfWorkA.AssertRequiresFlushCountIs(0);
                unitOfWorkA.AssertFlushCountIs(0);

                unitOfWorkB.AssertRequiresFlushCountIs(0);
                unitOfWorkB.AssertFlushCountIs(0);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ConflictException))]
        public async Task HandleStreamAsync_ThrowsConflictException_IfMessageIsCommand_And_UnitOfWorkThrowsConcurrencyException()
        {
            var someCommand = new SomeCommand();
            var concurrencyException = new ConcurrencyException();
            var unitOfWork = new UnitOfWorkSpy(true, concurrencyException);

            _processor.Implement<SomeCommandHandler>().As<SomeCommand>((message, context) =>
            {
                context.UnitOfWork.Enlist(unitOfWork);
            });

            try
            {
                await _processor.HandleAsync(someCommand);
            }
            catch (ConflictException exception)
            {
                Assert.AreSame(concurrencyException, exception.InnerException);
                throw;
            }
            finally
            {
                unitOfWork.AssertRequiresFlushCountIs(1);
                unitOfWork.AssertFlushCountIs(1);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task HandleStreamAsync_ThrowsInternalServerErrorException_IfMessageIsCommand_And_UnitOfWorkThrowsConcurrencyException()
        {
            var someCommand = new SomeCommand();
            var randomException = new Exception();
            var unitOfWork = new UnitOfWorkSpy(true, randomException);

            _processor.Implement<SomeCommandHandler>().As<SomeCommand>((message, context) =>
            {
                context.UnitOfWork.Enlist(unitOfWork);
            });

            try
            {
                await _processor.HandleAsync(someCommand);
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(randomException, exception.InnerException);
                throw;
            }
            finally
            {
                unitOfWork.AssertRequiresFlushCountIs(1);
                unitOfWork.AssertFlushCountIs(1);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task HandleStreamAsync_ThrowsInternalServerErrorException_IfMessageIsEvent_And_UnitOfWorkThrowsConcurrencyException()
        {
            var eventA = new EventA();
            var concurrencyException = new ConcurrencyException();
            var unitOfWork = new UnitOfWorkSpy(true, concurrencyException);

            _processor.Implement<EventHandlerAB>().As<EventA>((message, context) =>
            {
                context.UnitOfWork.Enlist(unitOfWork);
            });

            try
            {
                await _processor.HandleAsync(eventA);
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(concurrencyException, exception.InnerException);
                throw;
            }
            finally
            {
                unitOfWork.AssertRequiresFlushCountIs(1);
                unitOfWork.AssertFlushCountIs(1);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task HandleStreamAsync_ThrowsInternalServerErrorException_IfMessageIsMetadataEvent_And_UnitOfWorkThrowsConcurrencyException()
        {
            var someCommand = new SomeCommand();
            var eventA = new EventA();
            var concurrencyException = new ConcurrencyException();
            var unitOfWork = new UnitOfWorkSpy(true, concurrencyException);

            _processor.Implement<SomeCommandHandler>().As<SomeCommand>((message, context) =>
            {
                context.MetadataStream.Publish(eventA);
            });

            _processor.Implement<MetadataEventHandlerAB>().As<EventA>((message, context) =>
            {
                context.UnitOfWork.Enlist(unitOfWork);
            });

            try
            {
                await _processor.HandleAsync(someCommand);
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(concurrencyException, exception.InnerException);
                throw;
            }
            finally
            {
                unitOfWork.AssertRequiresFlushCountIs(1);
                unitOfWork.AssertFlushCountIs(1);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(TaskCanceledException))]
        public async Task HandleStreamAsync_ThrowsOperationCanceledException_IfOperationIsCanceledInsideCommandHandler_Through_TokenSource()
        {
            using (var tokenSource = new CancellationTokenSource())            
            {
                var someCommand = new SomeCommand();

                _processor.Implement<SomeCommandHandler>().As<SomeCommand>((message, context) =>
                {
                    tokenSource.Cancel();
                });
                
                var handleTask = _processor.HandleAsync(someCommand, tokenSource.Token);

                try
                {
                    await handleTask;
                }
                catch (TaskCanceledException exception)
                {
                    Assert.AreEqual(tokenSource.Token, exception.CancellationToken);
                    Assert.IsTrue(handleTask.IsCanceled);
                    throw;
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(TaskCanceledException))]
        public async Task HandleStreamAsync_ThrowsOperationCanceledException_IfOperationIsCanceledInsideCommandHandler_Through_Token()
        {
            using (var tokenSource = new CancellationTokenSource())
            {
                var someCommand = new SomeCommand();

                _processor.Implement<SomeCommandHandler>().As<SomeCommand>((message, context) =>
                {
                    tokenSource.Cancel();
                    context.Token.ThrowIfCancellationRequested();
                });

                var handleTask = _processor.HandleAsync(someCommand, tokenSource.Token);

                try
                {
                    await handleTask;
                }
                catch (TaskCanceledException exception)
                {
                    Assert.AreEqual(tokenSource.Token, exception.CancellationToken);
                    Assert.IsTrue(handleTask.IsCanceled);
                    throw;
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(TaskCanceledException))]
        public async Task HandleStreamAsync_ThrowsOperationCanceledException_IfOperationIsCanceledInsideEventHandler_Through_TokenSource()
        {
            using (var tokenSource = new CancellationTokenSource())
            {
                var someCommand = new SomeCommand();
                var eventA = new EventA();

                _processor.Implement<SomeCommandHandler>().As<SomeCommand>((message, context) =>
                {
                    context.OutputStream.Publish(eventA);
                });

                _processor.Implement<EventHandlerAB>().As<EventA>((message, context) =>
                {
                    tokenSource.Cancel();
                });

                var handleTask = _processor.HandleAsync(someCommand, tokenSource.Token);

                try
                {
                    await handleTask;
                }
                catch (TaskCanceledException exception)
                {
                    Assert.AreEqual(tokenSource.Token, exception.CancellationToken);
                    Assert.IsTrue(handleTask.IsCanceled);
                    throw;
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(TaskCanceledException))]
        public async Task HandleStreamAsync_ThrowsOperationCanceledException_IfOperationIsCanceledInsideEventHandler_Through_Token()
        {
            using (var tokenSource = new CancellationTokenSource())
            {
                var someCommand = new SomeCommand();
                var eventA = new EventA();

                _processor.Implement<SomeCommandHandler>().As<SomeCommand>((message, context) =>
                {
                    context.OutputStream.Publish(eventA);
                });

                _processor.Implement<EventHandlerAB>().As<EventA>((message, context) =>
                {
                    tokenSource.Cancel();
                    context.Token.ThrowIfCancellationRequested();
                });

                var handleTask = _processor.HandleAsync(someCommand, tokenSource.Token);

                try
                {
                    await handleTask;
                }
                catch (TaskCanceledException exception)
                {
                    Assert.AreEqual(tokenSource.Token, exception.CancellationToken);
                    Assert.IsTrue(handleTask.IsCanceled);
                    throw;
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(TaskCanceledException))]
        public async Task HandleStreamAsync_ThrowsOperationCanceledException_IfOperationIsCanceledInsideMetadataHandler_Through_TokenSource()
        {
            using (var tokenSource = new CancellationTokenSource())
            {
                var someCommand = new SomeCommand();
                var eventA = new EventA();

                _processor.Implement<SomeCommandHandler>().As<SomeCommand>((message, context) =>
                {
                    context.MetadataStream.Publish(eventA);
                });

                _processor.Implement<MetadataEventHandlerAB>().As<EventA>((message, context) =>
                {
                    tokenSource.Cancel();
                });

                var handleTask = _processor.HandleAsync(someCommand, tokenSource.Token);

                try
                {
                    await handleTask;
                }
                catch (TaskCanceledException exception)
                {
                    Assert.AreEqual(tokenSource.Token, exception.CancellationToken);
                    Assert.IsTrue(handleTask.IsCanceled);
                    throw;
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(TaskCanceledException))]
        public async Task HandleStreamAsync_ThrowsOperationCanceledException_IfOperationIsCanceledInsideMetadataHandler_Through_Token()
        {
            using (var tokenSource = new CancellationTokenSource())
            {
                var someCommand = new SomeCommand();
                var eventA = new EventA();

                _processor.Implement<SomeCommandHandler>().As<SomeCommand>((message, context) =>
                {
                    context.MetadataStream.Publish(eventA);
                });

                _processor.Implement<MetadataEventHandlerAB>().As<EventA>((message, context) =>
                {
                    tokenSource.Cancel();
                    context.Token.ThrowIfCancellationRequested();
                });

                var handleTask = _processor.HandleAsync(someCommand, tokenSource.Token);

                try
                {
                    await handleTask;
                }
                catch (TaskCanceledException exception)
                {
                    Assert.AreEqual(tokenSource.Token, exception.CancellationToken);
                    Assert.IsTrue(handleTask.IsCanceled);
                    throw;
                }
            }
        }        

        #endregion

        #region [====== Queries (1) ======]        

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Execute_1_Throws_IfQueryIsNull()
        {
            _processor.Execute(null as IQuery<object>);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExecuteAsync_1_Throws_IfQueryIsNull()
        {
            _processor.ExecuteAsync(null as IQuery<object>);
        }

        [TestMethod]
        public async Task ExecuteAsync_1_ReturnsResultOfQuery_IfQueryIsNotNull()
        {
            var messageOut = new object();

            Assert.AreSame(await _processor.ExecuteAsync(context =>
            {
                Assert.AreEqual(0, context.Messages.Count);
                return messageOut;
            }), messageOut);
        }

        [TestMethod]        
        public async Task ExecuteAsync_1_ReturnsResultOfQuery_IfUnitOfWorkIsEnlistedInsideQueryContext()
        {
            var unitOfWork = new UnitOfWorkSpy(true);           
            var messageOut = new object();           

            Assert.AreSame(await _processor.ExecuteAsync(context =>
            {
                context.UnitOfWork.Enlist(unitOfWork);
                return messageOut;
            }), messageOut);

            unitOfWork.AssertRequiresFlushCountIs(1);
            unitOfWork.AssertFlushCountIs(1);
        }

        [TestMethod]
        public async Task ExecuteAsync_1_ReturnsResultOfQuery_IfUnitOfWorkIsEnlistedInsideMetadataContext()
        {
            var unitOfWork = new UnitOfWorkSpy(true);
            var eventA = new EventA();
            var messageOut = new object();

            _processor.Implement<MetadataEventHandlerAB>().As<EventA>((message, context) =>
            {                
                context.UnitOfWork.Enlist(unitOfWork);
            });

            Assert.AreSame(await _processor.ExecuteAsync(context =>
            {
                context.MetadataStream.Publish(eventA);
                return messageOut;
            }), messageOut);

            unitOfWork.AssertRequiresFlushCountIs(1);
            unitOfWork.AssertFlushCountIs(1);
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public async Task ExecuteAsync_1_ThrowsBadRequestException_IfQueryThrowsInternalProcessorException()
        {
            var internalProcessorException = new SomeInternalProcessorException();
            Func<IMicroProcessorContext, object> query = context =>
            {
                throw internalProcessorException;
            };

            try
            {
                await _processor.ExecuteAsync(query);
            }
            catch (BadRequestException exception)
            {
                Assert.IsNull(exception.FailedMessage);
                Assert.AreSame(internalProcessorException, exception.InnerException);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task ExecuteAsync_1_ThrowsInternalServerErrorsException_IfQueryThrowsRandomException()
        {
            var randomException = new Exception();
            Func<IMicroProcessorContext, object> query = context => { throw randomException; };

            try
            {
                await _processor.ExecuteAsync(query);
            }
            catch (InternalServerErrorException exception)
            {
                Assert.IsNull(exception.FailedMessage);
                Assert.AreSame(randomException, exception.InnerException);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task ExecuteAsync_1_ThrowsInternalServerErrorException_IfUnitOfWorkThrowsConcurrencyException()
        {
            var concurrencyException = new ConcurrencyException();
            var unitOfWork = new UnitOfWorkSpy(true, concurrencyException);

            try
            {
                await _processor.ExecuteAsync(context =>
                {
                    context.UnitOfWork.Enlist(unitOfWork);
                    return new object();
                });
            }
            catch (InternalServerErrorException exception)
            {
                Assert.IsNull(exception.FailedMessage);
                Assert.AreSame(concurrencyException, exception.InnerException);
                throw;
            }
            finally
            {
                unitOfWork.AssertRequiresFlushCountIs(1);
                unitOfWork.AssertFlushCountIs(1);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task ExecuteAsync_1_ThrowsInternalServerErrorException_IfUnitOfWorkThrowsRandomException()
        {
            var randomException = new Exception();
            var unitOfWork = new UnitOfWorkSpy(true, randomException);

            try
            {
                await _processor.ExecuteAsync(context =>
                {
                    context.UnitOfWork.Enlist(unitOfWork);
                    return new object();
                });
            }
            catch (InternalServerErrorException exception)
            {
                Assert.IsNull(exception.FailedMessage);
                Assert.AreSame(randomException, exception.InnerException);
                throw;
            }
            finally
            {
                unitOfWork.AssertRequiresFlushCountIs(1);
                unitOfWork.AssertFlushCountIs(1);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task ExecuteAsync_1_ThrowsInternalServerErrorException_IfMetadataEventIsPublished_And_UnitOfWorkThrowsConcurrencyException()
        {
            var eventA = new EventA();
            var concurrencyException = new ConcurrencyException();
            var unitOfWork = new UnitOfWorkSpy(true, concurrencyException);

            _processor.Implement<MetadataEventHandlerAB>().As<EventA>((message, context) =>
            {
                context.UnitOfWork.Enlist(unitOfWork);
            });

            try
            {
                await _processor.ExecuteAsync(context =>
                {
                    context.MetadataStream.Publish(eventA);
                    return new object();
                });
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(eventA, exception.FailedMessage);
                Assert.AreSame(concurrencyException, exception.InnerException);
                throw;
            }
            finally
            {
                unitOfWork.AssertRequiresFlushCountIs(1);
                unitOfWork.AssertFlushCountIs(1);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException))]
        public async Task ExecuteAsync_1_ThrowsOperationCanceledException_IfOperationIsCanceledInsideQuery_Through_TokenSource()
        {
            using (var tokenSource = new CancellationTokenSource())
            {                
                var handleTask = _processor.ExecuteAsync(context =>
                {
                    tokenSource.Cancel();
                    return new object();
                }, tokenSource.Token);

                try
                {
                    await handleTask;
                }
                catch (OperationCanceledException exception)
                {
                    Assert.AreEqual(tokenSource.Token, exception.CancellationToken);
                    Assert.IsTrue(handleTask.IsCanceled);
                    throw;
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException))]
        public async Task ExecuteAsync_1_ThrowsOperationCanceledException_IfOperationIsCanceledInsideQuery_Through_Token()
        {
            using (var tokenSource = new CancellationTokenSource())
            {
                var handleTask = _processor.ExecuteAsync(context =>
                {
                    tokenSource.Cancel();
                    context.Token.ThrowIfCancellationRequested();
                    return new object();
                }, tokenSource.Token);

                try
                {
                    await handleTask;
                }
                catch (OperationCanceledException exception)
                {
                    Assert.AreEqual(tokenSource.Token, exception.CancellationToken);
                    Assert.IsTrue(handleTask.IsCanceled);
                    throw;
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException))]
        public async Task ExecuteAsync_1_ThrowsOperationCanceledException_IfOperationIsCanceledInsideMetadataHandler_Through_TokenSource()
        {
            using (var tokenSource = new CancellationTokenSource())
            {                
                var eventA = new EventA();                

                _processor.Implement<MetadataEventHandlerAB>().As<EventA>((message, context) =>
                {
                    tokenSource.Cancel();
                });

                var handleTask = _processor.ExecuteAsync(context =>
                {
                    context.MetadataStream.Publish(eventA);
                    return new object();
                }, tokenSource.Token);

                try
                {
                    await handleTask;
                }
                catch (OperationCanceledException exception)
                {
                    Assert.AreEqual(tokenSource.Token, exception.CancellationToken);
                    Assert.IsTrue(handleTask.IsCanceled);
                    throw;
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException))]
        public async Task ExecuteAsync_1_ThrowsOperationCanceledException_IfOperationIsCanceledInsideMetadataHandler_Through_Token()
        {
            using (var tokenSource = new CancellationTokenSource())
            {
                var eventA = new EventA();

                _processor.Implement<MetadataEventHandlerAB>().As<EventA>((message, context) =>
                {
                    tokenSource.Cancel();
                    context.Token.ThrowIfCancellationRequested();
                });

                var handleTask = _processor.ExecuteAsync(context =>
                {
                    context.MetadataStream.Publish(eventA);
                    return new object();
                }, tokenSource.Token);

                try
                {
                    await handleTask;
                }
                catch (OperationCanceledException exception)
                {
                    Assert.AreEqual(tokenSource.Token, exception.CancellationToken);
                    Assert.IsTrue(handleTask.IsCanceled);
                    throw;
                }
            }
        }

        #endregion

        #region [====== Queries (2) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Execute_2_Throws_IfMessageIsNull()
        {
            _processor.Execute(null as object, (message, context) => new object());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExecuteAsync_2_Throws_IfMessageIsNull()
        {
            _processor.ExecuteAsync(null as object, (message, context) => new object());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Execute_2_Throws_IfQueryIsNull()
        {
            _processor.Execute(new object(), null as IQuery<object, object>);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExecuteAsync_2_Throws_IfQueryIsNull()
        {
            _processor.ExecuteAsync(new object(), null as IQuery<object, object>);
        }

        [TestMethod]
        public async Task ExecuteAsync_2_ReturnsResultOfQuery_IfQueryIsNotNull()
        {
            var messageIn = new object();
            var messageOut = new object();

            Assert.AreSame(await _processor.ExecuteAsync(messageIn, (message, context) =>
            {
                AssertMessageStack(context.Messages, message, messageIn);
                return messageOut;
            }), messageOut);
        }

        [TestMethod]        
        public async Task ExecuteAsync_2_ReturnsResultOfQuery_IfUnitOfWorkIsEnlistedInsideQueryContext()
        {
            var unitOfWork = new UnitOfWorkSpy(true);            
            var messageOut = new object();            

            Assert.AreSame(await _processor.ExecuteAsync(new object(), (message, context) =>
            {
                context.UnitOfWork.Enlist(unitOfWork);
                return messageOut;
            }), messageOut);

            unitOfWork.AssertRequiresFlushCountIs(1);
            unitOfWork.AssertFlushCountIs(1);
        }

        [TestMethod]
        public async Task ExecuteAsync_2_ReturnsResultOfQuery_IfUnitOfWorkIsEnlistedInsideMetadataContext()
        {
            var unitOfWork = new UnitOfWorkSpy(true);
            var eventA = new EventA();
            var messageOut = new object();

            _processor.Implement<MetadataEventHandlerAB>().As<EventA>((message, context) =>
            {
                context.UnitOfWork.Enlist(unitOfWork);
            });

            Assert.AreSame(await _processor.ExecuteAsync(new object(), (message, context) =>
            {
                context.MetadataStream.Publish(eventA);
                return messageOut;
            }), messageOut);

            unitOfWork.AssertRequiresFlushCountIs(1);
            unitOfWork.AssertFlushCountIs(1);
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public async Task ExecuteAsync_2_ThrowsBadRequest_IfQueryThrowsInternalProcessorException()
        {
            var messageIn = new object();
            var internalProcessorException = new SomeInternalProcessorException();
            Func<object, IMicroProcessorContext, object> query = (message, context) =>
            {
                throw internalProcessorException;
            };

            try
            {
                await _processor.ExecuteAsync(messageIn, query);
            }
            catch (BadRequestException exception)
            {
                Assert.AreSame(messageIn, exception.FailedMessage);
                Assert.AreSame(internalProcessorException, exception.InnerException);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task ExecuteAsync_2_ThrowsInternalServerErrorException_IfQueryThrowsRandomException()
        {
            var messageIn = new object();
            var randomException = new Exception();
            Func<object, IMicroProcessorContext, object> query = (message, context) =>
            {
                throw randomException;
            };

            try
            {
                await _processor.ExecuteAsync(messageIn, query);
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(messageIn, exception.FailedMessage);
                Assert.AreSame(randomException, exception.InnerException);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task ExecuteAsync_2_ThrowsInternalServerErrorException_IfUnitOfWorkThrowsConcurrencyException()
        {
            var messageIn = new object();
            var concurrencyException = new ConcurrencyException();
            var unitOfWork = new UnitOfWorkSpy(true, concurrencyException);

            try
            {
                await _processor.ExecuteAsync(messageIn, (message, context) =>
                {
                    context.UnitOfWork.Enlist(unitOfWork);
                    return new object();
                });
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(messageIn, exception.FailedMessage);
                Assert.AreSame(concurrencyException, exception.InnerException);
                throw;
            }
            finally
            {
                unitOfWork.AssertRequiresFlushCountIs(1);
                unitOfWork.AssertFlushCountIs(1);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task ExecuteAsync_2_ThrowsInternalServerErrorException_IfUnitOfWorkThrowsRandomException()
        {
            var messageIn = new object();
            var randomException = new Exception();
            var unitOfWork = new UnitOfWorkSpy(true, randomException);

            try
            {
                await _processor.ExecuteAsync(messageIn, (message, context) =>
                {
                    context.UnitOfWork.Enlist(unitOfWork);
                    return new object();
                });
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(messageIn, exception.FailedMessage);
                Assert.AreSame(randomException, exception.InnerException);
                throw;
            }
            finally
            {
                unitOfWork.AssertRequiresFlushCountIs(1);
                unitOfWork.AssertFlushCountIs(1);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task ExecuteAsync_2_ThrowsInternalServerErrorException_IfMetadataEventIsPublished_And_UnitOfWorkThrowsConcurrencyException()
        {
            var messageIn = new object();
            var eventA = new EventA();
            var concurrencyException = new ConcurrencyException();
            var unitOfWork = new UnitOfWorkSpy(true, concurrencyException);

            _processor.Implement<MetadataEventHandlerAB>().As<EventA>((message, context) =>
            {
                context.UnitOfWork.Enlist(unitOfWork);
            });

            try
            {
                await _processor.ExecuteAsync(messageIn, (message, context) =>
                {
                    context.MetadataStream.Publish(eventA);
                    return new object();
                });
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(eventA, exception.FailedMessage);
                Assert.AreSame(concurrencyException, exception.InnerException);
                throw;
            }
            finally
            {
                unitOfWork.AssertRequiresFlushCountIs(1);
                unitOfWork.AssertFlushCountIs(1);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException))]
        public async Task ExecuteAsync_2_ThrowsOperationCanceledException_IfOperationIsCanceledInsideQuery_Through_TokenSource()
        {
            using (var tokenSource = new CancellationTokenSource())
            {
                var messageIn = new object();
                var handleTask = _processor.ExecuteAsync(messageIn, (message, context) =>
                {
                    tokenSource.Cancel();
                    return new object();
                }, tokenSource.Token);

                try
                {
                    await handleTask;
                }
                catch (OperationCanceledException exception)
                {
                    Assert.AreEqual(tokenSource.Token, exception.CancellationToken);
                    Assert.IsTrue(handleTask.IsCanceled);
                    throw;
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException))]
        public async Task ExecuteAsync_2_ThrowsOperationCanceledException_IfOperationIsCanceledInsideQuery_Through_Token()
        {
            using (var tokenSource = new CancellationTokenSource())
            {                
                var messageIn = new object();
                var handleTask = _processor.ExecuteAsync(messageIn, (message, context) =>
                {
                    tokenSource.Cancel();
                    context.Token.ThrowIfCancellationRequested();
                    return new object();
                }, tokenSource.Token);

                try
                {
                    await handleTask;
                }
                catch (OperationCanceledException exception)
                {
                    Assert.AreEqual(tokenSource.Token, exception.CancellationToken);
                    Assert.IsTrue(handleTask.IsCanceled);
                    throw;
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException))]
        public async Task ExecuteAsync_2_ThrowsOperationCanceledException_IfOperationIsCanceledInsideMetadataHandler_Through_TokenSource()
        {
            using (var tokenSource = new CancellationTokenSource())
            {
                var messageIn = new object();
                var eventA = new EventA();

                _processor.Implement<MetadataEventHandlerAB>().As<EventA>((message, context) =>
                {
                    tokenSource.Cancel();
                });

                var handleTask = _processor.ExecuteAsync(messageIn, (message, context) =>
                {
                    context.MetadataStream.Publish(eventA);
                    return new object();
                }, tokenSource.Token);

                try
                {
                    await handleTask;
                }
                catch (OperationCanceledException exception)
                {
                    Assert.AreEqual(tokenSource.Token, exception.CancellationToken);
                    Assert.IsTrue(handleTask.IsCanceled);
                    throw;
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException))]
        public async Task ExecuteAsync_2_ThrowsOperationCanceledException_IfOperationIsCanceledInsideMetadataHandler_Through_Token()
        {
            using (var tokenSource = new CancellationTokenSource())
            {
                var messageIn = new object();
                var eventA = new EventA();

                _processor.Implement<MetadataEventHandlerAB>().As<EventA>((message, context) =>
                {
                    tokenSource.Cancel();
                    context.Token.ThrowIfCancellationRequested();
                });

                var handleTask = _processor.ExecuteAsync(messageIn, (message, context) =>
                {
                    context.MetadataStream.Publish(eventA);
                    return new object();
                }, tokenSource.Token);

                try
                {
                    await handleTask;
                }
                catch (OperationCanceledException exception)
                {
                    Assert.AreEqual(tokenSource.Token, exception.CancellationToken);
                    Assert.IsTrue(handleTask.IsCanceled);
                    throw;
                }
            }
        }

        #endregion

        #region [====== Metadata in the face of Exceptions ======]

        [TestMethod]
        [ExpectedException(typeof(UnprocessableEntityException))]
        public async Task HandleAsync_StillHandlesMetadataStream_IfCommandHandlerThrowsException()
        {
            var someCommand = new SomeCommand();
            var eventA = new EventA();
            var remainingPublishCount = 3;

            _processor.Implement<SomeCommandHandler>().As<SomeCommand>((message, context) =>
            {
                context.MetadataStream.Publish(eventA);

                throw new IllegalOperationException("Test");
            });

            _processor.Implement<MetadataEventHandlerAB>(4).As<EventA>((message, context) =>
            {
                if (Interlocked.Decrement(ref remainingPublishCount) > 0)
                {
                    context.MetadataStream.Publish(message);
                }
            });

            try
            {
                await _processor.HandleAsync(someCommand);
            }
            finally
            {
                Assert.AreEqual(0, remainingPublishCount);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task ExecuteAsync_StillHandlesMetadataStream_IfQueryThrowsException()
        {            
            var eventA = new EventA();
            var remainingPublishCount = 3;
            Func<IMicroProcessorContext, object> query = context =>
            {
                context.MetadataStream.Publish(eventA);
                throw new Exception("Test");
            };

            _processor.Implement<MetadataEventHandlerAB>(4).As<EventA>((message, context) =>
            {
                if (Interlocked.Decrement(ref remainingPublishCount) > 0)
                {
                    context.MetadataStream.Publish(message);
                }
            });

            try
            {
                await _processor.ExecuteAsync(query);
            }
            finally
            {
                Assert.AreEqual(0, remainingPublishCount);
            }
        }

        #endregion

        #region [====== Pipeline ======]        

        private sealed class PublishExtraEventPipeline : MicroProcessorFilterSpy
        {
            private readonly object _event;
            private readonly bool _afterHandleAsync;

            public PublishExtraEventPipeline(object @event, bool afterHandleAsync)
            {
                _event = @event;
                _afterHandleAsync = afterHandleAsync;
            }

            public override async Task<HandleAsyncResult> HandleAsync<TMessage>(MessageHandler<TMessage> handler, TMessage message, IMicroProcessorContext context)
            {
                if (context.Messages.Current.Source == MessageSources.InputStream)
                {
                    if (_afterHandleAsync)
                    {
                        var result = await base.HandleAsync(handler, message, context);
                        context.OutputStream.Publish(_event);
                        return result;
                    }                    
                    context.OutputStream.Publish(_event);                    
                }
                return await base.HandleAsync(handler, message, context);
            }
        }                

        [TestMethod]
        public async Task HandleAsync_CanPublishMessagesToOutputStream_IfInputStreamIsBeingProcessed_And_EventIsPublishedBeforeMessageHandlerIsInvoked()
        {
            var someCommand = new SomeCommand();
            var eventA = new EventA();
            var eventB = new EventB();

            _processor.Implement<SomeCommandHandler>().As<SomeCommand>((message, context) =>
            {
                AssertMessageStack(context.Messages, message, someCommand);

                context.OutputStream.Publish(eventA);
            });

            _processor.Implement<EventHandlerAB>().As<EventB>((message, context) =>
            {
                AssertMessageStack(context.Messages, message, someCommand, eventB);
            });

            _processor.Implement<EventHandlerAB>().As<EventA>((message, context) =>
            {
                AssertMessageStack(context.Messages, message, someCommand, eventA);
            });

            _processor.Add(new PublishExtraEventPipeline(eventB, false));

            AssertStream(await _processor.HandleAsync(someCommand), eventB, eventA);
        }

        [TestMethod]
        public async Task HandleAsync_CanPublishMessagesToOutputStream_IfInputStreamIsBeingProcessed_And_EventIsPublishedAfterMessageHandlerIsInvoked()
        {
            var someCommand = new SomeCommand();
            var eventA = new EventA();
            var eventB = new EventB();

            _processor.Implement<SomeCommandHandler>().As<SomeCommand>((message, context) =>
            {
                AssertMessageStack(context.Messages, message, someCommand);

                context.OutputStream.Publish(eventA);
            });

            _processor.Implement<EventHandlerAB>().As<EventA>((message, context) =>
            {
                AssertMessageStack(context.Messages, message, someCommand, eventA);
            });

            _processor.Implement<EventHandlerAB>().As<EventB>((message, context) =>
            {
                AssertMessageStack(context.Messages, message, someCommand, eventB);
            });            

            _processor.Add(new PublishExtraEventPipeline(eventB, true));

            AssertStream(await _processor.HandleAsync(someCommand), eventA, eventB);
        }

        #endregion        

        private static void AssertIsEmpty(IMessageStream stream) =>
            AssertStream(stream);

        private static void AssertStream(IMessageStream stream, params object[] expectedMessages)
        {
            Assert.IsNotNull(stream);
            Assert.AreEqual(expectedMessages.Length, stream.Count);

            for (int index = 0; index < expectedMessages.Length; index++)
            {
                Assert.AreSame(expectedMessages[index], stream[index]);
            }
        }

        private static void AssertMessageStack(IMessageStackTrace stackTrace, object message, params object[] expectedMessages)
        {
            Assert.AreEqual(expectedMessages.Length, stackTrace.Count);
            var index = expectedMessages.Length - 1;

            Assert.AreSame(expectedMessages[index], message);                        

            foreach (var expectedMessage in expectedMessages)
            {
                Assert.AreSame(expectedMessage, stackTrace[index--].Message);
            }
        }        
    }
}
