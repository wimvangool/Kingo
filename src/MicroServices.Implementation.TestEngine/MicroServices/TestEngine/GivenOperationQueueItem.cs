using System.Collections.Generic;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class GivenOperationQueueItem : GivenOperationQueue
    {
        private readonly GivenOperation _operation;

        public GivenOperationQueueItem(GivenOperation operation)
        {
            _operation = operation;
        }

        public override int Count =>
            1;

        public override IEnumerator<GivenOperation> GetEnumerator()
        {
            yield return _operation;
        }
    }
}
