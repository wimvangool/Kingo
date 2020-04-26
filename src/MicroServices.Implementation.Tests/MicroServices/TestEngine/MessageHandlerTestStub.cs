using System;
using Kingo.MicroServices.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.TestEngine
{
    public sealed class MessageHandlerTestStub : MessageHandlerTest, IMicroProcessorTestStub
    {
        #region [====== IMicroProcessorTestStub ======]

        public new IGivenState Given() =>
            base.Given();

        public new IWhenCommandState<TCommand> WhenCommand<TCommand>() =>
            base.WhenCommand<TCommand>();

        public new IWhenEventState<TEvent> WhenEvent<TEvent>() =>
            base.WhenEvent<TEvent>();

        #endregion

        protected override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            return services.AddMicroProcessor(processor =>
            {
                processor.MessageHandlers.Add<NullHandler>();
            });
        }
    }
}
