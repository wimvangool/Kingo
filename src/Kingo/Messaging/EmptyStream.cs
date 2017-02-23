using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.Messaging
{
    internal sealed class EmptyStream : IMessageStream
    {
        #region [====== IReadOnlyList<IMessage> ======]

        public int Count => 0;

        public IMessage this[int index]
        {
            get { throw MessageStream.NewIndexOutOfRangeException(index, Count); }
        }

        public IEnumerator<IMessage> GetEnumerator()
        {
            return Enumerable.Empty<IMessage>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region [====== IMessageStream ======]

        public IMessageStream Append(IMessageStream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            return stream;
        }       

        public void Accept(IMessageHandler handler) { }

        #endregion
    }
}
