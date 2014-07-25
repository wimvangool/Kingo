using System.Windows.Input;

namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Represents a command that is composed of other commands that are connected and disconnected
    /// through <see cref="IConnection">connection instances</see>.
    /// </summary>
    public interface ICompositeCommand : ICommand
    {
        /// <summary>
        /// Creates and returns a new connection from this instance to the specified <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to connect to.</param>
        /// <returns>The newly created connection.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="command"/> is <c>null</c>.
        /// </exception>
        IConnection Connect(ICommand command);
    }
}
