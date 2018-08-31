using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Kingo.Threading.AsyncMethod;

namespace Kingo.Messaging
{
    [TestClass]
    public abstract class QueryTestBaseTest<TMessageOut> : QueryTestBase<TMessageOut>
    {
        private bool _setupWasInvoked;
        private bool _tearDownWasInvoked;

        protected override Task SetupAsync()
        {
            _setupWasInvoked = true;

            return base.SetupAsync();
        }

        protected override Task TearDownAsync()
        {
            _tearDownWasInvoked = true;

            return base.TearDownAsync();
        }       

        protected override Task<TMessageOut> WhenQueryIsExecuted(IMicroProcessor processor) =>
            processor.ExecuteAsync(ExecuteQuery);

        protected abstract TMessageOut ExecuteQuery(IMicroProcessorContext context);

        [TestMethod]
        public virtual Task ThenAsync()
        {
            return RunSynchronously(() =>
            {
                Assert.IsTrue(_setupWasInvoked, "Setup did not run.");
                Assert.IsTrue(_tearDownWasInvoked, "TearDown did not run.");
            });
        }

        protected override Exception NewAssertFailedException(string message, Exception innerException = null) =>
            new MetaAssertFailedException(message, innerException);
    }
}
