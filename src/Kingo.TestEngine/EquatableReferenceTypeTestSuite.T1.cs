using System;

namespace Kingo
{
    /// <summary>
    /// Contains a set of tests that verify if the <see cref="IEquatable{T}" /> interface and related operators (if defined)
    /// are implemented correctly.
    /// </summary>
    /// <typeparam name="TValue">Type implementing the <see cref="IEquatable{T}" /> interface.</typeparam>
    public sealed class EquatableReferenceTypeTestSuite<TValue> : EquatableTestSuite<TValue> where TValue : class, IEquatable<TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EquatableReferenceTypeTestSuite{T}" /> class.
        /// </summary>
        /// <param name="testEngine">The test engine to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="testEngine"/> is <c>null</c>.
        /// </exception>
        public EquatableReferenceTypeTestSuite(ITestEngine testEngine)
            : base(testEngine) { }

        #region [====== Equals Tests ======]

        internal override void ExecuteEqualsTests(EquatableTestParameters<TValue> parameters)
        {
            base.ExecuteEqualsTests(parameters);

            Equals_ReturnsFalse_IfInstanceIsComparedToNull(parameters.Instance);
        }

        private void Equals_ReturnsFalse_IfInstanceIsComparedToNull(TValue instance)
        {            
            AssertIsFalse(instance.Equals(null), TestArguments.Define(instance));
        }

        #endregion

        #region [====== Operator Tests ======]

        internal override void ExecuteOperatorEqualityTests(EquatableTestParameters<TValue> command, Func<TValue, TValue, bool> equalityOperator)
        {
            EqualsOperator_ReturnsTrue_IfNullIsComparedToNull(equalityOperator);            
            EqualsOperator_ReturnsFalse_IfInstanceIsComparedToNull(command.Instance, equalityOperator);

            base.ExecuteOperatorEqualityTests(command, equalityOperator);            
        }

        private void EqualsOperator_ReturnsTrue_IfNullIsComparedToNull(Func<TValue, TValue, bool> equalityOperator)
        {
            AssertIsTrue(equalityOperator.Invoke(null, null));
        }

        private void EqualsOperator_ReturnsFalse_IfInstanceIsComparedToNull(TValue instance, Func<TValue, TValue, bool> equalityOperator)
        {
            AssertIsFalse(equalityOperator.Invoke(instance, null), TestArguments.Define(instance));
            AssertIsFalse(equalityOperator.Invoke(null, instance), TestArguments.Define(instance));
        }        

        internal override void ExecuteOperatorInequalityTests(EquatableTestParameters<TValue> command, Func<TValue, TValue, bool> inequalityOperator)
        {
            NotEqualsOperator_ReturnsFalse_IfNullIsComparedToNull(inequalityOperator);            
            NotEqualsOperator_ReturnsTrue_IfInstanceIsComparedToNull(command.Instance, inequalityOperator);

            base.ExecuteOperatorInequalityTests(command, inequalityOperator);
        }

        private void NotEqualsOperator_ReturnsFalse_IfNullIsComparedToNull(Func<TValue, TValue, bool> inequalityOperator)
        {
            AssertIsFalse(inequalityOperator.Invoke(null, null));
        }

        private void NotEqualsOperator_ReturnsTrue_IfInstanceIsComparedToNull(TValue instance, Func<TValue, TValue, bool> inequalityOperator)
        {
            AssertIsTrue(inequalityOperator.Invoke(instance, null), TestArguments.Define(inequalityOperator));
            AssertIsTrue(inequalityOperator.Invoke(null, instance), TestArguments.Define(inequalityOperator));
        }

        #endregion
    }
}
