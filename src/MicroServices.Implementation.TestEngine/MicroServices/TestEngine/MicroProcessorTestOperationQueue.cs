using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.MicroServices.TestEngine
{
    internal abstract class MicroProcessorTestOperationQueue : IReadOnlyCollection<MicroProcessorTestOperation>
    {
        #region [====== Empty ======]

        private sealed class EmptyQueue : MicroProcessorTestOperationQueue
        {
            public override int Count =>
                0;

            public override IEnumerator<MicroProcessorTestOperation> GetEnumerator() =>
                Enumerable.Empty<MicroProcessorTestOperation>().GetEnumerator();

            public override MicroProcessorTestOperationQueue Enqueue(MicroProcessorTestOperation operation) =>
                new MicroProcessorTestOperationQueueItem(operation);
        }

        public static readonly MicroProcessorTestOperationQueue Empty = new EmptyQueue();

        #endregion

        #region [====== IReadOnlyCollection ======]

        public abstract int Count
        {
            get;
        }

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public abstract IEnumerator<MicroProcessorTestOperation> GetEnumerator();

        #endregion

        #region [====== Enqueue ======]

        public override string ToString() =>
            $"{Count} operation(s) scheduled";

        public virtual MicroProcessorTestOperationQueue Enqueue(MicroProcessorTestOperation operation) =>
            new MicroProcessorTestOperationQueuePair(this, new MicroProcessorTestOperationQueueItem(operation));

        #endregion
    }
}
