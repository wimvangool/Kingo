using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace System.ComponentModel
{
    internal sealed class RequestMessageAttachedCollection<TValue> : RequestMessageViewModel<RequestMessageAttachedCollection<TValue>>
    {
        private readonly ObservableCollection<TValue> _collection;        
        private readonly bool _isRequestMessageCollection;

        internal RequestMessageAttachedCollection(ObservableCollection<TValue> collection, bool makeReadOnly) : base(makeReadOnly)
        {
            _collection = collection;
            _collection.CollectionChanged += HandleCollectionChanged;          
            _isRequestMessageCollection = IsRequestMessageViewModel(typeof(TValue));

            Attach(collection);            
        }

        private RequestMessageAttachedCollection(RequestMessageAttachedCollection<TValue> message, bool makeReadOnly)  : base(message, makeReadOnly)
        {
            _collection = new ObservableCollection<TValue>(message._collection);
            _collection.CollectionChanged += HandleCollectionChanged;            
            _isRequestMessageCollection = message._isRequestMessageCollection;

            Attach(_collection);            
        }

        public override RequestMessageAttachedCollection<TValue> Copy(bool makeReadOnly)
        {
            return new RequestMessageAttachedCollection<TValue>(this, makeReadOnly);
        }        

        internal override bool TryGetValidationErrors(bool includeChildErrors, out ValidationErrorTree errorTree)
        {
            if (includeChildErrors)
            {
                var messages = _collection as IEnumerable<IRequestMessage>;
                ICollection<ValidationErrorTree> childErrorTrees;

                if (messages != null && TryGetValidationErrors(messages, out childErrorTrees))
                {
                    errorTree = ValidationErrorTree.Merge(typeof(ObservableCollection<TValue>), childErrorTrees);
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
                Attach(item as IRequestMessageViewModel);
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
                Detach(item as IRequestMessageViewModel);
            }
        }
    }
}
