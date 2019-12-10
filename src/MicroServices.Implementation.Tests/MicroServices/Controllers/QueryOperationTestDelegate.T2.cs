using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    internal sealed class QueryOperationTestDelegate<TRequest, TResponse> : QueryOperationTest<TRequest, TResponse>
        where TRequest : new()
    {
        private readonly GivenStatementCollection _givenStatements;

        public QueryOperationTestDelegate(TResponse defaultResponse = default)
        {
            _givenStatements = new GivenStatementCollection();
            _whenStatement = (processor, testContext) => processor.ExecuteAsync((request, context) => Task.FromResult(defaultResponse), new TRequest());
            _thenStatement = (request, result, testContext) => result.IsResponse(response => { });
        }

        #region [====== Given ======]

        public QueryOperationTestDelegate<TRequest, TResponse> Given(Func<IMicroProcessorOperationRunner, MicroProcessorOperationTestContext, Task> givenStatement)
        {
            _givenStatements.Given(givenStatement);
            return this;
        }

        protected override Task GivenAsync(IMicroProcessorOperationRunner processor, MicroProcessorOperationTestContext context) =>
            _givenStatements.GivenAsync(processor, context);

        #endregion

        #region [====== When ======]

        private Func<IQueryOperationRunner<TRequest, TResponse>, MicroProcessorOperationTestContext, Task> _whenStatement;

        public QueryOperationTestDelegate<TRequest, TResponse> When(Func<IQueryOperationRunner<TRequest, TResponse>, MicroProcessorOperationTestContext, Task> whenStatement)
        {
            _whenStatement = whenStatement ?? throw new ArgumentNullException(nameof(whenStatement));
            return this;
        }

        protected override Task WhenAsync(IQueryOperationRunner<TRequest, TResponse> processor, MicroProcessorOperationTestContext context) =>
            _whenStatement.Invoke(processor, context);

        #endregion

        #region [====== Then ======]

        private Action<TRequest, IQueryOperationTestResult<TResponse>, MicroProcessorOperationTestContext> _thenStatement;

        public QueryOperationTestDelegate<TRequest, TResponse> Then(Action<TRequest, IQueryOperationTestResult<TResponse>, MicroProcessorOperationTestContext> thenStatement)        
        {
            _thenStatement = thenStatement ?? throw new ArgumentNullException(nameof(thenStatement));
            return this;
        }

        protected override void Then(TRequest request, IQueryOperationTestResult<TResponse> result, MicroProcessorOperationTestContext context) =>
            _thenStatement.Invoke(request, result, context);

        #endregion
    }
}
