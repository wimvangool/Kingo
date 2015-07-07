using System;

namespace Syztem.ComponentModel.Server
{
    /// <summary>
    /// Represents a (collection of) source(s) a message can originate from.
    /// </summary>
    [Flags]
    public enum MessageSources
    {
        /// <summary>
        /// Represents a source that has not been defined.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Represents the <see cref="IMessageProcessorBus">Internal Bus</see>.
        /// </summary>
        InternalMessageBus = 1,

        /// <summary>
        /// Represents any external source or bus.
        /// </summary>
        ExternalMessageBus = 2,

        /// <summary>
        /// Represents all sources.
        /// </summary>
        Any = InternalMessageBus | ExternalMessageBus
    }
}
