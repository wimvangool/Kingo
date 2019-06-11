using System;
using System.Collections.Generic;
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
            public Task<object> ExecuteAsync(QueryOperationContext context) =>
                Task.FromResult(new object());
        }

        public abstract class AbstractQuery : IQuery<object>
        {
            public abstract Task<object> ExecuteAsync(QueryOperationContext context);
        }

        public sealed class GenericQuery<TResponse> : IQuery<TResponse>
        {
            public Task<TResponse> ExecuteAsync(QueryOperationContext context) =>
                Task.FromResult<TResponse>(default);
        }

        public sealed class Query1 : IQuery<object>
        {
            public Task<object> ExecuteAsync(QueryOperationContext context) =>
                Task.FromResult(new object());
        }

        public sealed class Query2 : IQuery<object, object>
        {
            public Task<object> ExecuteAsync(object message, QueryOperationContext context) =>
                Task.FromResult(message);
        }

        public sealed class Query3 : IQuery<object>, IQuery<object, object>
        {
            public Task<object> ExecuteAsync(QueryOperationContext context) =>
                Task.FromResult(new object());

            public Task<object> ExecuteAsync(object message, QueryOperationContext context) =>
                Task.FromResult(message);
        }

        #endregion

        private readonly MicroProcessorComponentCollection _components;

        public MicroProcessorComponentCollectionTest()
        {
            _components = new MicroProcessorComponentCollection();
        }

        #region [====== AddQueries ======]

        [TestMethod]
        public void AddQueries_AddsNoQueries_IfThereAreNoTypesToScan()
        {
            _components.AddQueries();

            Assert.AreEqual(1, BuildServiceCollection().Count);
        }

        [TestMethod]
        public void AddQueries_AddsNoQueries_IfThereAreNoQueryTypesToAdd()
        {
            _components.AddTypes(typeof(object), typeof(int));
            _components.AddTypes(typeof(AbstractQuery), typeof(GenericQuery<>));
            _components.AddQueries();

            Assert.AreEqual(1, BuildServiceCollection().Count);
        }

        [TestMethod]
        public void AddQueries_AddsExpectedQuery_IfQueryIsNonPublicType()
        {
            _components.AddTypes(typeof(NonPublicQuery));
            _components.AddQueries();

            var provider = BuildServiceProvider();

            Assert.IsInstanceOfType(provider.GetRequiredService<IQuery<object>>(), typeof(NonPublicQuery));
            Assert.IsNotNull(provider.GetRequiredService<NonPublicQuery>());
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

        private IServiceProvider BuildServiceProvider() =>
            BuildServiceCollection().BuildServiceProvider();

        private IServiceCollection BuildServiceCollection() =>
            (_components as IServiceCollectionBuilder).BuildServiceCollection();
    }
}
