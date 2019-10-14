using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// When implemented, represents a queue that is used to temporarily store all messages that are
    /// produced by a <see cref="IMicroProcessor"/> operation, such that any mutations in the write-store
    /// can be persisted within the same transaction as the messages that are produced by the operation.
    /// The <see cref="StoreAndForwardQueue"/> uses a message pump, which function is controlled by
    /// the <see cref="IHostedService.StartAsync"/> and <see cref="IHostedService.StopAsync"/> functions,
    /// to read these messages from the queue in an independent, asynchronous process/thread, which then
    /// attempts to forwards all messages to another <see cref="IMicroServiceBus" />.
    /// </summary>
    [MicroProcessorComponent(ServiceLifetime.Singleton)]
    public abstract class StoreAndForwardQueue : Disposable, IMicroServiceBus, IHostedService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoreAndForwardQueue" /> class.
        /// </summary>
        /// <param name="microServiceBus">The <see cref="IMicroServiceBus"/> to which all messages are to be forwarded.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="microServiceBus"/> is <c>null</c>.
        /// </exception>
        protected StoreAndForwardQueue(IMicroServiceBus microServiceBus)
        {
            MicroServiceBus = microServiceBus ?? throw new ArgumentNullException(nameof(microServiceBus));
        }

        /// <summary>
        /// Returns the <see cref="IMicroServiceBus"/> to which all messages are to be forwarded.
        /// </summary>
        protected IMicroServiceBus MicroServiceBus
        {
            get;
        }

        /// <summary>
        /// Starts the message pump of this queue.
        /// </summary>
        /// <param name="cancellationToken">Token that can be used to abort the operation.</param>
        /// <exception cref="ObjectDisposedException">
        /// The queue has already been disposed.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The message pump has already been started.
        /// </exception>
        public Task StartAsync(CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        /// <summary>
        /// Stops the message pump of this queue.
        /// </summary>
        /// <param name="cancellationToken">Token that can be used to abort the operation.</param>
        /// <exception cref="ObjectDisposedException">
        /// The queue has already been disposed.
        /// </exception>
        public Task StopAsync(CancellationToken cancellationToken) =>
            throw new NotImplementedException();

        /// <summary>
        /// Stores all specified <paramref name="commands"/> in the queue.
        /// </summary>
        /// <param name="commands">The commands to store.</param>
        /// <exception cref="ObjectDisposedException">
        /// The queue has already been disposed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="commands"/> is <c>null</c>.
        /// </exception>
        public Task SendCommandsAsync(IEnumerable<IMessageToDispatch> commands) =>
            throw new NotImplementedException();

        /// <summary>
        /// Stores all specified <paramref name="events"/> in the queue.
        /// </summary>
        /// <param name="events">The events to store.</param>
        /// <exception cref="ObjectDisposedException">
        /// The queue has already been disposed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="events"/> is <c>null</c>.
        /// </exception>
        public Task PublishEventsAsync(IEnumerable<IMessageToDispatch> events) =>
            throw new NotImplementedException();
    }
}
