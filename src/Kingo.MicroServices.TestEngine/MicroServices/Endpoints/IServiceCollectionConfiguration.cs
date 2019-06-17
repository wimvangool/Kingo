using System;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.Endpoints
{
    /// <summary>
    /// When implemented by a class, represents a configuration that can configure <see cref="IServiceCollection"/>
    /// instances.
    /// </summary>
    public interface IServiceCollectionConfiguration
    {
        /// <summary>
        /// Configures a <see cref="IServiceCollection"/> by invoking the specified <paramref name="serviceConfigurator"/>.
        /// </summary>
        /// <param name="serviceConfigurator">
        /// Delegate that will configure the supplied <see cref="IServiceCollection" />.
        /// </param>
        void Configure(Action<IServiceCollection> serviceConfigurator);
    }
}
