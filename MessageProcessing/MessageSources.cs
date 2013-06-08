using System;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// Specifies a set of sources of a message.
    /// </summary>
    [Flags]
    public enum MessageSources
    {
        /// <summary>
        /// Indicates no sources.
        /// </summary>
        None = 0,

        /// <summary>
        /// TODO
        /// </summary>
        EnterpriseServiceBus = 1,

        /// <summary>
        /// TODO
        /// </summary>
        DomainEventBus = 2,

        /// <summary>
        /// Indicates all possible sources.
        /// </summary>
        All = EnterpriseServiceBus | DomainEventBus
    }
}
