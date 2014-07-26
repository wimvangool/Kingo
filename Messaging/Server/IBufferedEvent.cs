
namespace System.ComponentModel.Messaging.Server
{
    internal interface IBufferedEvent
    {
        void Publish(IDomainEventBus domainEventBus);
    }
}
