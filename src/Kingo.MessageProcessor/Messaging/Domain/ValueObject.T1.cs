using System;
using Kingo.DynamicMethods;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents an object of which its identity (or equality) is defined by the value of its members.
    /// </summary>
    /// <typeparam name="TValue">Type of the implementing class.</typeparam>
    public abstract class ValueObject<TValue> : IEquatable<TValue> where TValue : ValueObject<TValue>
    {
        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as TValue);
        }

        /// <inheritdoc />
        public bool Equals(TValue other)
        {
            return EqualsMethod.Invoke(this, other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return GetHashCodeMethod.Invoke(this);
        }
    }
}
