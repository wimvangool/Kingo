namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents an operation where a specific <see cref="IAsyncMethod"/> is being executed.
    /// </summary>
    /// <typeparam name="TResult">Type of the result of the operation.</typeparam>
    public interface IAsyncMethodOperation<TResult> : IAsyncMethodOperation, IMicroProcessorOperation<TResult>
    {
        /// <summary>
        /// Converts this operation to a generic <see cref="AsyncMethodOperation{TResult}"/>.
        /// </summary>
        /// <returns>The base operation of this operation.</returns>
        AsyncMethodOperation<TResult> ToAsyncMethodOperation();
    }
}
