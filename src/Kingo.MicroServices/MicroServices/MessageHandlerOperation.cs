using System.Threading;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents an operation where a <see cref="MicroProcessor"/> invokes one or more message handlers to handle a message.
    /// </summary>
    public abstract class MessageHandlerOperation : MicroProcessorOperation<MessageHandlerOperationResult>
    {        
        internal MessageHandlerOperation(CancellationToken? token)
        {            
            Token = token ?? CancellationToken.None;
        }        

        /// <inheritdoc />
        public override CancellationToken Token
        {
            get;
        }

        /// <inheritdoc />
        public override MicroProcessorOperationType Type =>
            MicroProcessorOperationType.MessageHandlerOperation;
    }
}
