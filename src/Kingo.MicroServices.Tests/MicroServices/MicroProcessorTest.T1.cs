using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kingo.MicroServices.Endpoints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public abstract class MicroProcessorTest<TProcessor>
        where TProcessor : class, IMicroProcessor
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

        private readonly MicroProcessorBuilder<TProcessor> _builder;
        private readonly IServiceCollection _services;

        protected MicroProcessorTest()
        {
            _builder = new MicroProcessorBuilder<TProcessor>();
            _services = new ServiceCollection();
            _services.AddSingleton<IInstanceCollector, InstanceCollector>();
        }

        protected IMicroProcessorBuilder ProcessorBuilder =>
            _builder;

        protected IServiceCollection Services =>
            _services;

        protected virtual IMicroProcessor CreateProcessor() =>
            _builder.BuildServiceCollection(_services).BuildServiceProvider().GetRequiredService<IMicroProcessor>();

        [TestInitialize]
        public virtual void Setup() { }           
    }
}
