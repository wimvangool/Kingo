using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Serves as a base-class implementation of the <see cref="IMicroServiceBusClient"/> interface.
    /// </summary>
    /// <typeparam name="TMessage">Type of the messages that are sent to or received from the service-bus.</typeparam>
    public abstract class MicroServiceBusClient<TMessage> : MicroServiceBusConnection, IMicroServiceBusClient
    {
        private readonly List<IMicroServiceBusConnection> _connections;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceBusClient{T}" /> class.
        /// </summary>
        protected MicroServiceBusClient()
        {
            _connections = new List<IMicroServiceBusConnection>();
        }

        /// <summary>
        /// The bus to which all events that are created when processing command or events should be published.
        /// </summary>
        protected abstract IMicroServiceBus Bus
        {
            get;
        }

        #region [====== SendAsync ======]

        /// <inheritdoc />
        public Task SendAsync(IEnumerable<IMessageToDispatch> commands)
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (commands == null)
            {
                throw new ArgumentNullException(nameof(commands));
            }
            return SendAsync(commands.Select(Pack));
        }

        /// <summary>
        /// Sends all specified <paramref name="commands"/> to the appropriate service(s).
        /// By default, every command is sent one after another. If you wish to support sending
        /// entire batches at once, you may override this method to provide your own implementation.
        /// </summary>
        /// <param name="commands">The commands to send.</param>
        protected virtual async Task SendAsync(IEnumerable<TMessage> commands)
        {
            foreach (var command in commands)
            {
                await SendAsync(command).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Sends the specified <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to send.</param>
        protected abstract Task SendAsync(TMessage command);

        #endregion

        #region [====== PublishAsync ======]

        /// <inheritdoc />
        public Task PublishAsync(IEnumerable<IMessageToDispatch> events)
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (events == null)
            {
                throw new ArgumentNullException(nameof(events));
            }
            return PublishAsync(events.Select(Pack));
        }

        /// <summary>
        /// Publishes all specified <paramref name="events"/>. By default, every event is published
        /// one after another. If you wish to support publishing entire batches at once, you may
        /// override this method to provide your own implementation.
        /// </summary>
        /// <param name="events">The events to publish.</param>
        protected virtual async Task PublishAsync(IEnumerable<TMessage> events)
        {
            foreach (var @event in events)
            {
                await PublishAsync(@event).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Publishes the specified <paramref name="event"/>.
        /// </summary>
        /// <param name="event">The event to publish.</param>
        protected abstract Task PublishAsync(TMessage @event);

        #endregion

        #region [====== ConnectToEndpointAsync ======]

        /// <inheritdoc />
        public async Task<bool> ConnectToEndpointAsync(IMicroServiceBusEndpoint endpoint)
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (endpoint == null)
            {
                throw new ArgumentNullException(nameof(endpoint));
            }
            if (IsSupportedEndpoint(endpoint))
            {
                _connections.Add(await ConnectToQueueAsync(endpoint).ConfigureAwait(false));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines if the specified <paramref name="endpoint"/> is supported by this service-bus client. By default,
        /// this method compares the name of the service which this client is part of with the name of the service that
        /// the message is part of. If these are equal, it is assumed the endpoint is supported.
        /// </summary>
        /// <param name="endpoint">The endpoint to check.</param>
        /// <returns>
        /// <c>true</c> if the specified <paramref name="endpoint"/> is supported and a connection to it and the service-bus
        /// must be created; otherwise <c>false</c>.
        /// </returns>
        protected virtual bool IsSupportedEndpoint(IMicroServiceBusEndpoint endpoint) =>
            true; // TODO

        private Task<IMicroServiceBusConnection> ConnectToQueueAsync(IMicroServiceBusEndpoint endpoint)
        {
            switch (endpoint.MessageKind)
            {
                case MessageKind.Command:
                    return ConnectToCommandQueueAsync(endpoint);
                case MessageKind.Event:
                    return ConnectToEventQueueAsync(endpoint);
                default:
                    throw NewMessageKindNotSupportedException(endpoint);
            }
        }

        /// <summary>
        /// Connects the specified <paramref name="endpoint"/> to the associated command-queue of the service-bus.
        /// </summary>
        /// <param name="endpoint">The endpoint to connect.</param>
        /// <returns>The connection that has been made.</returns>
        protected virtual Task<IMicroServiceBusConnection> ConnectToCommandQueueAsync(IMicroServiceBusEndpoint endpoint) =>
            throw NewMessageKindNotSupportedException(endpoint);

        /// <summary>
        /// Connects the specified <paramref name="endpoint"/> to the associated event-queue of the service-bus.
        /// </summary>
        /// <param name="endpoint">The endpoint to connect.</param>
        /// <returns>The connection that has been made.</returns>
        protected virtual Task<IMicroServiceBusConnection> ConnectToEventQueueAsync(IMicroServiceBusEndpoint endpoint) =>
            throw NewMessageKindNotSupportedException(endpoint);

        private static Exception NewMessageKindNotSupportedException(IMicroServiceBusEndpoint endpoint)
        {
            var messageFormat = ExceptionMessages.MicroServiceBusClient_MessageKindNotSupported;
            var message = string.Format(messageFormat, endpoint, endpoint.MessageKind);
            return new ArgumentException(message, nameof(endpoint));
        }

        #endregion

        #region [====== Pack & Unpack ======]

        /// <summary>
        /// Packs the specified <paramref name="message"/> into a message that can be sent to the service-bus.
        /// </summary>
        /// <param name="message">The message to pack.</param>
        /// <returns>A message that is ready to be dispatched by the service-bus.</returns>
        protected abstract TMessage Pack(IMessageToDispatch message);

        /// <summary>
        /// Unpacks the specified <paramref name="message"/> and returns its (deserialized) contents.
        /// </summary>
        /// <param name="message">The message to unpack.</param>
        /// <returns>The contents of the specified <paramref name="message"/>.</returns>
        protected abstract object Unpack(TMessage message);

        #endregion

        #region [====== Close & Dispose ======]

        /// <inheritdoc />
        public override void Close()
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (IsClosed)
            {
                return;
            }
            foreach (var connection in _connections)
            {
                connection.Close();
            }
            base.Close();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }
            if (disposing)
            {
                Close();

                foreach (var connection in _connections)
                {
                    connection.Dispose();
                }
                _connections.Clear();
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
