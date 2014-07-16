
namespace YellowFlare.MessageProcessing
{
    internal interface IBufferedEvent
    {
        void Publish(IDomainEventBus domainEventBus);
    }
}
