using System;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.Endpoints
{
    /// <summary>
    /// When implemented by a class, represents a 
    /// </summary>
    public interface IServiceCollectionConfiguration
    {
        void Configure(Action<IServiceCollection> serviceConfigurator);
    }
}
