using System;
using Kingo.Messaging;
using Kingo.Messaging.Constraints;

namespace Kingo
{
    /// <summary>
    /// Represents a set of instances that are used as input for the <see cref="ComparableTestSuite{T}"/>.
    /// </summary>
    [Serializable]
    public sealed class ComparableTestParameters<TValue> : DataTransferObject
    {
        /// <summary>
        /// A certain instance.
        /// </summary>
        public TValue Instance
        {
            get;
            set;
        }

        /// <summary>
        /// An instance that is equal to, but not the same instance as <see cref="Instance"/>.
        /// </summary>
        public TValue EqualInstance
        {
            get;
            set;
        }

        /// <summary>
        /// An instance that is larger than <see cref="Instance"/>.
        /// </summary>
        public TValue LargerInstance
        {
            get;
            set;
        }

        /// <summary>
        /// Converts this parameter instance to an instance of <see cref="EquatableTestParameters{T}"/>.
        /// </summary>
        /// <returns>An instance of <see cref="EquatableTestParameters{T}"/>.</returns>
        public EquatableTestParameters<TValue> ToEquatableTestParameters()
        {
            return new EquatableTestParameters<TValue>()
            {
                Instance = Instance,
                EqualInstance = EqualInstance,
                UnequalInstance = LargerInstance
            };
        }

        /// <inheritdoc />
        protected override IMessageValidator CreateValidator()
        {
            var validator = new ConstraintMessageValidator<ComparableTestParameters<TValue>>();

            validator.VerifyThat(m => m.Instance)
                .IsNotNull()
                .IsNotSameInstanceAs(m => m.EqualInstance)
                .IsNotSameInstanceAs(m => m.LargerInstance);

            validator.VerifyThat(m => m.EqualInstance)
                .IsNotNull()
                .IsNotSameInstanceAs(m => m.LargerInstance);

            validator.VerifyThat(m => m.LargerInstance)
                .IsNotNull();

            return validator;
        }
    }
}
