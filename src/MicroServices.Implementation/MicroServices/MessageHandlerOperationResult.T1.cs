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

        internal MessageHandlerOperationResult(Message<TMessage> input, IEnumerable<Message<object>> output) :
            this(input, output.Select(message => message.CorrelateWith(input)).ToArray()) { }

        private MessageHandlerOperationResult(Message<TMessage> input, Message<object>[] output)
        {
            _input = input;
            _output = output;
        }

        internal MessageHandlerOperationResult<TOther> ConvertTo<TOther>() =>
            new MessageHandlerOperationResult<TOther>(_input.ConvertTo<TOther>(), _output);

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

        #endregion

        #region [====== Validate & Append ======]

        internal MessageHandlerOperationResult<TMessage> Validate(IServiceProvider serviceProvider) =>
            new MessageHandlerOperationResult<TMessage>(_input, _output.Select(message => message.Validate(serviceProvider)));

        internal MessageHandlerOperationResult<TMessage> Append(MessageHandlerOperationResult result) =>
            result.AppendTo(this);

        internal override MessageHandlerOperationResult<TOther> AppendTo<TOther>(MessageHandlerOperationResult<TOther> result)
        {
            var input = result._input;
            var output = result._output.Concat(CorrelateOutputWith(result._input)).ToArray();
            
            return new MessageHandlerOperationResult<TOther>(input, output);
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
