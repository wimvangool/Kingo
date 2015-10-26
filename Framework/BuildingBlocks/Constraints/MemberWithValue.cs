using System;

namespace Kingo.BuildingBlocks.Constraints
{    
    internal sealed class MemberWithValue : IMember
    {
        private readonly IMember _member;
        private readonly object _value;

        internal MemberWithValue(IMember member, object value)
        {
            _member = member;
            _value = value;
        }                

        /// <inheritdoc />
        public string FullName
        {
            get { return _member.FullName; }
        }

        /// <inheritdoc />
        public string Name
        {
            get { return _member.Name; }
        }

        /// <inheritdoc />
        public Type Type
        {
            get { return ReferenceEquals(_value, null) ? _member.Type : _value.GetType(); }
        }
        
        public object Value
        {
            get { return _value; }
        }

        public override string ToString()
        {
            return string.Format("{0} ({1}, {2})", FullName, Type, ToString(_value));
        }

        private static string ToString(object value)
        {
            return ReferenceEquals(value, null) ? StringTemplate.NullValue : value.ToString();
        }
    }
}
