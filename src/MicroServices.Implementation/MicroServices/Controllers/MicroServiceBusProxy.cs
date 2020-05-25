using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// When implemented, represents a <see cref="MicroServiceBus"/>-component that can either send or
    /// receive messages from a bus.
    /// </summary>
    public abstract class MicroServiceBusProxy : AsyncDisposable, IMicroServiceBus
    {
        /// <inheritdoc />
        public abstract Task SendAsync(IEnumerable<IMessage> messages);
    }
}
