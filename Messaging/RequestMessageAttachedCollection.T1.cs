using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace System.ComponentModel
{
    internal sealed class RequestMessageAttachedCollection<TValue> : RequestMessage<RequestMessageAttachedCollection<TValue>>
    {
        private readonly ObservableCollection<TValue> _collection;        
        private readonly bool _isRequestMessageCollection;

        internal RequestMessageAttachedCollection(ObservableCollection<TValue> collection, bool makeReadOnly) : base(makeReadOnly)
        {
            _collection = collection;
            _collection.CollectionChanged += HandleCollectionChanged;          
            _isRequestMessageCollection = IsRequestMessage(typeof(TValue));

            Attach(collection);
            Validator.ErrorInfo = null;
        }

        private RequestMessageAttachedCollection(RequestMessageAttachedCollection<TValue> message, bool makeReadOnly)  : base(message, makeReadOnly)
        {
            _collection = new ObservableCollection<TValue>(message._collection);
            _collection.CollectionChanged += HandleCollectionChanged;            
            _isRequestMessageCollection = message._isRequestMessageCollection;

            Attach(_collection);
            Validator.ErrorInfo = null;
        }

        public override RequestMessageAttachedCollection<TValue> Copy(bool makeReadOnly)
        {
            return new RequestMessageAttachedCollection<TValue>(this, makeReadOnly);
        }

        internal override void Validate(bool ignoreEditScope, bool validateAttachedMessages)
        {
            // This default implementation is overridden to prevent needless validation of this instance.
            if (ignoreEditScope || !RequestMessageEditScope.IsValidationSuppressed(this) && validateAttachedMessages)
            {                
                ValidateAttachedMessages();
            }
        }

        internal override bool IsNotValid(out MessageErrorTree errorTree)
        {
            var messages = _collection as IEnumerable<IRequestMessage>;
            if (messages != null)
            {
                var errorTrees = new LinkedList<MessageErrorTree>();

                foreach (var message in messages)
                {
                    MessageErrorTree messageErrorTree;

                    if (message.IsNotValid(out messageErrorTree))
                    {
                        errorTrees.AddLast(messageErrorTree);
                    }
                }
                if (errorTrees.Count > 0)
                {
                    errorTree = new MessageErrorTree(typeof(ObservableCollection<TValue>), null, errorTrees);
                    return true;
                }
            }
            errorTree = null;
            return false;
        }

        private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsReadOnly)
            {
                throw NewMessageIsReadOnlyException(_collection);
            }
            if (_isRequestMessageCollection)
            {
                Detach(e.OldItems);
                Attach(e.NewItems);

                MarkAsChangedAndValidate();
            }
            else
            {
                MarkAsChanged();
            }
        }        

        private void Attach(IEnumerable items)
        {
            if (items == null)
            {
                return;
            }
            foreach (var item in items)
            {
                Attach(item as IRequestMessage);
            }
        }

        private void Detach(IEnumerable items)
        {
            if (items == null)
            {
                return;
            }
            foreach (var item in items)
            {
                Detach(item as IRequestMessage);
            }
        }
    }
}
