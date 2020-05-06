using System.Collections.Generic;

namespace Kingo.MicroServices
{
    internal sealed class MessageBusResult : MessageHandlerOperationResult
    {
        public MessageBusResult(IReadOnlyList<IMessage> output)
        {
            Output = output;
        }

        public override IReadOnlyList<IMessage> Output
        {
            get;
        }

        public override int MessageHandlerCount =>
            1;
    }
}
