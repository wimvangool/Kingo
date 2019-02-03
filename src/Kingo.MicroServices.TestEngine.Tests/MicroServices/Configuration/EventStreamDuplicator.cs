namespace Kingo.MicroServices.Configuration
{
    internal sealed class EventStreamDuplicator : IEventStreamProcessor
    {
        public MessageStream Process(MessageStream stream) =>
            stream + stream;
    }
}
