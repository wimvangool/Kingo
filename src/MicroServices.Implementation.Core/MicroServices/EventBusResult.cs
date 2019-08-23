using System.Collections.Generic;

namespace Kingo.MicroServices
{
    internal sealed class EventBusResult : MessageHandlerOperationResult
    {
        public EventBusResult(IReadOnlyList<IMessage> events)
        {
            Events = events;
        }

        public override IReadOnlyList<IMessage> Events
        {
            get;
        }

        public override int MessageHandlerCount =>
            1;

        internal override EventBufferResult ToEventBufferResult() =>
            new EventBufferResult(new EventBuffer(Events), MessageHandlerCount);
    }
}
