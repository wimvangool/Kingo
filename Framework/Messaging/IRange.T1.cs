namespace System
{
    /// <summary>
    /// When implemented by a class, represents a range of values of type <typeparamref name="TValue"/>.
    /// </summary>
    /// <typeparam name="TValue">Type of the values in the range.</typeparam>
    public interface IRange<in TValue>
    {
        /// <summary>
        /// Indicates whether or not the specified <paramref name="value"/> lies within the current range.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns><c>true</c> when <paramref name="value"/> is part of this range; otherwise <c>false</c>.</returns>
        bool Contains(TValue value);
    }
}
