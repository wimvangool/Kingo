using System.Threading.Tasks;
using Kingo.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    internal sealed class QuerySpy<TResponse> : IQuery<TResponse>
    {
        private int _executeCount;

        public Task<TResponse> ExecuteAsync(QueryContext context)
        {
            return AsyncMethod.Run(() =>
            {
                _executeCount++;

                return default(TResponse);
            });
        }

        public void AssertExecuteCountIs(int count) =>
            Assert.AreEqual(count, _executeCount);
    }
}
