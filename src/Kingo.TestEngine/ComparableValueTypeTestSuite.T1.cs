using System;

namespace Kingo
{
    /// <summary>
    /// Contains a set of tests that verify if the <see cref="IComparable{T}" /> and <see cref="IComparable" /> interfaces and related operators (if defined)
    /// are implemented correctly.
    /// </summary>
    /// <typeparam name="TValue">Type implementing the <see cref="IComparable{T}" /> interface.</typeparam>
    public sealed class ComparableValueTypeTestSuite<TValue> : ComparableTestSuite<TValue>
        where TValue : struct, IEquatable<TValue>, IComparable<TValue>, IComparable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComparableValueTypeTestSuite{T}" /> class.
        /// </summary>
        /// <param name="testEngine">A test engine.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="testEngine"/> is <c>null</c>.
        /// </exception>
        public ComparableValueTypeTestSuite(ITestEngine testEngine) 
            : base(testEngine)
        {
            EquatableTestSuite = new EquatableValueTypeTestSuite<TValue>(testEngine);
        }

        private EquatableValueTypeTestSuite<TValue> EquatableTestSuite
        {
            get;
        }

        /// <inheritdoc />
        protected override void Run(ComparableTestParameters<TValue> parameters)
        {
            EquatableTestSuite.Execute(parameters.ToEquatableTestParameters());

            base.Run(parameters);
        }

        #region [====== CompareTo(Object) ======]

        internal override void ExecuteCompareToObjectTests(ComparableTestParameters<TValue> parameters)
        {
            CompareToObject_Throws_IfObjectIsNull(parameters.Instance);

            base.ExecuteCompareToObjectTests(parameters);
        }

        private void CompareToObject_Throws_IfObjectIsNull(IComparable instance)
        {
            AssertException<ArgumentNullException>(() => instance.CompareTo(null), TestArguments.Define(instance));
        }

        #endregion
    }
}
