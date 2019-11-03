using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    internal class BasicEndpointHandler<TMessage> : IEndpointMessageHandler<TMessage>
    {
        public Task HandleAsync(TMessage message) =>
            Task.CompletedTask;
    }
}
