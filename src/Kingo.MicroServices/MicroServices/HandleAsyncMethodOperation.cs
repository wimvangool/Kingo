namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents an operation of a <see cref="IMicroProcessor"/> where a message is being handled
    /// by a message handler.
    /// </summary>
    public abstract class HandleAsyncMethodOperation : MicroProcessorOperation<MessageHandlerOperationResult>, IAsyncMethodOperation<MessageHandlerOperationResult>
    {
        #region [====== IAsyncMethodOperation ======]

        IAsyncMethod IAsyncMethodOperation.Method =>
            Method;

        /// <summary>
        /// Returns the <see cref="IMessageHandler{TMessage}.HandleAsync"/> method that is being invoked
        /// in this operation.
        /// </summary>
        public abstract HandleAsyncMethod Method
        {
            get;
        }               

        MicroProcessorOperationContext IAsyncMethodOperation.Context =>
            Context;

        /// <summary>
        /// Returns the context of this operation.
        /// </summary>
        public abstract MessageHandlerOperationContext Context
        {
            get;
        }

        /// <inheritdoc />
        public override MicroProcessorOperationType Type =>
            MicroProcessorOperationType.MessageHandlerOperation;

        #endregion

        #region [====== IMicroProcessorOperation<HandleAsyncResult> ======]

        /// <inheritdoc />
        public AsyncMethodOperation<MessageHandlerOperationResult> ToAsyncMethodOperation() =>
            new AsyncMethodOperation<MessageHandlerOperationResult>(this);        

        #endregion
    }
}
