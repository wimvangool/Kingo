using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kingo.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    internal sealed class MessageProcessorSpy : IMessageProcessor, IReadOnlyList<object>
    {
        private readonly List<object> _messages;        

        public MessageProcessorSpy()
        {
            _messages = new List<object>();                   
        }

        #region [====== IReadOnlyList<object> ======]

        public int Count =>
            _messages.Count;

        public object this[int index] =>
            _messages[index];

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public IEnumerator<object> GetEnumerator() =>
            _messages.GetEnumerator();               

        #endregion

        public Task<MessageHandlerOperationResult> HandleAsync<TMessage>(IMessage<TMessage> message, MessageHandlerOperationContext context)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            Assert.IsInstanceOfType(message.Instance, typeof(TMessage));

            _messages.Add(message.Instance);

            return Task.FromResult<MessageHandlerOperationResult>(EventBufferResult.Empty);                      
        }             
    }
}
