using System;
using System.Collections.Generic;
using System.ComponentModel.Messaging.Server;
using System.Linq;
using System.Text;
using System.Transactions;

namespace System.ComponentModel.Messaging.Client
{        
    internal sealed class TransactionalMessageBuffer<TMessage> : EventBuffer<TMessage>, IEnlistmentNotification where TMessage : class
    {               
        public TransactionalMessageBuffer(IDomainEventBus eventBus, TMessage message)
            : base(eventBus, message) { }

        #region [====== IEnlistmentNotification ======]

        void IEnlistmentNotification.Prepare(PreparingEnlistment preparingEnlistment)
        {
            preparingEnlistment.Prepared();
        }

        void IEnlistmentNotification.Commit(Enlistment enlistment)
        {
            Flush();

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
    }    
}
