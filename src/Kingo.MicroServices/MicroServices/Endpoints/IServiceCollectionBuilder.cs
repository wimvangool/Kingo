using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.Endpoints
{
    internal interface IServiceCollectionBuilder
    {
        IServiceCollection BuildServiceCollection(IServiceCollection services = null);
    }
}
