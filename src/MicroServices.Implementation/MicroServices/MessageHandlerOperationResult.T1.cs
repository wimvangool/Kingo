using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents the result of a <see cref="IMessageHandler{TMessage}"/> operation executed by a <see cref="IMicroProcessor" />.
    /// </summary>
    /// <typeparam name="TMessage">Type of the input-message.</typeparam>
    public sealed class MessageHandlerOperationResult<TMessage> : MessageHandlerOperationResult
    {
        private readonly Message<TMessage> _input;
        private readonly Message<object>[] _output;
        private readonly int _messageHandlerCount;

        internal MessageHandlerOperationResult(Message<TMessage> input, IEnumerable<Message<object>> output, int messageHandlerCount = 0) :
            this(input, output.Select(message => message.CorrelateWith(input)).ToArray(), messageHandlerCount) { }

        private MessageHandlerOperationResult(Message<TMessage> input, Message<object>[] output, int messageHandlerCount)
        {
            _input = input;
            _output = output;
            _messageHandlerCount = messageHandlerCount;
        }

        internal MessageHandlerOperationResult<TOther> ConvertTo<TOther>() =>
            new MessageHandlerOperationResult<TOther>(_input.ConvertTo<TOther>(), _output, _messageHandlerCount);

        /// <inheritdoc />
        public override string ToString() =>
            $"{typeof(TMessage).FriendlyName()} -> {_output.Length} message(s)";

        #region [====== IReadOnlyCollection<Message<object>> ======]

        internal override IEnumerator<Message<object>> GetEnumerator() =>
            _output.AsEnumerable().GetEnumerator();

        #endregion

        #region [====== Input & Output ======]

        /// <summary>
        /// The command or event that was processed.
        /// </summary>
        public IMessage<TMessage> Input =>
            _input;

        /// <inheritdoc />
        public override IReadOnlyList<IMessage> Output =>
            _output;

        /// <inheritdoc />
        public override int MessageHandlerCount =>
            _messageHandlerCount;

        #endregion

        #region [====== Validate & Append ======]

        internal MessageHandlerOperationResult<TMessage> Validate(IServiceProvider serviceProvider) =>
            new MessageHandlerOperationResult<TMessage>(_input, _output.Select(message => message.Validate(serviceProvider)), _messageHandlerCount);

        internal MessageHandlerOperationResult<TMessage> Append(MessageHandlerOperationResult result) =>
            result.AppendTo(this);

        internal override MessageHandlerOperationResult<TOther> AppendTo<TOther>(MessageHandlerOperationResult<TOther> result)
        {
            var input = result._input;
            var output = result._output.Concat(CorrelateOutputWith(result._input)).ToArray();
            var messageHandlerCount = result._messageHandlerCount + _messageHandlerCount;
            return new MessageHandlerOperationResult<TOther>(input, output, messageHandlerCount);
        }

        private IEnumerable<Message<object>> CorrelateOutputWith(IMessage input)
        {
            // The output of the current (nested) result is only (re)correlated with the specified
            // input-message if this result is the result of an internal operation.
            if (_input.Direction == MessageDirection.Internal)
            {
                return _output.Select(message => message.CorrelateWith(input));
            }
            return _output;
        }

        #endregion
    }
}
