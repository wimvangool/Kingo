using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Contains extension methods for instance of type <see cref="IAsyncMethodOperationStackTrace" />.
    /// </summary>
    public static class AsyncMethodOperationStackTraceExtensions
    {
        /// <summary>
        /// Converts the stack-trace into a <see cref="MicroProcessorOperationStackTrace" /> that can be used to throw
        /// a specific type of <see cref="MicroProcessorOperationException" />.
        /// </summary>
        /// <param name="stackTrace">The stack-trace to convert.</param>
        /// <returns>A new <see cref="MicroProcessorOperationStackTrace"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stackTrace"/> is <c>null</c>.
        /// </exception>
        public static MicroProcessorOperationStackTrace ToMicroProcessorOperationStackTrace(this IAsyncMethodOperationStackTrace stackTrace) =>
            new MicroProcessorOperationStackTrace(NotNull(stackTrace).Select(ToMicroProcessorOperationStackItem));

        private static MicroProcessorOperationStackItem ToMicroProcessorOperationStackItem(IAsyncMethodOperation operation) =>
            new MicroProcessorOperationStackItem(operation.Method.ComponentType, operation.Message);

        private static IAsyncMethodOperationStackTrace NotNull(IAsyncMethodOperationStackTrace stackTrace) =>
            stackTrace ?? throw new ArgumentNullException(nameof(stackTrace));
    }
}
