//using System;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Kingo.MicroServices.TestEngine;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace Kingo.MicroServices.Controllers
//{
//    [TestClass]
//    public sealed class CreateMicroServiceBusEndpointsTest : MicroProcessorTest<MicroProcessor>
//    {
//        #region [====== Returned Collection/Endpoints ======]

//        private sealed class MessageHandler1 : IMessageHandler<object>
//        {
//            public Task HandleAsync(object message, IMessageHandlerOperationContext context) =>
//                Task.CompletedTask;
//        }

//        private sealed class MessageHandler2 : IMessageHandler<object>
//        {
//            [MicroServiceBusEndpoint(MicroServiceBusEndpointTypes.External)]
//            public Task HandleAsync(object message, IMessageHandlerOperationContext context) =>
//                Task.CompletedTask;
//        }

//        private sealed class MessageHandler3 : IMessageHandler<object>, IMessageHandler<int>, IMessageHandler<string>
//        {
//            public Task HandleAsync(object message, IMessageHandlerOperationContext context) =>
//                Task.CompletedTask;

//            [MicroServiceBusEndpoint(MicroServiceBusEndpointTypes.External)]
//            public Task HandleAsync(int message, IMessageHandlerOperationContext context) =>
//                Task.CompletedTask;

//            [MicroServiceBusEndpoint(MicroServiceBusEndpointTypes.External)]
//            public Task HandleAsync(string message, IMessageHandlerOperationContext context) =>
//                Task.CompletedTask;
//        }

//        [TestMethod]
//        public void CreateMicroServiceBusEndpoints_ReturnsEmptyCollection_IfNoMessageHandlersHaveBeenRegistered()
//        {
//            Assert.AreEqual(0, CreateProcessor().CreateMicroServiceBusEndpoints().Count());
//        }

//        [TestMethod]
//        public void CreateMicroServiceBusEndpoints_ReturnsEmptyCollection_IfMethodHasNoEndpointAttribute()
//        {
//            ProcessorBuilder.Messages.MessageKindResolver = new FixedMessageKindResolver(MessageKind.Command);
//            ProcessorBuilder.MessageHandlers.Add<MessageHandler1>();            

//            Assert.AreEqual(0, CreateProcessor().CreateMicroServiceBusEndpoints().Count());
//        }

//        [TestMethod]
//        public void CreateMicroServiceBusEndpoints_ReturnsOneEndpoint_IfMethodHasEndpointAttribute()
//        {
//            ProcessorBuilder.Messages.MessageKindResolver = new FixedMessageKindResolver(MessageKind.Command);
//            ProcessorBuilder.MessageHandlers.Add<MessageHandler2>();            

//            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();

//            Assert.AreEqual(typeof(MessageHandler2), endpoint.MessageHandlerType);
//            Assert.AreSame(typeof(object), endpoint.MessageParameterInfo.ParameterType);            
//        }

//        [TestMethod]
//        public void CreateMicroServiceBusEndpoints_ReturnsMultipleEndpoints_IfMultipleMethodsHaveEndpointAttribute()
//        {
//            ProcessorBuilder.Messages.MessageKindResolver = new FixedMessageKindResolver(MessageKind.Command);
//            ProcessorBuilder.MessageHandlers.Add<MessageHandler3>();            

//            var endpoints = CreateProcessor().CreateMicroServiceBusEndpoints().ToArray();

//            Assert.AreEqual(2, endpoints.Length);
//            Assert.AreSame(typeof(MessageHandler3), endpoints[0].MessageHandlerType);
//            Assert.AreSame(typeof(MessageHandler3), endpoints[1].MessageHandlerType);
//            Assert.IsTrue(endpoints.Any(endpoint => endpoint.MessageParameterInfo.ParameterType == typeof(int)));
//            Assert.IsTrue(endpoints.Any(endpoint => endpoint.MessageParameterInfo.ParameterType == typeof(string)));
//        }

//        [TestMethod]
//        public void CreateMicroServiceBusEndpoints_ReturnsMultipleEndpoints_IfMultipleMessageHandlersHaveEndpointAttribute()
//        {
//            ProcessorBuilder.Messages.MessageKindResolver = new FixedMessageKindResolver(MessageKind.Command);
//            ProcessorBuilder.MessageHandlers.Add<MessageHandler2>();
//            ProcessorBuilder.MessageHandlers.Add<MessageHandler3>();            

//            var endpoints = CreateProcessor().CreateMicroServiceBusEndpoints().ToArray();

//            Assert.AreEqual(3, endpoints.Length);            
//            Assert.IsTrue(endpoints.Any(endpoint => endpoint.MessageParameterInfo.ParameterType == typeof(object)));
//            Assert.IsTrue(endpoints.Any(endpoint => endpoint.MessageParameterInfo.ParameterType == typeof(int)));
//            Assert.IsTrue(endpoints.Any(endpoint => endpoint.MessageParameterInfo.ParameterType == typeof(string)));
//        }

//        [TestMethod]
//        public void CreateMicroServiceBusEndpoints_ReturnsExpectedEndpoints_IfMessageHandlerWasAddedAsSingleton()
//        {
//            ProcessorBuilder.Messages.MessageKindResolver = new FixedMessageKindResolver(MessageKind.Command);
//            ProcessorBuilder.MessageHandlers.AddInstance(new MessageHandler2());
//            ProcessorBuilder.MessageHandlers.AddInstance(new MessageHandler3());          

//            var endpoints = CreateProcessor().CreateMicroServiceBusEndpoints().ToArray();

//            Assert.AreEqual(3, endpoints.Length);
//            Assert.IsTrue(endpoints.Any(endpoint => endpoint.MessageParameterInfo.ParameterType == typeof(object)));
//            Assert.IsTrue(endpoints.Any(endpoint => endpoint.MessageParameterInfo.ParameterType == typeof(int)));
//            Assert.IsTrue(endpoints.Any(endpoint => endpoint.MessageParameterInfo.ParameterType == typeof(string)));
//        }

//        #endregion

//        #region [====== MessageKinds ======]

//        private sealed class UndefinedMessageHandler : IMessageHandler<object>
//        {
//            [MicroServiceBusEndpoint(MicroServiceBusEndpointTypes.External)]
//            public Task HandleAsync(object message, IMessageHandlerOperationContext context) =>
//                Task.CompletedTask;
//        }

//        private sealed class SomeRequest { }

//        private sealed class SomeRequestHandler : IMessageHandler<SomeRequest>
//        {
//            [MicroServiceBusEndpoint(MicroServiceBusEndpointTypes.External)]
//            public Task HandleAsync(SomeRequest message, IMessageHandlerOperationContext context) =>
//                Task.CompletedTask;
//        }

//        private sealed class SomeCommand { }

//        private sealed class SomeCommandHandler : IMessageHandler<SomeCommand>
//        {
//            [MicroServiceBusEndpoint(MicroServiceBusEndpointTypes.External)]
//            public Task HandleAsync(SomeCommand message, IMessageHandlerOperationContext context)
//            {
//                context.MessageBus.Publish(string.Empty);
//                return Task.CompletedTask;
//            }
//        }

//        private sealed class SomeEvent { }

//        private sealed class SomeEventHandler : IMessageHandler<SomeEvent>
//        {
//            [MicroServiceBusEndpoint(MicroServiceBusEndpointTypes.External)]
//            public Task HandleAsync(SomeEvent message, IMessageHandlerOperationContext context)
//            {
//                context.MessageBus.Publish(string.Empty);
//                return Task.CompletedTask;
//            }
//        }

//        [TestMethod]
//        [ExpectedException(typeof(InvalidOperationException))]
//        public void CreateMicroServiceBusEndpoints_Throws_IfMessageKindIsUndefined()
//        {
//            ProcessorBuilder.MessageHandlers.Add<UndefinedMessageHandler>();

//            CreateProcessor().CreateMicroServiceBusEndpoints().Single().IgnoreValue();
//        }

//        [TestMethod]
//        [ExpectedException(typeof(InvalidOperationException))]
//        public void CreateMicroServiceBusEndpoints_Throws_IfMessageKindIsRequest()
//        {
//            ProcessorBuilder.MessageHandlers.Add<SomeRequestHandler>();            

//            CreateProcessor().CreateMicroServiceBusEndpoints().Single().IgnoreValue();
//        }

//        [TestMethod]
//        public void CreateMicroServiceBusEndpoints_ReturnsEventHandler_IfMessageKindIsEvent()
//        {
//            ProcessorBuilder.MessageHandlers.Add<SomeEventHandler>();            

//            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();

//            Assert.AreEqual(MessageKind.Event, endpoint.MessageKind);
//        }

//        [TestMethod]
//        public void CreateMicroServiceBusEndpoints_ReturnsCommandHandler_IfMessageKindIsCommand()
//        {
//            ProcessorBuilder.MessageHandlers.Add<SomeCommandHandler>();            

//            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();

//            Assert.AreEqual(MessageKind.Command, endpoint.MessageKind);
//        }

//        #endregion

//        #region [====== Name ======]



//        #endregion

//        #region [====== InvokeAsync ======]

//        [TestMethod]
//        [ExpectedException(typeof(ArgumentNullException))]
//        public async Task InvokeAsync_Throws_IfMessageIsNull()
//        {
//            ProcessorBuilder.MessageHandlers.AddInstance(new SomeCommandHandler());

//            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();

//            await endpoint.InvokeAsync(null);
//        }

//        [TestMethod]        
//        public async Task InvokeAsync_ReturnsEmptyResult_IfMessageIsNotSupported()
//        {
//            ProcessorBuilder.MessageHandlers.AddInstance(new SomeCommandHandler());

//            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();
//            var result = await endpoint.InvokeAsync(new object());

//            Assert.AreEqual(0, result.MessageHandlerCount);
//            Assert.AreEqual(0, result.Output.Count);
//        }

//        [TestMethod]
//        public async Task InvokeAsync_ReturnsExpectedResult_IfMessageHandlerIsCommandHandler()
//        {
//            ProcessorBuilder.MessageHandlers.AddInstance(new SomeCommandHandler());
//            ProcessorBuilder.MessageHandlers.AddInstance<string>((message, context) =>
//            {
//                context.MessageBus.Publish(new object());
//            });

//            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();
//            var result = await endpoint.InvokeAsync(DateTimeOffset.UtcNow.Second);

//            Assert.AreEqual(2, result.MessageHandlerCount);
//            Assert.AreEqual(2, result.Output.Count);
//        }

//        [TestMethod]
//        [ExpectedException(typeof(OperationCanceledException), AllowDerivedTypes = true)]
//        public async Task InvokeAsync_ThrowsExpectedException_IfMessageHandlerIsCommandHandler_And_OperationWasCancelled()
//        {
//            using (var tokenSource = new CancellationTokenSource())
//            {
//                ProcessorBuilder.MessageHandlers.AddInstance(new SomeCommandHandler());
//                ProcessorBuilder.MessageHandlers.AddInstance<string>((message, context) =>
//                {
//                    tokenSource.Cancel();
//                });

//                var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();

//                try
//                {
//                    await endpoint.InvokeAsync(DateTimeOffset.UtcNow.Second, tokenSource.Token);
//                }
//                catch (OperationCanceledException exception)
//                {
//                    Assert.AreEqual(tokenSource.Token, exception.CancellationToken);
//                    throw;
//                }
//            }
//        }

//        [TestMethod]
//        [ExpectedException(typeof(BadRequestException), AllowDerivedTypes = true)]
//        public async Task InvokeAsync_ThrowsExpectedException_IfMessageHandlerIsCommandHandler_And_MessageHandlerOperationExceptionIsThrown()
//        {
//            var exceptionToThrow = new BusinessRuleException();

//            ProcessorBuilder.MessageHandlers.AddInstance(new SomeCommandHandler());
//            ProcessorBuilder.MessageHandlers.AddInstance<string>((message, context) =>
//            {
//                throw exceptionToThrow;
//            });

//            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();

//            try
//            {
//                await endpoint.InvokeAsync(DateTimeOffset.UtcNow.Second);
//            }
//            catch (BadRequestException exception)
//            {
//                Assert.AreSame(exceptionToThrow, exception.InnerException);
//                throw;
//            }            
//        }

//        [TestMethod]
//        [ExpectedException(typeof(BadRequestException))]
//        public async Task InvokeAsync_ThrowsExpectedException_IfMessageHandlerIsCommandHandler_And_BadRequestExceptionIsThrown()
//        {
//            var exceptionToThrow = new BadRequestException(null, null);

//            ProcessorBuilder.MessageHandlers.AddInstance(new SomeCommandHandler());
//            ProcessorBuilder.MessageHandlers.AddInstance<string>((message, context) =>
//            {
//                throw exceptionToThrow;
//            });

//            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();

//            try
//            {
//                await endpoint.InvokeAsync(DateTimeOffset.UtcNow.Second);
//            }
//            catch (BadRequestException exception)
//            {
//                Assert.AreSame(exceptionToThrow, exception);
//                throw;
//            }
//        }

//        [TestMethod]
//        public async Task InvokeAsync_ReturnsExpectedResult_IfMessageHandlerIsEventHandler()
//        {
//            ProcessorBuilder.MessageHandlers.AddInstance(new SomeEventHandler());
//            ProcessorBuilder.MessageHandlers.AddInstance<string>((message, context) =>
//            {
//                context.MessageBus.Publish(new object());
//            });

//            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();
//            var result = await endpoint.InvokeAsync(DateTimeOffset.UtcNow.Second);

//            Assert.AreEqual(2, result.MessageHandlerCount);
//            Assert.AreEqual(2, result.Output.Count);
//        }

//        [TestMethod]
//        [ExpectedException(typeof(OperationCanceledException), AllowDerivedTypes = true)]
//        public async Task InvokeAsync_ThrowsExpectedException_IfMessageHandlerIsEventHandler_And_OperationWasCancelled()
//        {
//            using (var tokenSource = new CancellationTokenSource())
//            {
//                ProcessorBuilder.MessageHandlers.AddInstance(new SomeEventHandler());
//                ProcessorBuilder.MessageHandlers.AddInstance<string>((message, context) =>
//                {
//                    tokenSource.Cancel();
//                });

//                var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();

//                try
//                {
//                    await endpoint.InvokeAsync(DateTimeOffset.UtcNow.Second, tokenSource.Token);
//                }
//                catch (OperationCanceledException exception)
//                {
//                    Assert.AreEqual(tokenSource.Token, exception.CancellationToken);
//                    throw;
//                }
//            }
//        }

//        [TestMethod]
//        [ExpectedException(typeof(InternalServerErrorException), AllowDerivedTypes = true)]
//        public async Task InvokeAsync_ThrowsExpectedException_IfMessageHandlerIsEventHandler_And_MessageHandlerOperationExceptionIsThrown()
//        {
//            var exceptionToThrow = new BusinessRuleException();

//            ProcessorBuilder.MessageHandlers.AddInstance(new SomeEventHandler());
//            ProcessorBuilder.MessageHandlers.AddInstance<string>((message, context) =>
//            {
//                throw exceptionToThrow;
//            });

//            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();

//            try
//            {
//                await endpoint.InvokeAsync(DateTimeOffset.UtcNow.Second);
//            }
//            catch (InternalServerErrorException exception)
//            {
//                Assert.AreSame(exceptionToThrow, exception.InnerException);
//                throw;
//            }
//        }

//        [TestMethod]
//        [ExpectedException(typeof(InternalServerErrorException))]
//        public async Task InvokeAsync_ThrowsExpectedException_IfMessageHandlerIsEventHandler_And_BadRequestExceptionIsThrown()
//        {
//            var exceptionToThrow = new BadRequestException(null, null);

//            ProcessorBuilder.MessageHandlers.AddInstance(new SomeEventHandler());
//            ProcessorBuilder.MessageHandlers.AddInstance<string>((message, context) =>
//            {
//                throw exceptionToThrow;
//            });

//            var endpoint = CreateProcessor().CreateMicroServiceBusEndpoints().Single();

//            try
//            {
//                await endpoint.InvokeAsync(DateTimeOffset.UtcNow.Second);
//            }
//            catch (InternalServerErrorException exception)
//            {
//                Assert.AreSame(exceptionToThrow, exception.InnerException);
//                throw;
//            }
//        }

//        #endregion
//    }
//}
