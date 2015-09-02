using System;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    /// <summary>
    /// Represents a member of a message.
    /// </summary>
    /// <typeparam name="TValue">Type of the member.</typeparam>
    public sealed class Member<TValue> : IMember
    {
        private readonly string _parentName;
        private readonly string _memberName;
        private readonly Lazy<TValue> _value;

        internal Member(Func<TValue> memberValueFactory, string memberName, string parentName = null)
        {
            if (memberValueFactory == null)
            {
                throw new ArgumentNullException("memberValueFactory");
            }
            if (memberName == null)
            {
                throw new ArgumentNullException("memberName");
            }
            _parentName = parentName;
            _memberName = memberName;
            _value = new Lazy<TValue>(memberValueFactory);
        }

        private Member(Lazy<TValue> value, string memberName, string parentName)
        {
            _parentName = parentName;
            _memberName = memberName;
            _value = value;
        }

        /// <summary>
        /// Returns the fully qualified name of this member.
        /// </summary>
        public string FullName
        {
            get
            {
                if (_parentName == null)
                {
                    return _memberName;
                }
                return string.Format("{0}.{1}", _parentName, _memberName);
            }
        }

        /// <summary>
        /// Returns the name of this member.
        /// </summary>
        public string Name
        {
            get { return _memberName; }
        }

        object IMember.Value
        {
            get { return _value.Value; }
        }

        /// <summary>
        /// Returns the type of the value of this member.
        /// </summary>
        public Type Type
        {
            get { return ReferenceEquals(Value, null) ? typeof(TValue) : Value.GetType(); }
        }

        /// <summary>
        /// Returns the value of this member.
        /// </summary>
        public TValue Value
        {
            get { return _value.Value; }
        }

        internal Member<TValue> Rename(Func<string, string> nameSelector = null)
        {
            if (nameSelector == null)
            {
                return this;
            }
            return new Member<TValue>(_value, nameSelector.Invoke(_memberName), _parentName);            
        }
    }
}
