using System;

namespace Syztem.ComponentModel
{
    /// <summary>
    /// Represents a wrapper-message for simple value-types.
    /// </summary>
    /// <typeparam name="TValue">Type of the value carries by the message.</typeparam>
    public sealed class ValueMessage<TValue> : Message<ValueMessage<TValue>> where TValue : struct, IEquatable<TValue>
    {
        private readonly TValue _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueMessage{TValue}" /> class.
        /// </summary>
        /// <param name="value">The value carried by this message.</param>
        public ValueMessage(TValue value)
        {
            _value = value;
        }

        /// <summary>
        /// The value carried by this message.
        /// </summary>
        public TValue Value
        {
            get { return _value; }
        }

        /// <inheritdoc />
        public override ValueMessage<TValue> Copy()
        {
            return new ValueMessage<TValue>(_value);
        }

        #region [====== Equals ======]

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as ValueMessage<TValue>);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>
        /// <c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(ValueMessage<TValue> other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            if (ReferenceEquals(other, this))
            {
                return true;
            }
            return _value.Equals(other._value);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        #endregion
    }
}
