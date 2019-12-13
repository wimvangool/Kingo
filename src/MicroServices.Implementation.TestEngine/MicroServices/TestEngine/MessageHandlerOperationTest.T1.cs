using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// Serves as a base-class for all test's that handle a message and return the resulting message-stream.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that is handled by this test.</typeparam>
    public abstract class MessageHandlerOperationTest<TMessage> : MicroProcessorOperationTest, IMessageHandlerOperationTest<TMessage>
    {
        private readonly MessageHandlerOperation<TMessage> _operation;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerOperationTest{TMessage}" /> class.
        /// </summary>
        protected MessageHandlerOperationTest() :
            this(new MessageHandlerOperation<TMessage>()) { }

        internal MessageHandlerOperationTest(MessageHandlerOperation<TMessage> operation)
        {
            _operation = operation;
        }

        MessageHandlerOperation<TMessage> IMessageHandlerOperationTest<TMessage>.Operation =>
            _operation;

        #region [====== IMessageHandlerOperationTest<TMessage> ======]

        public MessageEnvelope<TMessage> GetInputMessage(MicroProcessorOperationTestContext context) =>
            _operation.GetInputMessage(context);

        public MessageStream GetOutputStream(MicroProcessorOperationTestContext context) =>
            _operation.GetOutputStream(context);

        #endregion

        #region [====== WhenAsync ======]

        Task IMessageHandlerOperationTest<TMessage>.WhenAsync(IMessageHandlerOperationRunner<TMessage> runner, MicroProcessorOperationTestContext context) =>
            WhenAsync(runner, context);

        /// <summary>
        /// Executes this test by handling a specific message using the specified <paramref name="runner"/>.
        /// </summary>
        /// <param name="runner">The processor to handle the message with.</param>
        /// <param name="context">The context in which the test is running.</param> 
        protected abstract Task WhenAsync(IMessageHandlerOperationRunner<TMessage> runner, MicroProcessorOperationTestContext context);

        #endregion

        #region [====== Then ======]

        void IMessageHandlerOperationTest<TMessage>.Then(TMessage message, IMessageHandlerOperationTestResult result, MicroProcessorOperationTestContext context) =>
            Then(message, result, context);

        /// <summary>
        /// Verifies the <paramref name="result"/> of this test.
        /// </summary>
        /// <param name="message">The message that was handled by this test.</param>        
        /// <param name="result">The result of this test.</param>
        /// <param name="context">The context in which the test is running.</param>                
        protected abstract void Then(TMessage message, IMessageHandlerOperationTestResult result, MicroProcessorOperationTestContext context);

        #endregion
    }
}
