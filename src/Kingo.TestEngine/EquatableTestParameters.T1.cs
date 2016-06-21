using System;
using Kingo.Constraints;

namespace Kingo
{
    /// <summary>
    /// Represents a set of instances that are used as input for the <see cref="EquatableTestSuite{T}"/>.
    /// </summary>
    [Serializable]
    public sealed class EquatableTestParameters<TValue> : DataTransferObject
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
        /// An instance that is not equal to <see cref="Instance" />.
        /// </summary>
        public TValue UnequalInstance
        {
            get;
            set;
        }


        /// <inheritdoc />
        public override string ToString()
        {
            return $"{nameof(Instance)} = {Instance}, {nameof(EqualInstance)} = {EqualInstance}, {nameof(UnequalInstance)} = {UnequalInstance}";
        }

        /// <summary>
        /// Coverts this instance to an untyped version.
        /// </summary>
        /// <returns>Un untyped version of these parameters.</returns>
        public EquatableTestParameters ToUntypedParameters()
        {
            return new EquatableTestParameters()
            {
                Instance = Instance,
                EqualInstance = EqualInstance,
                UnequalInstance = UnequalInstance
            };
        }

        /// <inheritdoc />
        protected override IValidator CreateValidator()
        {
            var validator = new ConstraintValidator<EquatableTestParameters<TValue>>();

            validator.VerifyThat(m => m.Instance)
                .IsNotNull()
                .IsNotSameInstanceAs(m => m.EqualInstance)
                .IsNotSameInstanceAs(m => m.UnequalInstance);

            validator.VerifyThat(m => m.EqualInstance)
                .IsNotNull()
                .IsNotSameInstanceAs(m => m.UnequalInstance);

            validator.VerifyThat(m => m.UnequalInstance)
                .IsNotNull();

            return validator;
        }
    }
}
