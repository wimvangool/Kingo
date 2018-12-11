using System;
using System.Threading.Tasks;
using Kingo.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public abstract class MessageHandlerTestBaseTest<TMessage> : MessageHandlerTestBase<TMessage>
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

        [TestMethod]
        public virtual Task ThenAsync()
        {
            return AsyncMethod.Run(() =>
            {
                Assert.IsTrue(_setupWasInvoked, "Setup did not run.");
                Assert.IsTrue(_tearDownWasInvoked, "TearDown did not run.");
            });            
        }

        protected override Exception NewAssertFailedException(string message, Exception innerException = null) =>
            new MetaAssertFailedException(message, innerException);
    }
}
