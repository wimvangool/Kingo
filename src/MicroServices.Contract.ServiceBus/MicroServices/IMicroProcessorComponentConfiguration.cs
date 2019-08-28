using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents the configuration for a component.
    /// </summary>
    public interface IMicroProcessorComponentConfiguration
    {
        /// <summary>
        /// Indicates the lifetime of the component.
        /// </summary>
        ServiceLifetime Lifetime
        {
            get;
        }

        /// <summary>
        /// Returns all service types for which this component is registered.
        /// </summary>
        IEnumerable<Type> ServiceTypes
        {
            get;
        }
    }
}
