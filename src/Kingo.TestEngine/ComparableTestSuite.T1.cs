using System;

namespace Kingo
{
    /// <summary>
    /// Contains a set of tests that verify if the <see cref="IComparable{T}" /> and <see cref="IComparable" /> interfaces and related operators (if defined)
    /// are implemented correctly.
    /// </summary>
    /// <typeparam name="TValue">Type implementing the <see cref="IComparable{T}" /> interface.</typeparam>
    public abstract class ComparableTestSuite<TValue> : TestSuite<ComparableTestParameters<TValue>> where TValue : IEquatable<TValue>, IComparable<TValue>, IComparable
    {
        internal ComparableTestSuite(ITestEngine testEngine)
        {
            if (testEngine == null)
            {
                throw new ArgumentNullException(nameof(testEngine));
            }
            TestEngine = testEngine;
        }

        /// <inheritdoc />
        protected override ITestEngine TestEngine
        {
            get;
        }

        /// <inheritdoc />
        protected override void Run(ComparableTestParameters<TValue> parameters)
        {
            ExecuteCompareToObjectTests(parameters);
            ExecuteCompareToTests(parameters);
            ExecuteOperatorTests(parameters);
        }

        #region [====== CompareTo(Object) Tests ======]

        internal virtual void ExecuteCompareToObjectTests(ComparableTestParameters<TValue> parameters)
        {
            CompareToObject_Throws_IfObjectIsOfDifferentType(parameters.Instance);
            CompareToObject_ReturnsNegativeValue_IfLeftIsLessThanRight(parameters.Instance, parameters.LargerInstance);
            CompareToObject_ReturnsZero_IfLeftIsComparedToSelf(parameters.Instance);
            CompareToObject_ReturnsZero_IfLeftIsEqualToRight(parameters.Instance, parameters.EqualInstance);
            CompareToObject_ReturnsPositiveValueIfLeftIsGreaterThanRight(parameters.LargerInstance, parameters.Instance);
        }

        private void CompareToObject_Throws_IfObjectIsOfDifferentType(IComparable instance)
        {
            AssertException<ArgumentException>(() => instance.CompareTo(new object()), TestArguments.Define(instance));
        }

        private void CompareToObject_ReturnsNegativeValue_IfLeftIsLessThanRight(IComparable instance, object largerInstance)
        {
            AssertIsTrue(instance.CompareTo(largerInstance) < 0, TestArguments.Define(instance, largerInstance));
        }

        private void CompareToObject_ReturnsZero_IfLeftIsComparedToSelf(IComparable instance)
        {
            AssertAreEqual(0, instance.CompareTo(instance), TestArguments.Define(instance));
        }

        private void CompareToObject_ReturnsZero_IfLeftIsEqualToRight(IComparable instance, object equalInstance)
        {
            AssertAreEqual(0, instance.CompareTo(equalInstance), TestArguments.Define(instance, equalInstance));
        }

        private void CompareToObject_ReturnsPositiveValueIfLeftIsGreaterThanRight(IComparable largerInstance, object instance)
        {
            AssertIsTrue(0 < largerInstance.CompareTo(instance), TestArguments.Define(largerInstance, instance));
        }

        #endregion

        #region [====== CompareTo Tests ======]

        internal virtual void ExecuteCompareToTests(ComparableTestParameters<TValue> parameters)
        {
            CompareTo_ReturnsNegativeValue_IfLeftIsLessThanRight(parameters.Instance, parameters.LargerInstance);
            CompareTo_ReturnsZero_IfLeftIsComparedToSelf(parameters.Instance);
            CompareTo_ReturnsZero_IfLeftIsEqualToRight(parameters.Instance, parameters.EqualInstance);
            CompareTo_ReturnsPositiveValueIfLeftIsGreaterThanRight(parameters.LargerInstance, parameters.Instance);
        }

        private void CompareTo_ReturnsNegativeValue_IfLeftIsLessThanRight(TValue instance, TValue largerInstance)
        {
            AssertIsTrue(instance.CompareTo(largerInstance) < 0, TestArguments.Define(instance, largerInstance));
        }

        private void CompareTo_ReturnsZero_IfLeftIsComparedToSelf(TValue instance)
        {
            AssertAreEqual(0, instance.CompareTo(instance), TestArguments.Define(instance));
        }

        private void CompareTo_ReturnsZero_IfLeftIsEqualToRight(TValue instance, TValue equalInstance)
        {
            AssertAreEqual(0, instance.CompareTo(equalInstance), TestArguments.Define(instance, equalInstance));
        }

        private void CompareTo_ReturnsPositiveValueIfLeftIsGreaterThanRight(TValue largerInstance, TValue instance)
        {
            AssertIsTrue(0 < largerInstance.CompareTo(instance), TestArguments.Define(largerInstance, instance));
        }

        #endregion

        #region [====== Operator Tests ======]

        private void ExecuteOperatorTests(ComparableTestParameters<TValue> parameters)
        {
            Func<TValue, TValue, bool> lessThanOperator;

            if (typeof(TValue).TryGetLessThanOperator(out lessThanOperator))
            {
                ExecuteOperatorLessThanTests(parameters, lessThanOperator);
            }
            Func<TValue, TValue, bool> lessThanOrEqualOperator;

            if (typeof(TValue).TryGetLessThanOrEqualOperator(out lessThanOrEqualOperator))
            {
                ExecuteOperatorLessThanOrEqualTests(parameters, lessThanOrEqualOperator);
            }
            Func<TValue, TValue, bool> greaterThanOperator;

            if (typeof(TValue).TryGetGreaterThanOperator(out greaterThanOperator))
            {
                ExecuteOperatorGreaterThanTests(parameters, greaterThanOperator);
            }
            Func<TValue, TValue, bool> greaterThanOrEqualOperator;

            if (typeof(TValue).TryGetGreaterThanOrEqualOperator(out greaterThanOrEqualOperator))
            {
                ExecuteOperatorGreaterThanOrEqualTests(parameters, greaterThanOrEqualOperator);
            }
        }

        internal virtual void ExecuteOperatorLessThanTests(ComparableTestParameters<TValue> parameters, Func<TValue, TValue, bool> lessThanOperator)
        {
            OperatorLessThan_ReturnsTrue_IfLeftIsLessThanRight(parameters.Instance, parameters.LargerInstance, lessThanOperator);
            OperatorLessThan_ReturnsFalse_IfLeftIsComparedToSelf(parameters.Instance, lessThanOperator);
            OperatorLessThan_ReturnsFalse_IfLeftIsEqualToRight(parameters.Instance, parameters.EqualInstance, lessThanOperator);
            OperatorLessThan_ReturnsFalse_IfLeftIsGreaterThanRight(parameters.LargerInstance, parameters.Instance, lessThanOperator);
        }        

        private void OperatorLessThan_ReturnsTrue_IfLeftIsLessThanRight(TValue instance, TValue largerInstance, Func<TValue, TValue, bool> lessThanOperator)
        {
            AssertIsTrue(lessThanOperator.Invoke(instance, largerInstance), TestArguments.Define(instance, largerInstance));
        }

        private void OperatorLessThan_ReturnsFalse_IfLeftIsComparedToSelf(TValue instance, Func<TValue, TValue, bool> lessThanOperator)
        {
            AssertIsFalse(lessThanOperator.Invoke(instance, instance), TestArguments.Define(instance));
        }

        private void OperatorLessThan_ReturnsFalse_IfLeftIsEqualToRight(TValue instance, TValue equalInstance, Func<TValue, TValue, bool> lessThanOperator)
        {
            AssertIsFalse(lessThanOperator.Invoke(instance, equalInstance), TestArguments.Define(instance, equalInstance));
        }

        private void OperatorLessThan_ReturnsFalse_IfLeftIsGreaterThanRight(TValue largerInstance, TValue instance, Func<TValue, TValue, bool> lessThanOperator)
        {
            AssertIsFalse(lessThanOperator.Invoke(largerInstance, instance), TestArguments.Define(largerInstance, instance));
        }

        internal virtual void ExecuteOperatorLessThanOrEqualTests(ComparableTestParameters<TValue> parameters, Func<TValue, TValue, bool> lessThanOrEqualOperator)
        {
            OperatorLessThanOrEqual_ReturnsTrue_IfLeftIsLessThanRight(parameters.Instance, parameters.LargerInstance, lessThanOrEqualOperator);
            OperatorLessThanOrEqual_ReturnsTrue_IfLeftIsComparedToSelf(parameters.Instance, lessThanOrEqualOperator);
            OperatorLessThanOrEqual_ReturnsTrue_IfLeftIsEqualToRight(parameters.Instance, parameters.EqualInstance, lessThanOrEqualOperator);
            OperatorLessThanOrEqual_ReturnsFalse_IfLeftIsGreaterThanRight(parameters.LargerInstance, parameters.Instance, lessThanOrEqualOperator);
        }

        private void OperatorLessThanOrEqual_ReturnsTrue_IfLeftIsLessThanRight(TValue instance, TValue largerInstance, Func<TValue, TValue, bool> lessThanOrEqualOperator)
        {
            AssertIsTrue(lessThanOrEqualOperator.Invoke(instance, largerInstance), TestArguments.Define(instance, largerInstance));
        }

        private void OperatorLessThanOrEqual_ReturnsTrue_IfLeftIsComparedToSelf(TValue instance, Func<TValue, TValue, bool> lessThanOrEqualOperator)
        {
            AssertIsTrue(lessThanOrEqualOperator.Invoke(instance, instance), TestArguments.Define(instance));
        }

        private void OperatorLessThanOrEqual_ReturnsTrue_IfLeftIsEqualToRight(TValue instance, TValue equalInstance, Func<TValue, TValue, bool> lessThanOrEqualOperator)
        {
            AssertIsTrue(lessThanOrEqualOperator.Invoke(instance, equalInstance), TestArguments.Define(instance, equalInstance));
        }

        private void OperatorLessThanOrEqual_ReturnsFalse_IfLeftIsGreaterThanRight(TValue largerInstance, TValue instance, Func<TValue, TValue, bool> lessThanOrEqualOperator)
        {
            AssertIsFalse(lessThanOrEqualOperator.Invoke(largerInstance, instance), TestArguments.Define(largerInstance, instance));
        }

        internal virtual void ExecuteOperatorGreaterThanTests(ComparableTestParameters<TValue> parameters, Func<TValue, TValue, bool> greaterThanOperator)
        {
            OperatorGreaterThan_ReturnsFalse_IfLeftIsLessThanRight(parameters.Instance, parameters.LargerInstance, greaterThanOperator);
            OperatorGreaterThan_ReturnsFalse_IfLeftIsComparedToSelf(parameters.Instance, greaterThanOperator);
            OperatorGreaterThan_ReturnsFalse_IfLeftIsEqualToRight(parameters.Instance, parameters.EqualInstance, greaterThanOperator);
            OperatorGreaterThan_ReturnsTrue_IfLeftIsGreaterThanRight(parameters.LargerInstance, parameters.Instance, greaterThanOperator);
        }

        private void OperatorGreaterThan_ReturnsFalse_IfLeftIsLessThanRight(TValue instance, TValue largerInstance, Func<TValue, TValue, bool> greaterThanOperator)
        {
            AssertIsFalse(greaterThanOperator.Invoke(instance, largerInstance), TestArguments.Define(instance, largerInstance));
        }

        private void OperatorGreaterThan_ReturnsFalse_IfLeftIsComparedToSelf(TValue instance, Func<TValue, TValue, bool> greaterThanOperator)
        {
            AssertIsFalse(greaterThanOperator.Invoke(instance, instance), TestArguments.Define(instance));
        }

        private void OperatorGreaterThan_ReturnsFalse_IfLeftIsEqualToRight(TValue instance, TValue equalInstance, Func<TValue, TValue, bool> greaterThanOperator)
        {
            AssertIsFalse(greaterThanOperator.Invoke(instance, equalInstance), TestArguments.Define(instance, equalInstance));
        }

        private void OperatorGreaterThan_ReturnsTrue_IfLeftIsGreaterThanRight(TValue largerInstance, TValue instance, Func<TValue, TValue, bool> greaterThanOperator)
        {
            AssertIsTrue(greaterThanOperator.Invoke(largerInstance, instance), TestArguments.Define(largerInstance, instance));
        }

        internal virtual void ExecuteOperatorGreaterThanOrEqualTests(ComparableTestParameters<TValue> parameters, Func<TValue, TValue, bool> greaterThanOrEqualOperator)
        {
            OperatorGreaterThanOrEqual_ReturnsFalse_IfLeftIsLessThanRight(parameters.Instance, parameters.LargerInstance, greaterThanOrEqualOperator);
            OperatorGreaterThanOrEqual_ReturnsTrue_IfLeftIsComparedToSelf(parameters.Instance, greaterThanOrEqualOperator);
            OperatorGreaterThanOrEqual_ReturnsTrue_IfLeftIsEqualToRight(parameters.Instance, parameters.EqualInstance, greaterThanOrEqualOperator);
            OperatorGreaterThanOrEqual_ReturnsTrue_IfLeftIsGreaterThanRight(parameters.LargerInstance, parameters.Instance, greaterThanOrEqualOperator);
        }

        private void OperatorGreaterThanOrEqual_ReturnsFalse_IfLeftIsLessThanRight(TValue instance, TValue largerInstance, Func<TValue, TValue, bool> greaterThanOrEqualOperator)
        {
            AssertIsFalse(greaterThanOrEqualOperator.Invoke(instance, largerInstance), TestArguments.Define(instance, largerInstance));
        }

        private void OperatorGreaterThanOrEqual_ReturnsTrue_IfLeftIsComparedToSelf(TValue instance, Func<TValue, TValue, bool> greaterThanOrEqualOperator)
        {
            AssertIsTrue(greaterThanOrEqualOperator.Invoke(instance, instance), TestArguments.Define(instance));
        }

        private void OperatorGreaterThanOrEqual_ReturnsTrue_IfLeftIsEqualToRight(TValue instance, TValue equalInstance, Func<TValue, TValue, bool> greaterThanOrEqualOperator)
        {
            AssertIsTrue(greaterThanOrEqualOperator.Invoke(instance, equalInstance), TestArguments.Define(instance, equalInstance));
        }

        private void OperatorGreaterThanOrEqual_ReturnsTrue_IfLeftIsGreaterThanRight(TValue largerInstance, TValue instance, Func<TValue, TValue, bool> greaterThanOrEqualOperator)
        {
            AssertIsTrue(greaterThanOrEqualOperator.Invoke(largerInstance, instance), TestArguments.Define(largerInstance, instance));
        }

        #endregion
    }
}
