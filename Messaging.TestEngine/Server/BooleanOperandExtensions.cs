using System.ComponentModel.Resources;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// Provides some extra verification method for boolean operands.
    /// </summary>
    public static class BooleanOperandExtensions
    {
        /// <summary>
        /// Verifies that the operand's value is <c>true</c>.
        /// </summary>
        /// <param name="operand">The operand to verify.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="operand"/> is <c>null</c>.
        /// </exception>
        public static void IsTrue(this IOperand<bool> operand)
        {
            if (operand == null)
            {
                throw new ArgumentNullException("operand");
            }
            if (!operand.Value)
            {
                operand.Scenario.Fail(FailureMessages.ExpressionNotTrue);
            }
        }

        /// <summary>
        /// Verifies that the operand's value is <c>false</c>.
        /// </summary>
        /// <param name="operand">The operand to verify.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="operand"/> is <c>null</c>.
        /// </exception>
        public static void IsFalse(this IOperand<bool> operand)
        {
            if (operand == null)
            {
                throw new ArgumentNullException("operand");
            }
            if (operand.Value)
            {
                operand.Scenario.Fail(FailureMessages.ExpressionNotFalse);
            }
        }
    }
}
