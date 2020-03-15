using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    internal static class AsyncMethodOperationExtensions
    {
        public static MicroProcessorOperationStackTrace CaptureStackTrace(this IAsyncMethodOperation operation) =>
            operation.Context.StackTrace.ToMicroProcessorOperationStackTrace();
    }
}
