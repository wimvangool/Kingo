using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Serves as a base-class for all test's that handle a message and return the resulting message-stream.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that is handled by this test.</typeparam>
    /// <typeparam name="TMessageStream">Type of the resulting message-stream.</typeparam>
    public abstract class HandleMessageTest<TMessage, TMessageStream> : MicroProcessorOperationTest, IHandleMessageTest<TMessage, TMessageStream>
        where TMessageStream : MessageStream
    {
        Task IHandleMessageTest<TMessage, TMessageStream>.WhenAsync(IMessageProcessor<TMessage> processor, MicroProcessorOperationTestContext context) =>
            WhenAsync(processor, context);

        /// <summary>
        /// Executes this test by handling a specific message using the specified <paramref name="processor"/>.
        /// </summary>
        /// <param name="processor">The processor to handle the message with.</param>
        /// <param name="context">The context in which the test is running.</param> 
        protected abstract Task WhenAsync(IMessageProcessor<TMessage> processor, MicroProcessorOperationTestContext context);

        void IHandleMessageTest<TMessage, TMessageStream>.Then(TMessage message, IHandleMessageResult<TMessageStream> result, MicroProcessorOperationTestContext context) =>
            Then(message, result, context);

        /// <summary>
        /// Verifies the <paramref name="result"/> of this test.
        /// </summary>
        /// <param name="message">The message that was handled by this test.</param>        
        /// <param name="result">The result of this test.</param>
        /// <param name="context">The context in which the test is running.</param>                
        protected abstract void Then(TMessage message, IHandleMessageResult<TMessageStream> result, MicroProcessorOperationTestContext context);
    }
}
