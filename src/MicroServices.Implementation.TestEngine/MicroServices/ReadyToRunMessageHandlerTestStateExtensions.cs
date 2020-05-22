using System;
using System.Threading.Tasks;
using Kingo.MicroServices.DataContracts;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Contains extensions methods for instances of type <see cref="IReadyToRunMessageHandlerTestState{TMessage}" />
    /// </summary>
    public static class ReadyToRunMessageHandlerTestStateExtensions
    {
        /// <summary>
        /// Runs the test and expects the operation to publish and/or send a bunch of messages.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        /// <exception cref="TestFailedException">
        /// The operation threw an exception or the stream produced by the operation is not empty.
        /// </exception>
        public static Task ThenOutputIsEmptyStream<TMessage>(this IReadyToRunMessageHandlerTestState<TMessage> state)
        {
            return NotNull(state).ThenOutputIsMessageStream((message, stream, context) =>
            {
                if (stream.Count > 0)
                {
                    throw NewStreamNotEmptyException(stream.Count);
                }
            });
        }

        private static Exception NewStreamNotEmptyException(int streamCount)
        {
            var messageFormat = ExceptionMessages.MicroProcessorTest_StreamNotEmpty;
            var message = string.Format(messageFormat, streamCount);
            return new TestFailedException(message);
        }

        private static IReadyToRunMessageHandlerTestState<TMessage> NotNull<TMessage>(IReadyToRunMessageHandlerTestState<TMessage> state) =>
            state ?? throw new ArgumentNullException(nameof(state));
    }
}
