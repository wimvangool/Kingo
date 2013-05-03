using System;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// When implemented by a class, acts as one part of the pipeline that is constructed to execute a command.
    /// </summary>
    public interface IMessageHandlerBehavior
    {
        /// <summary>
        /// Executes the specified command while adding some extra logic to the pipeline.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="command"/> is <c>null</c>.
        /// </exception>
        /// <remarks>
        /// When a class implements this method, be sure to call the specified command's
        /// <see cref="IMessageCommand.Execute()" /> at the appropriate time, or the pipeline will be broken.
        /// </remarks>
        void Execute(IMessageCommand command);
    }
}
