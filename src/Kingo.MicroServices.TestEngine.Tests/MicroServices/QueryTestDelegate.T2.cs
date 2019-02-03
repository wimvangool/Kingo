using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class QueryTestDelegate<TRequest, TResponse> : QueryTest<TRequest, TResponse>
        where TRequest : new()
    {
        private readonly GivenStatementCollection _givenStatements;

        public QueryTestDelegate()
        {
            _givenStatements = new GivenStatementCollection();
            _whenStatement = (processor, testContext) => processor.ExecuteAsync(new TRequest(), (request, context) => Task.FromResult(default(TResponse)));
            _thenStatement = (request, result, testContext) => result.IsResponse(response => { });
        }

        #region [====== Given ======]

        public QueryTestDelegate<TRequest, TResponse> Given(Func<IMessageHandlerTestProcessor, MicroProcessorTestContext, Task> givenStatement)
        {
            _givenStatements.Given(givenStatement);
            return this;
        }

        protected override Task GivenAsync(IMessageHandlerTestProcessor processor, MicroProcessorTestContext context) =>
            _givenStatements.GivenAsync(processor, context);

        #endregion

        #region [====== When ======]

        private Func<IQueryProcessor<TRequest, TResponse>, MicroProcessorTestContext, Task> _whenStatement;

        public QueryTestDelegate<TRequest, TResponse> When(Func<IQueryProcessor<TRequest, TResponse>, MicroProcessorTestContext, Task> whenStatement)
        {
            _whenStatement = whenStatement ?? throw new ArgumentNullException(nameof(whenStatement));
            return this;
        }

        protected override Task WhenAsync(IQueryProcessor<TRequest, TResponse> processor, MicroProcessorTestContext context) =>
            _whenStatement.Invoke(processor, context);

        #endregion

        #region [====== Then ======]

        private Action<TRequest, IQueryResult<TResponse>, MicroProcessorTestContext> _thenStatement;

        public QueryTestDelegate<TRequest, TResponse> Then(Action<TRequest, IQueryResult<TResponse>, MicroProcessorTestContext> thenStatement)        
        {
            _thenStatement = thenStatement ?? throw new ArgumentNullException(nameof(thenStatement));
            return this;
        }

        protected override void Then(TRequest request, IQueryResult<TResponse> result, MicroProcessorTestContext context) =>
            _thenStatement.Invoke(request, result, context);

        #endregion
    }
}
