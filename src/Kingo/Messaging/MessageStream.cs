using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kingo.Resources;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a stream of <see cref="IMessage">messages</see>.
    /// </summary>
    public sealed class MessageStream : IMessageStream
    {
        /// <summary>
        /// Represents an empty stream.
        /// </summary>
        public static readonly IMessageStream Empty = new EmptyStream();

        private readonly IMessageStream _left;
        private readonly IMessageStream _right;
        private int? _count;

        internal MessageStream(IMessageStream left, IMessageStream right)
        {
            _left = left;
            _right = right;
        }

        #region [====== IReadOnlyList  ======]

        /// <inheritdoc />
        public int Count
        {
            get
            {
                if (!_count.HasValue)
                {
                    _count = _left.Count + _right.Count;
                }
                return _count.Value;
            }
        }

        /// <inheritdoc />
        public IMessage this[int index]
        {
            get
            {
                IMessage message;

                if (this.TryGetItem(index, out message))
                {
                    return message;
                }
                throw NewIndexOutOfRangeException(index, Count);
            }
        }

        internal static IndexOutOfRangeException NewIndexOutOfRangeException(int index, int count)
        {
            var messageFormat = ExceptionMessages.MessageStream_IndexOutOfRange;
            var message = string.Format(messageFormat, index, count);
            return new IndexOutOfRangeException(message);
        }

        /// <inheritdoc />
        public IEnumerator<IMessage> GetEnumerator() => _left.Concat(_right).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region [====== IMessageStream ======]

        /// <inheritdoc />
        public IMessageStream Append(IMessageStream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            if (stream.Count == 0)
            {
                return this;
            }
            return new MessageStream(this, stream);
        }        

        /// <inheritdoc />
        public void Accept(IMessageHandler handler)
        {
            _left.Accept(handler);
            _right.Accept(handler);
        }

        #endregion
    }
}
