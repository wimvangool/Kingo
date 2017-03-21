using System.Threading.Tasks;
using Kingo.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    internal sealed class QuerySpy<TMessageOut> : IQuery<TMessageOut>
    {
        private int _executeCount;

        public Task<TMessageOut> ExecuteAsync(IMicroProcessorContext context)
        {
            return AsyncMethod.RunSynchronously(() =>
            {
                _executeCount++;

                return default(TMessageOut);
            });
        }

        public void AssertExecuteCountIs(int count) =>
            Assert.AreEqual(count, _executeCount);
    }
}
