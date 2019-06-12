using Kingo.MicroServices.Endpoints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public abstract class MicroProcessorTest
    {
        private readonly MicroProcessorBuilder<MicroProcessor> _builder;

        protected MicroProcessorTest()
        {
            _builder = new MicroProcessorBuilder<MicroProcessor>();
        }

        protected IMicroProcessorBuilder ProcessorBuilder =>
            _builder;

        protected IMicroProcessor CreateProcessor() =>
            _builder.BuildServiceCollection().BuildServiceProvider().GetRequiredService<IMicroProcessor>();
    }
}
