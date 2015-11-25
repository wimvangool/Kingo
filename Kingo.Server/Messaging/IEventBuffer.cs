
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a buffer of events.
    /// </summary>
    public interface IEventBuffer
    {
        /// <summary>
        /// Flushes the event.
        /// </summary>
        Task FlushAsync();
    }
}
