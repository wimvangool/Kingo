using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Controllers
{
    [TestClass]
    public sealed class AddMessageHandlersTest : MicroProcessorTest<MicroProcessor>
    {
        #region [====== MessageHandlerTypes ======]

        private abstract class AbstractMessageHandler : IMessageHandler<object>
        {
            public abstract Task HandleAsync(object message, IMessageHandlerOperationContext context);
        }

        private sealed class GenericMessageHandler<TMessage> : IMessageHandler<TMessage>
        {
            public Task HandleAsync(TMessage message, IMessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        private sealed class MessageHandler1 : IMessageHandler<object>
        {
            public Task HandleAsync(object message, IMessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        private sealed class MessageHandler2 : IMessageHandler<object>, IMessageHandler<int>
        {
            public Task HandleAsync(object message, IMessageHandlerOperationContext context) =>
                Task.CompletedTask;

            public Task HandleAsync(int message, IMessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        [MicroProcessorComponent((ServiceLifetime) (-1))]
        private sealed class InvalidLifetimeMessageHandler : IMessageHandler<object>
        {
            public Task HandleAsync(object message, IMessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        [MicroProcessorComponent(ServiceLifetime.Scoped)]
        private sealed class ScopedMessageHandler : IMessageHandler<object>
        {
            public Task HandleAsync(object message, IMessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        [MicroProcessorComponent(ServiceLifetime.Singleton)]
        private sealed class SingletonMessageHandler : IMessageHandler<object>
        {
            public Task HandleAsync(object message, IMessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        #endregion

        #region [====== AddMessageHandlers (Registration & Mapping) ======]

        [TestMethod]
        public void AddMessageHandlers_AddsNoMessageHandlers_IfThereAreNoTypesToScan()
        {
            ProcessorBuilder.Components.AddMessageHandlers();

            Assert.AreEqual(DefaultServiceCount, BuildServiceCollection().Count);
        }

        [TestMethod]
        public void AddMessageHandlers_AddsNoMessageHandlers_IfThereAreNoMessageHandlerTypesToAdd()
        {
            ProcessorBuilder.Components.AddToSearchSet(typeof(object), typeof(int));
            ProcessorBuilder.Components.AddToSearchSet(typeof(AbstractMessageHandler), typeof(GenericMessageHandler<>));
            ProcessorBuilder.Components.AddMessageHandlers();

            Assert.AreEqual(DefaultServiceCount, BuildServiceCollection().Count);
        }

        [TestMethod]
        public void AddMessageHandlers_AddsExpectedMessageHandler_IfMessageHandlerIsClosedGenericType()
        {
            ProcessorBuilder.Components.AddToSearchSet(typeof(GenericMessageHandler<object>));
            ProcessorBuilder.Components.AddMessageHandlers();

            var provider = BuildServiceProvider();

            Assert.IsInstanceOfType(provider.GetRequiredService<IMessageHandler<object>>(), typeof(GenericMessageHandler<object>));
            Assert.IsNotNull(provider.GetRequiredService<GenericMessageHandler<object>>());
        }

        [TestMethod]
        public void AddMessageHandlers_AddsExpectedMessageHandler_IfMessageHandlerIsRegularType()
        {
            ProcessorBuilder.Components.AddToSearchSet(typeof(MessageHandler1));
            ProcessorBuilder.Components.AddMessageHandlers();

            var provider = BuildServiceProvider();

            Assert.IsInstanceOfType(provider.GetRequiredService<IMessageHandler<object>>(), typeof(MessageHandler1));
            Assert.IsNotNull(provider.GetRequiredService<MessageHandler1>());
        }

        [TestMethod]
        public void AddMessageHandlers_AddsExpectedMessageHandler_IfMessageHandlerImplementsMultipleInterfaces()
        {
            ProcessorBuilder.Components.AddToSearchSet(typeof(MessageHandler2));
            ProcessorBuilder.Components.AddMessageHandlers();

            var provider = BuildServiceProvider();

            Assert.IsInstanceOfType(provider.GetRequiredService<IMessageHandler<object>>(), typeof(MessageHandler2));
            Assert.IsInstanceOfType(provider.GetRequiredService<IMessageHandler<int>>(), typeof(MessageHandler2));
            Assert.IsNotNull(provider.GetRequiredService<MessageHandler2>());
        }

        [TestMethod]
        public void AddMessageHandlers_AddsExpectedMessageHandlers_IfMultipleMessageHandlersImplementTheSameInterface()
        {
            ProcessorBuilder.Components.AddToSearchSet(typeof(MessageHandler1), typeof(MessageHandler2));
            ProcessorBuilder.Components.AddMessageHandlers();

            var provider = BuildServiceProvider();

            Assert.IsInstanceOfType(provider.GetRequiredService<IMessageHandler<object>>(), typeof(MessageHandler2));
            Assert.IsNotNull(provider.GetRequiredService<MessageHandler1>());
            Assert.IsNotNull(provider.GetRequiredService<MessageHandler2>());
            Assert.AreEqual(2, provider.GetRequiredService<IEnumerable<IMessageHandler<object>>>().Count());
        }

        #endregion

        #region [====== AddMessageHandlers (Lifetime) ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BuildServiceProvider_Throws_IfMessageHandlerHasInvalidLifetime()
        {
            ProcessorBuilder.Components.AddToSearchSet(typeof(InvalidLifetimeMessageHandler));
            ProcessorBuilder.Components.AddMessageHandlers();

            BuildServiceProvider();
        }

        [TestMethod]
        public void AddMessageHandlers_AddsTransientMessageHandler_IfMessageHandlerHasTransientLifetime()
        {
            ProcessorBuilder.Components.AddToSearchSet(typeof(MessageHandler1));
            ProcessorBuilder.Components.AddMessageHandlers();

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
            ProcessorBuilder.Components.AddToSearchSet(typeof(ScopedMessageHandler));
            ProcessorBuilder.Components.AddMessageHandlers();

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
            ProcessorBuilder.Components.AddToSearchSet(typeof(SingletonMessageHandler));
            ProcessorBuilder.Components.AddMessageHandlers();

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
            ProcessorBuilder.Components.AddMessageHandler(null);
        }

        [TestMethod]
        public void AddMessageHandler_AddsAllExpectedMappings_IfMessageHandlerImplementsMultipleInterfaces()
        {
            var messageHandler = new MessageHandler2();

            ProcessorBuilder.Components.AddMessageHandler(messageHandler);

            var provider = BuildServiceProvider();

            Assert.AreSame(messageHandler, provider.GetRequiredService<IMessageHandler<object>>());
            Assert.AreSame(messageHandler, provider.GetRequiredService<IMessageHandler<int>>());
            Assert.AreSame(messageHandler, provider.GetRequiredService<MessageHandler2>());
        }        

        [TestMethod]
        public void AddMessageHandler_AddsAllExpectedMappings_IfMessageHandlerIsAction()
        {            
            ProcessorBuilder.Components.AddMessageHandler<object>((message, context) => { });

            var provider = BuildServiceProvider();

            var messageHandlerA = provider.GetRequiredService<IMessageHandler<object>>();
            var messageHandlerB = provider.GetRequiredService<IMessageHandler<object>>();

            Assert.AreSame(messageHandlerA, messageHandlerB);
        }

        [TestMethod]
        public void AddMessageHandler_AddsAllExpectedMappings_IfMessageHandlerIsFunc()
        {
            ProcessorBuilder.Components.AddMessageHandler<object>((message, context) => Task.CompletedTask);

            var provider = BuildServiceProvider();

            var messageHandlerA = provider.GetRequiredService<IMessageHandler<object>>();
            var messageHandlerB = provider.GetRequiredService<IMessageHandler<object>>();

            Assert.AreSame(messageHandlerA, messageHandlerB);
        }

        #endregion
    }
}
