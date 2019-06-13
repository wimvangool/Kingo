namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents the context in which a <see cref="MicroProcessor"/> executes a query.
    /// </summary>
    public sealed class QueryOperationContext : MicroProcessorOperationContext
    {
        internal QueryOperationContext(MicroProcessor processor, AsyncMethodOperationStackTrace stackTrace = null) :
            base(processor, stackTrace) { }
        
        internal QueryOperationContext(MicroProcessorOperationContext context, IAsyncMethodOperation operation) :
            base(context, operation) { }
    }
}
