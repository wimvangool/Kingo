using System;

namespace Kingo
{
    /// <summary>
    /// Contains a set of tests that verify if the <see cref="IEquatable{T}" /> interface and related operators (if defined)
    /// are implemented correctly.
    /// </summary>
    /// <typeparam name="TValue">Type implementing the <see cref="IEquatable{T}" /> interface.</typeparam>
    public abstract class EquatableTestSuite<TValue> : TestSuite<EquatableTestParameters<TValue>> where TValue : IEquatable<TValue>
    {        
        internal EquatableTestSuite(ITestEngine testEngine)
        {
            if (testEngine == null)
            {
                throw new ArgumentNullException(nameof(testEngine));
            }
            TestEngine = testEngine;
            UntypedEquatableTestSuite = new EquatableTestSuite(testEngine);
        }

        private EquatableTestSuite UntypedEquatableTestSuite
        {
            get;
        }

        /// <inheritdoc />
        protected override ITestEngine TestEngine
        {
            get;
        }

        /// <inheritdoc />
        protected override void Run(EquatableTestParameters<TValue> parameters)
        {
            UntypedEquatableTestSuite.Execute(parameters.ToUntypedParameters());

            ExecuteEqualsTests(parameters);
            ExecuteOperatorTests(parameters);
        }

        #region [====== Equals Tests ======]

        internal virtual void ExecuteEqualsTests(EquatableTestParameters<TValue> parameters)
        {
            Equals_ReturnsTrue_IfInstanceIsComparedToSelf(parameters.Instance);
            Equals_ReturnsTrue_IfLeftAndRightAreEqual(parameters.Instance, parameters.EqualInstance);                        
            Equals_ReturnsFalse_IfLeftAndRightAreNotEqual(parameters.Instance, parameters.UnequalInstance);
        }        

        private void Equals_ReturnsTrue_IfInstanceIsComparedToSelf(TValue instance)
        {
            AssertIsTrue(instance.Equals(instance), TestArguments.Define(instance));
        }

        private void Equals_ReturnsTrue_IfLeftAndRightAreEqual(TValue instance, TValue equalInstance)
        {
            AssertIsTrue(instance.Equals(equalInstance), TestArguments.Define(instance, equalInstance));
            AssertIsTrue(equalInstance.Equals(instance), TestArguments.Define(equalInstance, instance));
        }        

        private void Equals_ReturnsFalse_IfLeftAndRightAreNotEqual(TValue instance, TValue unequalInstance)
        {
            AssertIsFalse(instance.Equals(unequalInstance), TestArguments.Define(instance, unequalInstance));
            AssertIsFalse(unequalInstance.Equals(instance), TestArguments.Define(unequalInstance, instance));
        }

        #endregion

        #region [====== Operator Tests ======]

        private void ExecuteOperatorTests(EquatableTestParameters<TValue> command)
        {            
            Func<TValue, TValue, bool> equalityOperator;
            
            if (typeof(TValue).TryGetEqualityOperator(out equalityOperator))
            {
                ExecuteOperatorEqualityTests(command, equalityOperator);
            }
            Func<TValue, TValue, bool> inequalityOperator;

            if (typeof(TValue).TryGetInequalityOperator(out inequalityOperator))
            {
                ExecuteOperatorInequalityTests(command, inequalityOperator);
            }
        }        

        internal virtual void ExecuteOperatorEqualityTests(EquatableTestParameters<TValue> command, Func<TValue, TValue, bool> equalityOperator)
        {
            OperatorEquality_ReturnsTrue_IfInstanceIsComparedToSelf(command.Instance, equalityOperator);
            OperatorEquality_ReturnsTrue_IfLeftIsEqualToRight(command.Instance, command.EqualInstance, equalityOperator);
            OperatorEquality_ReturnsFalse_IfLeftIsNotEqualToRight(command.Instance, command.UnequalInstance, equalityOperator);
        }

        private void OperatorEquality_ReturnsTrue_IfInstanceIsComparedToSelf(TValue instance, Func<TValue, TValue, bool> equalityOperator)
        {
            AssertIsTrue(equalityOperator.Invoke(instance, instance), TestArguments.Define(instance));
        }

        private void OperatorEquality_ReturnsTrue_IfLeftIsEqualToRight(TValue instance, TValue equalInstance, Func<TValue, TValue, bool> equalityOperator)
        {
            AssertIsTrue(equalityOperator.Invoke(instance, equalInstance), TestArguments.Define(instance, equalInstance));
            AssertIsTrue(equalityOperator.Invoke(equalInstance, instance), TestArguments.Define(equalInstance, instance));
        }

        private void OperatorEquality_ReturnsFalse_IfLeftIsNotEqualToRight(TValue instance, TValue unequalInstance, Func<TValue, TValue, bool> equalityOperator)
        {
            AssertIsFalse(equalityOperator.Invoke(instance, unequalInstance), TestArguments.Define(instance, unequalInstance));
            AssertIsFalse(equalityOperator.Invoke(unequalInstance, instance), TestArguments.Define(unequalInstance, instance));
        }        

        internal virtual void ExecuteOperatorInequalityTests(EquatableTestParameters<TValue> command, Func<TValue, TValue, bool> inequalityOperator)
        {
            OperatorInequality_ReturnsFalse_IfInstanceIsComparedToSelf(command.Instance, inequalityOperator);
            OperatorInequality_ReturnsFalse_IfLeftIsEqualToRight(command.Instance, command.EqualInstance, inequalityOperator);
            OperatorInequality_ReturnsTrue_IfLeftIsNotEqualToRight(command.Instance, command.UnequalInstance, inequalityOperator);
        }

        private void OperatorInequality_ReturnsFalse_IfInstanceIsComparedToSelf(TValue instance, Func<TValue, TValue, bool> inequalityOperator)
        {
            AssertIsFalse(inequalityOperator.Invoke(instance, instance), TestArguments.Define(instance));
        }

        private void OperatorInequality_ReturnsFalse_IfLeftIsEqualToRight(TValue instance, TValue equalInstance, Func<TValue, TValue, bool> inequalityOperator)
        {
            AssertIsFalse(inequalityOperator.Invoke(instance, equalInstance), TestArguments.Define(instance, equalInstance));
            AssertIsFalse(inequalityOperator.Invoke(equalInstance, instance), TestArguments.Define(equalInstance, instance));
        }

        private void OperatorInequality_ReturnsTrue_IfLeftIsNotEqualToRight(TValue instance, TValue unequalInstance, Func<TValue, TValue, bool> inequalityOperator)
        {
            AssertIsTrue(inequalityOperator.Invoke(instance, unequalInstance), TestArguments.Define(instance, unequalInstance));
            AssertIsTrue(inequalityOperator.Invoke(unequalInstance, instance), TestArguments.Define(unequalInstance, instance));
        }

        #endregion        
    }
}
