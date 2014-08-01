using System;
using System.Collections.Generic;
using System.ComponentModel.Messaging.Server;
using System.Linq;
using System.Text;
using System.Threading;
using System.Transactions;

namespace System.ComponentModel.Messaging.Client
{
    internal sealed class ClientEventBusMessageBuffer : IUnitOfWork
    {
        private readonly ClientEventBus _eventBus;
        private readonly Transaction _transaction;
        private readonly List<object> _messages;

        internal ClientEventBusMessageBuffer(ClientEventBus eventBus, Transaction transaction)
        {
            _eventBus = eventBus;
            _transaction = transaction;
            _messages = new List<object>();
        }

        public void Write(object message)
        {
            _messages.Add(message);

            UnitOfWorkContext.Enlist(this);
        }

        #region [====== UnitOfWork ======]

        public event EventHandler FlushCompleted;

        private void OnFlushCompleted()
        {
            _messages.Clear();

            FlushCompleted.Raise(this);
        }

        Guid IUnitOfWork.FlushGroupId
        {
            get { return Guid.Empty; }
        }

        bool IUnitOfWork.CanBeFlushedAsynchronously
        {
            get { return false; }
        }

        bool IUnitOfWork.RequiresFlush()
        {
            return _messages.Count > 0;
        }

        void IUnitOfWork.Flush()
        {
            // If no transaction was present, we can flush immediately. Otherwise, we only flush
            // as soon as the transaction completes succesfully. In the regular case, if a transaction
            // was active while messages were being published, it is still active, and we hook up to the
            // TransactionCompleted-event to flush or abort as necessary. However, just for the sake of robustness,
            // we also check whether or not the transaction has already been committed or not and act accordingly.
            if (_transaction == null || _transaction.TransactionInformation.Status == TransactionStatus.Committed)
            {
                Flush();
            }
            else if (_transaction.TransactionInformation.Status != TransactionStatus.Active)
            {
                OnFlushCompleted();
            }
            else
            {
                _transaction.TransactionCompleted += (s, e) =>
                {
                    if (e.Transaction.TransactionInformation.Status == TransactionStatus.Committed)
                    {
                        Flush();
                    }
                    else
                    {
                        OnFlushCompleted();
                    }
                };
            }
        }

        private void Flush()
        {
            var messages = new List<object>(_messages);

            using (var scope = new SynchronizationContextScope(_eventBus.SynchronizationContext))
            {
                scope.Post(() => Flush(messages));
            }
            OnFlushCompleted();
        }

        private void Flush(IEnumerable<object> messages)
        {
            foreach (var message in messages)
            {
                _eventBus.Publish(message);
            }
        }

        #endregion
    }
}
