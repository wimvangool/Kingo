using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal abstract class NullEventStream : EventBus
    {
        private readonly IMessageStream _emptyStream;

        protected NullEventStream()
        {
            _emptyStream = MessageStream.Empty;
        }

        public override int Count =>
            _emptyStream.Count;

        public override IEnumerator<object> GetEnumerator() =>
            _emptyStream.GetEnumerator();

        public override IMessageStream AppendStream(IMessageStream stream) =>
            _emptyStream.AppendStream(stream);

        public override Task HandleMessagesWithAsync(IMessageHandler handler) =>
            _emptyStream.HandleMessagesWithAsync(handler);
    }
}
