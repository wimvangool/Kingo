using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents an <see cref="IAsyncMethodOperation" /> that can be executed inside a <see cref="MicroProcessor"/> pipeline.
    /// </summary>
    /// <typeparam name="TResult">Type of the result of the operation.</typeparam>
    public sealed class AsyncMethodOperation<TResult> : MicroProcessorOperation<TResult>, IAsyncMethodOperation
    {
        private readonly IAsyncMethodOperation<TResult> _operation;        

        internal AsyncMethodOperation(IAsyncMethodOperation<TResult> operation)
        {
            _operation = operation;            
        }

        #region [====== IAsyncMethodOperation ======]

        /// <inheritdoc />
        public IAsyncMethod Method =>
            _operation.Method;

        /// <inheritdoc />
        public override IMessageToProcess Message =>
            _operation.Message;

        /// <inheritdoc />
        public MicroProcessorOperationContext Context =>
            _operation.Context;

        /// <inheritdoc />
        public override CancellationToken Token =>
            _operation.Token;

        /// <inheritdoc />
        public override MicroProcessorOperationType Type =>
            _operation.Type;

        /// <inheritdoc />
        public override MicroProcessorOperationKind Kind =>
            _operation.Kind;

        /// <inheritdoc />
        public override string ToString() =>
            _operation.ToString();

        #endregion

        #region [====== IMicroProcessorOperation<TResult> ======]

        /// <inheritdoc />
        public override Task<TResult> ExecuteAsync() =>
            _operation.ExecuteAsync();

        #endregion
    }
}
