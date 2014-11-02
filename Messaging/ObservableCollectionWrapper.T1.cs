using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace System.ComponentModel
{
    internal sealed class ObservableCollectionWrapper<TValue> : RequestMessage
    {
        private readonly ObservableCollection<TValue> _collection;        
        private readonly bool _isRequestMessageCollection;

        internal ObservableCollectionWrapper(ObservableCollection<TValue> collection, bool makeReadOnly) : base(makeReadOnly)
        {
            _collection = collection;
            _collection.CollectionChanged += HandleCollectionChanged;          
            _isRequestMessageCollection = IsRequestMessage(typeof(TValue));

            Attach(collection);
            ErrorInfo = null;
        }

        private ObservableCollectionWrapper(ObservableCollectionWrapper<TValue> message, bool makeReadOnly)  : base(message, makeReadOnly)
        {
            _collection = new ObservableCollection<TValue>(message._collection);
            _collection.CollectionChanged += HandleCollectionChanged;            
            _isRequestMessageCollection = message._isRequestMessageCollection;

            Attach(_collection);
            ErrorInfo = null;
        }

        public override RequestMessage Copy(bool makeReadOnly)
        {
            return new ObservableCollectionWrapper<TValue>(this, makeReadOnly);
        }

        internal override void Validate(bool forceValidation)
        {
            // This default implementation is overridden to prevent needless validation of this instance.
            if (forceValidation || !RequestMessageEditScope.IsValidationSuppressed(this))
            {                
                ValidateAttachedMessages();
            }
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
