using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Kingo.Threading.AsyncMethod;

namespace Kingo.Messaging
{
    internal sealed class QuerySpy<TMessageOut> : IQuery<TMessageOut>
    {
        private int _executeCount;

        public Task<TMessageOut> ExecuteAsync(IMicroProcessorContext context)
        {
            return RunSynchronously(() =>
            {
                _executeCount++;

                return default(TMessageOut);
            });
        }

        public void AssertExecuteCountIs(int count) =>
            Assert.AreEqual(count, _executeCount);
    }
}
