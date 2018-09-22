using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Kingo.Threading.AsyncMethod;

namespace Kingo.Messaging
{
    public sealed class MessageHandlerImplementation : IMessageHandlerImplementation
    {
        public readonly Type MessageHandlerType;        
        private Action<object, IMicroProcessorContext> _implementation;

        public MessageHandlerImplementation(Type messageHandlerType)
        {
            MessageHandlerType = messageHandlerType;           
        }

        public void AsAsync<TMessage>(Func<TMessage, IMicroProcessorContext, Task> implementation) where TMessage : class =>
            As<TMessage>((message, context) => implementation.Invoke(message, context).Await());

        public void As<TMessage>(Action<TMessage, IMicroProcessorContext> implementation) where TMessage : class
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

        public Task HandleAsync(object message, IMicroProcessorContext context, Type messageHandlerType)
        {
            Assert.AreSame(MessageHandlerType, messageHandlerType, $"Expected invocation of handler '{MessageHandlerType.FriendlyName()}' but was '{messageHandlerType.FriendlyName()}'.");            

            return Run(() => _implementation?.Invoke(message, context), context.Token);
        }
    }
}
