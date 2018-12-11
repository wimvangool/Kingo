using System.Threading.Tasks;
using Kingo.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    internal sealed class QuerySpy<TMessageOut> : IQuery<TMessageOut>
    {
        private int _executeCount;

        public Task<TMessageOut> ExecuteAsync(QueryContext context)
        {
            return AsyncMethod.Run(() =>
            {
                _executeCount++;

                return default(TMessageOut);
            });
        }

        public void AssertExecuteCountIs(int count) =>
            Assert.AreEqual(count, _executeCount);
    }
}
