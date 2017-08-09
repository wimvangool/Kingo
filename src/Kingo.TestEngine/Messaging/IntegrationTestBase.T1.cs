using System;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a scenario that is used to test a query.
    /// </summary>    
    /// <typeparam name="TMessageOut">Type of the response that is returned by the query.</typeparam>
    public abstract class IntegrationTestBase<TMessageOut> : Scenario<IIntegrationTestResult<TMessageOut>>
    {
        #region [====== Executable ======]

        private sealed class Executable : ScenarioResult<TMessageOut>, IIntegrationTestResult<TMessageOut>
        {
            private readonly IntegrationTestBase<TMessageOut> _scenario;

            public Executable(IntegrationTestBase<TMessageOut> scenario)
            {
                _scenario = scenario;
            }

            public async Task IsResponseAsync(Action<TMessageOut> assertCallback)
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
                _scenario.WhenQueryIsExecuted(_scenario.Processor);

            protected override Task TearDownScenarioAsync() =>
                _scenario.TearDownAsync();

            protected override Exception NewAssertFailedException(string message, Exception exception) =>
                _scenario.NewAssertFailedException(message, exception);            
        }

        #endregion

        internal override IIntegrationTestResult<TMessageOut> CreateResult() =>
            new Executable(this);

        #region [====== Given / When / Then ======]

        /// <summary>
        /// Executes the query to test and returns its result.
        /// </summary>
        /// <param name="processor">The processor to execute the query with.</param>
        /// <returns>The result of the query.</returns>
        protected abstract Task<TMessageOut> WhenQueryIsExecuted(IMicroProcessor processor);

        #endregion
    }
}
