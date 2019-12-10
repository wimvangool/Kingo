using System;
using System.Collections.Generic;
using System.Text;
using Kingo.Reflection;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Represents a message handler operation for a specific type of message.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that is handled by this operation.</typeparam>
    public sealed class MessageHandlerOperation<TMessage> : IMessageHandlerOperation<TMessage>
    {
        private readonly Guid _operationId;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerOperation" /> class.
        /// </summary>
        public MessageHandlerOperation()
        {
            _operationId = Guid.NewGuid();
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().FriendlyName()} ({_operationId})";

        internal MessageStream SaveResult(MicroProcessorOperationTestContext context, MessageHandlerOperationResult<TMessage> result) =>
            NotNull(context).SetResult(_operationId, result);

        /// <inheritdoc />
        public MessageEnvelope<TMessage> GetInputMessage(MicroProcessorOperationTestContext context) =>
            NotNull(context).GetInputMessage<TMessage>(_operationId);

        /// <inheritdoc />
        public MessageStream GetOutputStream(MicroProcessorOperationTestContext context) =>
            NotNull(context).GetOutputStream(_operationId);

        private static MicroProcessorOperationTestContext NotNull(MicroProcessorOperationTestContext context) =>
            context ?? throw new ArgumentNullException(nameof(context));
    }
}
