using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.Controllers
{
    internal interface IServiceCollectionBuilder
    {
        IServiceCollection BuildServiceCollection(IServiceCollection services = null);
    }
}
