using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class HandleAsyncMethodTest
    {
        #region [====== MessageHandlers ======]

        private sealed class MessageHandler1 : IMessageHandler<object>
        {
            public Task HandleAsync(object message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        private sealed class MessageHandler2 : IMessageHandler<object>, IMessageHandler<string>
        {
            public Task HandleAsync(object message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;

            public Task HandleAsync(string message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        #endregion

        [TestMethod]
        public void Method_ReturnsExpectedMethod_IfMessageHandlerImplementsOneInterface_And_MessageTypeMatchesExactly()
        {
            var messageHandler = new MessageHandler1();
            var method = new HandleAsyncMethod<object>(messageHandler);

            Assert.AreSame(typeof(MessageHandler1), method.MessageHandler.Type);
            AssertMessageParameterTypeIs(typeof(object), method);
        }

        [TestMethod]
        public void Method_ReturnsExpectedMethod_IfMessageHandlerImplementsOneInterface_And_MessageTypeDoesNotMatchExactly()
        {
            var messageHandler = new MessageHandler1();
            var method = new HandleAsyncMethod<string>(messageHandler);

            Assert.AreSame(typeof(MessageHandler1), method.MessageHandler.Type);
            AssertMessageParameterTypeIs(typeof(object), method);
        }

        [TestMethod]
        public void Method_ReturnsExpectedMethod_IfMessageHandlerImplementsTwoInterfaces_And_MessageTypeMatchesExactly()
        {
            var messageHandler = new MessageHandler2();
            var methodOfObject = new HandleAsyncMethod<object>(messageHandler);
            var methodOfString = new HandleAsyncMethod<string>(messageHandler);

            Assert.AreSame(typeof(MessageHandler2), methodOfObject.MessageHandler.Type);
            AssertMessageParameterTypeIs(typeof(object), methodOfObject);

            Assert.AreSame(typeof(MessageHandler2), methodOfString.MessageHandler.Type);
            AssertMessageParameterTypeIs(typeof(string), methodOfString);
        }

        [TestMethod]
        public void Method_ReturnsExpectedMethod_IfMessageHandlerImplementsTwoInterfaces_And_MessageTypeDoesNotMatchExactly()
        {
            var messageHandler = new MessageHandler2();
            var method = new HandleAsyncMethod<IDisposable>(messageHandler);

            Assert.AreSame(typeof(MessageHandler2), method.MessageHandler.Type);
            AssertMessageParameterTypeIs(typeof(object), method);
        }

        private static void AssertMessageParameterTypeIs(Type parameterType, HandleAsyncMethod method) =>
            Assert.AreSame(parameterType, method.Info.GetParameters()[0].ParameterType);
    }
}
