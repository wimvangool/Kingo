using System;
using Kingo.Constraints;

namespace Kingo
{
    /// <summary>
    /// Represents a set of instances that are used as input for the <see cref="EquatableTestSuite"/>.
    /// </summary>
    [Serializable]
    public sealed class EquatableTestParameters : DataTransferObject
    {
        /// <summary>
        /// A certain instance.
        /// </summary>
        public object Instance
        {
            get;
            set;
        }

        /// <summary>
        /// An instance that is equal to, but not the same instance as <see cref="Instance"/>.
        /// </summary>
        public object EqualInstance
        {
            get;
            set;
        }

        /// <summary>
        /// An instance that is not equal to <see cref="Instance" />.
        /// </summary>
        public object UnequalInstance
        {
            get;
            set;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{nameof(Instance)} = {Instance}, {nameof(EqualInstance)} = {EqualInstance}, {nameof(UnequalInstance)} = {UnequalInstance}";
        }

        /// <inheritdoc />
        protected override IValidator CreateValidator()
        {
            var validator = new ConstraintValidator<EquatableTestParameters>();

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
