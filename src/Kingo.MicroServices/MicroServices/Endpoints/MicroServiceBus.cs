using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Collections.Generic;

namespace Kingo.MicroServices.Endpoints
{
    /// <summary>
    /// Provides a base implementation of the <see cref="IMicroServiceBus" /> interface.
    /// </summary>
    public abstract class MicroServiceBus : IMicroServiceBus
    {
        /// <inheritdoc />
        public virtual async Task PublishAsync(IEnumerable<IMessage> messages)
        {
            if (messages == null)
            {
                throw new ArgumentNullException(nameof(messages));
            }
            foreach (var message in messages.WhereNotNull())
            {
                await PublishAsync(message);
            }
        }

        /// <inheritdoc />
        public abstract Task PublishAsync(IMessage message);
    }
}
