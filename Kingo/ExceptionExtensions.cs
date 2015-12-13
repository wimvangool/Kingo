using System;
using System.Runtime.ExceptionServices;

namespace Kingo
{
    /// <summary>
    /// Contains extenion methods for the <see cref="Exception" /> class.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Rethrows the specified <paramref name="exception"/> without losing its stacktrace.
        /// </summary>
        /// <param name="exception">The exception to rethrow.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="exception"/> is <c>null</c>.
        /// </exception>
        public static void Rethrow(this Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }
            ExceptionDispatchInfo.Capture(exception).Throw();
        }
    }
}
