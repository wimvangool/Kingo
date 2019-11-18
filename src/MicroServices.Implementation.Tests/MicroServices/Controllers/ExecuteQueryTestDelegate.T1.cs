using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    internal sealed class ExecuteQueryTestDelegate<TResponse> : ExecuteQueryTest<TResponse>
    {
        private readonly GivenStatementCollection _givenStatements;

        public ExecuteQueryTestDelegate(TResponse defaultResponse = default)
        {
            _givenStatements = new GivenStatementCollection();
            _whenStatement = (processor, testContext) => processor.ExecuteAsync(context => Task.FromResult(defaultResponse));
            _thenStatement = (result, testContext) => result.IsResponse(response => { });
        }

        #region [====== Given ======]

        public ExecuteQueryTestDelegate<TResponse> Given(Func<IHandleMessageOperationTestProcessor, MicroProcessorOperationTestContext, Task> givenStatement)
        {
            _givenStatements.Given(givenStatement);
            return this;
        }

        protected override Task GivenAsync(IHandleMessageOperationTestProcessor processor, MicroProcessorOperationTestContext context) =>
            _givenStatements.GivenAsync(processor, context);

        #endregion

        #region [====== When ======]

        private Func<IQueryProcessor<TResponse>, MicroProcessorOperationTestContext, Task> _whenStatement;

        public ExecuteQueryTestDelegate<TResponse> When(Func<IQueryProcessor<TResponse>, MicroProcessorOperationTestContext, Task> whenStatement)
        {
            _whenStatement = whenStatement ?? throw new ArgumentNullException(nameof(whenStatement));
            return this;
        }

        protected override Task WhenAsync(IQueryProcessor<TResponse> processor, MicroProcessorOperationTestContext context) =>
            _whenStatement.Invoke(processor, context);

        #endregion

        #region [====== Then ======]

        private Action<IExecuteQueryResult<TResponse>, MicroProcessorOperationTestContext> _thenStatement;

        public ExecuteQueryTestDelegate<TResponse> Then(Action<IExecuteQueryResult<TResponse>, MicroProcessorOperationTestContext> thenStatement)        
        {
            _thenStatement = thenStatement ?? throw new ArgumentNullException(nameof(thenStatement));
            return this;
        }

        protected override void Then(IExecuteQueryResult<TResponse> result, MicroProcessorOperationTestContext context) =>
            _thenStatement.Invoke(result, context);

        #endregion
    }
}
