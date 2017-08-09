using System;
using Kingo.Resources;

namespace Kingo.Messaging.Validation
{
    /// <summary>
    /// Represents a level of severity of an error message in relation to a certain member. The level
    /// itself indicates whether or not the error is associated to the member itself (<c>0</c>), or is inherited
    /// through one of its child-members (1 or higher).
    /// </summary>
    [Serializable]
    public struct ErrorInheritanceLevel : IEquatable<ErrorInheritanceLevel>, IComparable<ErrorInheritanceLevel>, IComparable
    {
        /// <summary>
        /// Represent a direct association of an error with a member.
        /// </summary>
        public static readonly ErrorInheritanceLevel NotInherited = new ErrorInheritanceLevel(0);

        /// <summary>
        /// Represents the maximum level of inheritance, thus the weakest association possible.
        /// </summary>
        public static readonly ErrorInheritanceLevel MaxInherited = new ErrorInheritanceLevel(int.MaxValue);

        private readonly int _value;

        /// <summary>
        /// Initializes a new instance of a <see cref="ErrorInheritanceLevel" /> structure.
        /// </summary>
        /// <param name="value">Value of this structure.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is negative.
        /// </exception>
        public ErrorInheritanceLevel(int value)
        {
            if (value < 0)
            {
                throw NewInvalidErrorLevelException(value);
            }
            _value = value;
        }

        /// <summary>
        /// Increments the current inheritance level by one.
        /// </summary>
        /// <returns>An incremented inheritance level.</returns>
        public ErrorInheritanceLevel Increment() =>
            new ErrorInheritanceLevel(_value + 1);

        private static Exception NewInvalidErrorLevelException(int value)
        {
            var messageFormat = ExceptionMessages.ErrorLevel_InvalidErrorLevel;
            var message = string.Format(messageFormat, value);
            return new ArgumentOutOfRangeException(nameof(value), message);
        }

        #region [====== Equals, Compare & GetHashCode ======]

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is ErrorInheritanceLevel)
            {
                return Equals((ErrorInheritanceLevel) obj);
            }
            return false;
        }

        /// <inheritdoc />
        public bool Equals(ErrorInheritanceLevel other)
        {
            return _value.Equals(other._value);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _value;
        }

        /// <summary>
        /// Compares this instance to a specified object and returns an indication of their relative values.
        /// </summary>
        /// <param name="obj">An object to compare.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and <paramref name="obj" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="obj"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="obj"/> is not an instance of type <see cref="ErrorInheritanceLevel" />.
        /// </exception>
        int IComparable.CompareTo(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            try
            {
                return CompareTo((ErrorInheritanceLevel) obj);
            }
            catch (InvalidCastException)
            {
                throw NewIncomparableTypeException(obj);
            }
        }

        /// <summary>
        /// Compares this instance to another <see cref="ErrorInheritanceLevel"/>-value and returns an
        /// indication of their relative values.
        /// </summary>
        /// <param name="other">Another value</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and <paramref name="other" />.
        /// </returns>
        public int CompareTo(ErrorInheritanceLevel other)
        {
            return _value.CompareTo(other._value);
        }

        private static Exception NewIncomparableTypeException(object obj)
        {            
            string messageFormat = ExceptionMessages.Comparable_IncomparableType;
            string message = string.Format(messageFormat, obj.GetType(), typeof(ErrorInheritanceLevel));
            return new ArgumentException(message, "obj");
        }

        #endregion

        #region [====== Formatting and Parsing ======]

        /// <summary>
        /// Converts the error level back to an integer.
        /// </summary>
        /// <returns>The integer value of this error level.</returns>
        public int ToInt32()
        {
            return _value; ;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return _value.ToString();
        }        

        #endregion

        #region [====== Operator Overloads ======]

        /// <summary>
        /// Determines whether two specified <see cref="ErrorInheritanceLevel" />-instances have the same value.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare</param>
        /// <returns><c>true</c> if both instances have the same value; otherwise <c>false</c>.</returns>
        public static bool operator ==(ErrorInheritanceLevel left, ErrorInheritanceLevel right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two specified <see cref="ErrorInheritanceLevel" />-instances do not have the same value.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare</param>
        /// <returns><c>true</c> if both instances do not have the same value; otherwise <c>false</c>.</returns>
        public static bool operator !=(ErrorInheritanceLevel left, ErrorInheritanceLevel right)
        {
            return !left.Equals(right);
        }

        /// <summary>Determines whether one value is smaller than another.</summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <c>true</c> if the left operand is smaller than the right operand; otherwise <c>false</c>.
        /// </returns>
        public static bool operator <(ErrorInheritanceLevel left, ErrorInheritanceLevel right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>Determines whether one value is greater than another.</summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns><c>true</c> if the left operand is greater than the right operand; otherwise <c>false</c>.</returns>
        public static bool operator >(ErrorInheritanceLevel left, ErrorInheritanceLevel right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>Determines whether one value is smaller than or equal to another.</summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <c>true</c> if the left operand is smaller than or equal to the right operand; otherwise <c>false</c>.
        /// </returns>
        public static bool operator <=(ErrorInheritanceLevel left, ErrorInheritanceLevel right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>Determines whether one value is greater than or equal to another.</summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <c>true</c> if the left operand is greater than or equal to the right operand; otherwise <c>false</c>.
        /// </returns>
        public static bool operator >=(ErrorInheritanceLevel left, ErrorInheritanceLevel right)
        {
            return left.CompareTo(right) >= 0;
        }

        #endregion        
    }
}