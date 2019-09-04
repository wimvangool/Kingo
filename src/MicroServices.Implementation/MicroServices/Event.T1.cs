using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class Event<TEvent> : Event, IMessage<TEvent>
    {
        private readonly TEvent _event;

        public Event(TEvent @event) :
            base(typeof(TEvent))
        {
            if (ReferenceEquals(@event, null))
            {
                throw new ArgumentNullException(nameof(@event));
            }
            _event = @event;
        }

        public override object Instance =>
            _event;

        TEvent IMessage<TEvent>.Instance =>
            _event;

        public override Task<MessageHandlerOperationResult> HandleWith(IMessageProcessor processor, MessageHandlerOperationContext context) =>
            processor.HandleAsync(this, context);        
    }
}
