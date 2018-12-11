using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Serves as a simple base class implementation of the <see cref="IMicroServiceBus"/> interface.
    /// </summary>
    public abstract class MicroServiceBus : IMicroServiceBus
    {
        #region [====== NullServiceBus ======]
        
        private sealed class NullServiceBus : MicroServiceBus
        {
            /// <inheritdoc />
            public override Task PublishAsync(object message)
            {
                if (message == null)
                {
                    throw new ArgumentNullException(nameof(message));
                }
                return Task.CompletedTask;
            }
        }

        /// <summary>
        /// Represents a service bus that ignores every message published to it.
        /// </summary>
        public static readonly MicroServiceBus Null = new NullServiceBus();

        #endregion

        /// <inheritdoc />
        public virtual async Task PublishAsync(IEnumerable<object> messages)
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
        public abstract Task PublishAsync(object message);
    }
}
