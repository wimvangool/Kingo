using Kingo.MicroServices.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.TestEngine
{
    public sealed class BusinessLogicTestStub : BusinessLogicTest, IMicroProcessorTestStub
    {
        #region [====== IMicroProcessorTestStub ======]

        public new IGivenState Given() =>
            base.Given();

        public new IWhenBusinessLogicTestState When() =>
            base.When();

        #endregion

        protected override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            return services.AddMicroProcessor(processor =>
            {
                processor.ConfigureMessageHandlers(messageHandlers =>
                {
                    messageHandlers.Add<NullHandler>();
                });
            });
        }
    }
}
