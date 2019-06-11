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
                
        private readonly Context<IServiceProvider> _serviceProviderContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroProcessor" /> class.
        /// </summary>                     
        /// <param name="serviceProvider">
        /// Service-provider that will be used to resolve message-handlers, their dependencies and other components.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="serviceProvider"/> is <c>null</c>.
        /// </exception>
        public MicroProcessor(IServiceProvider serviceProvider)
        {                        
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }
            _serviceProviderContext = new Context<IServiceProvider>(serviceProvider);
        }        

        #region [====== ServiceProvider ======]        

        /// <inheritdoc />
        public virtual IServiceProvider ServiceProvider =>
            _serviceProviderContext.Current;

        /// <summary>
        /// Creates and returns a new <see cref="IServiceScope" /> that determines the lifetime of scoped
        /// dependencies. As a side-effect, this method updates the processor's <see cref="ServiceProvider" />
        /// to the provider of the scope and resets it as soon as the scope is disposed.
        /// </summary>
        public virtual IServiceScope CreateScope() =>
            new ServiceScope(this, ServiceProvider.CreateScope());

        #endregion

        #region [====== ExecuteAsync (Commands & Queries) ======]          

        /// <inheritdoc />
        public Task<MessageHandlerOperationResult> ExecuteAsync<TCommand>(IMessageHandler<TCommand> messageHandler, TCommand message, CancellationToken? token = null) =>
            ExecuteOperationAsync(new CommandHandlerOperation<TCommand>(this, messageHandler, message, token));                                    

        /// <inheritdoc />
        public Task<QueryOperationResult<TResponse>> ExecuteAsync<TResponse>(IQuery<TResponse> query, CancellationToken? token = null) =>
            ExecuteOperationAsync(new QueryOperationImplementation<TResponse>(this, query, token));

        /// <inheritdoc />
        public Task<QueryOperationResult<TResponse>> ExecuteAsync<TRequest, TResponse>(IQuery<TRequest, TResponse> query, TRequest message, CancellationToken? token = null) =>
            ExecuteOperationAsync(new QueryOperationImplementation<TRequest, TResponse>(this, query, message, token));

        #endregion

        #region [====== ExecuteAsync (Operations) ======]

        /// <summary>
        /// Executes the specified <paramref name="operation"/> and returns its result.
        /// </summary>        
        /// <param name="operation">The operation to execute.</param>
        /// <returns>The result of the operation.</returns>
        protected virtual Task<MessageHandlerOperationResult> ExecuteOperationAsync(MessageHandlerOperation operation) =>
            ExecuteOperationAsync<MessageHandlerOperationResult>(operation);

        /// <summary>
        /// Executes the specified <paramref name="operation"/> and returns its result.
        /// </summary>
        /// <typeparam name="TResponse">Type of the response of the query.</typeparam>
        /// <param name="operation">The operation to execute.</param>
        /// <returns>The result of the operation.</returns>
        protected virtual Task<QueryOperationResult<TResponse>> ExecuteOperationAsync<TResponse>(QueryOperation<TResponse> operation) =>
            ExecuteOperationAsync<QueryOperationResult<TResponse>>(operation);

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
