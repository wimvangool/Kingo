using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    internal sealed class MessageHandlerOperationTestDelegate<TEventStream> : MessageHandlerOperationTest<object, TEventStream>
        where TEventStream : MessageStream, new()
    {
        private readonly GivenStatementCollection _givenStatements;

        public MessageHandlerOperationTestDelegate()
        {
            _givenStatements = new GivenStatementCollection();
            _whenStatement = (messageProcessor, testContext) => messageProcessor.ExecuteCommandAsync((message, context) => { }, new object()); ;
            _thenStatement = (message, result, context) => result.IsMessageStream(stream => new TEventStream());
        }

        #region [====== Given ======]

        public MessageHandlerOperationTestDelegate<TEventStream> Given(Func<IMessageHandlerOperationTestProcessor, MicroProcessorOperationTestContext, Task> givenStatement)
        {
            _givenStatements.Given(givenStatement);
            return this;
        }

        protected override Task GivenAsync(IMessageHandlerOperationTestProcessor processor, MicroProcessorOperationTestContext context) =>
            _givenStatements.GivenAsync(processor, context);

        #endregion

        #region [====== When ======]

        private Func<IMessageProcessor<object>, MicroProcessorOperationTestContext, Task> _whenStatement;

        public MessageHandlerOperationTestDelegate<TEventStream> When(Func<IMessageProcessor<object>, MicroProcessorOperationTestContext, Task> whenStatement)
        {
            _whenStatement = whenStatement ?? throw new ArgumentNullException(nameof(whenStatement));
            return this;
        }

        protected override Task WhenAsync(IMessageProcessor<object> processor, MicroProcessorOperationTestContext context) =>
            _whenStatement.Invoke(processor, context);

        #endregion

        #region [====== Then ======]

        private Action<object, IMessageHandlerOperationTestResult<TEventStream>, MicroProcessorOperationTestContext> _thenStatement;

        public MessageHandlerOperationTestDelegate<TEventStream> Then(Action<object, IMessageHandlerOperationTestResult<TEventStream>, MicroProcessorOperationTestContext> thenStatement)
        {
            _thenStatement = thenStatement ?? throw new ArgumentNullException(nameof(thenStatement));
            return this;
        }

        protected override void Then(object message, IMessageHandlerOperationTestResult<TEventStream> result, MicroProcessorOperationTestContext context) =>
            _thenStatement.Invoke(message, result, context);

        #endregion
    }
}
