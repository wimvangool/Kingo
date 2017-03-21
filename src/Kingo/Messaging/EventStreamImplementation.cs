using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal sealed class EventStreamImplementation : EventStream
    {
        private IMessageStream _stream;

        public EventStreamImplementation()
        {
            _stream = MessageStream.Empty;
        }

        public override int Count =>
            _stream.Count;

        public override IEnumerator<object> GetEnumerator() =>
            _stream.GetEnumerator();

        public override IMessageStream AppendStream(IMessageStream stream) =>
            _stream.AppendStream(stream);

        public override Task HandleMessagesWithAsync(IMessageHandler handler) =>
            _stream.HandleMessagesWithAsync(handler);

        public override void Publish<TEvent>(TEvent message) =>
            _stream = _stream.Append(message);

        public override string ToString() =>
            _stream.ToString();
    }
}
