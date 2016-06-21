using System;

namespace Kingo
{
    /// <summary>
    /// Contains a set of tests that verify if the <see cref="IEquatable{T}" /> interface and related operators (if defined)
    /// are implemented correctly.
    /// </summary>
    /// <typeparam name="TValue">Type implementing the <see cref="IEquatable{T}" /> interface.</typeparam>
    public sealed class EquatableValueTypeTestSuite<TValue> : EquatableTestSuite<TValue> where TValue : struct, IEquatable<TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EquatableValueTypeTestSuite{T}" /> class.
        /// </summary>
        /// <param name="testEngine">The test engine to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="testEngine"/> is <c>null</c>.
        /// </exception>
        public EquatableValueTypeTestSuite(ITestEngine testEngine)
            : base(testEngine) { }
    }
}
