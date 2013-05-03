
namespace YellowFlare.MessageProcessing
{    
    internal sealed class TheCommandHandler : IExternalMessageHandler<TheCommand>
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
                    DomainEventBus.Publish(message);
                }
            }
        }
    }
}
