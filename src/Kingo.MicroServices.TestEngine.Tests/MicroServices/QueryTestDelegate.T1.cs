using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class QueryTestDelegate<TResponse> : QueryTest<TResponse>
    {
        private readonly GivenStatementCollection _givenStatements;

        public QueryTestDelegate()
        {
            _givenStatements = new GivenStatementCollection();
            _whenStatement = (processor, testContext) => processor.ExecuteAsync(context => Task.FromResult(default(TResponse)));
            _thenStatement = (result, testContext) => result.IsResponse(response => { });
        }

        #region [====== Given ======]

        public QueryTestDelegate<TResponse> Given(Func<IMessageHandlerTestProcessor, MicroProcessorTestContext, Task> givenStatement)
        {
            _givenStatements.Given(givenStatement);
            return this;
        }

        protected override Task GivenAsync(IMessageHandlerTestProcessor processor, MicroProcessorTestContext context) =>
            _givenStatements.GivenAsync(processor, context);

        #endregion

        #region [====== When ======]

        private Func<IQueryProcessor<TResponse>, MicroProcessorTestContext, Task> _whenStatement;

        public QueryTestDelegate<TResponse> When(Func<IQueryProcessor<TResponse>, MicroProcessorTestContext, Task> whenStatement)
        {
            _whenStatement = whenStatement ?? throw new ArgumentNullException(nameof(whenStatement));
            return this;
        }

        protected override Task WhenAsync(IQueryProcessor<TResponse> processor, MicroProcessorTestContext context) =>
            _whenStatement.Invoke(processor, context);

        #endregion

        #region [====== Then ======]

        private Action<IQueryResult<TResponse>, MicroProcessorTestContext> _thenStatement;

        public QueryTestDelegate<TResponse> Then(Action<IQueryResult<TResponse>, MicroProcessorTestContext> thenStatement)        
        {
            _thenStatement = thenStatement ?? throw new ArgumentNullException(nameof(thenStatement));
            return this;
        }

        protected override void Then(IQueryResult<TResponse> result, MicroProcessorTestContext context) =>
            _thenStatement.Invoke(result, context);

        #endregion
    }
}
