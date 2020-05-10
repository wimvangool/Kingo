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
        private readonly IMessage<TMessage> _input;

        internal MessageHandlerOperationResult(IMessageHandlerOperationResult result, IMessage<TMessage> input)
        {
            _result = result;
            _input = input;
        }

        /// <summary>
        /// The command or event that was processed.
        /// </summary>
        public IMessage<TMessage> Input =>
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
