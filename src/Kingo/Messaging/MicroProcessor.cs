using System;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a basic implementation of the <see cref="IMicroProcessor" /> interface.
    /// </summary>
    public class MicroProcessor : IMicroProcessor
    {             
        private readonly Lazy<MessageHandlerFactory> _messageHandlerFactory;
        private readonly Lazy<MicroProcessorPipeline> _pipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroProcessor" /> class.
        /// </summary>
        public MicroProcessor()
        {
            _messageHandlerFactory = new Lazy<MessageHandlerFactory>(CreateMessageHandlerFactory, true);             
            _pipeline = new Lazy<MicroProcessorPipeline>(() => BuildPipeline(new MicroProcessorPipeline()), true);
        }

        #region [====== Security ======]

        /// <summary>
        /// Returns the <see cref="IPrincipal" /> this processor is currently associated with. By default, it returns
        /// the <see cref="Thread.CurrentPrincipal">current thread's principal</see>. This property can be overridden
        /// to return a principal from a different context.
        /// </summary>
        protected internal virtual IPrincipal Principal =>
            Thread.CurrentPrincipal;

        #endregion

        #region [====== Command & Events ======]   

        /// <summary>
        /// Returns the <see cref="MessageHandlerFactory" /> of this processor.
        /// </summary>
        protected internal MessageHandlerFactory MessageHandlerFactory =>
            _messageHandlerFactory.Value;

        private MessageHandlerFactory CreateMessageHandlerFactory() =>
            BuildMessageHandlerFactory()
                .RegisterInstance<IMicroProcessor>(this)
                .RegisterInstance(MicroProcessorContext.Current)
                .RegisterInstance(typeof(ISchemaMap), BuildSchemaMap());             

        /// <summary>
        /// When overridden, creates and returns a <see cref="MessageHandlerFactory" /> for this processor.
        /// The default implementation returns <c>null</c>.
        /// </summary>        
        /// <returns>A new <see cref="MessageHandlerFactory" /> to be used by this processor.</returns>
        protected internal virtual MessageHandlerFactory BuildMessageHandlerFactory() =>
            new UnityContainerFactory();

        /// <summary>
        /// Builds and returns a <see cref="ISchemaMap" /> that will be registered as a dependency and can be used by repositories
        /// to serialize and deserialize objects using specific type-information.
        /// </summary>        
        /// <returns>A schema-map instance.</returns>
        protected virtual ISchemaMap BuildSchemaMap() =>
            SchemaMap.None;

        /// <inheritdoc />
        public Task<IMessageStream> HandleStreamAsync(IMessageStream inputStream, CancellationToken? token = null)
        {
            if (inputStream == null)
            {
                throw new ArgumentNullException(nameof(inputStream));
            }
            if (inputStream.Count == 0)
            {
                return Task.FromResult(MessageStream.Empty);
            }
            return HandleInputStreamAsync(new HandleInputStreamMethod(this, token, inputStream));
        }        

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

        #region [====== Queries ======]

        /// <inheritdoc />
        public Task<TMessageOut> ExecuteAsync<TMessageOut>(IQuery<TMessageOut> query, CancellationToken? token = null)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return ExecuteQueryAsync(new ExecuteQueryMethodImplementation<TMessageOut>(this, token, query));
        }

        /// <inheritdoc />
        public Task<TMessageOut> ExecuteAsync<TMessageIn, TMessageOut>(TMessageIn message, IQuery<TMessageIn, TMessageOut> query, CancellationToken? token = null)
        {
            if (ReferenceEquals(message, null))
            {
                throw new ArgumentNullException(nameof(message));
            }
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return ExecuteQueryAsync(new ExecuteQueryMethodImplementation<TMessageIn, TMessageOut>(this, token, query, message));
        }

        #endregion

        #region [====== MicroProcessorPipeline ======]

        internal MicroProcessorPipeline Pipeline =>
            _pipeline.Value;

        /// <summary>
        /// When overridden, this method can be used to add global filters and configuration to the pipeline of this processor.
        /// </summary>
        /// <param name="pipeline">The pipeline to configure.</param>
        /// <returns>The configured pipeline.</returns>
        protected virtual MicroProcessorPipeline BuildPipeline(MicroProcessorPipeline pipeline) =>
            pipeline;

        /// <summary>
        /// Handles an input-stream by invoking the specified <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The method to invoke for handling the input-stream.</param>
        /// <returns>All events that were published while handling the input-stream.</returns>
        /// <exception cref="ExternalProcessorException">
        /// Something went wrong while handling the input-stream.
        /// </exception>
        protected virtual Task<IMessageStream> HandleInputStreamAsync(HandleInputStreamMethod method) =>
            InvokeAsync(method);

        /// <summary>
        /// Handles a single message by invoking the specified <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The method to invoke for handling the message.</param>
        /// <returns>The task carrying out the operation.</returns>       
        /// <exception cref="ExternalProcessorException">
        /// Something went wrong while handling the message.
        /// </exception>
        protected internal virtual Task HandleMessageAsync(HandleMessageMethod method) =>
            method.InvokeAsync();

        /// <summary>
        /// Executes a query by invoking the specified <paramref name="method" />.
        /// </summary>        
        /// <param name="method">The method to invoke for executing the query.</param>
        /// <returns>The result of the query.</returns>
        /// <exception cref="ExternalProcessorException">
        /// Something went wrong while executing the query.
        /// </exception>
        protected virtual Task<TMessageOut> ExecuteQueryAsync<TMessageOut>(ExecuteQueryMethod<TMessageOut> method) =>
            InvokeAsync(method);

        /// <summary>
        /// Handles an input-stream or executed a query by invoking the specified <paramref name="method"/>.
        /// </summary>       
        /// <param name="method">The method to invoke for performing the operation.</param>
        /// <returns>The result of the specified <paramref name="method"/>.</returns>
        /// <exception cref="ExternalProcessorException">
        /// Something went wrong while handling the input-stream or executing the query.
        /// </exception>
        protected virtual Task<TResult> InvokeAsync<TResult>(MicroProcessorMethod<TResult> method) =>
            method.InvokeAsync();

        #endregion
    }
}
