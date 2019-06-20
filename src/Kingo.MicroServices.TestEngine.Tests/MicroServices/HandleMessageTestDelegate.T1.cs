using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class HandleMessageTestDelegate<TEventStream> : HandleMessageTest<object, TEventStream>
        where TEventStream : EventStream, new()
    {
        private readonly GivenStatementCollection _givenStatements;

        public HandleMessageTestDelegate()
        {
            _givenStatements = new GivenStatementCollection();
            _whenStatement = (messageProcessor, testContext) => messageProcessor.ExecuteCommandAsync((message, context) => { }, new object()); ;
            _thenStatement = (message, result, context) => result.IsEventStream(stream => new TEventStream());
        }

        #region [====== Given ======]

        public HandleMessageTestDelegate<TEventStream> Given(Func<IHandleMessageOperationTestProcessor, MicroProcessorOperationTestContext, Task> givenStatement)
        {
            _givenStatements.Given(givenStatement);
            return this;
        }

        protected override Task GivenAsync(IHandleMessageOperationTestProcessor processor, MicroProcessorOperationTestContext context) =>
            _givenStatements.GivenAsync(processor, context);

        #endregion

        #region [====== When ======]

        private Func<IMessageProcessor<object>, MicroProcessorOperationTestContext, Task> _whenStatement;

        public HandleMessageTestDelegate<TEventStream> When(Func<IMessageProcessor<object>, MicroProcessorOperationTestContext, Task> whenStatement)
        {
            _whenStatement = whenStatement ?? throw new ArgumentNullException(nameof(whenStatement));
            return this;
        }

        protected override Task WhenAsync(IMessageProcessor<object> processor, MicroProcessorOperationTestContext context) =>
            _whenStatement.Invoke(processor, context);

        #endregion

        #region [====== Then ======]

        private Action<object, IHandleMessageResult<TEventStream>, MicroProcessorOperationTestContext> _thenStatement;

        public HandleMessageTestDelegate<TEventStream> Then(Action<object, IHandleMessageResult<TEventStream>, MicroProcessorOperationTestContext> thenStatement)
        {
            _thenStatement = thenStatement ?? throw new ArgumentNullException(nameof(thenStatement));
            return this;
        }

        protected override void Then(object message, IHandleMessageResult<TEventStream> result, MicroProcessorOperationTestContext context) =>
            _thenStatement.Invoke(message, result, context);

        #endregion
    }
}
