using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    internal sealed class QueryOperationTestDelegate<TResponse> : QueryOperationTest<TResponse>
    {
        private readonly GivenStatementCollection _givenStatements;

        public QueryOperationTestDelegate(TResponse defaultResponse = default)
        {
            _givenStatements = new GivenStatementCollection();
            _whenStatement = (processor, testContext) => processor.ExecuteAsync(context => Task.FromResult(defaultResponse));
            _thenStatement = (result, testContext) => result.IsResponse(response => { });
        }

        #region [====== Given ======]

        public QueryOperationTestDelegate<TResponse> Given(Func<IMicroProcessorOperationRunner, MicroProcessorOperationTestContext, Task> givenStatement)
        {
            _givenStatements.Given(givenStatement);
            return this;
        }

        protected override Task GivenAsync(IMicroProcessorOperationRunner processor, MicroProcessorOperationTestContext context) =>
            _givenStatements.GivenAsync(processor, context);

        #endregion

        #region [====== When ======]

        private Func<IQueryOperationRunner<TResponse>, MicroProcessorOperationTestContext, Task> _whenStatement;

        public QueryOperationTestDelegate<TResponse> When(Func<IQueryOperationRunner<TResponse>, MicroProcessorOperationTestContext, Task> whenStatement)
        {
            _whenStatement = whenStatement ?? throw new ArgumentNullException(nameof(whenStatement));
            return this;
        }

        protected override Task WhenAsync(IQueryOperationRunner<TResponse> processor, MicroProcessorOperationTestContext context) =>
            _whenStatement.Invoke(processor, context);

        #endregion

        #region [====== Then ======]

        private Action<IQueryOperationTestResult<TResponse>, MicroProcessorOperationTestContext> _thenStatement;

        public QueryOperationTestDelegate<TResponse> Then(Action<IQueryOperationTestResult<TResponse>, MicroProcessorOperationTestContext> thenStatement)        
        {
            _thenStatement = thenStatement ?? throw new ArgumentNullException(nameof(thenStatement));
            return this;
        }

        protected override void Then(IQueryOperationTestResult<TResponse> result, MicroProcessorOperationTestContext context) =>
            _thenStatement.Invoke(result, context);

        #endregion
    }
}
