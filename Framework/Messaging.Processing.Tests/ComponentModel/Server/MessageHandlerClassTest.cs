using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Syztem.ComponentModel.Server.SampleHandlers.ForTryRegisterInTests;

namespace Syztem.ComponentModel.Server
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

            Assert.IsFalse(TryRegisterIn(_container, typeToRegister, null, out handlerClass));
            Assert.IsNull(handlerClass);
        }

        [TestMethod]
        public void TryRegisterIn_ReturnsFalse_IsTypeIsStruct()
        {
            Type typeToRegister = typeof(int);
            MessageHandlerClass handlerClass;

            Assert.IsFalse(TryRegisterIn(_container, typeToRegister, null, out handlerClass));
            Assert.IsNull(handlerClass);
        }

        [TestMethod]
        public void TryRegisterIn_ReturnsFalse_IfTypeIsAbstractClass()
        {
            Type typeToRegister = typeof(AbstractMessageHandler);
            MessageHandlerClass handlerClass;

            Assert.IsFalse(TryRegisterIn(_container, typeToRegister, null, out handlerClass));
            Assert.IsNull(handlerClass);
        }

        [TestMethod]
        public void TryRegisterIn_ReturnsFalse_IfTypeIsGenericTypeDefinition()
        {
            Type typeToRegister = typeof(GenericCommandHandler<>);
            MessageHandlerClass handlerClass;

            Assert.IsFalse(TryRegisterIn(_container, typeToRegister, null, out handlerClass));
            Assert.IsNull(handlerClass);
        }

        [TestMethod]
        public void TryRegisterIn_ReturnsFalse_IfTypeIsNoMessageHandler()
        {
            Type typeToRegister = typeof(string);
            MessageHandlerClass handlerClass;

            Assert.IsFalse(TryRegisterIn(_container, typeToRegister, null, out handlerClass));
            Assert.IsNull(handlerClass);
        }

        [TestMethod]
        public void TryRegisterIn_ReturnsFalse_IfTypeDoesNotSatisfyPredicate()
        {
            Type typeToRegister = typeof(MessageHandlerWithPerResolveLifetime);
            MessageHandlerClass handlerClass;

            Assert.IsFalse(TryRegisterIn(_container, typeToRegister, type => false, out handlerClass));
            Assert.IsNull(handlerClass);
        }

        [TestMethod]
        public void TryRegisterIn_ReturnsTrue_IfTypeIsMessageHandlerThatSatisfiesPredicateAndLifetimeIsPerResolve()
        {
            Type typeToRegister = typeof(MessageHandlerWithPerResolveLifetime);
            MessageHandlerClass handlerClass;

            Assert.IsTrue(TryRegisterIn(_container, typeToRegister, null, out handlerClass));
            Assert.IsNotNull(handlerClass);
            Assert.IsTrue(_container.HasRegistered(typeToRegister, InstanceLifetime.PerResolve));
        }

        [TestMethod]
        public void TryRegisterIn_ReturnsTrue_IfTypeIsMessageHandlerThatSatisfiesPredicateAndLifetimeIsPerUnitOfWork()
        {
            Type typeToRegister = typeof(MessageHandlerWithPerUnitOfWorkLifetime);
            MessageHandlerClass handlerClass;

            Assert.IsTrue(TryRegisterIn(_container, typeToRegister, null, out handlerClass));
            Assert.IsNotNull(handlerClass);
            Assert.IsTrue(_container.HasRegistered(typeToRegister, InstanceLifetime.PerUnitOfWork));
        }

        [TestMethod]
        public void TryRegisterIn_ReturnsTrue_IfTypeIsMessageHandlerThatSatisfiesPredicateAndLifetimeIsSingle()
        {
            Type typeToRegister = typeof(MessageHandlerWithSingleLifetime);
            MessageHandlerClass handlerClass;

            Assert.IsTrue(TryRegisterIn(_container, typeToRegister, null, out handlerClass));
            Assert.IsNotNull(handlerClass);
            Assert.IsTrue(_container.HasRegistered(typeToRegister, InstanceLifetime.Singleton));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TryRegisterIn_Throws_IfSpecifiedLifetimeOnMessageHandlerIsInvalid()
        {
            Type typeToRegister = typeof(MessageHandlerWithInvalidLifetimeAttribute);
            MessageHandlerClass handlerClass;

            TryRegisterIn(_container, typeToRegister, null, out handlerClass);
        }

        private static readonly Lazy<MethodInfo> _TryRegisterInMethod = new Lazy<MethodInfo>(ResolveMethod, true);

        private static MethodInfo ResolveMethod()
        {
            return typeof(MessageHandlerClass).GetMethod("TryRegisterIn", BindingFlags.NonPublic | BindingFlags.Static);
        }

        private static bool TryRegisterIn(MessageHandlerFactory factory, Type type, Func<Type, bool> typeSelector, out MessageHandlerClass handlerClass)
        {
            var arguments = new object[]
            {
                factory,
                type,
                typeSelector,
                null,
                null
            };
            try
            {
                var hasBeenRegistered = (bool) _TryRegisterInMethod.Value.Invoke(null, arguments);
                handlerClass = (MessageHandlerClass) arguments[4];
                return hasBeenRegistered;
            }
            catch (TargetInvocationException exception)
            {
                throw exception.InnerException;
            }            
        }
    }
}
