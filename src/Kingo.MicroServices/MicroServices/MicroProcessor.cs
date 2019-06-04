using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Reflection;
using Kingo.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a basic implementation of the <see cref="IMicroProcessor" /> interface.
    /// </summary>
    public class MicroProcessor : IMicroProcessor
    {
        #region [====== ServiceScope ======]

        private sealed class ServiceScope : IServiceScope
        {
            private readonly IDisposable _contextScope;
            private readonly IServiceScope _serviceScope;            

            public ServiceScope(MicroProcessor processor, IServiceScope serviceScope)
            {
                _contextScope = processor._serviceProviderContext.OverrideAsyncLocal(serviceScope.ServiceProvider);
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
        
        private readonly IMessageHandlerFactory _messageHandlers;
        private readonly IMicroProcessorPipelineFactory _pipeline;
        private readonly Context<IServiceProvider> _serviceProviderContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroProcessor" /> class.
        /// </summary>        
        /// <param name="messageHandlers">
        /// Optional factory that is used to create message handlers to handle a specific message.
        /// </param>
        /// <param name="pipeline">
        /// Optional pipeline factory that will be used by this processor to create a pipeline on top of a message handler or query
        /// right before it is invoked.
        /// </param>        
        /// <param name="serviceProvider">
        /// Optional service-provider that will be used to resolve message-handlers and their dependencies.
        /// </param> 
        public MicroProcessor(IMessageHandlerFactory messageHandlers = null, IMicroProcessorPipelineFactory pipeline = null, IServiceProvider serviceProvider = null)
        {            
            _messageHandlers = messageHandlers ?? MessageHandlerFactory.Null;
            _pipeline = pipeline ?? MicroProcessorPipelineFactory.Null;
            _serviceProviderContext = new Context<IServiceProvider>(serviceProvider ?? CreateDefaultServiceProvider());
        }

        #region [====== Components ======]                

        /// <summary>
        /// Returns the <see cref="IMessageHandlerFactory" /> of this processor.
        /// </summary>
        protected internal IMessageHandlerFactory MessageHandlers =>
            _messageHandlers;

        /// <summary>
        /// Returns the <see cref="IMicroProcessorPipelineFactory"/> of this processor.
        /// </summary>
        protected internal IMicroProcessorPipelineFactory Pipeline =>
            _pipeline;

        #endregion

        #region [====== ServiceProvider ======]        

        /// <inheritdoc />
        public virtual IServiceProvider ServiceProvider =>
            _serviceProviderContext.Current;

        /// <summary>
        /// Creates and returns a new <see cref="IServiceScope" /> that determined the lifetime of scoped
        /// dependencies. As a side-effect, this method updates the processor's <see cref="ServiceProvider" />
        /// to the provider of the scope and resets it as soon as the scope is disposed.
        /// </summary>
        public virtual IServiceScope CreateScope() =>
            new ServiceScope(this, ServiceProvider.CreateScope());

        private static IServiceProvider CreateDefaultServiceProvider() =>
            new ServiceCollection().BuildServiceProvider(true);

        #endregion

        #region [====== CreateEndpoints ======]

        /// <inheritdoc />
        public virtual IEnumerable<IMessageHandlerEndpoint> CreateEndpoints() =>
            throw new NotImplementedException();

        #endregion

        #region [====== HandleAsync ======]  

        Task<HandleAsyncResult> IMessageProcessor.HandleAsync<TMessage>(TMessage message) =>
            HandleAsync(message);        

        /// <inheritdoc />
        public virtual Task<HandleAsyncResult> HandleAsync<TMessage>(TMessage message, IMessageHandler<TMessage> handler = null, CancellationToken? token = null) =>
            new HandleMessageMethod<TMessage>(this, message, handler, token).InvokeAsync();

        /// <summary>
        /// Determines whether or not the specified message is a Command. By default,
        /// this method returns <c>true</c> when the type-name ends with 'Command'.
        /// </summary>
        /// <param name="message">The message to analyze.</param>
        /// <returns>
        /// <c>true</c> if the specified <paramref name="message"/> is a command; otherwise <c>false</c>.
        /// </returns>
        protected internal virtual bool IsCommand(object message) =>
            NameOf(message.GetType()).EndsWith("Command");

        private static string NameOf(Type messageType) =>
            messageType.IsGenericType ? messageType.Name.RemoveTypeParameterCount() : messageType.Name;

        #endregion

        #region [====== ExecuteAsync ======]

        /// <inheritdoc />
        public virtual Task<ExecuteAsyncResult<TResponse>> ExecuteAsync<TResponse>(IQuery<TResponse> query, CancellationToken? token = null) =>
            InvokeAsync(new ExecuteQueryMethod<TResponse>(this, token, query));

        /// <inheritdoc />
        public virtual Task<ExecuteAsyncResult<TResponse>> ExecuteAsync<TRequest, TResponse>(TRequest message, IQuery<TRequest, TResponse> query, CancellationToken? token = null) =>
            InvokeAsync(new ExecuteQueryMethod<TRequest, TResponse>(this, token, query, message));

        private static Task<TResult> InvokeAsync<TResult>(MicroProcessorMethod<TResult> method) =>
            method.InvokeAsync();

        #endregion        
    }
}
