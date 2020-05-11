using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Serves as a base-class for all <see cref="IMicroServiceBus" /> implementations. 
    /// </summary>
    public abstract class MicroServiceBus : IMicroServiceBus, IDisposable
    {
        #region [====== MessageSender ======]

        /// <summary>
        /// Starts the component of this bus that is responsible for sending messages.
        /// After this method is called, <see cref="SendAsync"/> may be used to send
        /// new messages on this bus.
        /// </summary>
        /// <param name="token">Token that can be used to cancel the operation.</param>
        /// <exception cref="InvalidOperationException">
        /// The message sender has already been started.
        /// </exception>
        public Task StartMessageSenderAsync(CancellationToken token) =>
            throw new NotImplementedException();

        /// <summary>
        /// Stops the component of this bus that is responsible for sending messages.
        /// After this method is called, it is no longer possible to use <see cref="SendAsync"/>
        /// to send new messages.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The message sender has already been stopped.
        /// </exception>
        public Task StopMessageSenderAsync() =>
            throw new NotImplementedException();
        
        /// <inheritdoc />
        public Task SendAsync(IEnumerable<IMessage> messages) =>
            throw new NotImplementedException();

        #endregion

        #region [====== MessageReceiver ======]

        /// <summary>
        /// Starts the component of this bus that is responsible for receiving messages.
        /// After this method is called, the bus will start feeding received messages into the
        /// system (e.g. by handing them over to a <see cref="IMicroProcessor"/> or another
        /// <see cref="IMicroServiceBus" />.
        /// </summary>
        /// <param name="token">Token that can be used to cancel the operation.</param>
        /// <exception cref="InvalidOperationException">
        /// The message receiver has already been started.
        /// </exception>
        public Task StartMessageReceiverAsync(CancellationToken token) =>
            throw new NotImplementedException();

        /// <summary>
        /// Stops the component of this bus that is responsible for sending messages.
        /// After this method is called, it is no longer possible to use <see cref="SendAsync"/>
        /// to send new messages.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The message receiver has already been stopped.
        /// </exception>
        public Task StopMessageReceiverAsync() =>
            throw new NotImplementedException();

        #endregion

        /// <inheritdoc />
        public void Dispose() =>
            throw new NotImplementedException();
    }
}
