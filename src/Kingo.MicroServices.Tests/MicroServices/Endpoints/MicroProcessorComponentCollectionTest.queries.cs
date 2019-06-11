using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Endpoints
{    
    public sealed partial class MicroProcessorComponentCollectionTest
    {
        #region [====== QueryTypes ======]        

        private abstract class AbstractQuery : IQuery<object>
        {
            public abstract Task<object> ExecuteAsync(QueryOperationContext context);
        }

        private sealed class GenericQuery<TResponse> : IQuery<TResponse>
        {
            public Task<TResponse> ExecuteAsync(QueryOperationContext context) =>
                Task.FromResult<TResponse>(default);
        }

        private sealed class Query1 : IQuery<object>
        {
            public Task<object> ExecuteAsync(QueryOperationContext context) =>
                Task.FromResult(new object());
        }

        private sealed class Query2 : IQuery<object, object>
        {
            public Task<object> ExecuteAsync(object message, QueryOperationContext context) =>
                Task.FromResult(message);
        }

        private sealed class Query3 : IQuery<object>, IQuery<object, object>
        {
            public Task<object> ExecuteAsync(QueryOperationContext context) =>
                Task.FromResult(new object());

            public Task<object> ExecuteAsync(object message, QueryOperationContext context) =>
                Task.FromResult(message);
        }

        [MicroProcessorComponent((ServiceLifetime) (-1))]
        private sealed class InvalidLifetimeQuery : IQuery<object>
        {
            public Task<object> ExecuteAsync(QueryOperationContext context) =>
                Task.FromResult(new object());
        }

        [MicroProcessorComponent(ServiceLifetime.Scoped)]
        private sealed class ScopedQuery : IQuery<object>
        {
            public Task<object> ExecuteAsync(QueryOperationContext context) =>
                Task.FromResult(new object());
        }

        [MicroProcessorComponent(ServiceLifetime.Singleton)]
        private sealed class SingletonQuery : IQuery<object>
        {
            public Task<object> ExecuteAsync(QueryOperationContext context) =>
                Task.FromResult(new object());
        }

        #endregion        

        #region [====== AddQueries (Registration & Mapping) ======]

        [TestMethod]
        public void AddQueries_AddsNoQueries_IfThereAreNoTypesToScan()
        {
            _components.AddQueries();

            Assert.AreEqual(1, BuildServiceCollection().Count);
        }

        [TestMethod]
        public void AddQueries_AddsNoQueries_IfThereAreNoQueryTypesToAdd()
        {
            _components.AddTypes(typeof(object), typeof(int), typeof(MessageHandler1));
            _components.AddTypes(typeof(AbstractQuery), typeof(GenericQuery<>));
            _components.AddQueries();

            Assert.AreEqual(1, BuildServiceCollection().Count);
        }        

        [TestMethod]
        public void AddQueries_AddsExpectedQuery_IfQueryIsClosedGenericType()
        {                        
            _components.AddTypes(typeof(GenericQuery<object>));
            _components.AddQueries();

            var provider = BuildServiceProvider();

            Assert.IsInstanceOfType(provider.GetRequiredService<IQuery<object>>(), typeof(GenericQuery<object>));
            Assert.IsNotNull(provider.GetRequiredService<GenericQuery<object>>());
        }

        [TestMethod]
        public void AddQueries_AddsExpectedQuery_IfQueryIsRegularTypeWithoutRequest()
        {            
            _components.AddTypes(typeof(Query1));
            _components.AddQueries();

            var provider = BuildServiceProvider();

            Assert.IsInstanceOfType(provider.GetRequiredService<IQuery<object>>(), typeof(Query1));
            Assert.IsNotNull(provider.GetRequiredService<Query1>());
        }

        [TestMethod]
        public void AddQueries_AddsExpectedQuery_IfQueryIsRegularTypeWithRequest()
        {            
            _components.AddTypes(typeof(Query2));
            _components.AddQueries();

            var provider = BuildServiceProvider();

            Assert.IsInstanceOfType(provider.GetRequiredService<IQuery<object, object>>(), typeof(Query2));
            Assert.IsNotNull(provider.GetRequiredService<Query2>());
        }

        [TestMethod]
        public void AddQueries_AddsExpectedQuery_IfQueryImplementsMultipleInterfaces()
        {            
            _components.AddTypes(typeof(Query3));
            _components.AddQueries();

            var provider = BuildServiceProvider();

            Assert.IsInstanceOfType(provider.GetRequiredService<IQuery<object>>(), typeof(Query3));
            Assert.IsInstanceOfType(provider.GetRequiredService<IQuery<object, object>>(), typeof(Query3));
            Assert.IsNotNull(provider.GetRequiredService<Query3>());
        }

        [TestMethod]
        public void AddQueries_AddsExpectedQueries_IfMultipleQueriesImplementTheSameInterface()
        {
            _components.AddTypes(typeof(Query1), typeof(Query3));
            _components.AddQueries();

            var provider = BuildServiceProvider();

            Assert.IsInstanceOfType(provider.GetRequiredService<IQuery<object>>(), typeof(Query3));
            Assert.IsNotNull(provider.GetRequiredService<Query1>());
            Assert.IsNotNull(provider.GetRequiredService<Query3>());
            Assert.AreEqual(2, provider.GetRequiredService<IEnumerable<IQuery<object>>>().Count());
        }

        #endregion

        #region [====== AddQueries (Lifetime) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddQueries_Throws_IfQueryHasInvalidLifetime()
        {
            _components.AddTypes(typeof(InvalidLifetimeQuery));
            _components.AddQueries();            
        }

        [TestMethod]
        public void AddQueries_AddsTransientQuery_IfQueryHasTransientLifetime()
        {
            _components.AddTypes(typeof(Query1));
            _components.AddQueries();

            var provider = BuildServiceProvider();
            var queryA = provider.GetRequiredService<Query1>();

            using (var scope = provider.CreateScope())
            {
                var queryB = scope.ServiceProvider.GetRequiredService<Query1>();
                var queryC = scope.ServiceProvider.GetRequiredService<Query1>();

                Assert.AreNotSame(queryA, queryB);
                Assert.AreNotSame(queryB, queryC);
            }

            var queryD = provider.GetRequiredService<Query1>();

            Assert.AreNotSame(queryA, queryD);
        }

        [TestMethod]
        public void AddQueries_AddsScopedQuery_IfQueryHasScopedLifetime()
        {
            _components.AddTypes(typeof(ScopedQuery));
            _components.AddQueries();

            var provider = BuildServiceProvider();
            var queryA = provider.GetRequiredService<ScopedQuery>();

            using (var scope = provider.CreateScope())
            {
                var queryB = scope.ServiceProvider.GetRequiredService<ScopedQuery>();
                var queryC = scope.ServiceProvider.GetRequiredService<ScopedQuery>();

                Assert.AreNotSame(queryA, queryB);
                Assert.AreSame(queryB, queryC);
            }

            var queryD = provider.GetRequiredService<ScopedQuery>();

            Assert.AreSame(queryA, queryD);
        }

        [TestMethod]
        public void AddQueries_AddsSingletonQuery_IfQueryHasSingletonLifetime()
        {
            _components.AddTypes(typeof(SingletonQuery));
            _components.AddQueries();

            var provider = BuildServiceProvider();
            var queryA = provider.GetRequiredService<SingletonQuery>();

            using (var scope = provider.CreateScope())
            {
                var queryB = scope.ServiceProvider.GetRequiredService<SingletonQuery>();
                var queryC = scope.ServiceProvider.GetRequiredService<SingletonQuery>();

                Assert.AreSame(queryA, queryB);
                Assert.AreSame(queryB, queryC);
            }

            var queryD = provider.GetRequiredService<SingletonQuery>();

            Assert.AreSame(queryA, queryD);
        }

        #endregion        
    }
}
