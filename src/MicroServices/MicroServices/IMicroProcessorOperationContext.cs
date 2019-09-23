using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents the context of a <see cref="IMicroProcessor" /> operation.
    /// </summary>
    public interface IMicroProcessorOperationContext
    {
        /// <summary>
        /// Gets the user that is executing the current operation.
        /// </summary>
        ClaimsPrincipal User
        {
            get;
        }

        /// <summary>
        /// Returns a stack trace of all operations that are currently being executed.
        /// </summary>
        IAsyncMethodOperationStackTrace StackTrace
        {
            get;
        }

        /// <summary>
        /// Returns the <see cref="IServiceProvider" /> of this context.
        /// </summary>
        IServiceProvider ServiceProvider
        {
            get;
        }

        /// <summary>
        /// Returns the processor that can be used to execute (sub)queries during an operation.
        /// </summary>
        IQueryProcessor QueryProcessor
        {
            get;
        }
    }
}
