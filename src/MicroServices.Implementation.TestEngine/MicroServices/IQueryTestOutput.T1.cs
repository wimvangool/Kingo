using System;

namespace Kingo.MicroServices
{
    internal interface IQueryTestOutput<out TResponse>
    {
        ITestOutputAssertMethod IsException<TException>(Action<TException, MicroProcessorTestContext> assertMethod) where TException : MicroProcessorOperationException;

        ITestOutputAssertMethod IsResponse(Action<TResponse, MicroProcessorTestContext> assertMethod);
    }
}
