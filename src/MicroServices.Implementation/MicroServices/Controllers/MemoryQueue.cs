using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Represents a <see cref="StoreAndForwardQueue"/> that buffers all messages in memory before
    /// forwarding them to the specified bus.
    /// </summary>
    public sealed class MemoryQueue : StoreAndForwardQueue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryQueue" /> class.
        /// </summary>
        /// <param name="bus">The bus to which the messages will be forwarded.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="bus"/> is <c>null</c>.
        /// </exception>
        public MemoryQueue(IMicroServiceBus bus) :
            base(bus) { }
    }
}
