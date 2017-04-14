using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    internal sealed class MessageHandlerImplementationSequence : IMessageHandlerImplementation
    {
        private readonly Queue<MessageHandlerImplementation> _implementations;

        public MessageHandlerImplementationSequence()
        {
            _implementations = new Queue<MessageHandlerImplementation>();
        }

        public MessageHandlerImplementation Implement(Type messageHandlerType, int count)
        {
            var implementation = new MessageHandlerImplementation(messageHandlerType);

            for (int index = 0; index < count; index++)
            {
                _implementations.Enqueue(implementation);
            }            
            return implementation;
        }

        public Task HandleAsync(object message, IMicroProcessorContext context, Type messageHandlerType) =>
            ExpectedImplementationFor(messageHandlerType).HandleAsync(message, context, messageHandlerType);

        private IMessageHandlerImplementation ExpectedImplementationFor(Type messageHandlerType)
        {
            try
            {
                return _implementations.Dequeue();
            }
            catch (InvalidOperationException)
            {
                throw new AssertFailedException($"Invocation of message handler '{messageHandlerType.FriendlyName()}' was not expected at this point.");
            }            
        }

        public void AssertNoMoreInvocationsExpected()
        {
            if (_implementations.Count > 0)
            {                
                Assert.Fail($"Expected another invocation of type '{_implementations.Dequeue().MessageHandlerType.FriendlyName()}'.");
            }
        }
    }
}
