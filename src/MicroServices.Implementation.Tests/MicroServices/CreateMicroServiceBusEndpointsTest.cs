using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kingo.MicroServices.TestEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class CreateMicroServiceBusEndpointsTest : MicroProcessorTest<MicroProcessor>
    {
        #region [====== Returned Collection/Endpoints ======]

        private sealed class MessageHandler1 : IMessageHandler<object>
        {
            public Task HandleAsync(object message, IMessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        private sealed class MessageHandler2 : IMessageHandler<object>
        {
            [MessageBusEndpoint(MessageBusTypes.MicroServiceBus)]
            public Task HandleAsync(object message, IMessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        private sealed class MessageHandler3 : IMessageHandler<object>, IMessageHandler<int>, IMessageHandler<string>
        {
            public Task HandleAsync(object message, IMessageHandlerOperationContext context) =>
                Task.CompletedTask;

            [MessageBusEndpoint(MessageBusTypes.MicroServiceBus)]
            public Task HandleAsync(int message, IMessageHandlerOperationContext context) =>
                Task.CompletedTask;

            [MessageBusEndpoint(MessageBusTypes.MicroServiceBus)]
            public Task HandleAsync(string message, IMessageHandlerOperationContext context) =>
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
            Processor.ConfigureMessages(messages =>
            {
                messages.MessageKindResolver = new FixedMessageKindResolver(MessageKind.Command);
            });
            Processor.ConfigureMessageHandlers(messageHandlers =>
            {
                messageHandlers.Add<MessageHandler1>();
            });

            Assert.AreEqual(0, CreateProcessor().CreateMicroServiceBusEndpoints().Count());
        }

        [TestMethod]
        public void CreateMicroServiceBusEndpoints_ReturnsOneEndpoint_IfMethodHasEndpointAttribute()
        {
            Processor.ConfigureMessages(messages =>
            {
                messages.MessageKindResolver = new FixedMessageKindResolver(MessageKind.Command);
            });
            Processor.ConfigureMessageHandlers(messageHandlers =>
            {
                messageHandlers.Add<MessageHandler2>();
            });

            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();

            Assert.AreEqual(typeof(MessageHandler2), endpoint.MessageHandlerType);
            Assert.AreSame(typeof(object), endpoint.MessageParameterInfo.ParameterType);
        }

        [TestMethod]
        public void CreateMicroServiceBusEndpoints_ReturnsMultipleEndpoints_IfMultipleMethodsHaveEndpointAttribute()
        {
            Processor.ConfigureMessages(messages =>
            {
                messages.MessageKindResolver = new FixedMessageKindResolver(MessageKind.Command);
            });
            Processor.ConfigureMessageHandlers(messageHandlers =>
            {
                messageHandlers.Add<MessageHandler3>();
            });

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
            Processor.ConfigureMessages(messages =>
            {
                messages.MessageKindResolver = new FixedMessageKindResolver(MessageKind.Command);
            });
            Processor.ConfigureMessageHandlers(messageHandlers =>
            {
                messageHandlers.Add<MessageHandler2>();
                messageHandlers.Add<MessageHandler3>();
            });

            var endpoints = CreateProcessor().CreateMicroServiceBusEndpoints().ToArray();

            Assert.AreEqual(3, endpoints.Length);
            Assert.IsTrue(endpoints.Any(endpoint => endpoint.MessageParameterInfo.ParameterType == typeof(object)));
            Assert.IsTrue(endpoints.Any(endpoint => endpoint.MessageParameterInfo.ParameterType == typeof(int)));
            Assert.IsTrue(endpoints.Any(endpoint => endpoint.MessageParameterInfo.ParameterType == typeof(string)));
        }

        [TestMethod]
        public void CreateMicroServiceBusEndpoints_ReturnsExpectedEndpoints_IfMessageHandlerWasAddedAsSingleton()
        {
            Processor.ConfigureMessages(messages =>
            {
                messages.MessageKindResolver = new FixedMessageKindResolver(MessageKind.Command);
            });
            Processor.ConfigureMessageHandlers(messageHandlers =>
            {
                messageHandlers.AddInstance(new MessageHandler2());
                messageHandlers.AddInstance(new MessageHandler3());
            });

            var endpoints = CreateProcessor().CreateMicroServiceBusEndpoints().ToArray();

            Assert.AreEqual(3, endpoints.Length);
            Assert.IsTrue(endpoints.Any(endpoint => endpoint.MessageParameterInfo.ParameterType == typeof(object)));
            Assert.IsTrue(endpoints.Any(endpoint => endpoint.MessageParameterInfo.ParameterType == typeof(int)));
            Assert.IsTrue(endpoints.Any(endpoint => endpoint.MessageParameterInfo.ParameterType == typeof(string)));
        }

        #endregion

        #region [====== MessageKinds ======]

        private sealed class UndefinedMessageHandler : IMessageHandler<object>
        {
            [MessageBusEndpoint(MessageBusTypes.MicroServiceBus)]
            public Task HandleAsync(object message, IMessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        private sealed class SomeRequest { }

        private sealed class SomeRequestHandler : IMessageHandler<SomeRequest>
        {
            [MessageBusEndpoint(MessageBusTypes.MicroServiceBus)]
            public Task HandleAsync(SomeRequest message, IMessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        private sealed class SomeCommand { }

        private sealed class SomeCommandHandler : IMessageHandler<SomeCommand>
        {
            [MessageBusEndpoint(MessageBusTypes.MicroServiceBus)]
            public Task HandleAsync(SomeCommand message, IMessageHandlerOperationContext context)
            {
                context.MessageBus.Publish(string.Empty);
                return Task.CompletedTask;
            }
        }

        private sealed class SomeEvent { }

        private sealed class SomeEventHandler : IMessageHandler<SomeEvent>
        {
            [MessageBusEndpoint(MessageBusTypes.MicroServiceBus)]
            public Task HandleAsync(SomeEvent message, IMessageHandlerOperationContext context)
            {
                context.MessageBus.Publish(string.Empty);
                return Task.CompletedTask;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CreateMicroServiceBusEndpoints_Throws_IfMessageKindIsUndefined()
        {
            Processor.ConfigureMessageHandlers(messageHandlers =>
            {
                messageHandlers.Add<UndefinedMessageHandler>();
            });

            CreateProcessor().CreateMicroServiceBusEndpoints().Single().IgnoreValue();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CreateMicroServiceBusEndpoints_Throws_IfMessageKindIsRequest()
        {
            Processor.ConfigureMessageHandlers(messageHandlers =>
            {
                messageHandlers.Add<SomeRequestHandler>();
            });

            CreateProcessor().CreateMicroServiceBusEndpoints().Single().IgnoreValue();
        }

        [TestMethod]
        public void CreateMicroServiceBusEndpoints_ReturnsEventHandler_IfMessageKindIsEvent()
        {
            Processor.ConfigureMessageHandlers(messageHandlers =>
            {
                messageHandlers.Add<SomeEventHandler>();
            });

            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();

            Assert.AreEqual(MessageKind.Event, endpoint.MessageKind);
        }

        [TestMethod]
        public void CreateMicroServiceBusEndpoints_ReturnsCommandHandler_IfMessageKindIsCommand()
        {
            Processor.ConfigureMessageHandlers(messageHandlers =>
            {
                messageHandlers.Add<SomeCommandHandler>();
            });

            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();

            Assert.AreEqual(MessageKind.Command, endpoint.MessageKind);
        }

        #endregion

        #region [====== ProcessAsync ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ProcessAsync_Throws_IfMessageIsNull()
        {
            Processor.ConfigureMessageHandlers(messageHandlers =>
            {
                messageHandlers.AddInstance(new SomeCommandHandler());
            });

            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();

            await endpoint.ProcessAsync(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task ProcessAsync_Throws_IfMessageIsNotSupported()
        {
            Processor.ConfigureMessageHandlers(messageHandlers =>
            {
                messageHandlers.AddInstance(new SomeCommandHandler());
            });

            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();
            var result = await endpoint.ProcessAsync(new object());

            Assert.AreEqual(0, result.MessageHandlerCount);
            Assert.AreEqual(0, result.Output.Count);
        }

        [TestMethod]
        public async Task ProcessAsync_ReturnsExpectedResult_IfMessageHandlerIsCommandHandler()
        {
            Processor.ConfigureMessageHandlers(messageHandlers =>
            {
                messageHandlers.AddInstance(new SomeCommandHandler());
                messageHandlers.AddInstance<string>((message, context) =>
                {
                    context.MessageBus.Publish(new object());
                });
            });

            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();
            var result = await endpoint.ProcessAsync(new SomeCommand());

            Assert.AreEqual(2, result.MessageHandlerCount);
            Assert.AreEqual(2, result.Output.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayTimeoutException))]
        public async Task ProcessAsync_ThrowsExpectedException_IfMessageHandlerIsCommandHandler_And_OperationWasCancelled()
        {
            using (var tokenSource = new CancellationTokenSource())
            {
                Processor.ConfigureMessageHandlers(messageHandlers =>
                {
                    messageHandlers.AddInstance(new SomeCommandHandler());
                    messageHandlers.AddInstance<string>((message, context) =>
                    {
                        tokenSource.Cancel();
                    });
                });

                var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();

                try
                {
                    await endpoint.ProcessAsync(new SomeCommand(), MessageHeader.Unspecified, tokenSource.Token);
                }
                catch (GatewayTimeoutException exception)
                {
                    Assert.IsInstanceOfType(exception.InnerException, typeof(OperationCanceledException));
                    throw;
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task ProcessAsync_ThrowsExpectedException_IfMessageHandlerIsCommandHandler_And_MessageHandlerOperationExceptionIsThrown()
        {
            var exceptionToThrow = new BusinessRuleViolationException();

            Processor.ConfigureMessageHandlers(messageHandlers =>
            {
                messageHandlers.AddInstance(new SomeCommandHandler());
                messageHandlers.AddInstance<string>((message, context) =>
                {
                    throw exceptionToThrow;
                });
            });

            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();

            try
            {
                await endpoint.ProcessAsync(new SomeCommand());
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(exceptionToThrow, exception.InnerException);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public async Task ProcessAsync_ThrowsExpectedException_IfMessageHandlerIsCommandHandler_And_BadRequestExceptionIsThrown()
        {
            var exceptionToThrow = new BadRequestException(MicroProcessorOperationStackTrace.Empty);

            Processor.ConfigureMessageHandlers(messageHandlers =>
            {
                messageHandlers.AddInstance(new SomeCommandHandler());
                messageHandlers.AddInstance<string>((message, context) =>
                {
                    throw exceptionToThrow;
                });
            });

            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();

            try
            {
                await endpoint.ProcessAsync(new SomeCommand());
            }
            catch (BadRequestException exception)
            {
                Assert.AreSame(exceptionToThrow, exception);
                throw;
            }
        }

        [TestMethod]
        public async Task ProcessAsync_ReturnsExpectedResult_IfMessageHandlerIsEventHandler()
        {
            Processor.ConfigureMessageHandlers(messageHandlers =>
            {
                messageHandlers.AddInstance(new SomeEventHandler());
                messageHandlers.AddInstance<string>((message, context) =>
                {
                    context.MessageBus.Publish(new object());
                });
            });

            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();
            var result = await endpoint.ProcessAsync(new SomeEvent());

            Assert.AreEqual(2, result.MessageHandlerCount);
            Assert.AreEqual(2, result.Output.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(GatewayTimeoutException))]
        public async Task ProcessAsync_ThrowsExpectedException_IfMessageHandlerIsEventHandler_And_OperationWasCancelled()
        {
            using (var tokenSource = new CancellationTokenSource())
            {
                Processor.ConfigureMessageHandlers(messageHandlers =>
                {
                    messageHandlers.AddInstance(new SomeEventHandler());
                    messageHandlers.AddInstance<string>((message, context) =>
                    {
                        tokenSource.Cancel();
                    });
                });

                var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();

                try
                {
                    await endpoint.ProcessAsync(new SomeEvent(), MessageHeader.Unspecified, tokenSource.Token);
                }
                catch (GatewayTimeoutException exception)
                {
                    Assert.IsInstanceOfType(exception.InnerException, typeof(OperationCanceledException));
                    throw;
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException), AllowDerivedTypes = true)]
        public async Task ProcessAsync_ThrowsExpectedException_IfMessageHandlerIsEventHandler_And_MessageHandlerOperationExceptionIsThrown()
        {
            var exceptionToThrow = new BusinessRuleViolationException();

            Processor.ConfigureMessageHandlers(messageHandlers =>
            {
                messageHandlers.AddInstance(new SomeEventHandler());
                messageHandlers.AddInstance<string>((message, context) =>
                {
                    throw exceptionToThrow;
                });
            });

            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();

            try
            {
                await endpoint.ProcessAsync(new SomeEvent());
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(exceptionToThrow, exception.InnerException);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task ProcessAsync_ThrowsExpectedException_IfMessageHandlerIsEventHandler_And_BadRequestExceptionIsThrown()
        {
            var exceptionToThrow = new BadRequestException(MicroProcessorOperationStackTrace.Empty);

            Processor.ConfigureMessageHandlers(messageHandlers =>
            {
                messageHandlers.AddInstance(new SomeEventHandler());
                messageHandlers.AddInstance<string>((message, context) =>
                {
                    throw exceptionToThrow;
                });
            });

            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();

            try
            {
                await endpoint.ProcessAsync(new SomeEvent());
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
