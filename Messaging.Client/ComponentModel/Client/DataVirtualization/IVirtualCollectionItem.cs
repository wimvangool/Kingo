namespace System.ComponentModel.Client.DataVirtualization
{
    /// <summary>
    /// Represents an item of a <see cref="VirtualCollection{T}" />.
    /// </summary>	
    public interface IVirtualCollectionItem
    {
        /// <summary>
        /// Index of the item in the collection.
        /// </summary>
        int Index
        {
            get;
        }

        /// <summary>
        /// Indicates whether or not this item represents an unloaded item.
        /// </summary>
        bool IsNotLoaded
        {
            get;
        }

        /// <summary>
        /// Indicates whether or not this item represents a loaded item.
        /// </summary>
        bool IsLoaded
        {
            get;
        }

        /// <summary>
        /// Indicates whether or not there was a problem loading this item.
        /// </summary>
        bool FailedToLoad
        {
            get;
        }

        /// <summary>
        /// Returns the value of this item. If <see cref="IsNotLoaded" /> is <c>false</c>, this property will return the default value.
        /// </summary>
        object Value
        {
            get;
        }
    }
}
