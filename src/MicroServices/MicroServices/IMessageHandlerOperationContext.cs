namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents the context of a <see cref="IMessageHandler{TMessage}"/> operation.
    /// </summary>
    public interface IMessageHandlerOperationContext : IMicroProcessorOperationContext
    {
        /// <summary>
        /// Represents the unit of work that is associated to the current operation.
        /// </summary>
        IUnitOfWork UnitOfWork
        {
            get;
        }

        /// <summary>
        /// Represents the message bus that can be used to schedule commands to be sent or events to be published
        /// after the operation has completed.
        /// </summary>
        IMessageBus MessageBus
        {
            get;
        }
    }
}
