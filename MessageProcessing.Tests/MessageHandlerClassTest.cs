using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YellowFlare.MessageProcessing.SampleHandlers.ForTryRegisterInTests;

namespace YellowFlare.MessageProcessing
{
    [TestClass]
    public class MessageHandlerClassTest
    {
        private MessageHandlerFactoryStub _container;

        [TestInitialize]
        public void Setup()
        {
            _container = new MessageHandlerFactoryStub();
        }

        [TestMethod]
        public void TryRegisterIn_ReturnsFalse_IfTypeIsInterface()
        {
            Type typeToRegister = typeof(IDisposable);
            MessageHandlerClass handlerClass;

            Assert.IsFalse(MessageHandlerClass.TryRegisterIn(_container, typeToRegister, null, out handlerClass));
            Assert.IsNull(handlerClass);
        }

        [TestMethod]
        public void TryRegisterIn_ReturnsFalse_IsTypeIsStruct()
        {
            Type typeToRegister = typeof(int);
            MessageHandlerClass handlerClass;

            Assert.IsFalse(MessageHandlerClass.TryRegisterIn(_container, typeToRegister, null, out handlerClass));
            Assert.IsNull(handlerClass);
        }

        [TestMethod]
        public void TryRegisterIn_ReturnsFalse_IfTypeIsAbstractClass()
        {
            Type typeToRegister = typeof(AbstractMessageHandler);
            MessageHandlerClass handlerClass;

            Assert.IsFalse(MessageHandlerClass.TryRegisterIn(_container, typeToRegister, null, out handlerClass));
            Assert.IsNull(handlerClass);
        }

        [TestMethod]
        public void TryRegisterIn_ReturnsFalse_IfTypeIsGenericTypeDefinition()
        {
            Type typeToRegister = typeof(GenericCommandHandler<>);
            MessageHandlerClass handlerClass;

            Assert.IsFalse(MessageHandlerClass.TryRegisterIn(_container, typeToRegister, null, out handlerClass));
            Assert.IsNull(handlerClass);
        }

        [TestMethod]
        public void TryRegisterIn_ReturnsFalse_IfTypeIsNoMessageHandler()
        {
            Type typeToRegister = typeof(string);
            MessageHandlerClass handlerClass;

            Assert.IsFalse(MessageHandlerClass.TryRegisterIn(_container, typeToRegister, null, out handlerClass));
            Assert.IsNull(handlerClass);
        }

        [TestMethod]
        public void TryRegisterIn_ReturnsFalse_IfTypeDoesNotSatisfyPredicate()
        {
            Type typeToRegister = typeof(MessageHandlerWithPerResolveLifetime);
            MessageHandlerClass handlerClass;

            Assert.IsFalse(MessageHandlerClass.TryRegisterIn(_container, typeToRegister, type => false, out handlerClass));
            Assert.IsNull(handlerClass);
        }

        [TestMethod]
        public void TryRegisterIn_ReturnsTrue_IfTypeIsMessageHandlerThatSatisfiesPredicateAndLifetimeIsPerResolve()
        {
            Type typeToRegister = typeof(MessageHandlerWithPerResolveLifetime);
            MessageHandlerClass handlerClass;

            Assert.IsTrue(MessageHandlerClass.TryRegisterIn(_container, typeToRegister, null, out handlerClass));
            Assert.IsNotNull(handlerClass);
            Assert.IsTrue(_container.HasRegistered(typeToRegister, InstanceLifetime.PerResolve));
        }

        [TestMethod]
        public void TryRegisterIn_ReturnsTrue_IfTypeIsMessageHandlerThatSatisfiesPredicateAndLifetimeIsPerUnitOfWork()
        {
            Type typeToRegister = typeof(MessageHandlerWithPerUnitOfWorkLifetime);
            MessageHandlerClass handlerClass;

            Assert.IsTrue(MessageHandlerClass.TryRegisterIn(_container, typeToRegister, null, out handlerClass));
            Assert.IsNotNull(handlerClass);
            Assert.IsTrue(_container.HasRegistered(typeToRegister, InstanceLifetime.PerUnitOfWork));
        }

        [TestMethod]
        public void TryRegisterIn_ReturnsTrue_IfTypeIsMessageHandlerThatSatisfiesPredicateAndLifetimeIsSingle()
        {
            Type typeToRegister = typeof(MessageHandlerWithSingleLifetime);
            MessageHandlerClass handlerClass;

            Assert.IsTrue(MessageHandlerClass.TryRegisterIn(_container, typeToRegister, null, out handlerClass));
            Assert.IsNotNull(handlerClass);
            Assert.IsTrue(_container.HasRegistered(typeToRegister, InstanceLifetime.Single));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TryRegisterIn_Throws_IfSpecifiedLifetimeOnMessageHandlerIsInvalid()
        {
            Type typeToRegister = typeof(MessageHandlerWithInvalidLifetimeAttribute);
            MessageHandlerClass handlerClass;

            MessageHandlerClass.TryRegisterIn(_container, typeToRegister, null, out handlerClass);
        }
    }
}
