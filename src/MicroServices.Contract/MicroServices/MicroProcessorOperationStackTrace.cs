using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.Collections.Generic;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a stack-trace of a processor at the time an exception was thrown.
    /// </summary>
    [Serializable]
    public sealed class MicroProcessorOperationStackTrace : ReadOnlyList<MicroProcessorOperationStackItem>
    {
        /// <summary>
        /// An empty stack-trace.
        /// </summary>
        public static readonly MicroProcessorOperationStackTrace Empty = new MicroProcessorOperationStackTrace(Enumerable.Empty<MicroProcessorOperationStackItem>());

        private readonly MicroProcessorOperationStackItem[] _items;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroProcessorOperationStackTrace" /> class.
        /// </summary>
        /// <param name="items">The items of the stack-trace.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items"/> is <c>null</c>.
        /// </exception>
        public MicroProcessorOperationStackTrace(IEnumerable<MicroProcessorOperationStackItem> items)
        {
            _items = items?.ToArray() ?? throw new ArgumentNullException(nameof(items));
        }

        #region [====== ReadOnlyList<MessageStackTraceItem> ======]

        /// <inheritdoc />
        public override int Count =>
            _items.Length;

        /// <inheritdoc />
        public override IEnumerator<MicroProcessorOperationStackItem> GetEnumerator() =>
            _items.AsEnumerable().GetEnumerator();

        /// <inheritdoc />
        public override string ToString() =>
            string.Join(" -> ", this);

        #endregion

        #region [====== Stack Trace Operations ======]

        /// <summary>
        /// Returns the root item/operation of the stack-trace, or <c>null</c> if the stack is empty.
        /// </summary>
        public MicroProcessorOperationStackItem RootOperation =>
            _items.Length == 0 ? null : _items[0];

        /// <summary>
        /// Returns the current item/operation of the stack-trace, or <c>null</c> if the stack is empty.
        /// </summary>
        public MicroProcessorOperationStackItem CurrentOperation =>
            _items.Length == 0 ? null : _items[_items.Length - 1];

        #endregion
    }
}
