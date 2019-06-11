using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// This attribute can be declared on any class or struct to configure its registration and run-time behavior.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class MicroProcessorComponentAttribute : Attribute, IMicroProcessorComponentConfiguration
    {       
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroProcessorComponentAttribute" /> class.
        /// </summary>
        public MicroProcessorComponentAttribute(ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            Lifetime = lifetime;
        }

        /// <inheritdoc />
        public ServiceLifetime Lifetime
        {
            get;            
        }
    }
}
