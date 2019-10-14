using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// When implemented by a class, represents a collection of <see cref="MicroProcessorComponent"/> instances
    /// that must be added as dependencies.
    /// </summary>
    public interface IMicroProcessorComponentCollection : IReadOnlyCollection<MicroProcessorComponent>
    {
        /// <summary>
        /// Adds components to the specified <paramref name="services"/> that are specific to this collection
        /// of components and must be added in addition to the components of this collection.
        /// </summary>
        /// <param name="services">A service-collection.</param>
        /// <returns>A service-collection that contains the newly added components.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="services"/> is <c>null</c>.
        /// </exception>
        IServiceCollection AddSpecificComponentsTo(IServiceCollection services);
    }
}
