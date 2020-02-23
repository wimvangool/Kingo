using System;

namespace Kingo.MicroServices.TestEngine
{
    internal abstract class MessageHandlerTestOperation<TMessage> : MicroProcessorTestOperation
    {
        private readonly Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> _configurator;

        protected MessageHandlerTestOperation(MessageHandlerTestOperation<TMessage> operation)
        {
            _configurator = operation._configurator;
        }

        protected MessageHandlerTestOperation(Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator)
        {
            _configurator = configurator ?? throw new ArgumentNullException(nameof(configurator));
        }

        public abstract Type MessageHandlerType
        {
            get;
        }

        protected MessageHandlerTestOperationInfo<TMessage> CreateOperationInfo(MicroProcessorTestContext context) =>
            context.CreateOperationInfo(_configurator);
    }
}
