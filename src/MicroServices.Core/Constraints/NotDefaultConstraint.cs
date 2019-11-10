using System;

namespace Kingo.Constraints
{
    /// <summary>
    /// Represents a constraint that verifies that a value-type (struct) does not have its default value.
    /// </summary>
    public sealed class NotDefaultConstraint : Constraint<ValueType>
    {
        /// <inheritdoc />
        public override bool IsValid(ValueType value) =>
            !Activator.CreateInstance(value.GetType()).Equals(value);
    }
}
