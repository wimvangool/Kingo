namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a state in which a specific time period
    /// can be scheduled before or in between operations.
    /// </summary>
    public interface IGivenTimeHasPassedForState
    {
        /// <summary>
        /// Specifies that an <c>x</c> number of days have passed.
        /// </summary>
        void Days();

        /// <summary>
        /// Specifies that an <c>x</c> number of hours have passed.
        /// </summary>
        void Hours();

        /// <summary>
        /// Specifies that an <c>x</c> number of minutes have passed.
        /// </summary>
        void Minutes();

        /// <summary>
        /// Specifies that an <c>x</c> number of seconds have passed.
        /// </summary>
        void Seconds();

        /// <summary>
        /// Specifies that an <c>x</c> number of milliseconds have passed.
        /// </summary>
        void Milliseconds();

        /// <summary>
        /// Specifies that an <c>x</c> number of ticks have passed.
        /// </summary>
        void Ticks();
    }
}
