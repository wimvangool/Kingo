using Kingo.MicroServices.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.TestEngine
{
    public sealed class DataAccessTestStub : DataAccessTest, IMicroProcessorTestStub
    {
        #region [====== IMicroProcessorTestStub ======]

        public new IGivenState Given() =>
            base.Given();

        public new IWhenDataAccessTestState When() =>
            base.When();

        #endregion

        protected override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            return services.AddMicroProcessor(processor =>
            {
                processor.ConfigureQueries(queries =>
                {
                    queries.Add<NullQuery>();
                });
            });
        }
    }
}
