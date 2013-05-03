using System;
using System.Diagnostics.CodeAnalysis;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// Represents a statement that allows one to drill down into nested verification when required.
    /// </summary>
    /// <typeparam name="TValue">Type of the value to verify.</typeparam>
    public interface IConjunctionStatement<out TValue>
    {
        /// <summary>
        /// This method allows one to specify a nested verification-statement, given a certain value.
        /// </summary>
        /// <param name="statement">Nested verification statement.</param>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "And", Justification = "There's no good alternative.")]
        void And(Action<TValue> statement);
    }
}
