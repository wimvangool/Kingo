using System.Collections.Generic;

namespace Kingo.MicroServices
{
    internal sealed class MicroProcessorTestOperationQueueItem : MicroProcessorTestOperationQueue
    {
        private readonly MicroProcessorTestOperation _operation;

        public MicroProcessorTestOperationQueueItem(MicroProcessorTestOperation operation)
        {
            _operation = operation;
        }

        public override int Count =>
            1;

        public override IEnumerator<MicroProcessorTestOperation> GetEnumerator()
        {
            yield return _operation;
        }
    }
}
