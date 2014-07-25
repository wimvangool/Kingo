using System.ComponentModel.Messaging.Resources;

namespace System.ComponentModel.Messaging.Server
{
    /// <summary>
    /// Provides some extra verification methods for values that implement the <see cref="IComparable{T}" /> interface.
    /// </summary>
    public static class ComparableOperandExtensions
    {
        /// <summary>
        /// Verifies that the operand's value is smaller than the specified value.
        /// </summary>
        /// <param name="left">The operand.</param>
        /// <param name="right">Another value.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="left"/> is <c>null</c>.
        /// </exception>
        public static void IsSmallerThan<T>(this IOperand<T> left, T right) where T : IComparable<T>
        {
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }
            if (left.Value.CompareTo(right) < 0)
            {
                return;
            }
            left.Scenario.Fail(FailureMessages.ExpressionNotSmaller, right, left.Value);
        }

        /// <summary>
        /// Verifies that the operand's value is smaller than or equal to the specified value.
        /// </summary>
        /// <param name="left">The operand.</param>
        /// <param name="right">Another value.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="left"/> is <c>null</c>.
        /// </exception>
        public static void IsSmallerThanOrEqualTo<T>(this IOperand<T> left, T right) where T : IComparable<T>
        {
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }
            if (left.Value.CompareTo(right) <= 0)
            {
                return;
            }
            left.Scenario.Fail(FailureMessages.ExpressionNotSmallerOrEqual, right, left.Value);
        }

        /// <summary>
        /// Verifies that the operand's value is greater than the specified value.
        /// </summary>
        /// <param name="left">The operand.</param>
        /// <param name="right">Another value.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="left"/> is <c>null</c>.
        /// </exception>
        public static void IsGreaterThan<T>(this IOperand<T> left, T right) where T : IComparable<T>
        {
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }
            if (left.Value.CompareTo(right) > 0)
            {
                return;
            }
            left.Scenario.Fail(FailureMessages.ExpressionNotGreater, right, left.Value);
        }

        /// <summary>
        /// Verifies that the operand's value is greater than or equal to the specified value.
        /// </summary>
        /// <param name="left">The operand.</param>
        /// <param name="right">Another value.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="left"/> is <c>null</c>.
        /// </exception>
        public static void IsGreaterThanOrEqualTo<T>(this IOperand<T> left, T right) where T : IComparable<T>
        {
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }
            if (left.Value.CompareTo(right) >= 0)
            {
                return;
            }
            left.Scenario.Fail(FailureMessages.ExpressionNotGreaterOrEqual, right, left.Value);
        }
    }
}
