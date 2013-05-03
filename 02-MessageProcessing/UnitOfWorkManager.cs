using System;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// Default manager to use when a <see cref="IUnitOfWork" /> has to be enlisted with the processor.
    /// </summary>
    public sealed class UnitOfWorkManager : IUnitOfWorkManager
    {
        /// <inheritdoc />
        public void Enlist(IUnitOfWork unitOfWork)
        {
            var controller = MessageProcessorContext.Current as IUnitOfWorkManager;
            if (controller == null)
            {
                throw NewFailedToEnlistUnitOfWorkException();
            }
            controller.Enlist(unitOfWork);
        }

        private static Exception NewFailedToEnlistUnitOfWorkException()
        {
            return new InvalidOperationException(ExceptionMessages.MessageProcessor_FailedToEnlistUnitOfWork);
        }
    }
}
