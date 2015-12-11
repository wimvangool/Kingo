using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a scenario that executes inside one single <see cref="UnitOfWorkScope" />, which
    /// makes it suitable for testing write-only scenario's where in-memory repositories can be
    /// used to temporarily store data.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that is processed on the When-phase.</typeparam>
    public abstract class WriteOnlyScenario<TMessage> : Scenario<TMessage> where TMessage : class, IMessage<TMessage>
    {
        internal override async Task ExecuteCoreAsync()
        {
            using (var scope = MessageProcessor.CreateUnitOfWorkScope())
            {
                await base.ExecuteCoreAsync();
                await scope.CompleteAsync();
            }
        }
    }
}
