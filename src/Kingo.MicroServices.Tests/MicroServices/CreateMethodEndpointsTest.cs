using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class CreateMethodEndpointsTest : MicroProcessorTest<MicroProcessor>
    {
        #region [====== Returned Collection/Endpoints ======]

        private sealed class MessageHandler1 : IMessageHandler<object>
        {
            public Task HandleAsync(object message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        [MessageHandler(HandlesExternalMessages = false)]
        private sealed class MessageHandler2 : IMessageHandler<object>
        {
            [Endpoint]
            public Task HandleAsync(object message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        private sealed class MessageHandler3 : IMessageHandler<object>
        {
            [Endpoint]
            public Task HandleAsync(object message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        private sealed class MessageHandler4 : IMessageHandler<object>, IMessageHandler<int>, IMessageHandler<string>
        {
            public Task HandleAsync(object message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;

            [Endpoint]
            public Task HandleAsync(int message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;

            [Endpoint]
            public Task HandleAsync(string message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        [TestMethod]
        public void CreateMethodEndpoints_ReturnsEmptyCollection_IfNoMessageHandlersHaveBeenRegistered()
        {
            Assert.AreEqual(0, CreateProcessor().CreateMethodEndpoints().Count());
        }

        [TestMethod]
        public void CreateMethodEndpoints_ReturnsEmptyCollection_IfMethodHasNoEndpointAttribute()
        {
            ProcessorBuilder.Components.AddType<MessageHandler1>();
            ProcessorBuilder.Components.AddMessageHandlers();

            Assert.AreEqual(0, CreateProcessor().CreateMethodEndpoints().Count());
        }

        [TestMethod]
        public void CreateMethodEndpoints_ReturnsEmptyCollection_IfMethodHasEndpointAttribute_But_MessageHandlerHandlesNoExternalMessages()
        {
            ProcessorBuilder.Components.AddType<MessageHandler2>();
            ProcessorBuilder.Components.AddMessageHandlers();

            Assert.AreEqual(0, CreateProcessor().CreateMethodEndpoints().Count());
        }

        [TestMethod]
        public void CreateMethodEndpoints_ReturnsOneEndpoint_IfMethodHasEndpointAttribute()
        {
            ProcessorBuilder.Components.AddType<MessageHandler3>();
            ProcessorBuilder.Components.AddMessageHandlers();

            var endpoint = CreateProcessor().CreateMethodEndpoints().Single();

            Assert.AreEqual(typeof(MessageHandler3), endpoint.MessageHandler.Type);
            Assert.AreSame(typeof(object), endpoint.MessageParameter.Type);            
        }

        [TestMethod]
        public void CreateMethodEndpoints_ReturnsMultipleEndpoints_IfMultipleMethodsHaveEndpointAttribute()
        {
            ProcessorBuilder.Components.AddType<MessageHandler4>();
            ProcessorBuilder.Components.AddMessageHandlers();

            var endpoints = CreateProcessor().CreateMethodEndpoints().ToArray();

            Assert.AreEqual(2, endpoints.Length);
            Assert.AreSame(typeof(MessageHandler4), endpoints[0].MessageHandler.Type);
            Assert.AreSame(typeof(MessageHandler4), endpoints[1].MessageHandler.Type);
            Assert.IsTrue(endpoints.Any(endpoint => endpoint.MessageParameter.Type == typeof(int)));
            Assert.IsTrue(endpoints.Any(endpoint => endpoint.MessageParameter.Type == typeof(string)));
        }

        [TestMethod]
        public void CreateMethodEndpoints_ReturnsMultipleEndpoints_IfMultipleMessageHandlersHaveEndpointAttribute()
        {
            ProcessorBuilder.Components.AddType<MessageHandler3>();
            ProcessorBuilder.Components.AddType<MessageHandler4>();
            ProcessorBuilder.Components.AddMessageHandlers();

            var endpoints = CreateProcessor().CreateMethodEndpoints().ToArray();

            Assert.AreEqual(3, endpoints.Length);            
            Assert.IsTrue(endpoints.Any(endpoint => endpoint.MessageParameter.Type == typeof(object)));
            Assert.IsTrue(endpoints.Any(endpoint => endpoint.MessageParameter.Type == typeof(int)));
            Assert.IsTrue(endpoints.Any(endpoint => endpoint.MessageParameter.Type == typeof(string)));
        }

        [TestMethod]
        public void CreateMethodEndpoints_ReturnsExpectedEndpoints_IfMessageHandlerWasAddedAsSingleton()
        {
            ProcessorBuilder.Components.AddMessageHandler(new MessageHandler3());
            ProcessorBuilder.Components.AddMessageHandler(new MessageHandler4());          

            var endpoints = CreateProcessor().CreateMethodEndpoints().ToArray();

            Assert.AreEqual(3, endpoints.Length);
            Assert.IsTrue(endpoints.Any(endpoint => endpoint.MessageParameter.Type == typeof(object)));
            Assert.IsTrue(endpoints.Any(endpoint => endpoint.MessageParameter.Type == typeof(int)));
            Assert.IsTrue(endpoints.Any(endpoint => endpoint.MessageParameter.Type == typeof(string)));
        }

        #endregion

        #region [====== MessageKind ======]

        private sealed class RequestHandler : IMessageHandler<object>
        {
            [Endpoint(MessageKind.Request)]
            public Task HandleAsync(object message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        private sealed class UnknownMessageKindHandler : IMessageHandler<object>
        {
            [Endpoint((MessageKind) (-1))]
            public Task HandleAsync(object message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        private sealed class ExplicitEventHandler : IMessageHandler<object>
        {
            [Endpoint(MessageKind.Event)]
            public Task HandleAsync(object message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        private sealed class ImplicitEventHandler : IMessageHandler<object>
        {
            [Endpoint]
            public Task HandleAsync(object message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        private sealed class ExplicitCommandHandler : IMessageHandler<object>
        {
            [Endpoint(MessageKind.Command)]
            public Task HandleAsync(object message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        private sealed class ImplicitCommandHandler : IMessageHandler<SomeCommand>
        {
            [Endpoint]
            public Task HandleAsync(SomeCommand message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        private sealed class SomeCommand { }

        private sealed class MessageKindResolver : IMessageKindResolver
        {
            private readonly MessageKind _messageKind;

            public MessageKindResolver(MessageKind messageKind)
            {
                _messageKind = messageKind;
            }

            public MessageKind ResolveMessageKind(Type messageType) =>
                _messageKind;
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CreateMethodEndpoints_Throws_IfMessageKindIsSetToRequest()
        {
            ProcessorBuilder.Components.AddType<RequestHandler>();
            ProcessorBuilder.Components.AddMessageHandlers();

            CreateProcessor().CreateMethodEndpoints().Single().IgnoreValue();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CreateMethodEndpoints_Throws_IfMessageKindIsSetToUnknownValue()
        {
            ProcessorBuilder.Components.AddType<UnknownMessageKindHandler>();
            ProcessorBuilder.Components.AddMessageHandlers();

            CreateProcessor().CreateMethodEndpoints().Single().IgnoreValue();
        }

        [TestMethod]
        public void CreateMethodEndpoints_ReturnsEventHandler_IfMessageKindIsEvent()
        {
            ProcessorBuilder.Components.AddType<ExplicitEventHandler>();
            ProcessorBuilder.Components.AddMessageHandlers();

            var endpoint = CreateProcessor().CreateMethodEndpoints().Single();

            Assert.AreEqual(MessageKind.Event, endpoint.MessageKind);
        }

        [TestMethod]
        public void CreateMethodEndpoints_ReturnsEventHandler_IfMessageKindIsUnspecified_And_NameOfMessageTypeDoesNotEndWithCommand()
        {
            ProcessorBuilder.Components.AddType<ImplicitEventHandler>();
            ProcessorBuilder.Components.AddMessageHandlers();

            var endpoint = CreateProcessor().CreateMethodEndpoints().Single();

            Assert.AreEqual(MessageKind.Event, endpoint.MessageKind);
        }

        [TestMethod]
        public void CreateMethodEndpoints_ReturnsCommandHandler_IfMessageKindIsCommand()
        {
            ProcessorBuilder.Components.AddType<ExplicitCommandHandler>();
            ProcessorBuilder.Components.AddMessageHandlers();

            var endpoint = CreateProcessor().CreateMethodEndpoints().Single();

            Assert.AreEqual(MessageKind.Command, endpoint.MessageKind);
        }

        [TestMethod]
        public void CreateMethodEndpoints_ReturnsCommandHandler_IfMessageKindIsUnspecified_And_NameOfMessageTypeEndsWithCommand()
        {
            ProcessorBuilder.Components.AddType<ImplicitCommandHandler>();
            ProcessorBuilder.Components.AddMessageHandlers();

            var endpoint = CreateProcessor().CreateMethodEndpoints().Single();

            Assert.AreEqual(MessageKind.Command, endpoint.MessageKind);
        }

        [TestMethod]        
        public void CreateMethodEndpoints_Returns_IfCustomMessageKindResolverReturnsUnspecified_And_NameOfMessageTypeEndsWithCommand()
        {
            ProcessorBuilder.Endpoints.MessageKindResolver = new MessageKindResolver(MessageKind.Unspecified);

            ProcessorBuilder.Components.AddType<ImplicitCommandHandler>();
            ProcessorBuilder.Components.AddMessageHandlers();

            var endpoint = CreateProcessor().CreateMethodEndpoints().Single();

            Assert.AreEqual(MessageKind.Command, endpoint.MessageKind);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CreateMethodEndpoints_Throws_IfCustomMessageKindResolver_ReturnsRequest()
        {
            ProcessorBuilder.Endpoints.MessageKindResolver = new MessageKindResolver(MessageKind.Request);

            ProcessorBuilder.Components.AddType<ImplicitCommandHandler>();
            ProcessorBuilder.Components.AddMessageHandlers();

            CreateProcessor().CreateMethodEndpoints().Single().IgnoreValue();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CreateMethodEndpoints_Throws_IfCustomMessageKindResolver_ReturnsUnknownMessageKind()
        {
            ProcessorBuilder.Endpoints.MessageKindResolver = new MessageKindResolver((MessageKind) (-1));

            ProcessorBuilder.Components.AddType<ImplicitCommandHandler>();
            ProcessorBuilder.Components.AddMessageHandlers();

            CreateProcessor().CreateMethodEndpoints().Single().IgnoreValue();
        }

        #endregion

        #region [====== InvokeAsync ======]

        private sealed class SomeCommandHandler : IMessageHandler<int>
        {            
            [Endpoint(MessageKind.Command)]
            public Task HandleAsync(int message, MessageHandlerOperationContext context)
            {
                context.EventBus.Publish(string.Empty);
                return Task.CompletedTask;
            }
        }

        private sealed class SomeEventHandler : IMessageHandler<int>
        {
            [Endpoint(MessageKind.Event)]
            public Task HandleAsync(int message, MessageHandlerOperationContext context)
            {
                context.EventBus.Publish(string.Empty);
                return Task.CompletedTask;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task InvokeAsync_Throws_IfMessageIsNull()
        {
            ProcessorBuilder.Components.AddMessageHandler(new SomeCommandHandler());

            var endpoint = CreateProcessor().CreateMethodEndpoints().Single();

            await endpoint.InvokeAsync(null);
        }

        [TestMethod]        
        public async Task InvokeAsync_ReturnsEmptyResult_IfMessageIsNotSupported()
        {
            ProcessorBuilder.Components.AddMessageHandler(new SomeCommandHandler());

            var endpoint = CreateProcessor().CreateMethodEndpoints().Single();
            var result = await endpoint.InvokeAsync(new object());

            Assert.AreEqual(0, result.MessageHandlerCount);
            Assert.AreEqual(0, result.Events.Count);
        }

        [TestMethod]
        public async Task InvokeAsync_ReturnsExpectedResult_IfMessageHandlerIsCommandHandler()
        {
            ProcessorBuilder.Components.AddMessageHandler(new SomeCommandHandler());
            ProcessorBuilder.Components.AddMessageHandler<string>((message, context) =>
            {
                context.EventBus.Publish(new object());
            });

            var endpoint = CreateProcessor().CreateMethodEndpoints().Single();
            var result = await endpoint.InvokeAsync(DateTimeOffset.UtcNow.Second);

            Assert.AreEqual(2, result.MessageHandlerCount);
            Assert.AreEqual(2, result.Events.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException), AllowDerivedTypes = true)]
        public async Task InvokeAsync_ThrowsExpectedException_IfMessageHandlerIsCommandHandler_And_OperationWasCancelled()
        {
            using (var tokenSource = new CancellationTokenSource())
            {
                ProcessorBuilder.Components.AddMessageHandler(new SomeCommandHandler());
                ProcessorBuilder.Components.AddMessageHandler<string>((message, context) =>
                {
                    tokenSource.Cancel();
                });

                var endpoint = CreateProcessor().CreateMethodEndpoints().Single();

                try
                {
                    await endpoint.InvokeAsync(DateTimeOffset.UtcNow.Second, tokenSource.Token);
                }
                catch (OperationCanceledException exception)
                {
                    Assert.AreEqual(tokenSource.Token, exception.CancellationToken);
                    throw;
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException), AllowDerivedTypes = true)]
        public async Task InvokeAsync_ThrowsExpectedException_IfMessageHandlerIsCommandHandler_And_MessageHandlerOperationExceptionIsThrown()
        {
            var exceptionToThrow = new BusinessRuleException();

            ProcessorBuilder.Components.AddMessageHandler(new SomeCommandHandler());
            ProcessorBuilder.Components.AddMessageHandler<string>((message, context) =>
            {
                throw exceptionToThrow;
            });

            var endpoint = CreateProcessor().CreateMethodEndpoints().Single();

            try
            {
                await endpoint.InvokeAsync(DateTimeOffset.UtcNow.Second);
            }
            catch (BadRequestException exception)
            {
                Assert.AreSame(exceptionToThrow, exception.InnerException);
                throw;
            }            
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public async Task InvokeAsync_ThrowsExpectedException_IfMessageHandlerIsCommandHandler_And_BadRequestExceptionIsThrown()
        {
            var exceptionToThrow = new BadRequestException();

            ProcessorBuilder.Components.AddMessageHandler(new SomeCommandHandler());
            ProcessorBuilder.Components.AddMessageHandler<string>((message, context) =>
            {
                throw exceptionToThrow;
            });

            var endpoint = CreateProcessor().CreateMethodEndpoints().Single();

            try
            {
                await endpoint.InvokeAsync(DateTimeOffset.UtcNow.Second);
            }
            catch (BadRequestException exception)
            {
                Assert.AreSame(exceptionToThrow, exception);
                throw;
            }
        }

        [TestMethod]
        public async Task InvokeAsync_ReturnsExpectedResult_IfMessageHandlerIsEventHandler()
        {
            ProcessorBuilder.Components.AddMessageHandler(new SomeEventHandler());
            ProcessorBuilder.Components.AddMessageHandler<string>((message, context) =>
            {
                context.EventBus.Publish(new object());
            });

            var endpoint = CreateProcessor().CreateMethodEndpoints().Single();
            var result = await endpoint.InvokeAsync(DateTimeOffset.UtcNow.Second);

            Assert.AreEqual(2, result.MessageHandlerCount);
            Assert.AreEqual(2, result.Events.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException), AllowDerivedTypes = true)]
        public async Task InvokeAsync_ThrowsExpectedException_IfMessageHandlerIsEventHandler_And_OperationWasCancelled()
        {
            using (var tokenSource = new CancellationTokenSource())
            {
                ProcessorBuilder.Components.AddMessageHandler(new SomeEventHandler());
                ProcessorBuilder.Components.AddMessageHandler<string>((message, context) =>
                {
                    tokenSource.Cancel();
                });

                var endpoint = CreateProcessor().CreateMethodEndpoints().Single();

                try
                {
                    await endpoint.InvokeAsync(DateTimeOffset.UtcNow.Second, tokenSource.Token);
                }
                catch (OperationCanceledException exception)
                {
                    Assert.AreEqual(tokenSource.Token, exception.CancellationToken);
                    throw;
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException), AllowDerivedTypes = true)]
        public async Task InvokeAsync_ThrowsExpectedException_IfMessageHandlerIsEventHandler_And_MessageHandlerOperationExceptionIsThrown()
        {
            var exceptionToThrow = new BusinessRuleException();

            ProcessorBuilder.Components.AddMessageHandler(new SomeEventHandler());
            ProcessorBuilder.Components.AddMessageHandler<string>((message, context) =>
            {
                throw exceptionToThrow;
            });

            var endpoint = CreateProcessor().CreateMethodEndpoints().Single();

            try
            {
                await endpoint.InvokeAsync(DateTimeOffset.UtcNow.Second);
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(exceptionToThrow, exception.InnerException);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task InvokeAsync_ThrowsExpectedException_IfMessageHandlerIsEventHandler_And_BadRequestExceptionIsThrown()
        {
            var exceptionToThrow = new BadRequestException();

            ProcessorBuilder.Components.AddMessageHandler(new SomeEventHandler());
            ProcessorBuilder.Components.AddMessageHandler<string>((message, context) =>
            {
                throw exceptionToThrow;
            });

            var endpoint = CreateProcessor().CreateMethodEndpoints().Single();

            try
            {
                await endpoint.InvokeAsync(DateTimeOffset.UtcNow.Second);
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(exceptionToThrow, exception.InnerException);
                throw;
            }
        }

        #endregion
    }
}
