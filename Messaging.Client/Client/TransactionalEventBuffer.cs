using System.ComponentModel.Messaging.Server;
using System.Runtime.InteropServices;
using System.Transactions;

namespace System.ComponentModel.Messaging.Client
{        
    internal sealed class TransactionalMessageBuffer : IEnlistmentNotification
    {
        private readonly ClientEventBus _eventBus;
        private readonly object _message;

        private TransactionalMessageBuffer(ClientEventBus eventBus, object message)
        {
            _eventBus = eventBus;
            _message = message;
        }
          
        #region [====== IEnlistmentNotification ======]

        void IEnlistmentNotification.Prepare(PreparingEnlistment preparingEnlistment)
        {
            preparingEnlistment.Prepared();
        }

        void IEnlistmentNotification.Commit(Enlistment enlistment)
        {
            _eventBus.Commit(_message);

            enlistment.Done();
        }

        void IEnlistmentNotification.InDoubt(Enlistment enlistment)
        {
            enlistment.Done();
        }

        void IEnlistmentNotification.Rollback(Enlistment enlistment)
        {
            enlistment.Done();
        }

        #endregion
        
        internal static void Write(ClientEventBus eventBus, object message)
        {
            using (var scope = new TransactionScope())
            {
                EnlistInTransaction(new TransactionalMessageBuffer(eventBus, message));

                scope.Complete();
            }
        }

        private static void EnlistInTransaction(IEnlistmentNotification buffer)
        {
            Transaction.Current.EnlistVolatile(buffer, EnlistmentOptions.None);
        }
    }    
}
