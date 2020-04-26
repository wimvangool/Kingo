using System;

namespace Kingo.MicroServices.TestEngine
{
    internal interface IMessageHandlerTestOutput<out TMessage>
    {
        ITestOutputAssertMethod IsException<TException>(Action<TMessage, TException, MicroProcessorTestContext> assertMethod) where TException : MicroProcessorOperationException;

        ITestOutputAssertMethod IsMessageStream(Action<TMessage, MessageStream, MicroProcessorTestContext> assertMethod);
    }
}
