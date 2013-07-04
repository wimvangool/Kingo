using System;
using System.Globalization;
using YellowFlare.MessageProcessing.Resources;

namespace YellowFlare.MessageProcessing.Aggregates
{
    public struct AggregateVersion : IEquatable<AggregateVersion>, IComparable<AggregateVersion>, IComparable
    {
        private readonly int _value;

        public AggregateVersion(int value)
        {
            if (value < 0)
            {
                throw NewInvalidVersionException(value);
            }
            _value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj is AggregateVersion)
            {
                return Equals((AggregateVersion) obj);
            }
            return false;
        }

        public bool Equals(AggregateVersion other)
        {
            return _value.Equals(other._value);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        int IComparable.CompareTo(object obj)
        {
            try
            {
                return _value.CompareTo(((AggregateVersion) obj)._value);
            }
            catch (InvalidCastException)
            {
                throw NewInvalidInstanceException(obj);
            }
        }

        public int CompareTo(AggregateVersion other)
        {
            return _value.CompareTo(other._value);
        }        

        public int ToInt32()
        {
            return _value;
        }

        public override string ToString()
        {
            return _value.ToString(CultureInfo.InvariantCulture);
        }

        public AggregateVersion Increment()
        {
            checked
            {
                return new AggregateVersion(_value + 1);
            }
        }

        public static readonly AggregateVersion Zero = new AggregateVersion(0);

        public static AggregateVersion Increment(ref AggregateVersion version)
        {
            return version = version.Increment();
        }

        private static Exception NewInvalidVersionException(int value)
        {
            var messageFormat = ExceptionMessages.Version_NegativeValue;
            var message = string.Format(CultureInfo.CurrentCulture, messageFormat, value);
            return new ArgumentOutOfRangeException("value", message);
        }

        private static Exception NewInvalidInstanceException(object obj)
        {
            var messageFormat = ExceptionMessages.Version_InvalidType;
            var message = string.Format(CultureInfo.CurrentCulture, messageFormat, typeof(AggregateVersion), obj.GetType());
            return new ArgumentException(message, "obj");
        }

        #region [====== Operator Overloads ======]

        public static bool operator ==(AggregateVersion left, AggregateVersion right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AggregateVersion left, AggregateVersion right)
        {
            return !left.Equals(right);
        }

        public static bool operator >(AggregateVersion left, AggregateVersion right)
        {
            return left._value > right._value;
        }

        public static bool operator <(AggregateVersion left, AggregateVersion right)
        {
            return left._value < right._value;
        }

        public static bool operator >=(AggregateVersion left, AggregateVersion right)
        {
            return left._value >= right._value;
        }

        public static bool operator <=(AggregateVersion left, AggregateVersion right)
        {
            return left._value <= right._value;
        }

        #endregion
    }
}
