using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    public sealed class FixedMessageIdGenerator : IMessageIdGenerator
    {
        private readonly string _messageId;

        public FixedMessageIdGenerator(string messageId)
        {
            _messageId = messageId;
        }

        public string GenerateMessageId(object content) =>
            _messageId;
    }
}
