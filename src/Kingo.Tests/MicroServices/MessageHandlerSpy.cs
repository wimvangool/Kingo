using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    internal sealed class MessageHandlerSpy : IMessageHandler
    {
        private readonly List<object> _messages;
        private readonly List<Type> _messageTypes;

        public MessageHandlerSpy()
        {
            _messages = new List<object>();       
            _messageTypes = new List<Type>();
        }

        public Task<MessageStream> HandleAsync<TMessage>(TMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            return AsyncMethod.Run(() =>
            {
                _messages.Add(message); 
                _messageTypes.Add(typeof(TMessage));                

                return MessageStream.Empty;
            });            
        }

        public void AssertMessageCountIs(int count)
        {
            Assert.AreEqual(count, _messages.Count);
        }

        public void AssertVisitedAll(MessageStream stream)
        {
            for (int index = 0; index < stream.Count; index++)
            {
                AssertAreEqual(stream[index], index);
            }
        }

        public void AssertAreEqual(object message, int index)
        {
            Assert.AreEqual(message, _messages[index]);
        }

        public void VerifyGenericTypeInvocations()
        {
            for (var index = 0; index < _messages.Count; index++)
            {
                Assert.AreEqual(_messages[index].GetType(), _messageTypes[index]);
            }
        }
    }
}
