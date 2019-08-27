using System;
using Kingo.MicroServices.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class MicroProcessorTest
    {
        #region [====== Processor Types ======]

        private abstract class AbstractProcessor : MicroProcessor
        {
            protected AbstractProcessor(IServiceProvider serviceProvider) :
                base(serviceProvider) { }
        }        

        [MicroProcessorComponent(ServiceLifetime.Scoped)]
        private sealed class ScopedProcessor : MicroProcessor
        {
            public ScopedProcessor(IServiceProvider serviceProvider) :
                base(serviceProvider) { }
        }

        [MicroProcessorComponent(ServiceLifetime.Singleton)]
        private sealed class SingletonProcessor : MicroProcessor
        {
            public SingletonProcessor(IServiceProvider serviceProvider) :
                base(serviceProvider) { }
        }

        #endregion

        #region [====== ResolveProcessor ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ResolveProcessor_Throws_IfProcessorIsAbstract()
        {
            var test = new MicroProcessorTest<AbstractProcessor>();

            test.BuildServiceProvider();
        }

        [TestMethod]
        public void ResolveProcessor_ReturnsNewInstanceEveryTime_IfLifetimeIsTransient()
        {
            var test = new MicroProcessorTest<MicroProcessor>();
            var provider = test.BuildServiceProvider();
            var instances = provider.GetRequiredService<IInstanceCollector>();

            using (var scope = provider.CreateScope())
            {
                AddProcessor(scope.ServiceProvider, instances);
                AddProcessor(scope.ServiceProvider, instances);
            }
            instances.AssertInstanceCountIs(2);

            using (var scope = provider.CreateScope())
            {
                AddProcessor(scope.ServiceProvider, instances);
                AddProcessor(scope.ServiceProvider, instances);
            }
            instances.AssertInstanceCountIs(4);
        }

        [TestMethod]
        public void ResolveProcessor_ReturnsNewInstancePerScope_IfLifetimeIsScoped()
        {
            var test = new MicroProcessorTest<ScopedProcessor>();
            var provider = test.BuildServiceProvider();
            var instances = provider.GetRequiredService<IInstanceCollector>();

            using (var scope = provider.CreateScope())
            {
                AddProcessor(scope.ServiceProvider, instances);
                AddProcessor(scope.ServiceProvider, instances);
            }
            instances.AssertInstanceCountIs(1);

            using (var scope = provider.CreateScope())
            {
                AddProcessor(scope.ServiceProvider, instances);
                AddProcessor(scope.ServiceProvider, instances);
            }
            instances.AssertInstanceCountIs(2);
        }

        [TestMethod]
        public void ResolveProcessor_ReturnsSameInstanceEveryTime_IfLifetimeIsSingleton()
        {
            var test = new MicroProcessorTest<SingletonProcessor>();
            var provider = test.BuildServiceProvider();
            var instances = provider.GetRequiredService<IInstanceCollector>();

            using (var scope = provider.CreateScope())
            {
                AddProcessor(scope.ServiceProvider, instances);
                AddProcessor(scope.ServiceProvider, instances);
            }
            instances.AssertInstanceCountIs(1);

            using (var scope = provider.CreateScope())
            {
                AddProcessor(scope.ServiceProvider, instances);
                AddProcessor(scope.ServiceProvider, instances);
            }
            instances.AssertInstanceCountIs(1);
        }

        #endregion        

        private static void AddProcessor(IServiceProvider provider, IInstanceCollector instances) =>
            instances.Add(provider.GetRequiredService<IMicroProcessor>());
    }
}
