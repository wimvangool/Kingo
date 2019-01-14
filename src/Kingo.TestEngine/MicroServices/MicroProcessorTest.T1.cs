using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Serves as a base-class for all test's.
    /// </summary>    
    public abstract class MicroProcessorTest<TMessageOrQuery> : IMicroProcessorTest<TMessageOrQuery>
    {
        Task IMicroProcessorTest<TMessageOrQuery>.GivenAsync(MicroProcessorTestContext context) =>
            GivenAsync(context);       

        /// <summary>
        /// Prepares this test for execution.
        /// </summary>
        /// <param name="context">The context in which the test is running.</param>        
        protected virtual Task GivenAsync(MicroProcessorTestContext context) =>
            Task.CompletedTask;

        TMessageOrQuery IMicroProcessorTest<TMessageOrQuery>.When(MicroProcessorTestContext context) =>
            When(context);

        /// <summary>
        /// Creates and returns the message to handle to query to execute.
        /// </summary>
        /// <param name="context">The context in which the test is running.</param>                
        /// <returns>The message to handle or query to execute.</returns>
        protected abstract TMessageOrQuery When(MicroProcessorTestContext context);
    }
}
