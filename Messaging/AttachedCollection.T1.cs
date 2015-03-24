using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace System.ComponentModel
{
    /// <summary>
    /// Represents a collection of items that is attached to a parent message.
    /// </summary>
    /// <typeparam name="TValue">Type of the items in the collection.</typeparam>
    [Serializable]
    public sealed class AttachedCollection<TValue> : RequestMessageViewModel<AttachedCollection<TValue>>, IList<TValue>, INotifyCollectionChanged
    {
        private readonly List<TValue> _items;
        
        [NonSerialized]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]       
        private readonly bool _isRequestMessageCollection;

        internal AttachedCollection(IEnumerable<TValue> collection, bool makeReadOnly) : base(makeReadOnly)
        {
            _items = new List<TValue>(collection);
            _isRequestMessageCollection = IsRequestMessageViewModel(typeof(TValue));
            
            Attach(_items);            
        }

        private AttachedCollection(AttachedCollection<TValue> message, bool makeReadOnly)  : base(message, makeReadOnly)
        {
            _items = new List<TValue>(message._items);                      
            _isRequestMessageCollection = message._isRequestMessageCollection;
            
            Attach(_items);            
        }

         /// <summary>
        /// Initializes a new instance of the <see cref="AttachedCollection{TMessage}" /> class by deserializing it's contents
        /// from a <see cref="SerializationInfo" /> instance.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        private AttachedCollection(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _items = new List<TValue>();
            _isRequestMessageCollection = IsRequestMessageViewModel(typeof(TValue));
        }

        #region [====== Copy & Validation ======]

        /// <inheritdoc />
        public override AttachedCollection<TValue> Copy(bool makeReadOnly)
        {
            return new AttachedCollection<TValue>(this, makeReadOnly);
        }

        internal override ValidationErrorTree Validate(bool includeChildErrors)
        {
            if (includeChildErrors)
            {
                var messages = _items as IEnumerable<IMessage>;
                if (messages != null)
                {
                    return ValidationErrorTree.Merge(Validate(messages));
                }                
            }
            return ValidationErrorTree.NoErrors;
        }

        #endregion      

        #region [====== Collection Members ======]

        /// <inheritdoc />
        public event NotifyCollectionChangedEventHandler CollectionChanged;   
     
        private void OnCollectionChanged()
        {            
            // At the moment, only the Reset-option is supported.
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var handlers = CollectionChanged;
            if (handlers != null)
            {
                handlers.Invoke(this, e);
            }
            NotifyOfPropertyChange(() => Count);

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

        /// <inheritdoc />
        public int Count
        {
            get { return _items.Count; }
        }

        /// <inheritdoc />
        public TValue this[int index]
        {
            get { return _items[index]; }
            set
            {
                if (IsReadOnly)
                {
                    throw NewMessageIsReadOnlyException(_items);
                }
                var oldValue = _items[index];
                var newValue = value;                

                if (ReferenceEquals(oldValue, newValue))
                {
                    return;
                }
                _items[index] = newValue;

                Detach(oldValue);
                Attach(newValue);
                OnCollectionChanged();
            }
        }

        /// <inheritdoc />
        public void Add(TValue item)
        {
            if (IsReadOnly)
            {
                throw NewMessageIsReadOnlyException(_items);
            }            
            _items.Add(item);

            Attach(item);
            OnCollectionChanged();
        }

        /// <inheritdoc />
        public void Insert(int index, TValue item)
        {
            if (IsReadOnly)
            {
                throw NewMessageIsReadOnlyException(_items);
            }            
            _items.Insert(index, item);

            Attach(item);
            OnCollectionChanged();
        }

        /// <inheritdoc />
        public bool Remove(TValue item)
        {
            if (IsReadOnly)
            {
                throw NewMessageIsReadOnlyException(_items);
            }
            if (Count == 0)
            {
                return false;
            }            
            if (_items.Remove(item))
            {
                Detach(item);
                OnCollectionChanged();
                return true;
            }
            return false;
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            if (IsReadOnly)
            {
                throw NewMessageIsReadOnlyException(_items);
            }
            var oldItem = _items[index];
            
            _items.RemoveAt(index);

            Detach(oldItem);
            OnCollectionChanged();
        }

        /// <inheritdoc />
        public void Clear()
        {
            if (IsReadOnly)
            {
                throw NewMessageIsReadOnlyException(_items);
            }
            if (Count == 0)
            {
                return;
            }
            var oldItems = _items.ToArray();            

            _items.Clear();

            foreach (var oldItem in oldItems)
            {
                Detach(oldItem);
            }
            OnCollectionChanged();
        }

        /// <inheritdoc />
        public bool Contains(TValue item)
        {
            return _items.Contains(item);
        }

        /// <inheritdoc />
        public int IndexOf(TValue item)
        {
            return _items.IndexOf(item);
        }

        /// <inheritdoc />
        public void CopyTo(TValue[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public IEnumerator<TValue> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }        

        #endregion

        #region [====== Attach and Detach ======]

        private void Attach(object item)
        {
            base.Attach(item as IRequestMessageViewModel);
        }

        private void Detach(object item)
        {
            base.Detach(item as IRequestMessageViewModel);
        }

        #endregion
    }
}
