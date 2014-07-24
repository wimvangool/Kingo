
namespace YellowFlare.MessageProcessing.Server
{    
    /// <summary>
    /// When implemented by a class, represents a unit of work that can be flushed.
    /// </summary>    
    public interface IUnitOfWork
    {
        /// <summary>
        /// Indicates which group this unit of work belongs to.
        /// </summary>
        string FlushGroup
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
