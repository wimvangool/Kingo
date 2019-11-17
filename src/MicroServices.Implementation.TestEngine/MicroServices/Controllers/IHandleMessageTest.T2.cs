using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// When implemented by a class, represents a test that handles a specific message with a <see cref="IMicroProcessor" />
    /// and produces a set of events as a result.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that is handled by this test.</typeparam>
    /// <typeparam name="TOutputStream">Type of the output-stream that is produced by this test.</typeparam>
    public interface IHandleMessageTest<TMessage, out TOutputStream> : IMicroProcessorOperationTest
        where TOutputStream : MessageStream
    {
        /// <summary>
        /// Executes this test by handling a specific message using the specified <paramref name="processor"/>.
        /// </summary>
        /// <param name="processor">The processor to handle the message with.</param>
        /// <param name="context">The context in which the test is running.</param>        
        Task WhenAsync(IMessageProcessor<TMessage> processor, MicroProcessorOperationTestContext context);

        /// <summary>
        /// Verifies the <paramref name="result"/> of this test.
        /// </summary>
        /// <param name="message">The message that was handled by this test.</param>        
        /// <param name="result">The result of this test.</param>
        /// <param name="context">The context in which the test is running.</param>                
        void Then(TMessage message, IHandleMessageResult<TOutputStream> result, MicroProcessorOperationTestContext context);
    }
}
