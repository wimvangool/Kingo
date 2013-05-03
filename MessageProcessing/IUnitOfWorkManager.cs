
namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// When implemented by a class, manages all enlisted units of work by flushing them at the appropriate time.
    /// </summary>
    public interface IUnitOfWorkManager
    {
        /// <summary>
        /// Enlists the specified unit of work to this controller.
        /// </summary>
        /// <param name="unitOfWork">The unit of work to enlist.</param>
        /// <remarks>
        /// The controller will not flush the unit of work multiple times due to multiple enlistments.
        /// </remarks>
        void Enlist(IUnitOfWork unitOfWork);
    }
}
