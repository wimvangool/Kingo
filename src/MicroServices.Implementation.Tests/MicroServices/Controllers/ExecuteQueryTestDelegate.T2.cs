using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    internal sealed class ExecuteQueryTestDelegate<TRequest, TResponse> : ExecuteQueryTest<TRequest, TResponse>
        where TRequest : new()
    {
        private readonly GivenStatementCollection _givenStatements;

        public ExecuteQueryTestDelegate()
        {
            _givenStatements = new GivenStatementCollection();
            _whenStatement = (processor, testContext) => processor.ExecuteAsync((request, context) => Task.FromResult(default(TResponse)), new TRequest());
            _thenStatement = (request, result, testContext) => result.IsResponse(response => { });
        }

        #region [====== Given ======]

        public ExecuteQueryTestDelegate<TRequest, TResponse> Given(Func<IHandleMessageOperationTestProcessor, MicroProcessorOperationTestContext, Task> givenStatement)
        {
            _givenStatements.Given(givenStatement);
            return this;
        }

        protected override Task GivenAsync(IHandleMessageOperationTestProcessor processor, MicroProcessorOperationTestContext context) =>
            _givenStatements.GivenAsync(processor, context);

        #endregion

        #region [====== When ======]

        private Func<IQueryProcessor<TRequest, TResponse>, MicroProcessorOperationTestContext, Task> _whenStatement;

        public ExecuteQueryTestDelegate<TRequest, TResponse> When(Func<IQueryProcessor<TRequest, TResponse>, MicroProcessorOperationTestContext, Task> whenStatement)
        {
            _whenStatement = whenStatement ?? throw new ArgumentNullException(nameof(whenStatement));
            return this;
        }

        protected override Task WhenAsync(IQueryProcessor<TRequest, TResponse> processor, MicroProcessorOperationTestContext context) =>
            _whenStatement.Invoke(processor, context);

        #endregion

        #region [====== Then ======]

        private Action<TRequest, IExecuteQueryResult<TResponse>, MicroProcessorOperationTestContext> _thenStatement;

        public ExecuteQueryTestDelegate<TRequest, TResponse> Then(Action<TRequest, IExecuteQueryResult<TResponse>, MicroProcessorOperationTestContext> thenStatement)        
        {
            _thenStatement = thenStatement ?? throw new ArgumentNullException(nameof(thenStatement));
            return this;
        }

        protected override void Then(TRequest request, IExecuteQueryResult<TResponse> result, MicroProcessorOperationTestContext context) =>
            _thenStatement.Invoke(request, result, context);

        #endregion
    }
}
