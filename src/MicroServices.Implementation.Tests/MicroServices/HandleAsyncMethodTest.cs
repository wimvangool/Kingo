using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class HandleAsyncMethodTest : AsyncMethodTest
    {
        #region [====== MessageHandler Types ======]
        
        [Value(10)]
        private sealed class MessageHandler1 : IMessageHandler<object>
        {
            [Value(2)]
            public Task HandleAsync([Value(0)] object message, [Value(1)] IMessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        [Value(20)]
        private sealed class MessageHandler2 : IMessageHandler<object>, IMessageHandler<string>
        {
            [Value(10)]
            Task IMessageHandler<string>.HandleAsync([Value(2)] string message, [Value(3)] IMessageHandlerOperationContext context) =>
                HandleAsync(message, context);

            [Value(18)]
            public Task HandleAsync([Value(4)] object message, [Value(5)] IMessageHandlerOperationContext context) =>
                Task.CompletedTask;            
        }

        #endregion                

        [TestMethod]
        public void Method_ReturnsExpectedMethod_IfMessageHandlerImplementsOneInterface_And_MessageTypeMatchesExactly()
        {
            var messageHandler = new MessageHandler1();
            var method = new HandleAsyncMethod<object>(messageHandler);
            
            AssertComponentProperties<MessageHandler1>(method, 10);
            AssertMethodProperties<object>(method, 0, 1);
        }

        [TestMethod]
        public void Method_ReturnsExpectedMethod_IfMessageHandlerImplementsOneInterface_And_MessageTypeDoesNotMatchExactly()
        {
            var messageHandler = new MessageHandler1();
            var method = new HandleAsyncMethod<string>(messageHandler);

            AssertComponentProperties<MessageHandler1>(method, 10);
            AssertMethodProperties<object>(method, 0, 1);
        }

        [TestMethod]
        public void Method_ReturnsExpectedMethod_IfMessageHandlerImplementsTwoInterfaces_And_MessageTypeMatchesExactly()
        {
            var messageHandler = new MessageHandler2();
            var methodOfString = new HandleAsyncMethod<string>(messageHandler);
            var methodOfObject = new HandleAsyncMethod<object>(messageHandler);            

            AssertComponentProperties<MessageHandler2>(methodOfString, 20);
            AssertMethodProperties<string>(methodOfString, 2, 3);

            AssertComponentProperties<MessageHandler2>(methodOfObject, 20);
            AssertMethodProperties<object>(methodOfObject, 4, 5);
        }

        [TestMethod]
        public void Method_ReturnsExpectedMethod_IfMessageHandlerImplementsTwoInterfaces_And_MessageTypeDoesNotMatchExactly()
        {
            var messageHandler = new MessageHandler2();
            var method = new HandleAsyncMethod<IDisposable>(messageHandler);

            AssertComponentProperties<MessageHandler2>(method, 20);
            AssertMethodProperties<object>(method, 4, 5);
        }        

        private static void AssertMethodProperties<TParameter>(IAsyncMethod method, int messageValue, int contextValue)
        {
            Assert.AreSame(typeof(TParameter), method.MessageParameterInfo.ParameterType);
            AssertValue(method.MessageParameterInfo, messageValue);
            AssertValue(method.ContextParameterInfo, contextValue);
            AssertValue(method.MethodInfo, (messageValue + contextValue) * 2);
        }        
    }
}
