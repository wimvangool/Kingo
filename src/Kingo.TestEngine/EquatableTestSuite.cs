using System;

namespace Kingo
{
    /// <summary>
    /// Contains a set of tests that verify if the <see cref="object.Equals(object)" /> and <see cref="object.GetHashCode()" /> methods
    /// are implemented correctly.
    /// </summary>
    public sealed class EquatableTestSuite : TestSuite<EquatableTestParameters>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EquatableTestSuite" /> class.
        /// </summary>
        /// <param name="testEngine">The test engine to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="testEngine"/> is <c>null</c>.
        /// </exception>
        public EquatableTestSuite(ITestEngine testEngine)
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
        protected override void Run(EquatableTestParameters parameters)
        {
            RunEqualsTests(parameters);
            RunGetHashCodeTests(parameters);
        }

        #region [====== Equals ======]

        private void RunEqualsTests(EquatableTestParameters parameters)
        {
            Equals_ReturnsFalse_IfInstanceIsComparedToNull(parameters.Instance);
            Equals_ReturnsFalse_IfInstanceIsComparedToPlainObjectInstance(parameters.Instance);
            Equals_ReturnsFalse_IfInstanceIsComparedToUnequalInstance(parameters.Instance, parameters.UnequalInstance);

            Equals_ReturnsTrue_IfInstanceIsComparedToSelf(parameters.Instance);
            Equals_ReturnsTrue_IfInstanceIsComparedToEqualInstance(parameters.Instance, parameters.EqualInstance);
        }

        private void Equals_ReturnsFalse_IfInstanceIsComparedToNull(object instance)
        {
            AssertIsFalse(instance.Equals(null), TestArguments.Define(instance));
        }

        private void Equals_ReturnsFalse_IfInstanceIsComparedToPlainObjectInstance(object instance)
        {
            AssertIsFalse(instance.Equals(new object()), TestArguments.Define(instance));
        }

        private void Equals_ReturnsFalse_IfInstanceIsComparedToUnequalInstance(object instance, object unequalInstance)
        {
            AssertIsFalse(instance.Equals(unequalInstance), TestArguments.Define(instance, unequalInstance));
            AssertIsFalse(unequalInstance.Equals(instance), TestArguments.Define(unequalInstance, instance));
        }

        private void Equals_ReturnsTrue_IfInstanceIsComparedToSelf(object instance)
        {
            AssertIsTrue(instance.Equals(instance), TestArguments.Define(instance));
        }

        private void Equals_ReturnsTrue_IfInstanceIsComparedToEqualInstance(object instance, object equalInstance)
        {
            AssertIsTrue(instance.Equals(equalInstance), TestArguments.Define(instance, equalInstance));
            AssertIsTrue(equalInstance.Equals(instance), TestArguments.Define(equalInstance, instance));
        }

        #endregion

        #region [====== GetHashCode ======]

        private void RunGetHashCodeTests(EquatableTestParameters command)
        {
            GetHashCode_ReturnsSameValueEachTimeItIsCalled(command.Instance);
            GetHashCode_ReturnsSameValueAsGetHashCodeOfOtherInstance_IfInstancesAreEqual(command.Instance, command.EqualInstance);
        }

        private void GetHashCode_ReturnsSameValueEachTimeItIsCalled(object instance)
        {
            var arguments = TestArguments.Define(instance);
            var hashcode = instance.GetHashCode();

            for (int index = 0; index < 100; index++)
            {
                AssertAreEqual(hashcode, instance.GetHashCode(), arguments);
            }
        }

        private void GetHashCode_ReturnsSameValueAsGetHashCodeOfOtherInstance_IfInstancesAreEqual(object instance, object equalInstance)
        {
            AssertAreEqual(instance, equalInstance, TestArguments.Define(instance, equalInstance));
        }

        #endregion
    }
}
