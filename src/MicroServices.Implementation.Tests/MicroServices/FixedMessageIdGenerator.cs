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
