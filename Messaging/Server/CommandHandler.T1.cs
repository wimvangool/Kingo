namespace System.ComponentModel.Messaging.Server
{
    /// <summary>
    /// Represents a handler that specializes in handling a specific type of command.
    /// </summary>
    /// <typeparam name="TCommand">Type of the command to handle.</typeparam>
    /// <remarks>
    /// This class first validates the command to handle and throws an <see cref="InvalidCommandException" />
    /// if validation fails.
    /// </remarks>
    public abstract class CommandHandler<TCommand> : IMessageHandler<TCommand> where TCommand : class, IMessage
    {
        void IMessageHandler<TCommand>.Handle(TCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            command.Validate();

            if (command.IsValid)
            {
                Execute(command);                
                return;
            }
            throw new InvalidCommandException(command);
        }

        /// <summary>
        /// When overridden, executes the specified <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        protected abstract void Execute(TCommand command);              
    }
}
