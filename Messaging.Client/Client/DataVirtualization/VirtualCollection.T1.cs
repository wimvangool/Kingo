using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Messaging.Resources;
using System.Linq;
using System.Text;

namespace System.ComponentModel.Messaging.Client.DataVirtualization
{
    /// <summary>
    /// Represents a read-only collection of items that is partially loaded in memory while items are accessed or iterated.
    /// </summary>
    /// <typeparam name="T">Type of the items in this collection.</typeparam>
    public class VirtualCollection<T> : ReadOnlyCollection<VirtualCollectionItem<T>>, INotifyCollectionChanged
    {               
        private readonly IVirtualCollectionImplementation<T> _implementation;
        private readonly IndexSet _errorItemCollection;               

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualCollection{T}" /> class.
        /// </summary>
        /// <param name="implementation">The provider that is used to load all items asynchronously.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="implementation"/> is <c>null</c>.
        /// </exception>        
        public VirtualCollection(IVirtualCollectionImplementation<T> implementation)
        {
            if (implementation == null)
            {
                throw new ArgumentNullException("implementation");
            }
            _implementation = implementation;
            _implementation.CountLoaded += HandleCountLoaded;
            _implementation.ItemFailedToLoad += HandleItemFailedToLoad;
            _implementation.ItemLoaded += HandleItemLoaded;

            _errorItemCollection = new IndexSet();
        }

        #region [====== List - Count ======]

        /// <summary>
        /// Returns the size of this collection.
        /// </summary>
        /// <remarks>
        /// If the size of this collection has not yet been determined, this property will return <c>0</c> and then
        /// trigger an action that will determine the actual size of this collection. 
        /// </remarks>
        public override int Count
        {
            get
            {
                int count;

                if (_implementation.TryGetCount(out count))
                {
                    return count;
                }
                return 0;
            }              
        }  
        
        /// <summary>
        /// Notifies the <see cref="Count" /> property and the whole collection has changed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Argument that contains the loaded count value.</param>
        /// <remarks>This method is invoked when the count has been (re)loaded.</remarks>
        protected virtual void HandleCountLoaded(object sender, CountLoadedEventArgs e)
        {
            OnPropertyChanged(() => Count);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        #endregion

        #region [====== List - Indexer ======]

        /// <summary>
        /// Returns the item at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">Index of the item.</param>
        /// <returns>
        /// The loaded item if this item has been loaded, or an error-item if a problem occurred while loading the item.
        /// In any other case, it will return an item that indicates the item is not (yet) loaded.
        /// </returns>        
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is negative or larger than largest index of this collection.
        /// </exception>        
        public override VirtualCollectionItem<T> this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw NewIndexOutOfRangeException(index);
                }
                if (_errorItemCollection.Contains(index))
                {
                    return VirtualCollectionItem<T>.ErrorItem;
                }
                T item;

                if (_implementation.TryGetItem(index, out item))
                {
                    return new VirtualCollectionItem<T>(item);
                }
                return VirtualCollectionItem<T>.NotLoadedItem;
            }           
        }

        /// <summary>
        /// Notifies a specific item of the collection was replaced (from NotLoaded to Error).
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Argument that contains the index of the item that failed to load.</param>
        /// <remarks>This method is invoked when an item has failed to load.</remarks>
        protected virtual void HandleItemFailedToLoad(object sender, ItemFailedToLoadEventArgs e)
        {
            _errorItemCollection.Add(e.Index);

            OnCollectionItemsReplaced();
        }

        /// <summary>
        /// Notifies a specific item of the collection was replaced (from NotLoaded to Loaded).
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Argument that contains the loaded item.</param>
        /// <remarks>This method is invoked when an item has been loaded.</remarks>
        protected virtual void HandleItemLoaded(object sender, ItemLoadedEventArgs<T> e)
        {
            _errorItemCollection.Remove(e.Index);

            OnCollectionItemsReplaced();
        }

        /// <summary>
        /// Forces a reload of all items that failed to load earlier. As such, this method notifies
        /// that one or more items of the collection were replaced (from Error to NotLoaded).
        /// </summary>
        /// <remarks>
        /// Note that this method does not cause an immediate reload of all items that failed to load earlier.
        /// Rather, it simply marks all error-items as not loaded, causing them to be reloaded once the UI
        /// requests them.
        /// </remarks>
        public virtual void ReloadErrorItems()
        {
            _errorItemCollection.Clear();

            OnCollectionItemsReplaced();
        }

        #endregion

        #region [====== Read Methods ======]

        /// <inheritdoc />
        protected override int IndexOf(VirtualCollectionItem<T> item)
        {
            if (item.IsLoaded)
            {
                int index = 0;

                foreach (var itemOfCollection in _implementation)
                {
                    if (Equals(itemOfCollection, item.Value))
                    {
                        return index;
                    }
                    index++;
                }
            }
            return -1;
        }

        /// <inheritdoc />
        protected override bool Contains(VirtualCollectionItem<T> item)
        {
            if (item.IsLoaded)
            {
                return _implementation.Any(itemOfCollection => Equals(itemOfCollection, item.Value));
            }
            return false;
        }

        /// <inheritdoc />
        protected override void CopyTo(VirtualCollectionItem<T>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override IEnumerator<VirtualCollectionItem<T>> GetEnumerator()
        {
            return _implementation.Select(item => new VirtualCollectionItem<T>(item)).GetEnumerator();
        }

        #endregion

        #region [====== NotifyCollectionChanged ======]

        /// <inheritdoc />
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Notifies that one or more items of the collection were replaced.
        /// </summary>
        protected virtual void OnCollectionItemsReplaced()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace));
        }

        /// <summary>
        /// Raises the <see cref="CollectionChanged" /> event.
        /// </summary>
        /// <param name="e">Arguments of the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="e"/> is <c>null</c>.
        /// </exception>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged.Raise(this, e);
        }

        #endregion

        #region [====== Exception Factory Methods ======]

        private static Exception NewIndexOutOfRangeException(int index)
        {
            var messageFormat = ExceptionMessages.Object_IndexOutOfRange;
            var message = string.Format(messageFormat, index);
            return new ArgumentOutOfRangeException("index", message);
        }

        #endregion
    }
}
