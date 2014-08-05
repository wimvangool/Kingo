using System.ComponentModel.Messaging.Resources;

namespace System.ComponentModel.Messaging.Server
{
    /// <summary>
    /// Represents the progress of a certain task.
    /// </summary>
    [Serializable]
    public struct Progress : IEquatable<Progress>, IComparable<Progress>, IComparable
    {
        /// <summary>
        /// The instance that represents zero progress.
        /// </summary>
        public static readonly Progress MinValue = new Progress(_MinValue);

        /// <summary>
        /// The instance that represents a completed task.
        /// </summary>
        public static readonly Progress MaxValue = new Progress(_MaxValue);

        private const double _MinValue = 0.0;
        private const double _MaxValue = 1.0;
        private readonly double _value;

        /// <summary>
        /// Initializes a new instance of a <see cref="Progress" /> structure.
        /// </summary>
        /// <param name="value">Value of this structure.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is less than <c>0.0</c> or greater than <c>1.0</c>.
        /// </exception>
        public Progress(double value)
        {
            if (value < _MinValue || value > _MaxValue)
            {
                throw NewValueOutOfRangeException("value", value);
            }
            _value = value;
        }

        #region [====== Object Identity ======]

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is an instance of <see cref="Progress" />
        /// and equals the value of this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is Progress)
            {
                return Equals((Progress) obj);
            }
            return false;
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="Progress" /> value.
        /// </summary>
        /// <param name="other">A <see cref="Progress" /> value to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="other"/> has the same value as this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Progress other)
        {
            return _value.Equals(other._value);
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return _value.GetHashCode();
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
        /// <paramref name="obj"/> is not an instance of type <see cref="Progress" />.
        /// </exception>
        int IComparable.CompareTo(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            try
            {
                return CompareTo((Progress) obj);
            }
            catch (InvalidCastException)
            {
                throw NewIncomparableTypeException(obj);
            }
        }

        /// <summary>
        /// Compares this instance to another <see cref="Progress"/>-value and returns an
        /// indication of their relative values.
        /// </summary>
        /// <param name="other">Another value</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and <paramref name="other" />.
        /// </returns>
        public int CompareTo(Progress other)
        {
            return _value.CompareTo(other._value);
        }

        #endregion   
     
        #region [====== Conversion ======]

        /// <summary>
        /// Converts this instance to a <see cref="Double" />.
        /// </summary>
        /// <returns>A double-value of this instance.</returns>
        public double ToDouble()
        {
            return _value;
        }

        /// <summary>
        /// Converts this instance to an <see cref="Int32" />.
        /// </summary>
        /// <returns>An int-value of this instance.</returns>
        public int ToInt32()
        {
            return (int) Math.Round(_value);
        }

        #endregion

        #region [====== Formatting and Parsing ======]

        /// <summary>Converts this value to its equivalent string-representation.</summary>
        /// <returns>The string-representation of this value.</returns>
        public override string ToString()
        {
            return _value.ToString();
        }

        /// <summary>Converts a string-representation back to its <see cref="Progress" /> equivalent.</summary>
        /// <param name="value">String-representation of a <see cref="Progress" /> instance.</param>
        /// <returns>The <see cref="Progress" />-equivalent of the specified <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="value"/> is not of the correct format.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is not in the range of valid values.
        /// </exception>
        public static Progress Parse(string value)
        {
            return new Progress(double.Parse(value));
        }

        /// <summary>
        /// Converts the string representation of a number to its 32-bit signed integer equivalent.
        /// A return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">String-representation of a <see cref="Progress" /> instance.</param>
        /// <param name="result">
        /// When this method returns, contains the converted value of <paramref name="value"/>, if the
        /// conversion succeeded, or the default value if the conversion failed. The conversion fails
        /// if <paramref name="value"/> is null, is not of the correct format, or is not in the range of
        /// valid values. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="value"/> was converted successfully; otherwise, <c>false</c>.
        /// </returns>
        public static bool TryParse(string value, out Progress result)
        {
            double progressValue;

            if (double.TryParse(value, out progressValue))
            {
                result = new Progress(progressValue);
                return true;
            }
            result = new Progress();
            return false;
        }

        #endregion

        #region [====== Calculation ======]

        /// <summary>
        /// Calculates and returns a new <see cref="Progress" /> value based on a total amount of work and the progress that has been made.
        /// </summary>
        /// <param name="total">The total amount of work.</param>
        /// <param name="progress">The progress that has been made relative to the total.</param>
        /// <returns>A calculated <see cref="Progress" />.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="total"/> or <paramref name="progress"/> are negative, or <paramref name="progress"/>
        /// exceeds <paramref name="total"/>.
        /// </exception>
        public static Progress Calculate(int total, int progress)
        {
            return Calculate((double) total, progress);
        }

        /// <summary>
        /// Calculates and returns a new <see cref="Progress" /> value based on a total amount of work and the progress that has been made.
        /// </summary>
        /// <param name="total">The total amount of work.</param>
        /// <param name="progress">The progress that has been made relative to the total.</param>
        /// <returns>A calculated <see cref="Progress" />.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="total"/> or <paramref name="progress"/> are negative, or <paramref name="progress"/>
        /// exceeds <paramref name="total"/>.
        /// </exception>
        public static Progress Calculate(double total, double progress)
        {
            if (total < _MinValue)
            {
                throw NewNegativeValueException("total", total);
            }
            if (progress < _MinValue)
            {
                throw NewNegativeValueException("progress", progress);
            }
            return new Progress(progress / total);
        }

        #endregion

        #region [====== Exception Factory Methods ======]

        private static Exception NewValueOutOfRangeException(string paramName, double value)
        {
            var messageFormat = ExceptionMessages.Progress_ValueOutOfRange;
            var message = string.Format(messageFormat, value, _MinValue, _MaxValue);
            return new ArgumentOutOfRangeException(paramName, message);
        }

        private static Exception NewNegativeValueException(string paramName, double value)
        {
            var messageFormat = ExceptionMessages.Progress_NegativeValue;
            var message = string.Format(messageFormat, paramName, value);
            return new ArgumentOutOfRangeException(paramName, message);
        }

        private static Exception NewIncomparableTypeException(object obj)
        {
            var messageFormat = ExceptionMessages.Comparable_IncompatbileType;
            var message = string.Format(messageFormat, obj.GetType(), typeof(Progress));
            return new ArgumentException(message, "obj");
        }

        #endregion

        #region [====== Operator Overloads ======]

        /// <summary>
        /// Determines whether two specified <see cref="Progress" />-instances have the same value.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare</param>
        /// <returns><c>true</c> if both instances have the same value; otherwise <c>false</c>.</returns>
        public static bool operator ==(Progress left, Progress right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two specified <see cref="Progress" />-instances do not have the same value.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare</param>
        /// <returns><c>true</c> if both instances do not have the same value; otherwise <c>false</c>.</returns>
        public static bool operator !=(Progress left, Progress right)
        {
            return !left.Equals(right);
        }

        /// <summary>Determines whether one value is smaller than another.</summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <c>true</c> if the left operand is smaller than the right operand; otherwise <c>false</c>.
        /// </returns>
        public static bool operator <(Progress left, Progress right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>Determines whether one value is greater than another.</summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns><c>true</c> if the left operand is greater than the right operand; otherwise <c>false</c>.</returns>
        public static bool operator >(Progress left, Progress right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>Determines whether one value is smaller than or equal to another.</summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <c>true</c> if the left operand is smaller than or equal to the right operand; otherwise <c>false</c>.
        /// </returns>
        public static bool operator <=(Progress left, Progress right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>Determines whether one value is greater than or equal to another.</summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>
        /// <c>true</c> if the left operand is greater than or equal to the right operand; otherwise <c>false</c>.
        /// </returns>
        public static bool operator >=(Progress left, Progress right)
        {
            return left.CompareTo(right) >= 0;
        }

        /// <summary>
        /// Implicitly converts the specified <paramref name="value"/> to a <see cref="Double" />.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The double-value of the <paramref name="value"/>.</returns>
        public static implicit operator double(Progress value)
        {
            return value.ToDouble();
        }

        /// <summary>
        /// Implicitly converts the specified <paramref name="value"/> to an <see cref="Int32" />.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The int-value of the <paramref name="value"/>.</returns>
        public static implicit  operator int(Progress value)
        {
            return value.ToInt32();
        }

        #endregion
    }
}