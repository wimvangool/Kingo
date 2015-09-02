using System.Threading.Tasks;

namespace Kingo.BuildingBlocks.Messaging
{
    [MessageHandler(InstanceLifetime.PerUnitOfWork)]
    internal sealed class TheCommandHandler : MessageHandler<TheCommand>
    {
        public override Task HandleAsync(TheCommand command)
        {
            return AsyncMethod.RunSynchronously(() =>
            {
                if (command.ExceptionToThrow != null)
                {
                    throw command.ExceptionToThrow;
                }
                if (command.DomainEventsToPublish != null)
                {
                    foreach (var message in command.DomainEventsToPublish)
                    {
                        Publish(message);
                    }
                }
            });                                 
        }
    }
}
