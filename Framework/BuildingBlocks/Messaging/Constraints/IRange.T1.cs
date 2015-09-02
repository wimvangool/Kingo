namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    /// <summary>
    /// When implemented by a class, represents a range of values of type <typeparamref name="TValue"/>.
    /// </summary>
    /// <typeparam name="TValue">Type of the values in the range.</typeparam>
    public interface IRange<TValue>
    {
        /// <summary>
        /// Returns the left boundary of this range.
        /// </summary>
        TValue Left
        {
            get;
        }

        /// <summary>
        /// Returns the right boundary of the range.
        /// </summary>
        TValue Right
        {
            get;
        }

        /// <summary>
        /// Indicates whether or not <see cref="Left" /> is part of this range.
        /// </summary>
        bool IsLeftInclusive
        {
            get;
        }

        /// <summary>
        /// Indicates whether or not <see cref="Right" /> is part of this range.
        /// </summary>
        bool IsRightInclusive
        {
            get;
        }

        /// <summary>
        /// Indicates whether or not the specified <paramref name="value"/> lies within the current range.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns><c>true</c> when <paramref name="value"/> is part of this range; otherwise <c>false</c>.</returns>
        bool Contains(TValue value);
    }
}
