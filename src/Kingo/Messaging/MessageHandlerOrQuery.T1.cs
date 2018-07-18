using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a <see cref="IMessageHandler{T}" />, <see cref="IQuery{T}" /> or <see cref="IQuery{T, S} "/>.
    /// </summary>
    /// <typeparam name="TResult">Type of the resulting operation.</typeparam>
    public abstract class MessageHandlerOrQuery<TResult> : MessageHandlerOrQuery
    {        
        /// <summary>
        /// Invokes the message handler or query and returns the result.
        /// </summary>
        /// <param name="context">Context of the <see cref="IMicroProcessor" />.</param>
        /// <returns>The result of the operation.</returns>
        public abstract Task<InvokeAsyncResult<TResult>> InvokeAsync(MicroProcessorContext context);        
    }
}
