using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class ExecuteAsyncMethodTest1 : AsyncMethodTest
    {
        #region [====== Query Types ======]

        [Value(11)]
        private sealed class Query1 : IQuery<string>
        {
            [Value(2)]
            public Task<string> ExecuteAsync([Value(1)] QueryOperationContext context) =>
                Task.FromResult(string.Empty);
        }

        [Value(21)]
        private sealed class Query2 : IQuery<string>, IQuery<object>
        {
            [Value(4)]
            async Task<object> IQuery<object>.ExecuteAsync([Value(2)] QueryOperationContext context) =>
                await ExecuteAsync(context);

            [Value(6)]
            public Task<string> ExecuteAsync([Value(3)] QueryOperationContext context) =>
                Task.FromResult(string.Empty);
        }        

        #endregion

        [TestMethod]
        public void Method_ReturnsExpectedMethod_IfQueryImplementsOneInterface()
        {
            var query = new Query1();
            var method = new ExecuteAsyncMethod<string>(query);

            AssertComponentProperties<Query1>(method, 11);
            AssertMethodProperties<string>(method, 1);
        }        

        [TestMethod]
        public void Method_ReturnsExpectedMethod_IfQueryImplementsTwoInterfaces()
        {
            var query = new Query2();
            var methodOfObject = new ExecuteAsyncMethod<object>(query);
            var methodOfString = new ExecuteAsyncMethod<string>(query);

            AssertComponentProperties<Query2>(methodOfObject, 21);
            AssertMethodProperties<object>(methodOfObject, 2);

            AssertComponentProperties<Query2>(methodOfString, 21);
            AssertMethodProperties<string>(methodOfString, 3);
        }        

        private static void AssertMethodProperties<TResponse>(IAsyncMethod method, int contextValue)
        {            
            Assert.AreSame(typeof(Task<TResponse>), method.Info.ReturnType);
            Assert.IsNull(method.MessageParameter);
            AssertValue(method.ContextParameter, contextValue);
            AssertValue(method, contextValue * 2);
        }
    }
}
