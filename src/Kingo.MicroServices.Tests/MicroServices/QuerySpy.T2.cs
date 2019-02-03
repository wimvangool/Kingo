using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    internal sealed class QuerySpy<TRequest, TResponse> : IQuery<TRequest, TResponse> where TRequest : class
    {
        private readonly List<TRequest> _messages;

        public QuerySpy()
        {
            _messages = new List<TRequest>();
        } 

        public Task<TResponse> ExecuteAsync(TRequest message, QueryContext context)
        {
            return AsyncMethod.Run(() =>
            {
                _messages.Add(message);

                return default(TResponse);
            });
        }

        public void AssertExecuteCountIs(int count) =>
            Assert.AreEqual(count, _messages.Count);

        public void AssertMessageReceived(int index, TRequest message) =>
            Assert.AreSame(message, _messages[index]);
    }
}
