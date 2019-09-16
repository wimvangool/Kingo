using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Serves as a base-class implementation of the <see cref="IMicroServiceBusClient"/> interface.
    /// </summary>
    /// <typeparam name="TMessage">Type of the messages that are sent to or received from the service-bus.</typeparam>
    public abstract class MicroServiceBusClient<TMessage> : MicroServiceBusConnection, IMicroServiceBusClient
    {
        private readonly SemaphoreSlim _lock;
        private readonly List<IMicroServiceBusConnection> _connections;
        private readonly Lazy<Dictionary<MessageKind, EndpointNameResolver>> _nameResolverMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceBusClient{T}" /> class.
        /// </summary>
        protected MicroServiceBusClient()
        {
            _lock = new SemaphoreSlim(1);
            _connections = new List<IMicroServiceBusConnection>();
            _nameResolverMap = new Lazy<Dictionary<MessageKind, EndpointNameResolver>>(CreateNameResolverMap);
        }

        #region [====== NameResolvers ======]

        /// <summary>
        /// Returns the map that contains one endpoint name-resolver per message type.
        /// </summary>
        protected IReadOnlyDictionary<MessageKind, EndpointNameResolver> NameResolverMap =>
            _nameResolverMap.Value;

        private Dictionary<MessageKind, EndpointNameResolver> CreateNameResolverMap() =>
            new Dictionary<MessageKind, EndpointNameResolver>()
            {
                { MessageKind.Command, CreateCommandEndpointNameResolver() },
                { MessageKind.Event, CreateEventEndpointNameResolver() },
                { MessageKind.QueryRequest, CreateQueryRequestEndpointNameResolver() },
                { MessageKind.QueryResponse, CreateQueryResponseEndpointNameResolver() }
            };

        /// <summary>
        /// Creates and returns a new <see cref="EndpointNameResolver" /> that will be used to resolve
        /// the name of each endpoint that handles <see cref="MessageKind.Command">commands</see>.
        /// </summary>
        /// <returns>A new <see cref="EndpointNameResolver" />.</returns>
        protected virtual EndpointNameResolver CreateCommandEndpointNameResolver() =>
            new EndpointNameResolver("[service].Commands");

        /// <summary>
        /// Creates and returns a new <see cref="EndpointNameResolver" /> that will be used to resolve
        /// the name of each endpoint that handles <see cref="MessageKind.Event">events</see>.
        /// </summary>
        /// <returns>A new <see cref="EndpointNameResolver" />.</returns>
        protected virtual EndpointNameResolver CreateEventEndpointNameResolver() =>
            new EndpointNameResolver("[service].[handler].[message]");

        /// <summary>
        /// Creates and returns a new <see cref="EndpointNameResolver" /> that will be used to resolve
        /// the name of each endpoint that handles <see cref="MessageKind.QueryRequest">(query) requests</see>.
        /// </summary>
        /// <returns>A new <see cref="EndpointNameResolver" />.</returns>
        protected virtual EndpointNameResolver CreateQueryRequestEndpointNameResolver() =>
            new EndpointNameResolver("[service].QueryRequests");

        /// <summary>
        /// Creates and returns a new <see cref="EndpointNameResolver" /> that will be used to resolve
        /// the name of each endpoint that handles <see cref="MessageKind.QueryResponse">(query) responses</see>.
        /// </summary>
        /// <returns>A new <see cref="EndpointNameResolver" />.</returns>
        protected virtual EndpointNameResolver CreateQueryResponseEndpointNameResolver() =>
            new EndpointNameResolver("[service].QueryResponses");

        #endregion

        #region [====== ConnectToEndpointAsync ======]

        /// <inheritdoc />
        public async Task ConnectToEndpointAsync(IMicroServiceBusEndpoint endpoint)
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (endpoint == null)
            {
                throw new ArgumentNullException(nameof(endpoint));
            }
            await _lock.WaitAsync();

            try
            {
                _connections.Add(await ConnectToQueueAsync(endpoint));
            }
            finally
            {
                _lock.Release();
            }
        }

        private Task<IMicroServiceBusConnection> ConnectToQueueAsync(IMicroServiceBusEndpoint endpoint)
        {
            if (NameResolverMap.TryGetValue(endpoint.MessageKind, out var nameResolver))
            {
                var endpointName = nameResolver.ResolveName(endpoint);

                switch (endpoint.MessageKind)
                {
                    case MessageKind.Command:
                        return ConnectToCommandQueueAsync(endpoint, endpointName);
                    case MessageKind.Event:
                        return ConnectToEventQueueAsync(endpoint, endpointName);
                    case MessageKind.QueryRequest:
                        return ConnectToQueryRequestQueueAsync(endpoint, endpointName);
                    case MessageKind.QueryResponse:
                        return ConnectToQueryResponseQueueAsync(endpoint, endpointName);
                }
            }
            throw NewEndpointNotSupportedException(endpoint, string.Empty);
        }

        /// <summary>
        /// Connects the specified <paramref name="endpoint"/> to the associated command-queue of the service-bus.
        /// </summary>
        /// <param name="endpoint">The endpoint to connect.</param>
        /// <param name="endpointName">Name of the endpoint/queue to connect to.</param>
        /// <returns>The connection that has been made.</returns>
        /// <exception cref="ArgumentException">
        /// The specified <paramref name="endpoint"/> is not supported by this client.
        /// </exception>
        protected virtual Task<IMicroServiceBusConnection> ConnectToCommandQueueAsync(IMicroServiceBusEndpoint endpoint, string endpointName) =>
            throw NewEndpointNotSupportedException(endpoint, endpointName);

        /// <summary>
        /// Connects the specified <paramref name="endpoint"/> to the associated event-queue of the service-bus.
        /// </summary>
        /// <param name="endpoint">The endpoint to connect.</param>
        /// <param name="endpointName">Name of the endpoint/queue to connect to.</param>
        /// <returns>The connection that has been made.</returns>
        /// <exception cref="ArgumentException">
        /// The specified <paramref name="endpoint"/> is not supported by this client.
        /// </exception>
        protected virtual Task<IMicroServiceBusConnection> ConnectToEventQueueAsync(IMicroServiceBusEndpoint endpoint, string endpointName) =>
            throw NewEndpointNotSupportedException(endpoint, endpointName);

        /// <summary>
        /// Connects the specified <paramref name="endpoint"/> to the associated query request-queue of the service-bus.
        /// </summary>
        /// <param name="endpoint">The endpoint to connect.</param>
        /// <param name="endpointName">Name of the endpoint/queue to connect to.</param>
        /// <returns>The connection that has been made.</returns>
        /// <exception cref="ArgumentException">
        /// The specified <paramref name="endpoint"/> is not supported by this client.
        /// </exception>
        protected virtual Task<IMicroServiceBusConnection> ConnectToQueryRequestQueueAsync(IMicroServiceBusEndpoint endpoint, string endpointName) =>
            throw NewEndpointNotSupportedException(endpoint, endpointName);

        /// <summary>
        /// Connects the specified <paramref name="endpoint"/> to the associated query response-queue of the service-bus.
        /// </summary>
        /// <param name="endpoint">The endpoint to connect.</param>
        /// <param name="endpointName">Name of the endpoint/queue to connect to.</param>
        /// <returns>The connection that has been made.</returns>
        /// <exception cref="ArgumentException">
        /// The specified <paramref name="endpoint"/> is not supported by this client.
        /// </exception>
        protected virtual Task<IMicroServiceBusConnection> ConnectToQueryResponseQueueAsync(IMicroServiceBusEndpoint endpoint, string endpointName) =>
            throw NewEndpointNotSupportedException(endpoint, endpointName);

        /// <summary>
        /// Creates and returns an exception that can be thrown when an attempt is made to connect to the specified
        /// <paramref name="endpoint"/> while this endpoint is not supported (e.g. the message kind is not supported).
        /// </summary>
        /// <param name="endpoint">The endpoint that is not supported by this client.</param>
        /// <param name="endpointName">Name of the endpoint/queue to connect to.</param>
        /// <returns>A new exception that indicates that the specified <paramref name="endpoint"/> is not supported.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="endpoint"/> or <paramref name="endpointName"/> is <c>null</c>.
        /// </exception>
        protected static Exception NewEndpointNotSupportedException(IMicroServiceBusEndpoint endpoint, string endpointName)
        {
            if (endpoint == null)
            {
                throw new ArgumentNullException(nameof(endpoint));
            }
            if (endpointName == null)
            {
                throw new ArgumentNullException(nameof(endpointName));
            }
            var messageFormat = ExceptionMessages.MicroServiceBusClient_EndpointNotSupported;
            var message = string.Format(messageFormat, endpoint, endpointName);
            return new ArgumentException(message, nameof(endpoint));
        }

        #endregion

        #region [====== SendAsync ======]

        /// <inheritdoc />
        public async Task SendAsync(IEnumerable<IMessageToDispatch> commands)
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (commands == null)
            {
                throw new ArgumentNullException(nameof(commands));
            }
            await _lock.WaitAsync();

            try
            {
                await SendAsync(commands.Select(Pack));
            }
            finally
            {
                _lock.Release();
            }
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
        public async Task PublishAsync(IEnumerable<IMessageToDispatch> events)
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (events == null)
            {
                throw new ArgumentNullException(nameof(events));
            }
            await _lock.WaitAsync();

            try
            {
                await PublishAsync(events.Select(Pack));
            }
            finally
            {
                _lock.Release();
            }
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
                _lock.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
