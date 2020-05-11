using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Clocks;
using Kingo.Reflection;
using Kingo.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a basic implementation of a <see cref="IMicroProcessor" />.
    /// </summary>
    public class MicroProcessor : Disposable, IMicroProcessor
    {
        #region [====== ServiceScope ======]

        private sealed class MicroProcessorServiceProvider : IMicroProcessorServiceProvider
        {
            private readonly MicroProcessor _processor;
            private readonly IServiceProvider _serviceProvider;

            public MicroProcessorServiceProvider(MicroProcessor processor, IServiceProvider serviceProvider)
            {
                _processor = processor;
                _serviceProvider = serviceProvider;
            }

            public object GetService(Type serviceType) =>
                _serviceProvider.GetService(serviceType);

            public IServiceScope CreateScope() =>
                new MicroProcessorServiceScope(_processor, _serviceProvider.CreateScope());
        }

        private sealed class MicroProcessorServiceScope : IServiceScope
        {
            private readonly IDisposable _contextScope;
            private readonly IServiceScope _serviceScope;            

            public MicroProcessorServiceScope(MicroProcessor processor, IServiceScope serviceScope)
            {
                // Every time a new scope is created by a client of the processor, the local service provider
                // property is updated to refer to the scoped provider. This mechanism makes sure that every time
                // the internal components of the processor resolve a new dependency, it will do so in the latest
                // created scope.
                _contextScope = processor._serviceProviderContext.OverrideAsyncLocal(new MicroProcessorServiceProvider(processor, serviceScope.ServiceProvider));
                _serviceScope = serviceScope;                
            }

            public void Dispose()
            {
                _contextScope.Dispose();
                _serviceScope.Dispose();                
            }

            public IServiceProvider ServiceProvider =>
                _serviceScope.ServiceProvider;
        }

        #endregion
                
        private readonly Context<IMicroProcessorServiceProvider> _serviceProviderContext;
        private readonly Context<IPrincipal> _principalContext;
        private readonly Context<IClock> _clockContext;

        private readonly Lazy<IClock> _defaultClock;
        private readonly Lazy<MessageFactory> _messageFactory;
        private readonly Lazy<IMicroProcessorOptions> _options;
        private readonly Lazy<IMicroServiceBus> _microServiceBus;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroProcessor" /> class.
        /// </summary>                     
        /// <param name="serviceProvider">
        /// Service-provider that will be used to resolve message-handlers, their dependencies and other components.
        /// </param>        
        public MicroProcessor(IServiceProvider serviceProvider)
        {                                    
            _serviceProviderContext = new Context<IMicroProcessorServiceProvider>(CreateServiceProvider(serviceProvider));
            _principalContext = new Context<IPrincipal>();
            _clockContext = new Context<IClock>();

            _defaultClock = new Lazy<IClock>(CreateDefaultClock);
            _messageFactory = new Lazy<MessageFactory>(CreateMessageFactory, true);
            _options = new Lazy<IMicroProcessorOptions>(ResolveOptions, true);
            _microServiceBus = new Lazy<IMicroServiceBus>(CreateMicroServiceBus, true);
        }

        /// <inheritdoc />
        public override string ToString() =>
            GetType().FriendlyName();

        #region [====== Dispose ======]

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_defaultClock.IsValueCreated && _defaultClock.Value is IDisposable clock)
                {
                    clock.Dispose();
                }
                _clockContext.Dispose();
                _principalContext.Dispose();
                _serviceProviderContext.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        #region [====== Messages ======]

        internal MessageFactory MessageFactory =>
            _messageFactory.Value;

        private MessageFactory CreateMessageFactory() =>
            Options.Messages.BuildMessageFactory();

        internal MessageBus CreateMessageBus(IClock clock) =>
            new MessageBus(MessageFactory, ServiceProvider, clock);

        internal TContent Validate<TContent>(ref Message<TContent> message) =>
            Interlocked.Exchange(ref message, message.Validate(ServiceProvider)).Content;

        #endregion

        #region [====== Options ======]

        internal IMicroProcessorOptions Options =>
            _options.Value;

        private IMicroProcessorOptions ResolveOptions()
        {
            var options = ServiceProvider.GetService<MicroProcessorOptions>();
            if (options == null)
            {
                return new MicroProcessorOptions();
            }
            return options;
        }

        #endregion

        #region [====== Current User ======]        

        /// <inheritdoc />
        public IDisposable AssignUser(IPrincipal user) =>
            _principalContext.OverrideAsyncLocal(CreateClaimsPrincipal(user));

        internal ClaimsPrincipal CurrentUser() =>
            CreateClaimsPrincipal(_principalContext.Current);

        /// <summary>
        /// Creates and returns a <see cref="ClaimsPrincipal" /> based on the specified <paramref name="user"/>,
        /// or a default principal if <paramref name="user"/> is <c>null</c>.
        /// </summary>
        /// <param name="user">The principal to convert to a <see cref="ClaimsPrincipal"/>.</param>
        /// <returns>A new <see cref="ClaimsPrincipal"/> based on the specified <paramref name="user"/>.</returns>
        protected virtual ClaimsPrincipal CreateClaimsPrincipal(IPrincipal user = null) =>
            user == null ? CreateDefaultPrincipal() : new ClaimsPrincipal(user);

        /// <summary>
        /// Creates and returns a new <see cref="ClaimsPrincipal"/> that serves as the default user if
        /// no explicit principal has been set using <see cref="AssignUser" />.
        /// </summary>
        /// <returns>A default principal.</returns>
        protected virtual ClaimsPrincipal CreateDefaultPrincipal() =>
            new ClaimsPrincipal(new ClaimsIdentity(Enumerable.Empty<Claim>(), "Basic"));

        #endregion

        #region [====== Current Clock ======]

        /// <inheritdoc />
        public IDisposable AssignClock(Func<IClock, IClock> clockFactory) =>
            AssignClock(clockFactory?.Invoke(CurrentClock()));

        /// <inheritdoc />
        public IDisposable AssignClock(IClock clock) =>
            _clockContext.OverrideAsyncLocal(clock ?? throw new ArgumentNullException(nameof(clock)));

        internal IClock CurrentClock() =>
            _clockContext.Current ?? _defaultClock.Value;

        /// <summary>
        /// Creates and returns a <see cref="IClock"/> that serves as the default clock of this processor.
        /// </summary>
        /// <returns>A clock that serves as the default clock of this processor.</returns>
        protected virtual IClock CreateDefaultClock() =>
            HighResolutionClock.StartNew();

        #endregion

        #region [====== ServiceProvider ======]    

        /// <inheritdoc />
        public virtual IMicroProcessorServiceProvider ServiceProvider =>
            _serviceProviderContext.Current;

        private IMicroProcessorServiceProvider CreateServiceProvider(IServiceProvider serviceProvider) =>
            new MicroProcessorServiceProvider(this, serviceProvider ?? CreateDefaultServiceProvider());

        private static IServiceProvider CreateDefaultServiceProvider() =>
            new ServiceCollection().BuildServiceProvider(true);

        #endregion

        #region [====== MicroServiceBus ======]        

        private sealed class MicroServiceBusRelay : IMicroServiceBus
        {
            private readonly IMicroServiceBus[] _microServiceBusCollection;

            public MicroServiceBusRelay(IEnumerable<IMicroServiceBus> microServiceBusCollection)
            {
                _microServiceBusCollection = microServiceBusCollection.ToArray();
            }

            public Task SendAsync(IEnumerable<IMessage> messages) =>
                SendAsync(messages.ToArray());

            private async Task SendAsync(IMessage[] messages)
            {
                foreach (var microServiceBus in _microServiceBusCollection)
                {
                    await microServiceBus.SendAsync(messages);
                }
            }
        }

        /// <summary>
        /// Represents the <see cref="IMicroServiceBus"/> that that is used by this processor to publish all produced commands and events.
        /// </summary>
        protected IMicroServiceBus MicroServiceBus =>
            _microServiceBus.Value;

        /// <summary>
        /// Creates and returns the <see cref="IMicroServiceBus"/> that is used by this processor to publish all produced commands and events.
        /// </summary>
        /// <returns>A resolved <see cref="IMicroServiceBus"/>.</returns>
        protected virtual IMicroServiceBus CreateMicroServiceBus() =>
            new MicroServiceBusRelay(ServiceProvider.GetServices<IMicroServiceBus>());

        /// <inheritdoc />
        public virtual IEnumerable<IMicroServiceBusEndpoint> CreateMicroServiceBusEndpoints()
        {
            var methodFactory = ServiceProvider.GetService<IMessageBusEndpointFactory>();
            if (methodFactory == null)
            {
                return Enumerable.Empty<IMicroServiceBusEndpoint>();
            }
            return methodFactory.CreateMicroServiceBusEndpoints(this);
        }

        #endregion

        #region [====== Commands ======]                  

        /// <inheritdoc />
        public Task<MessageHandlerOperationResult<TCommand>> ExecuteCommandAsync<TCommand>(IMessageHandler<TCommand> messageHandler, TCommand message, MessageHeader messageHeader, CancellationToken? token = null) =>
            ExecuteCommandAsync(messageHandler, CreateCommand(message, messageHeader), token);

        private async Task<MessageHandlerOperationResult<TCommand>> ExecuteCommandAsync<TCommand>(IMessageHandler<TCommand> messageHandler, Message<TCommand> message, CancellationToken? token = null) =>
            (await ExecuteWriteOperationAsync(new CommandHandlerOperation<TCommand>(this, messageHandler, message, token)).ConfigureAwait(false)).WithInput(message);

        private Message<TCommand> CreateCommand<TCommand>(TCommand message, MessageHeader messageHeader) =>
            MessageFactory.CreateCommand(MessageDirection.Input, messageHeader, message);

        #endregion

        #region [====== Events ======]

        /// <inheritdoc />
        public Task<MessageHandlerOperationResult<TEvent>> HandleEventAsync<TEvent>(IMessageHandler<TEvent> messageHandler, TEvent message, MessageHeader messageHeader, CancellationToken? token = null) =>
            HandleEventAsync(messageHandler, CreateEvent(message, messageHeader), token);

        private async Task<MessageHandlerOperationResult<TEvent>> HandleEventAsync<TEvent>(IMessageHandler<TEvent> messageHandler, Message<TEvent> message, CancellationToken? token = null) =>
            (await ExecuteWriteOperationAsync(new EventHandlerOperation<TEvent>(this, messageHandler, message, token)).ConfigureAwait(false)).WithInput(message);

        private Message<TEvent> CreateEvent<TEvent>(TEvent message, MessageHeader messageHeader) =>
            MessageFactory.CreateEvent(MessageDirection.Input, messageHeader, message);

        #endregion

        #region [====== Requests ======]

        /// <inheritdoc />
        public async Task<QueryOperationResult<TResponse>> ExecuteQueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken? token = null) =>
            await ExecuteReadOperationAsync(new QueryOperationImplementation<TResponse>(this, query, token)).ConfigureAwait(false);

        /// <inheritdoc />
        public Task<QueryOperationResult<TRequest, TResponse>> ExecuteQueryAsync<TRequest, TResponse>(IQuery<TRequest, TResponse> query, TRequest message, MessageHeader messageHeader, CancellationToken? token = null) =>
            ExecuteQueryAsync(query, CreateRequest(message, messageHeader), token);

        private async Task<QueryOperationResult<TRequest, TResponse>> ExecuteQueryAsync<TRequest, TResponse>(IQuery<TRequest, TResponse> query, Message<TRequest> message, CancellationToken? token = null) =>
            (await ExecuteReadOperationAsync(new QueryOperationImplementation<TRequest, TResponse>(this, query, message, token)).ConfigureAwait(false)).WithInput(message);

        private Message<TRequest> CreateRequest<TRequest>(TRequest message, MessageHeader messageHeader) =>
            MessageFactory.CreateRequest(MessageDirection.Input, messageHeader, message);

        #endregion

        #region [====== ExecuteOperationAsync ======]

        /// <summary>
        /// Executes the specified (write) <paramref name="operation"/> and returns its result. If the operation
        /// produces any commands or events, all messages are sent or published on the resolved <see cref="IMicroServiceBus" />.
        /// </summary>        
        /// <param name="operation">The operation to execute.</param>
        /// <returns>The result of the operation.</returns>
        protected internal virtual async Task<MessageHandlerOperationResult> ExecuteWriteOperationAsync(MessageHandlerOperation operation)
        {
            var result = await ExecuteOperationAsync(operation).ConfigureAwait(false);
            if (result.Output.Count > 0)
            {
                await MicroServiceBus.SendAsync(result.Output).ConfigureAwait(false);
            }
            return result;
        }

        /// <summary>
        /// Executes the specified (read) <paramref name="operation"/> and returns its result.
        /// </summary>
        /// <typeparam name="TResponse">Type of the response of the query.</typeparam>
        /// <param name="operation">The operation to execute.</param>
        /// <returns>The result of the operation.</returns>
        protected virtual Task<QueryOperationResult<TResponse>> ExecuteReadOperationAsync<TResponse>(QueryOperation<TResponse> operation) =>
            ExecuteOperationAsync(operation);

        /// <summary>
        /// Executes the specified <paramref name="operation"/> and returns its result.
        /// </summary>
        /// <typeparam name="TResult">Type of the result of the operation.</typeparam>
        /// <param name="operation">The operation to execute.</param>
        /// <returns>The result of the operation.</returns>
        protected virtual Task<TResult> ExecuteOperationAsync<TResult>(MicroProcessorOperation<TResult> operation) =>
            operation.ExecuteAsync();

        #endregion        
    }
}
