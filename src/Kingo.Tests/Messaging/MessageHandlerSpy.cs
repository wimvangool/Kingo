using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    internal sealed class MessageHandlerSpy : IMessageHandler
    {
        private readonly List<object> _messages;
        private readonly List<object> _handlers; 

        public MessageHandlerSpy()
        {
            _messages = new List<object>();
            _handlers = new List<object>();
        }

        public Task HandleAsync<TMessage>(TMessage message, IMessageHandler<TMessage> handler)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            return AsyncMethod.RunSynchronously(() =>
            {
                _messages.Add(message);
                _handlers.Add(handler);                
            });            
        }

        public void AssertMessageCountIs(int count)
        {
            Assert.AreEqual(count, _messages.Count);
        }

        public void AssertVisitedAll(IMessageStream stream)
        {
            for (int index = 0; index < stream.Count; index++)
            {
                AssertAreSame(stream[index], index);
            }
        }

        public void AssertAreSame(object message, int index)
        {
            Assert.AreSame(message, _messages[index]);
        }
    }
}
