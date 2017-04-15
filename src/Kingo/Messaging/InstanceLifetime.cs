﻿
namespace Kingo.Messaging
{
    /// <summary>
    /// A value that is used to specify the lifetime of a certain instance that is resolved at runtime.
    /// </summary>
    public enum InstanceLifetime
    {
        /// <summary>
        /// Specifies that a new instance of a type should be created each time it is resolved.
        /// </summary>        
        PerResolve,

        /// <summary>
        /// Specifies that the lifetime of the instance is bound to a single <see cref="MicroProcessorContext" />.       
        /// </summary>        
        PerUnitOfWork,        

        /// <summary>
        /// Specifies that the lifetime of the instance is bound to a single <see cref="MicroProcessor" />.
        /// </summary>        
        PerProcessor
    }
}