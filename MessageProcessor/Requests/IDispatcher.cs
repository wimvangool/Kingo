using System;

namespace YellowFlare.MessageProcessing.Requests
{
    /// <summary>
    /// Represents a dispatcher that can be used to post messages to a UI-thread.
    /// </summary>
    public interface IDispatcher
    {
        /// <summary>
        /// Posts a message to a UI-thread.
        /// </summary>
        /// <param name="action">The action to invoke on the UI-thread.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action"/> is <c>null</c>.
        /// </exception>
        void Invoke(Action action);
    }
}
