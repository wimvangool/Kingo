
namespace System.ComponentModel.Server
{
    /// <summary>
    /// Represents a buffer of events.
    /// </summary>
    public interface IEventBuffer
    {
        /// <summary>
        /// Flushes the event.
        /// </summary>
        void Flush();
    }
}
