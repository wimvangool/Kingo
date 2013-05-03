using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YellowFlare.MessageProcessing.SampleHandlers;
using YellowFlare.MessageProcessing.SampleHandlers.ForMessageHandlerFactoryTests;

namespace YellowFlare.MessageProcessing
{
    [TestClass]
    public abstract class MessageHandlerFactoryTest
    {
        private MessageHandlerFactory _factory;

        [TestInitialize]
        public void Setup()
        {
            _factory = CreateFactory();
            _factory.RegisterMessageHandlers(Assembly.GetExecutingAssembly(), IsMessageHandlerForContainerTests);

            MessageCommandRecorder.Current = new MessageCommandRecorder();
        }        

        [TestCleanup]
        public void Teardown()
        {
            MessageCommandRecorder.Current = null;
        }

        protected abstract MessageHandlerFactory CreateFactory();

        [TestMethod]
        public virtual void CreateInternalHandlersForObject_ReturnsExpectedHandler_IfOneHandlerForMessageExists()
        {
            var message = new object();

            HandleInternal(message);

            Assert.AreEqual(0, MessageCommandRecorder.Current.RecordCount(typeof(MessageHandlerA), message));
            Assert.AreEqual(0, MessageCommandRecorder.Current.RecordCount(typeof(MessageHandlerB), message));
            Assert.AreEqual(1, MessageCommandRecorder.Current.RecordCount(typeof(MessageHandlerC), message));
        }

        [TestMethod]
        public virtual void CreateExternalHandlersForObject_ReturnsNoHandlers_IfNoHandlersForMessageExist()
        {
            var message = new object();

            HandleExternal(message);

            Assert.AreEqual(0, MessageCommandRecorder.Current.RecordCount(typeof(MessageHandlerA), message));
            Assert.AreEqual(0, MessageCommandRecorder.Current.RecordCount(typeof(MessageHandlerB), message));
            Assert.AreEqual(0, MessageCommandRecorder.Current.RecordCount(typeof(MessageHandlerC), message));
        }

        [TestMethod]
        public virtual void CreateInternalHandlersForCommand_ReturnsNoHandlers_IfNoHandlersForMessageExist()
        {
            var message = new Command();

            HandleInternal(message);

            Assert.AreEqual(0, MessageCommandRecorder.Current.RecordCount(typeof(MessageHandlerA), message));
            Assert.AreEqual(0, MessageCommandRecorder.Current.RecordCount(typeof(MessageHandlerB), message));
            Assert.AreEqual(1, MessageCommandRecorder.Current.RecordCount(typeof(MessageHandlerC), message));
        }

        [TestMethod]
        public virtual void CreateExternalHandlersForCommand_ReturnsExpectedHandlers_IfTwoHandlersForMessageExist()
        {
            var message = new Command();

            HandleExternal(message);

            Assert.AreEqual(1, MessageCommandRecorder.Current.RecordCount(typeof(MessageHandlerA), message));
            Assert.AreEqual(1, MessageCommandRecorder.Current.RecordCount(typeof(MessageHandlerB), message));
            Assert.AreEqual(0, MessageCommandRecorder.Current.RecordCount(typeof(MessageHandlerC), message));
        }

        [TestMethod]
        public virtual void CreateInternalHandlersForDomainEvent_ReturnsExpectedHandlers_IfThreeHandlersForMessageExist()
        {
            var message = new DomainEvent();

            HandleInternal(message);

            Assert.AreEqual(0, MessageCommandRecorder.Current.RecordCount(typeof(MessageHandlerA), message));
            Assert.AreEqual(1, MessageCommandRecorder.Current.RecordCount(typeof(MessageHandlerB), message));
            Assert.AreEqual(2, MessageCommandRecorder.Current.RecordCount(typeof(MessageHandlerC), message));
        }

        [TestMethod]
        public virtual void CreateExternalHandlersForDomainEvent_ReturnsNoHandlers_IfNoHandlersForMessageExist()
        {
            var message = new DomainEvent();

            HandleExternal(message);

            Assert.AreEqual(0, MessageCommandRecorder.Current.RecordCount(typeof(MessageHandlerA), message));
            Assert.AreEqual(0, MessageCommandRecorder.Current.RecordCount(typeof(MessageHandlerB), message));
            Assert.AreEqual(0, MessageCommandRecorder.Current.RecordCount(typeof(MessageHandlerC), message));
        }

        private void HandleInternal<TMessage>(TMessage message) where TMessage : class
        {
            foreach (var handler in _factory.CreateInternalHandlersFor(message))
            {
                handler.Handle(message);
            }
        }

        private void HandleExternal<TMessage>(TMessage message) where TMessage : class
        {
            foreach (var handler in _factory.CreateExternalHandlersFor(message))
            {
                handler.Handle(message);
            }
        }

        private static bool IsMessageHandlerForContainerTests(Type type)
        {
            return type.Namespace == "YellowFlare.MessageProcessing.SampleHandlers.ForMessageHandlerFactoryTests";
        }
    }
}
