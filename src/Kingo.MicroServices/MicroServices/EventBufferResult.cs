using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    internal sealed class EventBufferResult : MessageHandlerOperationResult
    {
        public static readonly EventBufferResult Empty = new EventBufferResult(MicroServices.EventBuffer.Empty, 0);        

        public EventBufferResult(EventBuffer eventBuffer, int messageHandlerCount)
        {
            EventBuffer = eventBuffer;
            MessageHandlerCount = messageHandlerCount;
        }

        public override IReadOnlyList<object> Events =>
            EventBuffer;

        internal EventBuffer EventBuffer
        {
            get;
        }

        public override int MessageHandlerCount
        {
            get;
        }

        public EventBufferResult Append(MessageHandlerOperationResult result) =>
            Append(result.ToEventBufferResult());

        private EventBufferResult Append(EventBufferResult result)
        {
            var eventStream = EventBuffer.Append(result.EventBuffer);
            var messageHandlerCount = MessageHandlerCount + result.MessageHandlerCount;
            return new EventBufferResult(eventStream, messageHandlerCount);
        }            

        internal override EventBufferResult ToEventBufferResult() =>
            this;
    }
}
