using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    internal sealed class MessageHandlerSpy : IMessageHandler
    {
        private readonly List<object> _messages;

        public MessageHandlerSpy()
        {
            _messages = new List<object>();
        }

        public void Handle<TMessage>(TMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            _messages.Add(message);
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

        public void AssertAreSame(IMessageStream message, int index)
        {
            Assert.AreSame(message, _messages[index]);
        }
    }
}
