using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    internal sealed class QuerySpy<TMessageIn, TMessageOut> : IQuery<TMessageIn, TMessageOut> where TMessageIn : class
    {
        private readonly List<TMessageIn> _messages;

        public QuerySpy()
        {
            _messages = new List<TMessageIn>();
        } 

        public Task<TMessageOut> ExecuteAsync(TMessageIn message, IMicroProcessorContext context)
        {
            return AsyncMethod.RunSynchronously(() =>
            {
                _messages.Add(message);

                return default(TMessageOut);
            });
        }

        public void AssertExecuteCountIs(int count) =>
            Assert.AreEqual(count, _messages.Count);

        public void AssertMessageReceived(int index, TMessageIn message) =>
            Assert.AreSame(message, _messages[index]);
    }
}
