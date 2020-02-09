using System.Collections.Generic;
using System.Linq;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class GivenOperationQueuePair : GivenOperationQueue
    {
        private readonly GivenOperationQueue _left;
        private readonly GivenOperationQueue _right;

        public GivenOperationQueuePair(GivenOperationQueue left, GivenOperationQueue right)
        {
            _left = left;
            _right = right;
        }

        public override int Count =>
            _left.Count + _right.Count;

        public override IEnumerator<GivenOperation> GetEnumerator() =>
            _left.Concat(_right).GetEnumerator();
    }
}
