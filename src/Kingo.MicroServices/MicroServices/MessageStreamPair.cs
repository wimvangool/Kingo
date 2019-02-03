using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class MessageStreamPair : MessageStream
    {
        private readonly MessageStream _left;
        private readonly MessageStream _right;

        public MessageStreamPair(MessageStream left, MessageStream right)
        {
            _left = left;
            _right = right;
        }

        #region [======= IReadOnlyList<object> ======]

        public override int Count =>
            _left.Count + _right.Count;

        public override IEnumerator<object> GetEnumerator() =>
            Enumerable.Concat(_left, _right).GetEnumerator();

        #endregion

        #region [====== HandleWithAsync ======]

        public override async Task<MessageStream> HandleWithAsync(IMessageHandler handler) =>
            (await _left.HandleWithAsync(handler).ConfigureAwait(false)).Concat(await _right.HandleWithAsync(handler).ConfigureAwait(false));

        #endregion
    }
}
