using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Controllers
{    
    [TestClass]
    public sealed class ConfigureQueriesTest : MicroProcessorTest<MicroProcessor>
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

        #region [====== Add (Registration & Mapping) ======]

        [TestMethod]
        public void Add_AddsNoQueries_IfThereAreNoQueryTypesToAdd()
        {
            Processor.ConfigureQueries(queries =>
            {
                queries.Add(typeof(object), typeof(int));
                queries.Add(typeof(AbstractQuery), typeof(GenericQuery<>));
            });

            Assert.AreEqual(DefaultServiceCount, BuildServiceCollection().Count);
        }        

        [TestMethod]
        public void Add_AddsExpectedQuery_IfQueryIsClosedGenericType()
        {
            Processor.ConfigureQueries(queries =>
            {
                queries.Add(typeof(GenericQuery<object>));
            });

            var provider = BuildServiceProvider();

            Assert.IsInstanceOfType(provider.GetRequiredService<IQuery<object>>(), typeof(GenericQuery<object>));
            Assert.IsNotNull(provider.GetRequiredService<GenericQuery<object>>());
        }

        [TestMethod]
        public void Add_AddsExpectedQuery_IfQueryIsRegularTypeWithoutRequest()
        {
            Processor.ConfigureQueries(queries =>
            {
                queries.Add(typeof(Query1));
            });

            var provider = BuildServiceProvider();

            Assert.IsInstanceOfType(provider.GetRequiredService<IQuery<object>>(), typeof(Query1));
            Assert.IsNotNull(provider.GetRequiredService<Query1>());
        }

        [TestMethod]
        public void Add_AddsExpectedQuery_IfQueryIsRegularTypeWithRequest()
        {
            Processor.ConfigureQueries(queries =>
            {
                queries.Add(typeof(Query2));
            });

            var provider = BuildServiceProvider();

            Assert.IsInstanceOfType(provider.GetRequiredService<IQuery<object, object>>(), typeof(Query2));
            Assert.IsNotNull(provider.GetRequiredService<Query2>());
        }

        [TestMethod]
        public void Add_AddsExpectedQuery_IfQueryImplementsMultipleInterfaces()
        {
            Processor.ConfigureQueries(queries =>
            {
                queries.Add(typeof(Query3));
            });

            var provider = BuildServiceProvider();

            Assert.IsInstanceOfType(provider.GetRequiredService<IQuery<object>>(), typeof(Query3));
            Assert.IsInstanceOfType(provider.GetRequiredService<IQuery<object, object>>(), typeof(Query3));
            Assert.IsNotNull(provider.GetRequiredService<Query3>());
        }

        [TestMethod]
        public void Add_AddsExpectedQueries_IfMultipleQueriesImplementTheSameInterface()
        {
            Processor.ConfigureQueries(queries =>
            {
                queries.Add(typeof(Query1), typeof(Query3));
            });

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
            Processor.ConfigureQueries(queries =>
            {
                queries.Add(typeof(InvalidLifetimeQuery));
            });

            BuildServiceProvider();
        }

        [TestMethod]
        public void Add_AddsTransientQuery_IfQueryHasTransientLifetime()
        {
            Processor.ConfigureQueries(queries =>
            {
                queries.Add(typeof(Query1));
            });

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
            Processor.ConfigureQueries(queries =>
            {
                queries.Add(typeof(ScopedQuery));
            });

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
            Processor.ConfigureQueries(queries =>
            {
                queries.Add(typeof(SingletonQuery));
            });

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
