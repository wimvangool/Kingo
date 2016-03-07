
using System.Threading.Tasks;

namespace Kingo.Messaging
{    
    internal interface IEventToPublish
    {        
        Task PublishAsync(IMessageProcessorBus eventBus);
    }
}
