using System;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.TestEngine
{
    public sealed class MessageHandlerTestStub : MessageHandlerTest, IMicroProcessorTestStub
    {
        #region [====== IMicroProcessorTestStub ======]

        public new IGivenState Given() =>
            base.Given();

        public new IGivenCommandOrEventState<TMessage> Given<TMessage>() =>
            base.Given<TMessage>();

        public new IWhenCommandOrEventState<TMessage> When<TMessage>() =>
            base.When<TMessage>();

        #endregion
    }
}
