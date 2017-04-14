using System;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a scenario that is used to test a query.
    /// </summary>    
    /// <typeparam name="TMessageOut">Type of the response that is returned by the query.</typeparam>
    public abstract class ReadScenario<TMessageOut> : Scenario<IReadScenarioResult<TMessageOut>>
    {
        #region [====== Executable ======]

        private sealed class Executable : ScenarioResult<TMessageOut>, IReadScenarioResult<TMessageOut>
        {
            private readonly ReadScenario<TMessageOut> _scenario;

            public Executable(ReadScenario<TMessageOut> scenario)
            {
                _scenario = scenario;
            }

            public async Task IsMessageAsync(Action<TMessageOut> assertCallback)
            {
                if (assertCallback == null)
                {
                    throw new ArgumentNullException(nameof(assertCallback));
                }
                await SetupScenarioAsync();

                try
                {
                    assertCallback.Invoke(await ExecuteScenarioAsync());
                }
                finally
                {
                    await TearDownScenarioAsync();
                }
            }            

            protected override Task SetupScenarioAsync() =>
                _scenario.SetupAsync();

            protected override Task<TMessageOut> ExecuteScenarioAsync() =>
                _scenario.ExecuteQuery(_scenario.Processor);

            protected override Task TearDownScenarioAsync() =>
                _scenario.TearDownAsync();

            protected override Exception NewAssertFailedException(string message, Exception exception) =>
                _scenario.NewAssertFailedException(message, exception);            
        }

        #endregion

        internal override IReadScenarioResult<TMessageOut> CreateResult() =>
            new Executable(this);

        #region [====== Given / When / Then ======]

        /// <summary>
        /// Executes the query to test and returns its result.
        /// </summary>
        /// <param name="processor">The processor to execute the query with.</param>
        /// <returns>The result of the query.</returns>
        protected abstract Task<TMessageOut> ExecuteQuery(IMicroProcessor processor);

        #endregion
    }
}
