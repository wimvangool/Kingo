using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a test that either handles a message or executes a query through a <see cref="IMicroProcessor"/>.
    /// </summary>
    /// <typeparam name="TMessageOrQuery">Type of the message or query to execute.</typeparam>
    public interface IMicroProcessorTest<out TMessageOrQuery>
    {
        /// <summary>
        /// Prepares the test for execution.
        /// </summary>
        /// <param name="context">The context in which the test is running.</param>                
        Task GivenAsync(MicroProcessorTestContext context);

        /// <summary>
        /// Creates and returns the message to handle to query to execute.
        /// </summary>
        /// <param name="context">The context in which the test is running.</param>                
        /// <returns>The message to handle or query to execute.</returns>
        TMessageOrQuery When(MicroProcessorTestContext context);
    }
}
