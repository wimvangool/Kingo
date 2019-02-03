using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Configuration
{
    internal sealed class CustomProcessor : MicroProcessor
    {
        private readonly IEventStreamProcessor _eventStreamProcessor;

        public CustomProcessor(IMicroServiceBus serviceBus, IEventStreamProcessor eventStreamProcessor) :
            base(serviceBus)
        {
            _eventStreamProcessor = eventStreamProcessor ?? throw new ArgumentNullException(nameof(eventStreamProcessor));
        }

        protected override Task PublishAsync(MessageStream events) =>
            base.PublishAsync(_eventStreamProcessor.Process(events));
    }
}
