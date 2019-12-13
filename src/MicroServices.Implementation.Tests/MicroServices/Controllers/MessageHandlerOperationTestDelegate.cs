using System;
using System.Threading.Tasks;
using Kingo.MicroServices.TestEngine;

namespace Kingo.MicroServices.Controllers
{
    internal sealed class MessageHandlerOperationTestDelegate : MessageHandlerOperationTest<object>
    {
        private readonly GivenStatementCollection _givenStatements;

        public MessageHandlerOperationTestDelegate()
        {
            _givenStatements = new GivenStatementCollection();
            _whenStatement = (runner, testContext) => runner.ExecuteCommandAsync((message, context) => { }, new object());
            _thenStatement = (message, result, context) => result.IsEmptyStream();
        }

        #region [====== Given ======]

        public MessageHandlerOperationTestDelegate Given(Func<IMicroProcessorOperationRunner, MicroProcessorOperationTestContext, Task> givenStatement)
        {
            _givenStatements.Given(givenStatement);
            return this;
        }

        protected override Task GivenAsync(IMicroProcessorOperationRunner runner, MicroProcessorOperationTestContext context) =>
            _givenStatements.GivenAsync(runner, context);

        #endregion

        #region [====== When ======]

        private Func<IMessageHandlerOperationRunner<object>, MicroProcessorOperationTestContext, Task> _whenStatement;

        public MessageHandlerOperationTestDelegate When(Func<IMessageHandlerOperationRunner<object>, MicroProcessorOperationTestContext, Task> whenStatement)
        {
            _whenStatement = whenStatement ?? throw new ArgumentNullException(nameof(whenStatement));
            return this;
        }

        protected override Task WhenAsync(IMessageHandlerOperationRunner<object> runner, MicroProcessorOperationTestContext context) =>
            _whenStatement.Invoke(runner, context);

        #endregion        

        #region [====== Then ======]

        private Action<object, IMessageHandlerOperationTestResult, MicroProcessorOperationTestContext> _thenStatement;        

        public MessageHandlerOperationTestDelegate Then(Action<object, IMessageHandlerOperationTestResult, MicroProcessorOperationTestContext> thenStatement)
        {
            _thenStatement = thenStatement ?? throw new ArgumentNullException(nameof(thenStatement));
            return this;
        }

        protected override void Then(object message, IMessageHandlerOperationTestResult result, MicroProcessorOperationTestContext context) =>
            _thenStatement.Invoke(message, result, context);

        #endregion
    }
}
