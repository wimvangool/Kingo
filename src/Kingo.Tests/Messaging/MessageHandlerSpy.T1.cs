using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Kingo.Threading.AsyncMethod;

namespace Kingo.Messaging
{
    internal sealed class MessageHandlerSpy<TMessage> : IMessageHandler<TMessage> where TMessage : class
    {
        private readonly List<TMessage> _messages;

        public MessageHandlerSpy()
        {
            _messages = new List<TMessage>();
        }

        public Task HandleAsync(TMessage message, IMicroProcessorContext context) => Run(() =>
            _messages.Add(message));

        public void AssertHandleCountIs(int count) =>
            Assert.AreEqual(count, _messages.Count);

        public void AssertMessageReceived(int index, TMessage message) =>
            Assert.AreSame(message, _messages[index]);
    }
}
