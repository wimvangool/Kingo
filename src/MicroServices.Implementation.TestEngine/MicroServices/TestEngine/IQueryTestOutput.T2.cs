﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices.TestEngine
{
    internal interface IQueryTestOutput<out TRequest, out TResponse>
    {
        ITestOutputAssertMethod IsException<TException>(Action<TRequest, TException, MicroProcessorTestContext> assertMethod) where TException : MicroProcessorOperationException;

        ITestOutputAssertMethod IsResponse(Action<TRequest, TResponse, MicroProcessorTestContext> assertMethod);
    }
}
