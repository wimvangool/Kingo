namespace System.ComponentModel.Messaging
{
    /// <summary>
    /// Represents a connection that can be made with an event-bus, for example.
    /// </summary>
    public interface IConnection : IDisposable
    {
        /// <summary>
        /// Opens the connection.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The connection is already open.
        /// </exception>
        void Open();

        /// <summary>
        /// Closes the connection.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The connection is already closed.
        /// </exception>
        void Close();
    }
}
