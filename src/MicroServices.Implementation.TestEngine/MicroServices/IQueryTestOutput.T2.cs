using System;

namespace Kingo.MicroServices
{
    internal interface IQueryTestOutput<out TRequest, out TResponse>
    {
        ITestOutputAssertMethod IsException<TException>(Action<TRequest, TException, MicroProcessorTestContext> assertMethod) where TException : MicroProcessorOperationException;

        ITestOutputAssertMethod IsResponse(Action<TRequest, TResponse, MicroProcessorTestContext> assertMethod);
    }
}
