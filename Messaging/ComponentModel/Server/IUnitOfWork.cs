
namespace System.ComponentModel.Server
{    
    /// <summary>
    /// When implemented by a class, represents a unit of work that can be flushed.
    /// </summary>    
    public interface IUnitOfWork
    {
        /// <summary>
        /// Indicates which group this <see cref="IUnitOfWork" /> belongs to when <see cref="UnitOfWorkScope.Complete()" />
        /// is called, so that can be determined on which thread this instance's <see cref="Flush()" /> will be called.
        /// </summary>
        /// <remarks>
        /// When this property returns <c>0</c>, this <see cref="IUnitOfWork" /> will never be grouped
        /// with other instances. For all other values, this id can be used to ensure that all <see cref="IUnitOfWork" />
        /// instances with the same id are flushed on the same thread.
        /// </remarks>
        int FlushGroupId
        {
            get;
        }

        /// <summary>
        /// Indicates whether or not the controller may flush this unit of work on a thread different than it was
        /// created on.
        /// </summary>
        bool CanBeFlushedAsynchronously
        {
            get;
        }

        /// <summary>
        /// Indicates whether or not the unit of work maintains any changes that need to flushed.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the current instance needs to be flushed; otherwise <c>false</c>.
        /// </returns>
        bool RequiresFlush();

        /// <summary>
        /// Flushes any pending changes to the underlying infrastructure.
        /// </summary>
        void Flush();
    }
}
