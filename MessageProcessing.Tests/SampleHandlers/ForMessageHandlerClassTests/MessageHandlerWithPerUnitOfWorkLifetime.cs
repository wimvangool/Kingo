
namespace YellowFlare.MessageProcessing.SampleHandlers.ForMessageHandlerClassTests
{
    [InstanceLifetime(InstanceLifetime.PerUnitOfWork)]
    internal sealed class MessageHandlerWithPerUnitOfWorkLifetime : IMessageHandler<Command>
    {
        public void Handle(Command message) {}
    }
}
