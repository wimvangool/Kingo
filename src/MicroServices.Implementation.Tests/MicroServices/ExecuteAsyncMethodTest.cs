using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class ExecuteAsyncMethodTest : AsyncMethodTest
    {
        #region [====== Query Types ======]        

        [Value(12)]
        private sealed class Query1 : IQuery<object, int>
        {
            [Value(10)]
            public Task<int> ExecuteAsync([Value(2)] object message, [Value(3)] QueryOperationContext context) =>
                Task.FromResult(0);
        }

        [Value(22)]
        private sealed class Query2 : IQuery<object, int>, IQuery<string, int>
        {
            [Value(18)]
            async Task<int> IQuery<string, int>.ExecuteAsync([Value(4)] string message, [Value(5)] QueryOperationContext context) =>
                await ExecuteAsync(message, context);

            [Value(26)]
            public Task<int> ExecuteAsync([Value(6)] object message, [Value(7)] QueryOperationContext context) =>
                Task.FromResult(0);
        }

        #endregion

        [TestMethod]
        public void Method_ReturnsExpectedMethod_IfQueryImplementsOneInterface_And_MessageTypeMatchesExactly()
        {
            var query = new Query1();
            var method = new ExecuteAsyncMethod<object, int>(CreateQuery(query));

            AssertComponentProperties<Query1>(method, 12);
            AssertMethodProperties<object, int>(method, 2, 3);
        }

        [TestMethod]
        public void Method_ReturnsExpectedMethod_IfQueryImplementsOneInterface_And_MessageTypeDoesNotMatchExactly()
        {
            var query = new Query1();
            var method = new ExecuteAsyncMethod<string, int>(CreateQuery<string, int>(query));

            AssertComponentProperties<Query1>(method, 12);
            AssertMethodProperties<object, int>(method, 2, 3);
        }

        [TestMethod]
        public void Method_ReturnsExpectedMethod_IfQueryImplementsTwoInterfaces_And_MessageTypeMatchesExactly()
        {
            var query = new Query2();
            var methodOfString = new ExecuteAsyncMethod<string, int>(CreateQuery<string, int>(query));
            var methodOfObject = new ExecuteAsyncMethod<object, int>(CreateQuery<object, int>(query));            

            AssertComponentProperties<Query2>(methodOfString, 22);
            AssertMethodProperties<string, int>(methodOfString, 4, 5);

            AssertComponentProperties<Query2>(methodOfObject, 22);
            AssertMethodProperties<object, int>(methodOfObject, 6, 7);
        }

        [TestMethod]
        public void Method_ReturnsExpectedMethod_IfQueryImplementsTwoInterfaces_And_MessageTypeDoesNotMatchExactly()
        {
            var query = new Query2();
            var method = new ExecuteAsyncMethod<IDisposable, int>(CreateQuery<IDisposable, int>(query));

            AssertComponentProperties<Query2>(method, 22);
            AssertMethodProperties<object, int>(method, 6, 7);
        }

        private static Query<TRequest, TResponse> CreateQuery<TRequest, TResponse>(IQuery<TRequest, TResponse> query) =>
            new QueryAdapter<TRequest, TResponse>(query);

        private static void AssertMethodProperties<TRequest, TResponse>(IAsyncMethod method, int messageValue, int contextValue)
        {
            Assert.AreSame(typeof(Task<TResponse>), method.MethodInfo.ReturnType);
            Assert.AreSame(typeof(TRequest), method.MessageParameterInfo.ParameterType);
            AssertValue(method.MessageParameterInfo, messageValue);
            AssertValue(method.ContextParameterInfo, contextValue);
            AssertValue(method.MethodInfo, (messageValue + contextValue) * 2);
        }            
    }
}
