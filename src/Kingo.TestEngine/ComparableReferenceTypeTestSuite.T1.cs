using System;

namespace Kingo
{
    /// <summary>
    /// Contains a set of tests that verify if the <see cref="IComparable{T}" /> and <see cref="IComparable" /> interfaces and related operators (if defined)
    /// are implemented correctly.
    /// </summary>
    /// <typeparam name="TValue">Type implementing the <see cref="IComparable{T}" /> interface.</typeparam>
    public sealed class ComparableReferenceTypeTestSuite<TValue> : ComparableTestSuite<TValue> where TValue : class, IEquatable<TValue>, IComparable<TValue>, IComparable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComparableReferenceTypeTestSuite{T}" /> class.
        /// </summary>
        /// <param name="testEngine">A test engine.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="testEngine"/> is <c>null</c>.
        /// </exception>
        public ComparableReferenceTypeTestSuite(ITestEngine testEngine) 
            : base(testEngine)
        {
            EquatableTestSuite = new EquatableReferenceTypeTestSuite<TValue>(testEngine);
        }

        private EquatableReferenceTypeTestSuite<TValue> EquatableTestSuite
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
            CompareToObject_ReturnsPositiveValue_IfObjectIsNull(parameters.Instance);

            base.ExecuteCompareToObjectTests(parameters);
        }

        private void CompareToObject_ReturnsPositiveValue_IfObjectIsNull(IComparable instance)
        {
            AssertIsTrue(0 < instance.CompareTo(null), TestArguments.Define(instance));
        }

        #endregion

        #region [====== CompareTo ======]

        internal override void ExecuteCompareToTests(ComparableTestParameters<TValue> parameters)
        {
            CompareToObject_ReturnsPositiveValue_IfRightIsNull(parameters.Instance);

            base.ExecuteCompareToTests(parameters);
        }

        private void CompareToObject_ReturnsPositiveValue_IfRightIsNull(TValue instance)
        {
            AssertIsTrue(0 < instance.CompareTo(null), TestArguments.Define(instance));
        }

        #endregion

        #region [====== Operators =======]

        internal override void ExecuteOperatorLessThanTests(ComparableTestParameters<TValue> parameters, Func<TValue, TValue, bool> lessThanOperator)
        {
            OperatorLessThan_ReturnsTrue_IfLeftIsNull_And_RightIsNot(parameters.Instance, lessThanOperator);
            OperatorLessThan_ReturnsFalse_IfLeftIsNull_And_RightIsNull(lessThanOperator);

            base.ExecuteOperatorLessThanTests(parameters, lessThanOperator);
        }

        private void OperatorLessThan_ReturnsTrue_IfLeftIsNull_And_RightIsNot(TValue instance, Func<TValue, TValue, bool> lessThanOperator)
        {
            AssertIsTrue(lessThanOperator.Invoke(null, instance), TestArguments.Define(instance));
        }

        private void OperatorLessThan_ReturnsFalse_IfLeftIsNull_And_RightIsNull(Func<TValue, TValue, bool> lessThanOperator)
        {
            AssertIsFalse(lessThanOperator.Invoke(null, null));
        }

        internal override void ExecuteOperatorLessThanOrEqualTests(ComparableTestParameters<TValue> parameters, Func<TValue, TValue, bool> lessThanOrEqualOperator)
        {
            OperatorLessThanOrEqual_ReturnsTrue_IfLeftIsNull_And_RightIsNot(parameters.Instance, lessThanOrEqualOperator);
            OperatorLessThanOrEqual_ReturnsTrue_IfLeftIsNull_And_RightIsNull(lessThanOrEqualOperator);

            base.ExecuteOperatorLessThanOrEqualTests(parameters, lessThanOrEqualOperator);
        }

        private void OperatorLessThanOrEqual_ReturnsTrue_IfLeftIsNull_And_RightIsNot(TValue instance, Func<TValue, TValue, bool> lessThanOrEqualOperator)
        {
            AssertIsTrue(lessThanOrEqualOperator.Invoke(null, instance), TestArguments.Define(instance));
        }

        private void OperatorLessThanOrEqual_ReturnsTrue_IfLeftIsNull_And_RightIsNull(Func<TValue, TValue, bool> lessThanOrEqualOperator)
        {
            AssertIsTrue(lessThanOrEqualOperator.Invoke(null, null));
        }

        internal override void ExecuteOperatorGreaterThanTests(ComparableTestParameters<TValue> parameters, Func<TValue, TValue, bool> greaterThanOperator)
        {
            OperatorGreaterThan_ReturnsFalse_IfLeftIsNull_And_RightIsNot(parameters.Instance, greaterThanOperator);
            OperatorGreaterThan_ReturnsFalse_IfLeftIsNull_And_RightIsNull(greaterThanOperator);

            base.ExecuteOperatorGreaterThanTests(parameters, greaterThanOperator);
        }

        private void OperatorGreaterThan_ReturnsFalse_IfLeftIsNull_And_RightIsNot(TValue instance, Func<TValue, TValue, bool> greaterThanOperator)
        {
            AssertIsFalse(greaterThanOperator.Invoke(null, instance), TestArguments.Define(instance));
        }

        private void OperatorGreaterThan_ReturnsFalse_IfLeftIsNull_And_RightIsNull(Func<TValue, TValue, bool> greaterThanOperator)
        {
            AssertIsFalse(greaterThanOperator.Invoke(null, null));
        }

        internal override void ExecuteOperatorGreaterThanOrEqualTests(ComparableTestParameters<TValue> parameters, Func<TValue, TValue, bool> greaterThanOrEqualOperator)
        {
            OperatorGreaterThanOrEqual_ReturnsFalse_IfLeftIsNull_And_RightIsNot(parameters.Instance, greaterThanOrEqualOperator);
            OperatorGreaterThanOrEqual_ReturnsTrue_IfLeftIsNull_And_RightIsNull(greaterThanOrEqualOperator);

            base.ExecuteOperatorGreaterThanOrEqualTests(parameters, greaterThanOrEqualOperator);
        }

        private void OperatorGreaterThanOrEqual_ReturnsFalse_IfLeftIsNull_And_RightIsNot(TValue instance, Func<TValue, TValue, bool> greaterThanOrEqualOperator)
        {
            AssertIsFalse(greaterThanOrEqualOperator.Invoke(null, instance), TestArguments.Define(instance));
        }

        private void OperatorGreaterThanOrEqual_ReturnsTrue_IfLeftIsNull_And_RightIsNull(Func<TValue, TValue, bool> greaterThanOrEqualOperator)
        {
            AssertIsTrue(greaterThanOrEqualOperator.Invoke(null, null));
        }

        #endregion
    }
}
