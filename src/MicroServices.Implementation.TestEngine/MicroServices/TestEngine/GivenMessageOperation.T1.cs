using System;

namespace Kingo.MicroServices.TestEngine
{
    internal abstract class GivenMessageOperation<TMessage> : GivenOperation
    {
        private readonly Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> _configurator;

        protected GivenMessageOperation(GivenMessageOperation<TMessage> operation)
        {
            _configurator = operation._configurator;
        }

        protected GivenMessageOperation(Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator)
        {
            _configurator = configurator ?? throw new ArgumentNullException(nameof(configurator));
        }

        protected MessageHandlerTestOperationInfo<TMessage> CreateOperationInfo(MicroProcessorTestContext context) =>
            context.CreateOperationInfo(_configurator);
    }
}
