using System.Collections.Generic;

namespace Kingo.MicroServices
{
    internal sealed class MessageBusResult : MessageHandlerOperationResult
    {
        public MessageBusResult(IReadOnlyList<MessageToDispatch> output)
        {
            Output = output;
        }

        public override IReadOnlyList<MessageToDispatch> Output
        {
            get;
        }

        public override int MessageHandlerCount =>
            1;
    }
}
