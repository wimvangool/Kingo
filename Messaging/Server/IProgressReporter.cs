namespace System.ComponentModel.Messaging.Server
{
    /// <summary>
    /// Represents a component that can report back the progress of an asynchronous task.
    /// </summary>
    /// <remarks>
    /// Implementors of this class should make sure that the <see cref="Report(Progress)" /> method
    /// correctly marshalls back the result to any listeners of the progress, which are typically running
    /// on another thread.
    /// </remarks>
    public interface IProgressReporter
    {
        /// <summary>
        /// Reports the specified <paramref name="progress"/> based on a certain amount of work.
        /// </summary>
        /// <param name="total">The total amount of work.</param>
        /// <param name="progress">The amount of finished work.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="total"/> or <paramref name="progress"/> are negative, or <paramref name="progress"/>
        /// exceeds <paramref name="total"/>.
        /// </exception>
        void Report(int total, int progress);
    
        /// <summary>
        /// Reports the specified <paramref name="progress"/> based on a certain amount of work.
        /// </summary>
        /// <param name="total">The total amount of work.</param>
        /// <param name="progress">The amount of finished work.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="total"/> or <paramref name="progress"/> are negative, or <paramref name="progress"/>
        /// exceeds <paramref name="total"/>.
        /// </exception>
        void Report(double total, double progress);

        /// <summary>
        /// Reports the specified <paramref name="progress"/> back to someone.
        /// </summary>
        /// <param name="progress">The progress that has been made.</param>
        void Report(Progress progress);
    }               
}
