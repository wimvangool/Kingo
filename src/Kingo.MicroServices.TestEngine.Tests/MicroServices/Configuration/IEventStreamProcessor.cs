namespace Kingo.MicroServices.Configuration
{
    internal interface IEventStreamProcessor
    {
        MessageStream Process(MessageStream stream);
    }
}
