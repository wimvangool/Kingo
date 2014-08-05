using System.ComponentModel.Messaging.Client;

namespace System.ComponentModel.Messaging.Server
{
    /// <summary>
    /// Arguments for the <see cref="ProgressReporter.ProgressChanged" /> event.
    /// </summary>
    public class ProgressChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The current prgress.
        /// </summary>
        public readonly Progress Progress;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressChangedEventArgs" /> class.
        /// </summary>
        /// <param name="progress">The current progress.</param>
        public ProgressChangedEventArgs(Progress progress)
        {
            Progress = progress;
        }
    }
}
