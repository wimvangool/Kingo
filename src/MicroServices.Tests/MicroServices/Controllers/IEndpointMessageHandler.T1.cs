using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    internal interface IEndpointMessageHandler<in TMessage>
    {
        Task HandleAsync(TMessage message);
    }
}
