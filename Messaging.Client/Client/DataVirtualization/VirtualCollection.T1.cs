using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Resources;
using System.Linq;

namespace System.ComponentModel.Client.DataVirtualization
{
    /// <summary>
    /// Represents a read-only collection of items that is partially loaded in memory while items are accessed or iterated.
    /// </summary>
    /// <typeparam name="T">Type of the items in this collection.</typeparam>
    public class VirtualCollection<T> : ReadOnlyCollection<VirtualCollectionItem<T>>, INotifyCollectionChanged
    {               
        private readonly IVirtualCollectionPageLoader<T> _loader;        

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualCollection{T}" /> class.
        /// </summary>
        /// <param name="loader">The loader that is used to load all pages/items asynchronously.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="loader"/> is <c>null</c>.
        /// </exception>        
        public VirtualCollection(IVirtualCollectionPageLoader<T> loader)
        {
            if (loader == null)
            {
                throw new ArgumentNullException("loader");
            }
            _loader = loader;
            _loader.CountLoaded += HandleCountLoaded;
            _loader.PageFailedToLoad += HandlePageFailedToLoad;
            _loader.PageLoaded += HandlePageLoaded;            
        }

        /// <summary>
        /// Returns the <see cref="IVirtualCollectionPageLoader{T}" /> that is used by this collection to load its pages.
        /// </summary>
        protected IVirtualCollectionPageLoader<T> Loader
        {
            get { return _loader; }
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

                if (Loader.TryGetCount(out count))
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
            NotifyOfPropertyChange(() => Count);
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
                if (Loader.HasFailedToLoadItem(index))
                {
                    return VirtualCollectionItem<T>.ErrorItem;
                }
                T item;

                if (Loader.TryGetItem(index, out item))
                {
                    return new VirtualCollectionItem<T>(item);
                }
                return VirtualCollectionItem<T>.NotLoadedItem;
            }           
        }

        /// <summary>
        /// Notifies a specific page of the collection failed to load (item go from NotLoaded to Error).
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Argument that contains the index of the item that failed to load.</param>
        /// <remarks>This method is invoked when an item has failed to load.</remarks>
        protected virtual void HandlePageFailedToLoad(object sender, PageFailedToLoadEventArgs e)
        {            
            OnCollectionChanged();
        }

        /// <summary>
        /// Notifies a specific page of the collection was loaded (items go from NotLoaded to Loaded).
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Argument that contains the loaded item.</param>
        /// <remarks>This method is invoked when an item has been loaded.</remarks>
        protected virtual void HandlePageLoaded(object sender, PageLoadedEventArgs<T> e)
        {           
            OnCollectionChanged();
        }       

        #endregion

        #region [====== Read Methods ======]

        /// <inheritdoc />
        protected override int IndexOf(VirtualCollectionItem<T> item)
        {
            if (item.IsLoaded)
            {
                int index = 0;

                foreach (var itemOfCollection in Loader)
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
                return _loader.Any(itemOfCollection => Equals(itemOfCollection, item.Value));
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
            int count;

            if (_loader.TryGetCount(out count))
            {
                for (int index = 0; index < count; index++)
                {
                    yield return this[index];
                }
            }
        }

        #endregion

        #region [====== NotifyCollectionChanged ======]

        /// <inheritdoc />
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Notifies that one or more items of the collection were replaced.
        /// </summary>
        protected virtual void OnCollectionChanged()
        {
            // NB: We would to use 'Replace' but constructor currently only supports 'Reset'.
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
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
