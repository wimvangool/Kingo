using System;
using System.Security.Claims;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents the context in which a <see cref="MicroProcessor"/> operates.
    /// </summary>    
    public abstract class MicroProcessorOperationContext
    {        
        private readonly MicroProcessor _processor;
        private readonly ClaimsPrincipal _user;
        private readonly AsyncMethodOperationStackTrace _stackTrace;
        private readonly IQueryProcessor _queryProcessor;

        internal MicroProcessorOperationContext(MicroProcessor processor, AsyncMethodOperationStackTrace stackTrace = null)
        {
            _processor = processor;
            _user = processor.CreatePrincipal();
            _stackTrace = stackTrace ?? AsyncMethodOperationStackTrace.Empty;
            _queryProcessor = new QueryProcessor(this);
        }

        internal MicroProcessorOperationContext(MicroProcessorOperationContext context, IAsyncMethodOperation operation)
        {
            _processor = context._processor;
            _user = context._user;
            _stackTrace = context._stackTrace.Push(operation);
            _queryProcessor = new QueryProcessor(this);
        }

        internal MicroProcessor Processor =>
            _processor;        

        /// <summary>
        /// Gets the user that is executing the current operation.
        /// </summary>
        public ClaimsPrincipal User =>
            _user;

        /// <summary>
        /// Returns a stack trace of all operations that are currently being executed.
        /// </summary>
        public IAsyncMethodOperationStackTrace StackTrace =>
            _stackTrace;

        /// <summary>
        /// Returns the <see cref="IServiceProvider" /> of this context.
        /// </summary>
        public IServiceProvider ServiceProvider =>
            _processor.ServiceProvider;

        /// <summary>
        /// Returns the processor that can be used to execute (sub)queries during an operation.
        /// </summary>
        public IQueryProcessor QueryProcessor =>
            _queryProcessor;

        /// <inheritdoc />
        public override string ToString() =>
            GetType().FriendlyName();

        internal QueryOperationContext PushOperation<TResponse>(ExecuteAsyncMethodOperation<TResponse> operation) =>
            new QueryOperationContext(this, operation);
    }
}
