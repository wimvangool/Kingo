using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices.TestEngine
{
    internal interface IMessageHandlerTestOutput<out TMessage>
    {
        void AssertOutputIsException<TException>(Action<TMessage, TException, MicroProcessorTestContext> assertMethod) where TException : MicroProcessorOperationException;

        void AssertOutputIsMessageStream(Action<TMessage, MessageStream, MicroProcessorTestContext> assertMethod);
    }
}
