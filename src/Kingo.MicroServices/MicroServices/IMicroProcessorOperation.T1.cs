using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents an operation of a <see cref="IMicroProcessor" /> that can
    /// be executed.
    /// </summary>
    /// <typeparam name="TResult">Type of the result of this operation.</typeparam>
    public interface IMicroProcessorOperation<TResult> : IMicroProcessorOperation
    {
        /// <summary>
        /// Executes this operation.
        /// </summary>
        /// <returns>The result of this operation.</returns>
        Task<TResult> ExecuteAsync();
    }
}
