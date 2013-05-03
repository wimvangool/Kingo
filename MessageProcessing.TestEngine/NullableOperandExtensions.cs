using System;
using System.Diagnostics.CodeAnalysis;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// Provides some extra verification method for nullable operands.
    /// </summary>
    public static class NullableOperandExtensions
    {
        /// <summary>
        /// Verifies that the nullable operand's has a value and returns a statement that can be used to verify this value.
        /// </summary>
        /// <param name="operand">The operand to verify.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="operand"/> is <c>null</c>.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static IConjunctionStatement<TValue> HasValue<TValue>(this IOperand<TValue?> operand) where TValue : struct
        {
            if (operand == null)
            {
                throw new ArgumentNullException("operand");
            }
            if (operand.Value.HasValue)
            {
                return new ConjunctionStatement<TValue>(operand.Value.Value, true);
            }
            operand.Scenario.Fail(FailureMessages.ExpressionHasNoValue, typeof(TValue));
            return new ConjunctionStatement<TValue>(default(TValue), false);
        }

        /// <summary>
        /// Verifies that the nullable operand's has no value.
        /// </summary>
        /// <param name="operand">The operand to verify.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="operand"/> is <c>null</c>.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static void HasNoValue<TValue>(this IOperand<TValue?> operand) where TValue : struct
        {
            if (operand == null)
            {
                throw new ArgumentNullException("operand");
            }
            if (operand.Value.HasValue)
            {
                operand.Scenario.Fail(FailureMessages.ExpressionHasValue, typeof(TValue), operand.Value.Value);
            }
        }
    }
}
