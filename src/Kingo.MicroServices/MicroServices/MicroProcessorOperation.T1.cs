using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented, represents an operation of a <see cref="IMicroProcessor" /> that can be executed.
    /// </summary>
    /// <typeparam name="TResult">Type of the result of this operation.</typeparam>
    public abstract class MicroProcessorOperation<TResult> : MicroProcessorOperation, IMicroProcessorOperation<TResult>
    {
        /// <inheritdoc />
        public abstract Task<TResult> ExecuteAsync();
    }
}
