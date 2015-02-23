using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Server
{
    [TestClass]
    public sealed class TransactionScopeModuleTest
    {
        #region [====== TransactionSpy ======]

        private sealed class TransactionSpy<TMessage> : IMessageHandler<TMessage> where TMessage : class
        {
            private TMessage _message;
            private Transaction _transaction;
            private IsolationLevel _isolationLevel;

            public void Handle(TMessage message)
            {
                _message = message;
                _transaction = Transaction.Current;
                _isolationLevel = _transaction == null ? IsolationLevel.Unspecified : _transaction.IsolationLevel;
            }

            internal void VerifyMessageWas(TMessage message)
            {
                Assert.AreSame(message, _message);
            }

            internal void VerifyTransactionWasNotNull()
            {
                Assert.IsNotNull(_transaction);
            }

            internal void VerifyTransactionWasNull()
            {
                Assert.IsNull(_transaction);
            }

            internal void VerifyTransactionWas(Transaction transaction)
            {
                Assert.AreSame(transaction, _transaction);
            }

            internal void VerifyTransactionWasNot(Transaction transaction)
            {
                Assert.AreNotSame(transaction, _transaction);
            }

            internal void VerifyIsolationLevelWas(IsolationLevel isolationLevel)
            {
                Assert.AreEqual(isolationLevel, _isolationLevel);
            }
        }

        #endregion

        #region [====== Messages ======]

        [TransactionScope(TransactionScopeOption.RequiresNew)]
        private sealed class RequiresNewMessage { }

        [TransactionScope(TransactionScopeOption.Suppress, "00:20")]
        private sealed class SuppressMessage { }

        #endregion

        [TestMethod]
        public void Module_CreatesNewTransaction_IfOptionIsRequiredAndNoTransactionIsActive()
        {
            var messageSink = new TransactionSpy<object>();
            var message = new object();

            IMessageHandler<object> module = new TransactionScopeModule<object>(messageSink);
            
            Assert.IsNull(Transaction.Current);
            module.Handle(message);
            Assert.IsNull(Transaction.Current);

            messageSink.VerifyMessageWas(message);
            messageSink.VerifyTransactionWasNotNull();
            messageSink.VerifyIsolationLevelWas(IsolationLevel.ReadCommitted);
        }

        [TestMethod]
        public void Module_KeepsExistingTransaction_IfOptionIsRequiredAndTransactionIsAlreadyActive()
        {
            var messageSink = new TransactionSpy<object>();
            var message = new object();
            Transaction transaction;

            IMessageHandler<object> module = new TransactionScopeModule<object>(messageSink);

            using (var scope = new TransactionScope())
            {
                Assert.IsNotNull(transaction = Transaction.Current);
                module.Handle(message);
                Assert.AreSame(transaction, Transaction.Current);

                scope.Complete();
            }            

            messageSink.VerifyMessageWas(message);
            messageSink.VerifyTransactionWas(transaction);
            messageSink.VerifyIsolationLevelWas(IsolationLevel.Serializable);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Module_ThrowsException_IfSpecifiedIsolationLevelIsNotEqualToExistingIsolationLevel()
        {
            var messageSink = new TransactionSpy<object>();
            var message = new object();            

            IMessageHandler<object> module = new TransactionScopeModule<object>(messageSink, TransactionScopeOption.Required, TimeSpan.FromMinutes(1), IsolationLevel.ReadCommitted);

            using (var scope = new TransactionScope())
            {                
                module.Handle(message);                

                scope.Complete();
            }  
        }

        [TestMethod]
        public void Module_CreatesNewTransaction_IfTransactionExistsAndMessageRequiresNewTransaction()
        {
            var messageSink = new TransactionSpy<RequiresNewMessage>();
            var message = new RequiresNewMessage();
            Transaction transaction;

            IMessageHandler<RequiresNewMessage> module = new TransactionScopeModule<RequiresNewMessage>(messageSink);

            using (var scope = new TransactionScope())
            {
                Assert.IsNotNull(transaction = Transaction.Current);
                module.Handle(message);
                Assert.AreSame(transaction, Transaction.Current);

                scope.Complete();                
            }

            messageSink.VerifyMessageWas(message);
            messageSink.VerifyTransactionWasNot(transaction);
            messageSink.VerifyIsolationLevelWas(IsolationLevel.ReadCommitted);
        }

        [TestMethod]
        public void Module_HidesTransaction_IfTransactionExistsAndMessageSuppressesTransaction()
        {
            var messageSink = new TransactionSpy<SuppressMessage>();
            var message = new SuppressMessage();            

            IMessageHandler<SuppressMessage> module = new TransactionScopeModule<SuppressMessage>(messageSink);

            using (var scope = new TransactionScope())
            {                
                module.Handle(message);             
                scope.Complete();
            }

            messageSink.VerifyMessageWas(message);
            messageSink.VerifyTransactionWasNull();            
        }
    }
}
