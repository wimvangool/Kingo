using System.Collections.Generic;
using System.Linq;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class MicroProcessorTestOperationQueuePair : MicroProcessorTestOperationQueue
    {
        private readonly MicroProcessorTestOperationQueue _left;
        private readonly MicroProcessorTestOperationQueue _right;

        public MicroProcessorTestOperationQueuePair(MicroProcessorTestOperationQueue left, MicroProcessorTestOperationQueue right)
        {
            _left = left;
            _right = right;
        }

        public override int Count =>
            _left.Count + _right.Count;

        public override IEnumerator<MicroProcessorTestOperation> GetEnumerator() =>
            _left.Concat(_right).GetEnumerator();
    }
}
