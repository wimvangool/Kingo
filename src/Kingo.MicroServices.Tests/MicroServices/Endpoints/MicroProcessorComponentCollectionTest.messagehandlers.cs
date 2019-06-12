using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Endpoints
{
    public sealed partial class MicroProcessorComponentCollectionTest
    {
        #region [====== MessageHandlerTypes ======]

        private abstract class AbstractMessageHandler : IMessageHandler<object>
        {
            public abstract Task HandleAsync(object message, MessageHandlerOperationContext context);
        }

        private sealed class GenericMessageHandler<TMessage> : IMessageHandler<TMessage>
        {
            public Task HandleAsync(TMessage message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        private sealed class MessageHandler1 : IMessageHandler<object>
        {
            public Task HandleAsync(object message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        private sealed class MessageHandler2 : IMessageHandler<object>, IMessageHandler<int>
        {
            public Task HandleAsync(object message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;

            public Task HandleAsync(int message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        [MicroProcessorComponent((ServiceLifetime) (-1))]
        private sealed class InvalidLifetimeMessageHandler : IMessageHandler<object>
        {
            public Task HandleAsync(object message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        [MicroProcessorComponent(ServiceLifetime.Scoped)]
        private sealed class ScopedMessageHandler : IMessageHandler<object>
        {
            public Task HandleAsync(object message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        [MicroProcessorComponent(ServiceLifetime.Singleton)]
        private sealed class SingletonMessageHandler : IMessageHandler<object>
        {
            public Task HandleAsync(object message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        #endregion

        #region [====== AddMessageHandlers (Registration & Mapping) ======]

        [TestMethod]
        public void AddMessageHandlers_AddsNoMessageHandlers_IfThereAreNoTypesToScan()
        {
            _components.AddMessageHandlers();

            Assert.AreEqual(1, BuildServiceCollection().Count);
        }

        [TestMethod]
        public void AddMessageHandlers_AddsNoMessageHandlers_IfThereAreNoMessageHandlerTypesToAdd()
        {
            _components.AddTypes(typeof(object), typeof(int), typeof(Query1));
            _components.AddTypes(typeof(AbstractMessageHandler), typeof(GenericMessageHandler<>));
            _components.AddMessageHandlers();

            Assert.AreEqual(1, BuildServiceCollection().Count);
        }

        [TestMethod]
        public void AddMessageHandlers_AddsExpectedMessageHandler_IfMessageHandlerIsClosedGenericType()
        {
            _components.AddTypes(typeof(GenericMessageHandler<object>));
            _components.AddMessageHandlers();

            var provider = BuildServiceProvider();

            Assert.IsInstanceOfType(provider.GetRequiredService<IMessageHandler<object>>(), typeof(GenericMessageHandler<object>));
            Assert.IsNotNull(provider.GetRequiredService<GenericMessageHandler<object>>());
        }

        [TestMethod]
        public void AddMessageHandlers_AddsExpectedMessageHandler_IfMessageHandlerIsRegularType()
        {
            _components.AddTypes(typeof(MessageHandler1));
            _components.AddMessageHandlers();

            var provider = BuildServiceProvider();

            Assert.IsInstanceOfType(provider.GetRequiredService<IMessageHandler<object>>(), typeof(MessageHandler1));
            Assert.IsNotNull(provider.GetRequiredService<MessageHandler1>());
        }

        [TestMethod]
        public void AddMessageHandlers_AddsExpectedMessageHandler_IfMessageHandlerImplementsMultipleInterfaces()
        {
            _components.AddTypes(typeof(MessageHandler2));
            _components.AddMessageHandlers();

            var provider = BuildServiceProvider();

            Assert.IsInstanceOfType(provider.GetRequiredService<IMessageHandler<object>>(), typeof(MessageHandler2));
            Assert.IsInstanceOfType(provider.GetRequiredService<IMessageHandler<int>>(), typeof(MessageHandler2));
            Assert.IsNotNull(provider.GetRequiredService<MessageHandler2>());
        }

        [TestMethod]
        public void AddMessageHandlers_AddsExpectedMessageHandlers_IfMultipleMessageHandlersImplementTheSameInterface()
        {
            _components.AddTypes(typeof(MessageHandler1), typeof(MessageHandler2));
            _components.AddMessageHandlers();

            var provider = BuildServiceProvider();

            Assert.IsInstanceOfType(provider.GetRequiredService<IMessageHandler<object>>(), typeof(MessageHandler2));
            Assert.IsNotNull(provider.GetRequiredService<MessageHandler1>());
            Assert.IsNotNull(provider.GetRequiredService<MessageHandler2>());
            Assert.AreEqual(2, provider.GetRequiredService<IEnumerable<IMessageHandler<object>>>().Count());
        }

        #endregion

        #region [====== AddMessageHandlers (Lifetime) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddMessageHandlers_Throws_IfMessageHandlerHasInvalidLifetime()
        {
            _components.AddTypes(typeof(InvalidLifetimeMessageHandler));
            _components.AddMessageHandlers();
        }

        [TestMethod]
        public void AddMessageHandlers_AddsTransientMessageHandler_IfMessageHandlerHasTransientLifetime()
        {
            _components.AddTypes(typeof(MessageHandler1));
            _components.AddMessageHandlers();

            var provider = BuildServiceProvider();
            var messageHandlerA = provider.GetRequiredService<MessageHandler1>();

            using (var scope = provider.CreateScope())
            {
                var messageHandlerB = scope.ServiceProvider.GetRequiredService<MessageHandler1>();
                var messageHandlerC = scope.ServiceProvider.GetRequiredService<MessageHandler1>();

                Assert.AreNotSame(messageHandlerA, messageHandlerB);
                Assert.AreNotSame(messageHandlerB, messageHandlerC);
            }

            var messageHandlerD = provider.GetRequiredService<MessageHandler1>();

            Assert.AreNotSame(messageHandlerA, messageHandlerD);
        }

        [TestMethod]
        public void AddMessageHandlers_AddsScopedMessageHandler_IfMessageHandlerHasScopedLifetime()
        {
            _components.AddTypes(typeof(ScopedMessageHandler));
            _components.AddMessageHandlers();

            var provider = BuildServiceProvider();
            var messageHandlerA = provider.GetRequiredService<ScopedMessageHandler>();

            using (var scope = provider.CreateScope())
            {
                var messageHandlerB = scope.ServiceProvider.GetRequiredService<ScopedMessageHandler>();
                var messageHandlerC = scope.ServiceProvider.GetRequiredService<ScopedMessageHandler>();

                Assert.AreNotSame(messageHandlerA, messageHandlerB);
                Assert.AreSame(messageHandlerB, messageHandlerC);
            }

            var messageHandlerD = provider.GetRequiredService<ScopedMessageHandler>();

            Assert.AreSame(messageHandlerA, messageHandlerD);
        }

        [TestMethod]
        public void AddMessageHandlers_AddsSingletonMessageHandler_IfMessageHandlerHasSingletonLifetime()
        {
            _components.AddTypes(typeof(SingletonMessageHandler));
            _components.AddMessageHandlers();

            var provider = BuildServiceProvider();
            var messageHandlerA = provider.GetRequiredService<SingletonMessageHandler>();

            using (var scope = provider.CreateScope())
            {
                var messageHandlerB = scope.ServiceProvider.GetRequiredService<SingletonMessageHandler>();
                var messageHandlerC = scope.ServiceProvider.GetRequiredService<SingletonMessageHandler>();

                Assert.AreSame(messageHandlerA, messageHandlerB);
                Assert.AreSame(messageHandlerB, messageHandlerC);
            }

            var messageHandlerD = provider.GetRequiredService<SingletonMessageHandler>();

            Assert.AreSame(messageHandlerA, messageHandlerD);
        }

        #endregion

        #region [====== AddMessageHandler ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddMessageHandler_Throws_IfMessageHandlerIsNull()
        {
            _components.AddMessageHandler(null);
        }

        [TestMethod]
        public void AddMessageHandler_AddsAllExpectedMappings_IfMessageHandlerImplementsMultipleInterfaces()
        {
            var messageHandler = new MessageHandler2();

            _components.AddMessageHandler(messageHandler);

            var provider = BuildServiceProvider();

            Assert.AreSame(messageHandler, provider.GetRequiredService<IMessageHandler<object>>());
            Assert.AreSame(messageHandler, provider.GetRequiredService<IMessageHandler<int>>());
            Assert.AreSame(messageHandler, provider.GetRequiredService<MessageHandler2>());
        }

        [TestMethod]
        public void AddMessageHandler_AddsAllExpectedMappings_IfMessageTypeIsSpecifiedExplicitly()
        {
            var messageHandler = new MessageHandler2();

            _components.AddMessageHandler<int>(messageHandler);

            var provider = BuildServiceProvider();

            Assert.IsNull(provider.GetService<IMessageHandler<object>>());            
            Assert.AreSame(messageHandler, provider.GetRequiredService<IMessageHandler<int>>());
            Assert.AreSame(messageHandler, provider.GetRequiredService<MessageHandler2>());
        }

        [TestMethod]
        public void AddMessageHandler_AddsAllExpectedMappings_IfMessageHandlerIsAction()
        {            
            _components.AddMessageHandler<object>((message, context) => { });

            var provider = BuildServiceProvider();

            var messageHandlerA = provider.GetRequiredService<IMessageHandler<object>>();
            var messageHandlerB = provider.GetRequiredService<IMessageHandler<object>>();

            Assert.AreSame(messageHandlerA, messageHandlerB);
        }

        [TestMethod]
        public void AddMessageHandler_AddsAllExpectedMappings_IfMessageHandlerIsFunc()
        {
            _components.AddMessageHandler<object>((message, context) => Task.CompletedTask);

            var provider = BuildServiceProvider();

            var messageHandlerA = provider.GetRequiredService<IMessageHandler<object>>();
            var messageHandlerB = provider.GetRequiredService<IMessageHandler<object>>();

            Assert.AreSame(messageHandlerA, messageHandlerB);
        }

        #endregion
    }
}
