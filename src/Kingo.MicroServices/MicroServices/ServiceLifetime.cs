
using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// A value that is used to specify the lifetime of a certain instance that is resolved at runtime.
    /// </summary>
    public enum ServiceLifetime
    {
        /// <summary>
        /// Specifies that a new instance of a type should be created each time it is resolved.
        /// </summary>        
        Transient,

        /// <summary>
        /// Specifies that the lifetime is scoped. What this scope is depends on the context or
        /// container used to register and resolve the service, but it typically spans a single
        /// request or invocation to a <see cref="MicroProcessor" />.
        /// </summary>        
        Scoped,        

        /// <summary>
        /// Specifies that only one instance of the service should ever be created (inside an <see cref="AppDomain"/>).
        /// </summary>        
        Singleton
    }
}
