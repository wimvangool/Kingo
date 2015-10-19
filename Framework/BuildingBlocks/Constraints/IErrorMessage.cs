using System;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Represents a (formattable) error message.
    /// </summary>
    public interface IErrorMessage
    {
        /// <summary>
        /// Returns the constraint that failed.
        /// </summary>
        IConstraintWithErrorMessage FailedConstraint
        {
            get;
        }

        /// <summary>
        /// Returns a string-representation of the error message formatted using the specified <paramref name="formatProvider"/>.
        /// </summary>
        /// <param name="formatProvider">
        /// The <see cref="IFormatProvider" /> to use to format the error message.
        /// If <c>null</c>, the default formatter is used.
        /// </param>
        /// <returns>A string-representation of the error message.</returns>
        string ToString(IFormatProvider formatProvider);
    }
}
