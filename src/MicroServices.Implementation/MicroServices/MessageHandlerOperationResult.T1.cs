using System.Collections.Generic;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents the result of a write-operation.
    /// </summary>
    /// <typeparam name="TMessage">Type of the input-message.</typeparam>
    public sealed class MessageHandlerOperationResult<TMessage> : IMessageHandlerOperationResult
    {
        private readonly IMessageHandlerOperationResult _result;
        private readonly Message<TMessage> _input;

        internal MessageHandlerOperationResult(IMessageHandlerOperationResult result, Message<TMessage> input)
        {
            _result = result;
            _input = input;
        }

        /// <summary>
        /// The command or event that was processed.
        /// </summary>
        public Message<TMessage> Input =>
            _input;

        /// <inheritdoc />
        public IReadOnlyList<IMessage> Output =>
            _result.Output;

        /// <inheritdoc />
        public int MessageHandlerCount =>
            _result.MessageHandlerCount;

        /// <inheritdoc />
        public override string ToString() =>
            $"{typeof(TMessage).FriendlyName()} -> {_result}";
    }
}
