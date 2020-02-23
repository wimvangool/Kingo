using System;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.TestEngine
{
    public sealed class MessageHandlerTestStub : MessageHandlerTest, IMicroProcessorTestStub
    {
        #region [====== IMicroProcessorTestStub ======]

        public new IGivenState Given() =>
            base.Given();

        public new IWhenCommandOrEventState<TMessage> When<TMessage>() =>
            base.When<TMessage>();

        #endregion

        protected override IServiceCollection ConfigureServices(IServiceCollection services) =>
            throw new NotImplementedException();
    }
}
