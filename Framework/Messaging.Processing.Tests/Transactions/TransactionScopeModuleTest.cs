using System;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceComponents.ComponentModel;
using ServiceComponents.ComponentModel.Server;
using ServiceComponents.Threading;

namespace ServiceComponents.Transactions
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

            public Task HandleAsync(TMessage message)
            {
                return AsyncMethod.RunSynchronously(() =>
                {
                    _message = message;
                    _transaction = Transaction.Current;
                    _isolationLevel = _transaction == null ? IsolationLevel.Unspecified : _transaction.IsolationLevel;
                });                
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
        
        private sealed class DefaultMessage : Message<DefaultMessage>
        {
            public override DefaultMessage Copy()
            {
                return new DefaultMessage();
            }
        }

        [TransactionScope(TransactionScopeOption.Required)]
        private sealed class RequiredMessage : Message<RequiredMessage>
        {
            public override RequiredMessage Copy()
            {
                return new RequiredMessage();
            }
        }

        [TransactionScope(TransactionScopeOption.RequiresNew)]
        private sealed class RequiresNewMessage : Message<RequiresNewMessage>
        {
            public override RequiresNewMessage Copy()
            {
                return new RequiresNewMessage();
            }
        }

        [TransactionScope(TransactionScopeOption.Suppress, "00:20")]
        private sealed class SuppressMessage : Message<SuppressMessage>
        {
            public override SuppressMessage Copy()
            {
                return new SuppressMessage();
            }
        }

        #endregion        

        [TestMethod]
        public void Module_CreatesNewTransaction_IfOptionIsRequiredAndNoTransactionIsActive()
        {
            var spy = new TransactionSpy<DefaultMessage>();
            var message = new DefaultMessage();
            var handler = new MessageHandlerWrapper<DefaultMessage>(message, spy);

            var module = new TransactionScopeModule();
             
            Assert.IsNull(Transaction.Current);
            module.InvokeAsync(handler).Wait();
            Assert.IsNull(Transaction.Current);            

            spy.VerifyMessageWas(message);
            spy.VerifyTransactionWasNotNull();
            spy.VerifyIsolationLevelWas(IsolationLevel.ReadCommitted);
        }

        [TestMethod]
        public void Module_KeepsExistingTransaction_IfOptionIsRequiredAndTransactionIsAlreadyActive()
        {
            var spy = new TransactionSpy<RequiredMessage>();
            var message = new RequiredMessage();
            var handler = new MessageHandlerWrapper<RequiredMessage>(message, spy);

            Transaction transaction;

            var module = new TransactionScopeModule();
                       
            using (var scope = new TransactionScope())
            {
                Assert.IsNotNull(transaction = Transaction.Current);
                module.InvokeAsync(handler).Wait();
                Assert.AreSame(transaction, Transaction.Current);

                scope.Complete();
            }            

            spy.VerifyMessageWas(message);
            spy.VerifyTransactionWas(transaction);
            spy.VerifyIsolationLevelWas(IsolationLevel.Serializable);
        }

        [TestMethod]        
        public void Module_Throws_IfSpecifiedIsolationLevelIsNotEqualToExistingIsolationLevel()
        {
            var spy = new TransactionSpy<DefaultMessage>();
            var message = new DefaultMessage();
            var handler = new MessageHandlerWrapper<DefaultMessage>(message, spy);

            var module = new TransactionScopeModule(TransactionScopeOption.Required, TimeSpan.FromMinutes(1), IsolationLevel.ReadCommitted);
                            
            using (var scope = new TransactionScope())
            {
                module.InvokeAsync(handler).WaitAndHandle<ArgumentException>();

                scope.Complete();
            }            
        }

        [TestMethod]
        public void Module_CreatesNewTransaction_IfTransactionExistsAndMessageRequiresNewTransaction()
        {
            var spy = new TransactionSpy<RequiresNewMessage>();
            var message = new RequiresNewMessage();
            var handler = new MessageHandlerWrapper<RequiresNewMessage>(message, spy);

            Transaction transaction;

            var module = new TransactionScopeModule();
           
            using (var scope = new TransactionScope())
            {
                Assert.IsNotNull(transaction = Transaction.Current);
                module.InvokeAsync(handler).Wait();
                Assert.AreSame(transaction, Transaction.Current);

                scope.Complete();
            }
            
            spy.VerifyMessageWas(message);
            spy.VerifyTransactionWasNot(transaction);
            spy.VerifyIsolationLevelWas(IsolationLevel.ReadCommitted);
        }

        [TestMethod]
        public void Module_HidesTransaction_IfTransactionExistsAndMessageSuppressesTransaction()
        {
            var spy = new TransactionSpy<SuppressMessage>();
            var message = new SuppressMessage();
            var handler = new MessageHandlerWrapper<SuppressMessage>(message, spy);

            var module = new TransactionScopeModule();
            
            using (var scope = new TransactionScope())
            {
                module.InvokeAsync(handler).Wait();
                scope.Complete();
            }
            
            spy.VerifyMessageWas(message);
            spy.VerifyTransactionWasNull();            
        }
    }
}
