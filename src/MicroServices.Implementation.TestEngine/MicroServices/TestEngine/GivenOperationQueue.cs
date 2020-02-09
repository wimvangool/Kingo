using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.MicroServices.TestEngine
{
    internal abstract class GivenOperationQueue : IReadOnlyCollection<GivenOperation>
    {
        #region [====== Empty ======]

        private sealed class EmptyQueue : GivenOperationQueue
        {
            public override int Count =>
                0;

            public override IEnumerator<GivenOperation> GetEnumerator() =>
                Enumerable.Empty<GivenOperation>().GetEnumerator();

            public override GivenOperationQueue Enqueue(GivenOperation operation) =>
                new GivenOperationQueueItem(operation);
        }

        public static readonly GivenOperationQueue Empty = new EmptyQueue();

        #endregion

        #region [====== IReadOnlyCollection ======]

        public abstract int Count
        {
            get;
        }

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public abstract IEnumerator<GivenOperation> GetEnumerator();

        #endregion

        #region [====== Enqueue ======]

        public override string ToString() =>
            $"{Count} operation(s) scheduled";

        public virtual GivenOperationQueue Enqueue(GivenOperation operation) =>
            new GivenOperationQueuePair(this, new GivenOperationQueueItem(operation));

        #endregion
    }
}
