using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.Configuration
{
    /// <summary>
    /// When implemented by a class, represents a collection of <see cref="MicroProcessorComponent">components</see>
    /// that should be registered as a dependency in a <see cref="IServiceCollection" />.
    /// </summary>
    public interface IMicroProcessorComponentCollection : IEnumerable<MicroProcessorComponent>
    {
        /// <summary>
        /// Adds types and mappings to the specified <paramref name="services"/> that are specific to this collection.
        /// </summary>
        /// <param name="services">A service collection.</param>
        /// <returns>The resulting collection.</returns>
        IServiceCollection AddSpecificComponentsTo(IServiceCollection services);
    }
}
