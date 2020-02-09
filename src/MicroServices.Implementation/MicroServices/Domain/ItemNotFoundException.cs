using System;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// Represents an exception that is thrown when a <see cref="Repository{TKey,TVersion,TItem}"/>
    /// was unable to retrieve an item from the data store.
    /// </summary>
    [Serializable]
    public sealed class ItemNotFoundException : MessageHandlerOperationException
    {
        internal ItemNotFoundException(string message, object id) :
            base(message)
        {
            Id = id;
        }

        /// <summary>
        /// Identifier of the item that could not be found.
        /// </summary>
        public object Id
        {
            get;
        }
    }
}
