using System;
using Kingo.MicroServices.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.TestEngine
{
    public sealed class QueryTestStub : QueryTest, IMicroProcessorTestStub
    {
        #region [====== IMicroProcessorTestStub ======]

        public new IGivenState Given() =>
            base.Given();

        public new IWhenRequestState WhenRequest() =>
            base.WhenRequest();

        public new IWhenRequestState<TRequest> When<TRequest>() =>
            base.When<TRequest>();

        #endregion

        protected override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            return services.AddMicroProcessor(processor =>
            {
                processor.Queries.Add<NullQuery>();
            });
        }
    }
}
