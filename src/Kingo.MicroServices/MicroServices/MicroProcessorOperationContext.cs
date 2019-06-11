using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents the context in which a <see cref="MicroProcessor"/> operates.
    /// </summary>    
    public abstract class MicroProcessorOperationContext
    {
        private readonly MicroProcessor _processor;
        private readonly AsyncMethodOperationStackTrace _stackTrace;

        internal MicroProcessorOperationContext(MicroProcessor processor, AsyncMethodOperationStackTrace stackTrace = null)
        {
            _processor = processor;
            _stackTrace = stackTrace ?? AsyncMethodOperationStackTrace.Empty;
        }

        internal MicroProcessorOperationContext(MicroProcessorOperationContext context, IAsyncMethodOperation operation)
        {
            _processor = context._processor;
            _stackTrace = context._stackTrace.Push(operation);
        }

        internal MicroProcessor Processor =>
            _processor;

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

        internal QueryOperationContext PushOperation<TResponse>(ExecuteAsyncMethodOperation<TResponse> operation) =>
            new QueryOperationContext(Processor, _stackTrace).PushOperation(operation);
    }
}
