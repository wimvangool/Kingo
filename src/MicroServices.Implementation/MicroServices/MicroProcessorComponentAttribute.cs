using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// This attribute can be declared on any class or struct to configure its registration and run-time behavior.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class MicroProcessorComponentAttribute : Attribute, IMicroProcessorComponentConfiguration
    {       
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroProcessorComponentAttribute" /> class.
        /// </summary>
        public MicroProcessorComponentAttribute(ServiceLifetime lifetime = ServiceLifetime.Transient, params Type[] serviceTypes)
        {
            Lifetime = lifetime;
            ServiceTypes = serviceTypes ?? throw new ArgumentNullException(nameof(serviceTypes));
        }

        /// <inheritdoc />
        public ServiceLifetime Lifetime
        {
            get;            
        }

        /// <inheritdoc />
        public IEnumerable<Type> ServiceTypes
        {
            get;
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{nameof(Lifetime)} = {Lifetime}";
    }
}
