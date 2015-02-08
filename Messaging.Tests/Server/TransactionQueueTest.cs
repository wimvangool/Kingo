using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Server
{
    [TestClass]
    public sealed class TransactionQueueTest
    {        
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Enqueue_Throws_IfActionIsNull()
        {
            MessageProcessor.Enqueue(null);
        }

        [TestMethod]
        public void Enqueue_InvokesSpecifiedActionImmediately_IfNoTransactionIsActive()
        {
            bool hasBeenInvoked = false;

            MessageProcessor.Enqueue(() => hasBeenInvoked = true);

            Assert.IsTrue(hasBeenInvoked);
        }

        [TestMethod]
        public void Enqueue_InvokesSpecifiedActionWhenTransactionHasCommitted_IfTransactionIsActive()
        {
            bool hasBeenInvoked = false;

            using (var scope = new TransactionScope())
            {
                MessageProcessor.Enqueue(() => hasBeenInvoked = true);

                Assert.IsFalse(hasBeenInvoked);

                scope.Complete();                
            }
            Assert.IsTrue(hasBeenInvoked);
        }        
    }
}
