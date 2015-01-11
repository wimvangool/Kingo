
namespace System.ComponentModel.Server
{
    [MessageHandler(InstanceLifetime.PerUnitOfWork)]
    internal sealed class TheCommandHandler : IMessageHandler<TheCommand>
    {
        public void Handle(TheCommand command)
        {
            if (command.ExceptionToThrow != null)
            {
                throw command.ExceptionToThrow;
            }
            if (command.DomainEventsToPublish != null)
            {
                foreach (var message in command.DomainEventsToPublish)
                {
                    ScenarioTestProcessor.Instance.DomainEventBus.Publish(message);
                }
            }
        }
    }
}
