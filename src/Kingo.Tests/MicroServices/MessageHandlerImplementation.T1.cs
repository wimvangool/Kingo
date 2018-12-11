using System;
using System.Threading.Tasks;
using Kingo.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    public sealed class MessageHandlerImplementation : IMessageHandlerImplementation
    {
        public readonly Type MessageHandlerType;        
        private Action<object, MessageHandlerContext> _implementation;

        public MessageHandlerImplementation(Type messageHandlerType)
        {
            MessageHandlerType = messageHandlerType;           
        }

        public void AsAsync<TMessage>(Func<TMessage, MessageHandlerContext, Task> implementation) where TMessage : class =>
            As<TMessage>((message, context) => implementation.Invoke(message, context).Await());

        public void As<TMessage>(Action<TMessage, MessageHandlerContext> implementation) where TMessage : class
        {
            _implementation = (message, context) =>
            {
                var convertedMessage = message as TMessage;
                if (convertedMessage == null)
                {
                    throw new AssertFailedException($"Expected message of type '{typeof(TMessage).FriendlyName()}' but was '{message.GetType().FriendlyName()}'.");
                }
                implementation.Invoke(convertedMessage, context);
            };
        }            

        public Task HandleAsync(object message, MessageHandlerContext context, Type messageHandlerType)
        {
            Assert.AreSame(MessageHandlerType, messageHandlerType, $"Expected invocation of handler '{MessageHandlerType.FriendlyName()}' but was '{messageHandlerType.FriendlyName()}'.");            

            return AsyncMethod.Run(() => _implementation?.Invoke(message, context), context.Token);
        }
    }
}
