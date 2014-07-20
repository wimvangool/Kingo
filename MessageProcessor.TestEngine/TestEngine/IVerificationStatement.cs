
namespace YellowFlare.MessageProcessing.TestEngine
{
    /// <summary>
    /// When implemented by a class, represents a single verification-statement for a specific scenario.
    /// </summary>
    public interface IVerificationStatement
    {
        /// <summary>
        /// Wraps the specified value in an operand so that it can easily be verified.
        /// </summary>
        /// <typeparam name="T">Type of value to verify.</typeparam>
        /// <param name="expression">Value to verify.</param>
        /// <returns>An operand wrapping the specified value.</returns>
        IOperand<T> That<T>(T expression);               
    }
}
