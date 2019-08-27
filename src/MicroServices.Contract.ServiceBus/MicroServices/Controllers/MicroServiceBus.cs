using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Collections.Generic;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Provides a base implementation of the <see cref="IMicroServiceBus" /> interface.
    /// </summary>
    public abstract class MicroServiceBus : IMicroServiceBus
    {
        /// <inheritdoc />
        public virtual async Task PublishAsync(IEnumerable<object> events)
        {
            if (events == null)
            {
                throw new ArgumentNullException(nameof(events));
            }
            foreach (var message in events.WhereNotNull())
            {
                await PublishAsync(message);
            }
        }

        /// <inheritdoc />
        public abstract Task PublishAsync(object @event);
    }
}
