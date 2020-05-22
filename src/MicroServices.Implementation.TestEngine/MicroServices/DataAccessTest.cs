using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Serves as a base-class for all tests that verify the behavior of a query.
    /// </summary>
    public abstract class DataAccessTest : MicroProcessorTest
    {
        /// <summary>
        /// Prepares the test-engine to execute a command or handle an event
        /// and verify the outcome of the operation.
        /// </summary>
        /// <returns>A <see cref="IWhenBusinessLogicTestState"/> that can be used to configure the operation.</returns>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        protected IWhenDataAccessTestState When() =>
            State.WhenDataAccessTest();
    }
}
