using System.Collections.Generic;
using System.Linq;
using Kingo.MicroServices.Endpoints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public abstract class MicroProcessorTest
    {
        #region [===== InstanceCollector ======]

        protected interface IInstanceCollector
        {
            void Add(object instance);

            void AssertInstanceCountIs(int count);
        }

        private sealed class InstanceCollector : IInstanceCollector
        {
            private readonly List<object> _instances;

            public InstanceCollector()
            {
                _instances = new List<object>();
            }

            public void Add(object instance)
            {
                _instances.Add(instance);
            }

            public void AssertInstanceCountIs(int count) =>
                Assert.AreEqual(count, _instances.Distinct().Count());
        }

        #endregion

        private readonly MicroProcessorBuilder<MicroProcessor> _builder;
        private readonly IServiceCollection _services;

        protected MicroProcessorTest()
        {
            _builder = new MicroProcessorBuilder<MicroProcessor>();
            _services = new ServiceCollection();
            _services.AddSingleton<IInstanceCollector, InstanceCollector>();
        }

        protected IMicroProcessorBuilder ProcessorBuilder =>
            _builder;

        protected IServiceCollection Services =>
            _services;

        protected IMicroProcessor CreateProcessor() =>
            _builder.BuildServiceCollection(_services).BuildServiceProvider().GetRequiredService<IMicroProcessor>();
    }
}
