using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Controllers
{
    [TestClass]
    public sealed class CreateMicroServiceBusEndpointsTest : MicroProcessorTest<MicroProcessor>
    {
        #region [====== Returned Collection/Endpoints ======]

        private sealed class MessageHandler1 : IMessageHandler<object>
        {
            public Task HandleAsync(object message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        private sealed class MessageHandler2 : IMessageHandler<object>
        {
            [MicroServiceBusEndpoint]
            public Task HandleAsync(object message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        private sealed class MessageHandler3 : IMessageHandler<object>, IMessageHandler<int>, IMessageHandler<string>
        {
            public Task HandleAsync(object message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;

            [MicroServiceBusEndpoint]
            public Task HandleAsync(int message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;

            [MicroServiceBusEndpoint]
            public Task HandleAsync(string message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        [TestMethod]
        public void CreateMicroServiceBusEndpoints_ReturnsEmptyCollection_IfNoMessageHandlersHaveBeenRegistered()
        {
            Assert.AreEqual(0, CreateProcessor().CreateMicroServiceBusEndpoints().Count());
        }

        [TestMethod]
        public void CreateMicroServiceBusEndpoints_ReturnsEmptyCollection_IfMethodHasNoEndpointAttribute()
        {
            ProcessorBuilder.Components.AddMessageHandler<MessageHandler1>();            

            Assert.AreEqual(0, CreateProcessor().CreateMicroServiceBusEndpoints().Count());
        }

        [TestMethod]
        public void CreateMicroServiceBusEndpoints_ReturnsOneEndpoint_IfMethodHasEndpointAttribute()
        {
            ProcessorBuilder.Components.AddMessageHandler<MessageHandler2>();            

            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();

            Assert.AreEqual(typeof(MessageHandler2), endpoint.MessageHandlerType);
            Assert.AreSame(typeof(object), endpoint.MessageParameterInfo.ParameterType);            
        }

        [TestMethod]
        public void CreateMicroServiceBusEndpoints_ReturnsMultipleEndpoints_IfMultipleMethodsHaveEndpointAttribute()
        {
            ProcessorBuilder.Components.AddMessageHandler<MessageHandler3>();            

            var endpoints = CreateProcessor().CreateMicroServiceBusEndpoints().ToArray();

            Assert.AreEqual(2, endpoints.Length);
            Assert.AreSame(typeof(MessageHandler3), endpoints[0].MessageHandlerType);
            Assert.AreSame(typeof(MessageHandler3), endpoints[1].MessageHandlerType);
            Assert.IsTrue(endpoints.Any(endpoint => endpoint.MessageParameterInfo.ParameterType == typeof(int)));
            Assert.IsTrue(endpoints.Any(endpoint => endpoint.MessageParameterInfo.ParameterType == typeof(string)));
        }

        [TestMethod]
        public void CreateMicroServiceBusEndpoints_ReturnsMultipleEndpoints_IfMultipleMessageHandlersHaveEndpointAttribute()
        {
            ProcessorBuilder.Components.AddMessageHandler<MessageHandler2>();
            ProcessorBuilder.Components.AddMessageHandler<MessageHandler3>();            

            var endpoints = CreateProcessor().CreateMicroServiceBusEndpoints().ToArray();

            Assert.AreEqual(3, endpoints.Length);            
            Assert.IsTrue(endpoints.Any(endpoint => endpoint.MessageParameterInfo.ParameterType == typeof(object)));
            Assert.IsTrue(endpoints.Any(endpoint => endpoint.MessageParameterInfo.ParameterType == typeof(int)));
            Assert.IsTrue(endpoints.Any(endpoint => endpoint.MessageParameterInfo.ParameterType == typeof(string)));
        }

        [TestMethod]
        public void CreateMicroServiceBusEndpoints_ReturnsExpectedEndpoints_IfMessageHandlerWasAddedAsSingleton()
        {
            ProcessorBuilder.Components.AddMessageHandler(new MessageHandler2());
            ProcessorBuilder.Components.AddMessageHandler(new MessageHandler3());          

            var endpoints = CreateProcessor().CreateMicroServiceBusEndpoints().ToArray();

            Assert.AreEqual(3, endpoints.Length);
            Assert.IsTrue(endpoints.Any(endpoint => endpoint.MessageParameterInfo.ParameterType == typeof(object)));
            Assert.IsTrue(endpoints.Any(endpoint => endpoint.MessageParameterInfo.ParameterType == typeof(int)));
            Assert.IsTrue(endpoints.Any(endpoint => endpoint.MessageParameterInfo.ParameterType == typeof(string)));
        }

        #endregion

        #region [====== MessageKind ======]

        private sealed class RequestHandler : IMessageHandler<object>
        {
            [MicroServiceBusEndpoint(MessageKind.QueryRequest)]
            public Task HandleAsync(object message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        private sealed class UnknownMessageKindHandler : IMessageHandler<object>
        {
            [MicroServiceBusEndpoint((MessageKind) (-1))]
            public Task HandleAsync(object message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        private sealed class ExplicitEventHandler : IMessageHandler<object>
        {
            [MicroServiceBusEndpoint(MessageKind.Event)]
            public Task HandleAsync(object message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        private sealed class ImplicitEventHandler : IMessageHandler<object>
        {
            [MicroServiceBusEndpoint]
            public Task HandleAsync(object message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        private sealed class ExplicitCommandHandler : IMessageHandler<object>
        {
            [MicroServiceBusEndpoint(MessageKind.Command)]
            public Task HandleAsync(object message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        private sealed class ImplicitCommandHandler : IMessageHandler<SomeCommand>
        {
            [MicroServiceBusEndpoint]
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
        public void CreateMicroServiceBusEndpoints_Throws_IfMessageKindIsSetToRequest()
        {
            ProcessorBuilder.Components.AddMessageHandler<RequestHandler>();            

            CreateProcessor().CreateMicroServiceBusEndpoints().Single().IgnoreValue();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CreateMicroServiceBusEndpoints_Throws_IfMessageKindIsSetToUnknownValue()
        {
            ProcessorBuilder.Components.AddMessageHandler<UnknownMessageKindHandler>();            

            CreateProcessor().CreateMicroServiceBusEndpoints().Single().IgnoreValue();
        }

        [TestMethod]
        public void CreateMicroServiceBusEndpoints_ReturnsEventHandler_IfMessageKindIsEvent()
        {
            ProcessorBuilder.Components.AddMessageHandler<ExplicitEventHandler>();            

            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();

            Assert.AreEqual(MessageKind.Event, endpoint.MessageKind);
        }

        [TestMethod]
        public void CreateMicroServiceBusEndpoints_ReturnsUnspecifiedHandler_IfMessageKindIsUnspecified_And_NameOfMessageTypeDoesNotEndWithCommand()
        {
            ProcessorBuilder.Components.AddMessageHandler<ImplicitEventHandler>();            

            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();

            Assert.AreEqual(MessageKind.Unspecified, endpoint.MessageKind);
        }

        [TestMethod]
        public void CreateMicroServiceBusEndpoints_ReturnsCommandHandler_IfMessageKindIsCommand()
        {
            ProcessorBuilder.Components.AddMessageHandler<ExplicitCommandHandler>();            

            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();

            Assert.AreEqual(MessageKind.Command, endpoint.MessageKind);
        }

        [TestMethod]
        public void CreateMicroServiceBusEndpoints_ReturnsCommandHandler_IfMessageKindIsUnspecified_And_NameOfMessageTypeEndsWithCommand()
        {
            ProcessorBuilder.Components.AddMessageHandler<ImplicitCommandHandler>();            

            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();

            Assert.AreEqual(MessageKind.Command, endpoint.MessageKind);
        }

        [TestMethod]
        public void CreateMicroServiceBusEndpoints_ReturnsCustomMessageKind_IfCustomMessageKindResolver_ReturnsUnknownMessageKind()
        {
            var messageKind = (MessageKind) (-1);

            ProcessorBuilder.Endpoints.MessageKindResolver = new MessageKindResolver(messageKind);
            ProcessorBuilder.Components.AddMessageHandler<ImplicitCommandHandler>();            

            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();

            Assert.AreEqual(messageKind, endpoint.MessageKind);
        }

        #endregion

        #region [====== InvokeAsync ======]

        private sealed class SomeCommandHandler : IMessageHandler<int>
        {            
            [MicroServiceBusEndpoint(MessageKind.Command)]
            public Task HandleAsync(int message, MessageHandlerOperationContext context)
            {
                context.MessageBus.Publish(string.Empty);
                return Task.CompletedTask;
            }
        }

        private sealed class SomeEventHandler : IMessageHandler<int>
        {
            [MicroServiceBusEndpoint(MessageKind.Event)]
            public Task HandleAsync(int message, MessageHandlerOperationContext context)
            {
                context.MessageBus.Publish(string.Empty);
                return Task.CompletedTask;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task InvokeAsync_Throws_IfMessageIsNull()
        {
            ProcessorBuilder.Components.AddMessageHandler(new SomeCommandHandler());

            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();

            await endpoint.InvokeAsync(null);
        }

        [TestMethod]        
        public async Task InvokeAsync_ReturnsEmptyResult_IfMessageIsNotSupported()
        {
            ProcessorBuilder.Components.AddMessageHandler(new SomeCommandHandler());

            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();
            var result = await endpoint.InvokeAsync(new object());

            Assert.AreEqual(0, result.MessageHandlerCount);
            Assert.AreEqual(0, result.Messages.Count);
        }

        [TestMethod]
        public async Task InvokeAsync_ReturnsExpectedResult_IfMessageHandlerIsCommandHandler()
        {
            ProcessorBuilder.Components.AddMessageHandler(new SomeCommandHandler());
            ProcessorBuilder.Components.AddMessageHandler<string>((message, context) =>
            {
                context.MessageBus.Publish(new object());
            });

            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();
            var result = await endpoint.InvokeAsync(DateTimeOffset.UtcNow.Second);

            Assert.AreEqual(2, result.MessageHandlerCount);
            Assert.AreEqual(2, result.Messages.Count);
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

                var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();

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

            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();

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

            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();

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
                context.MessageBus.Publish(new object());
            });

            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();
            var result = await endpoint.InvokeAsync(DateTimeOffset.UtcNow.Second);

            Assert.AreEqual(2, result.MessageHandlerCount);
            Assert.AreEqual(2, result.Messages.Count);
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

                var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();

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

            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();

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

            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();

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
