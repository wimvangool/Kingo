using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kingo.MicroServices.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Configuration
{    
    [TestClass]
    public sealed class AddQueriesTest : MicroProcessorTest<MicroProcessor>
    {
        #region [====== QueryTypes ======]        

        private abstract class AbstractQuery : IQuery<object>
        {
            public abstract Task<object> ExecuteAsync(IQueryOperationContext context);
        }

        private sealed class GenericQuery<TResponse> : IQuery<TResponse>
        {
            public Task<TResponse> ExecuteAsync(IQueryOperationContext context) =>
                Task.FromResult<TResponse>(default);
        }

        private sealed class Query1 : IQuery<object>
        {
            public Task<object> ExecuteAsync(IQueryOperationContext context) =>
                Task.FromResult(new object());
        }

        private sealed class Query2 : IQuery<object, object>
        {
            public Task<object> ExecuteAsync(object message, IQueryOperationContext context) =>
                Task.FromResult(message);
        }

        private sealed class Query3 : IQuery<object>, IQuery<object, object>
        {
            public Task<object> ExecuteAsync(IQueryOperationContext context) =>
                Task.FromResult(new object());

            public Task<object> ExecuteAsync(object message, IQueryOperationContext context) =>
                Task.FromResult(message);
        }

        [MicroProcessorComponent((ServiceLifetime) (-1))]
        private sealed class InvalidLifetimeQuery : IQuery<object>
        {
            public Task<object> ExecuteAsync(IQueryOperationContext context) =>
                Task.FromResult(new object());
        }

        [MicroProcessorComponent(ServiceLifetime.Scoped)]
        private sealed class ScopedQuery : IQuery<object>
        {
            public Task<object> ExecuteAsync(IQueryOperationContext context) =>
                Task.FromResult(new object());
        }

        [MicroProcessorComponent(ServiceLifetime.Singleton)]
        private sealed class SingletonQuery : IQuery<object>
        {
            public Task<object> ExecuteAsync(IQueryOperationContext context) =>
                Task.FromResult(new object());
        }

        #endregion        

        #region [====== Add (Registration & Mapping) ======]

        [TestMethod]
        public void Add_AddsNoQueries_IfThereAreNoQueryTypesToAdd()
        {
            ProcessorBuilder.Queries.Add(typeof(object), typeof(int));
            ProcessorBuilder.Queries.Add(typeof(AbstractQuery), typeof(GenericQuery<>));

            Assert.AreEqual(DefaultServiceCount, BuildServiceCollection().Count);
        }        

        [TestMethod]
        public void Add_AddsExpectedQuery_IfQueryIsClosedGenericType()
        {                        
            ProcessorBuilder.Queries.Add(typeof(GenericQuery<object>));

            var provider = BuildServiceProvider();

            Assert.IsInstanceOfType(provider.GetRequiredService<IQuery<object>>(), typeof(GenericQuery<object>));
            Assert.IsNotNull(provider.GetRequiredService<GenericQuery<object>>());
        }

        [TestMethod]
        public void Add_AddsExpectedQuery_IfQueryIsRegularTypeWithoutRequest()
        {            
            ProcessorBuilder.Queries.Add(typeof(Query1));

            var provider = BuildServiceProvider();

            Assert.IsInstanceOfType(provider.GetRequiredService<IQuery<object>>(), typeof(Query1));
            Assert.IsNotNull(provider.GetRequiredService<Query1>());
        }

        [TestMethod]
        public void Add_AddsExpectedQuery_IfQueryIsRegularTypeWithRequest()
        {            
            ProcessorBuilder.Queries.Add(typeof(Query2));

            var provider = BuildServiceProvider();

            Assert.IsInstanceOfType(provider.GetRequiredService<IQuery<object, object>>(), typeof(Query2));
            Assert.IsNotNull(provider.GetRequiredService<Query2>());
        }

        [TestMethod]
        public void Add_AddsExpectedQuery_IfQueryImplementsMultipleInterfaces()
        {            
            ProcessorBuilder.Queries.Add(typeof(Query3));

            var provider = BuildServiceProvider();

            Assert.IsInstanceOfType(provider.GetRequiredService<IQuery<object>>(), typeof(Query3));
            Assert.IsInstanceOfType(provider.GetRequiredService<IQuery<object, object>>(), typeof(Query3));
            Assert.IsNotNull(provider.GetRequiredService<Query3>());
        }

        [TestMethod]
        public void Add_AddsExpectedQueries_IfMultipleQueriesImplementTheSameInterface()
        {
            ProcessorBuilder.Queries.Add(typeof(Query1), typeof(Query3));

            var provider = BuildServiceProvider();

            Assert.IsInstanceOfType(provider.GetRequiredService<IQuery<object>>(), typeof(Query3));
            Assert.IsNotNull(provider.GetRequiredService<Query1>());
            Assert.IsNotNull(provider.GetRequiredService<Query3>());
            Assert.AreEqual(2, provider.GetRequiredService<IEnumerable<IQuery<object>>>().Count());
        }

        #endregion

        #region [====== Add (Lifetime) ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BuildServiceProvider_Throws_IfQueryHasInvalidLifetime()
        {
            ProcessorBuilder.Queries.Add(typeof(InvalidLifetimeQuery));

            BuildServiceProvider();
        }

        [TestMethod]
        public void Add_AddsTransientQuery_IfQueryHasTransientLifetime()
        {
            ProcessorBuilder.Queries.Add(typeof(Query1));

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
        public void Add_AddsScopedQuery_IfQueryHasScopedLifetime()
        {
            ProcessorBuilder.Queries.Add(typeof(ScopedQuery));

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
        public void Add_AddsSingletonQuery_IfQueryHasSingletonLifetime()
        {
            ProcessorBuilder.Queries.Add(typeof(SingletonQuery));

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
