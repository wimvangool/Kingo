using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    internal sealed class EventBusResult : MessageHandlerOperationResult
    {
        public EventBusResult(IReadOnlyList<object> events)
        {
            Events = events;
        }

        public override IReadOnlyList<object> Events
        {
            get;
        }

        public override int MessageHandlerCount =>
            1;

        internal override EventBufferResult ToEventBufferResult() =>
            new EventBufferResult(new EventBuffer(Events), MessageHandlerCount);
    }
}
