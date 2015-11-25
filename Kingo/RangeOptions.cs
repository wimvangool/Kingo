using System;

namespace Kingo
{
    /// <summary>
    /// Represents a set of options that can be used to determine whether or not a <see cref="Range{TValue}" />'s
    /// boundaries are part of that range themselves.
    /// </summary>
    [Flags]
    public enum RangeOptions
    {
        /// <summary>
        /// Specifies that both boundaries are included in the range themselves.
        /// </summary>
        AllInclusive = 0,

        /// <summary>
        /// Specifies that the left boundary of not part of the range.
        /// </summary>
        LeftExclusive = 1,

        /// <summary>
        /// Specifies that the right boundary is not part of the range.
        /// </summary>
        RightExclusive = 2,

        /// <summary>
        /// Specifies that both the left and the right boundary are not part of the range.
        /// </summary>
        AllExclusive = LeftExclusive | RightExclusive
    }
}
