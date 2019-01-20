using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.Configuration
{
    internal interface IServiceCollectionConfigurator
    {
        void Configure(IServiceCollection services);
    }
}
