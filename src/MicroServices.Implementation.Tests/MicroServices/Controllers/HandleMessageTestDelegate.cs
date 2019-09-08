using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    internal sealed class HandleMessageTestDelegate : HandleMessageTest<object>
    {
        private readonly GivenStatementCollection _givenStatements;

        public HandleMessageTestDelegate()
        {
            _givenStatements = new GivenStatementCollection();
            _whenStatement = (messageProcessor, testContext) => messageProcessor.ExecuteCommandAsync((message, context) => { }, new object());
            _thenStatement = (message, result, context) => result.IsMessageStream();
        }

        #region [====== Given ======]

        public HandleMessageTestDelegate Given(Func<IHandleMessageOperationTestProcessor, MicroProcessorOperationTestContext, Task> givenStatement)
        {
            _givenStatements.Given(givenStatement);
            return this;
        }

        protected override Task GivenAsync(IHandleMessageOperationTestProcessor processor, MicroProcessorOperationTestContext context) =>
            _givenStatements.GivenAsync(processor, context);

        #endregion

        #region [====== When ======]

        private Func<IMessageProcessor<object>, MicroProcessorOperationTestContext, Task> _whenStatement;

        public HandleMessageTestDelegate When(Func<IMessageProcessor<object>, MicroProcessorOperationTestContext, Task> whenStatement)
        {
            _whenStatement = whenStatement ?? throw new ArgumentNullException(nameof(whenStatement));
            return this;
        }

        protected override Task WhenAsync(IMessageProcessor<object> processor, MicroProcessorOperationTestContext context) =>
            _whenStatement.Invoke(processor, context);

        #endregion        

        #region [====== Then ======]

        private Action<object, IHandleMessageResult, MicroProcessorOperationTestContext> _thenStatement;        

        public HandleMessageTestDelegate Then(Action<object, IHandleMessageResult, MicroProcessorOperationTestContext> thenStatement)
        {
            _thenStatement = thenStatement ?? throw new ArgumentNullException(nameof(thenStatement));
            return this;
        }

        protected override void Then(object message, IHandleMessageResult result, MicroProcessorOperationTestContext context) =>
            _thenStatement.Invoke(message, result, context);

        #endregion
    }
}
