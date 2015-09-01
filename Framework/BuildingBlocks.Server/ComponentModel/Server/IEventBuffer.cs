
using System.Threading.Tasks;

namespace Kingo.BuildingBlocks.ComponentModel.Server
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
