using System.Threading;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a method that is to be invoked by a <see cref="MicroProcessor"/> for handling an input-stream or executing a query.
    /// </summary>
    /// <typeparam name="TResult">Type of the result of the operation.</typeparam>
    public abstract class MicroProcessorMethod<TResult>
    {
        /// <summary>
        /// Cancellation token that was provided to the processor.
        /// </summary>
        public abstract CancellationToken Token
        {
            get;
        }

        /// <summary>
        /// Invokes the method and returns its result.
        /// </summary>
        /// <returns>The result of the method.</returns>
        /// <exception cref="ExternalProcessorException">
        /// Something went wrong while handling a message or executing the query.
        /// </exception>
        public abstract Task<TResult> InvokeAsync();
    }
}
