﻿using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents the configuration for a <see cref="MicroProcessorComponent" />.
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
    }
}