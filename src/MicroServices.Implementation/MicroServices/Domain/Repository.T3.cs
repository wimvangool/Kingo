using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// When implemented by a class, represents a repository of items that can be identified by
    /// a unique key of type <typeparamref name="TKey"/> and are versioned with a version number
    /// or timestamp of type <typeparamref name="TVersion"/>.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier.</typeparam>
    /// <typeparam name="TVersion">Type of the version number or timestamp.</typeparam>
    /// <typeparam name="TItem">Type of the items managed by this repository.</typeparam>
    public abstract class Repository<TKey, TVersion, TItem> : Disposable, IChangeTracker
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        where TItem : class, IVersionedItem<TKey, TVersion>
    {
        private readonly string _groupId;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TKey, TVersion, TSnapshot}" /> class.
        /// </summary>
        /// <param name="groupId">
        /// If specified, defines the group this repository belongs to when it comes to saving changes
        /// to the data store.
        /// </param>
        protected Repository(string groupId = null)
        {
            _groupId = groupId ?? string.Empty;
        }

        #region [====== Read Methods ======]

        /// <summary>
        /// Reads an item from the data store and returns it.
        /// </summary>
        /// <param name="id">Unique identifier of the snapshot to retrieve.</param>
        /// <param name="context">Context of the current operation.</param>
        /// <returns>The snapshot that has the specified <paramref name="id"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ItemNotFoundException">
        /// The snapshot with the specified <paramref name="id"/> was not found.
        /// </exception>
        public async Task<TItem> GetItemByIdAsync(TKey id, IMicroProcessorOperationContext context) =>
            await GetItemByIdOrNullAsync(id, context) ?? throw NewItemNotFoundException(id);

        /// <summary>
        /// Reads an item from the data store and returns it or returns <c>null</c> if the item was not found.
        /// </summary>
        /// <param name="id">Unique identifier of the snapshot to retrieve.</param>
        /// <param name="context">Context of the current operation.</param>
        /// <returns>
        /// The snapshot that has the specified <paramref name="id"/> or <c>null</c> if the item was not found.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> is <c>null</c>.
        /// </exception>
        public Task<TItem> GetItemByIdOrNullAsync(TKey id, IMicroProcessorOperationContext context) =>
            throw new NotImplementedException();

        private static Exception NewItemNotFoundException(TKey id)
        {
            var messageFormat = ExceptionMessages.Repository_ItemNotFound;
            var message = string.Format(messageFormat, typeof(TItem), id);
            return new ItemNotFoundException(message, id);
        }

        #endregion

        #region [====== Write Methods ======]

        string IChangeTracker.GroupId =>
            _groupId;

        bool IChangeTracker.HasChanges(Guid unitOfWorkId) =>
            throw new NotImplementedException();

        void IChangeTracker.UndoChanges(Guid unitOfWorkId)
        {
            throw new NotImplementedException();
        }

        Task IChangeTracker.SaveChangesAsync(Guid unitOfWorkId) =>
            throw new NotImplementedException();

        #endregion
    }
}
