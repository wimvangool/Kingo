using System;
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

        protected override IServiceCollection ConfigureServices(IServiceCollection services) =>
            throw new NotImplementedException();
    }
}
