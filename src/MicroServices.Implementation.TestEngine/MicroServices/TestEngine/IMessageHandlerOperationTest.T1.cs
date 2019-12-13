using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// When implemented by a class, represents a test that handles a specific message with a <see cref="IMicroProcessor" />
    /// and produces a set of events as a result.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that is handled by this test.</typeparam>
    public interface IMessageHandlerOperationTest<TMessage> : IMicroProcessorOperationTest, IMessageHandlerOperation<TMessage>
    {
        /// <summary>
        /// Returns the operation that is tested.
        /// </summary>
        MessageHandlerOperation<TMessage> Operation
        {
            get;
        }

        /// <summary>
        /// Executes this test by handling a specific message using the specified <paramref name="runner"/>.
        /// </summary>
        /// <param name="runner">The processor to handle the message with.</param>
        /// <param name="context">The context in which the test is running.</param>        
        Task WhenAsync(IMessageHandlerOperationRunner<TMessage> runner, MicroProcessorOperationTestContext context);

        /// <summary>
        /// Verifies the <paramref name="result"/> of this test.
        /// </summary>
        /// <param name="message">The message that was handled by this test.</param>        
        /// <param name="result">The result of this test.</param>
        /// <param name="context">The context in which the test is running.</param>                
        void Then(TMessage message, IMessageHandlerOperationTestResult result, MicroProcessorOperationTestContext context);
    }
}
