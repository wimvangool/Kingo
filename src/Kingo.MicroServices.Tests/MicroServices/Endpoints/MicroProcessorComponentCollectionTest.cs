using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Endpoints
{
    [TestClass]
    public sealed class MicroProcessorComponentCollectionTest
    {
        #region [====== QueryTypes ======]

        private sealed class NonPublicQuery : IQuery<object>
        {
            public Task<object> ExecuteAsync(QueryContext context) =>
                Task.FromResult(new object());
        }

        public abstract class AbstractQuery : IQuery<object>
        {
            public abstract Task<object> ExecuteAsync(QueryContext context);
        }

        public sealed class GenericQuery<TResponse> : IQuery<TResponse>
        {
            public Task<TResponse> ExecuteAsync(QueryContext context) =>
                Task.FromResult<TResponse>(default);
        }

        public sealed class Query1 : IQuery<object>
        {
            public Task<object> ExecuteAsync(QueryContext context) =>
                Task.FromResult(new object());
        }

        public sealed class Query2 : IQuery<object, object>
        {
            public Task<object> ExecuteAsync(object message, QueryContext context) =>
                Task.FromResult(message);
        }

        public sealed class Query3 : IQuery<object>, IQuery<object, object>
        {
            public Task<object> ExecuteAsync(QueryContext context) =>
                Task.FromResult(new object());

            public Task<object> ExecuteAsync(object message, QueryContext context) =>
                Task.FromResult(message);
        }

        #endregion

        private readonly MicroProcessorComponentCollection _components;

        public MicroProcessorComponentCollectionTest()
        {
            _components = new MicroProcessorComponentCollection();
        }

        [TestMethod]
        public void AddQueries_AddsNoQueries_IfThereAreNoTypesToScan()
        {
            _components.AddQueries();

            AssertRegisteredQueries(0);
        }

        [TestMethod]
        public void AddQueries_AddsNoQueries_IfThereAreNoQueryTypesToAdd()
        {
            _components.AddTypes(typeof(object), typeof(int));
            _components.AddTypes(typeof(NonPublicQuery), typeof(AbstractQuery), typeof(GenericQuery<>));
            _components.AddQueries();

            AssertRegisteredQueries(0);
        }

        [TestMethod]
        public void AddQueries_AddsExpectedQueries_IfThereAreSomeQueryTypesToAdd()
        {            
            _components.AddTypes(typeof(object), typeof(int));
            _components.AddTypes(typeof(GenericQuery<object>), typeof(Query1), typeof(Query2), typeof(Query3));
            _components.AddQueries();

            AssertRegisteredQueries(5, services =>
            {
                AssertContainsMapping<IQuery<object>, GenericQuery<object>>(services);
                AssertContainsMapping<IQuery<object>, Query1>(services);
                AssertContainsMapping<IQuery<object, object>, Query2>(services);
                AssertContainsMapping<IQuery<object>, Query3>(services);
                AssertContainsMapping<IQuery<object, object>, Query3>(services);
            });
        }

        private void AssertRegisteredQueries(int count, Action<IServiceCollection> assertCallback = null)
        {            
            var services = BuildServiceCollection();

            // We decrement by one because the service collection always contains the IMessageHandlerFactory.
            Assert.AreEqual(count, services.Count - 1);

            assertCallback?.Invoke(services);
        }

        private static void AssertContainsMapping<TInterfaceType, TType>(IServiceCollection services) where TType : TInterfaceType =>
            AssertContainsMapping(services, typeof(TInterfaceType), typeof(TType));
        
        private static void AssertContainsMapping(IServiceCollection services, Type interfaceType, Type type) =>
            Assert.IsTrue(services.Any(service => service.ServiceType == interfaceType && service.ImplementationType == type));

        private IServiceCollection BuildServiceCollection() =>
            (_components as IServiceCollectionBuilder).BuildServiceCollection();
    }
}
