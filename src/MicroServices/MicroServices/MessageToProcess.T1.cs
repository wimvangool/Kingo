using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a message that is being handled or executed by a processor.
    /// </summary>
    /// <typeparam name="TMessage">Type of the content of this message.</typeparam>
    public sealed class MessageToProcess<TMessage> : MessageEnvelope<TMessage>, IMessageToProcess
    {
        internal MessageToProcess(MessageEnvelope<TMessage> message, MessageKind kind) :
            base(message)
        {
            Kind = kind.Validate();
        }

        /// <inheritdoc />
        public MessageKind Kind
        {
            get;
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{Content.GetType().FriendlyName()} ({nameof(Kind)} = {Kind})";
    }
}
