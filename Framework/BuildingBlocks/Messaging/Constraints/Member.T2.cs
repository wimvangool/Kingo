using System;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    /// <summary>
    /// Represents a member of a message.
    /// </summary>
    /// <typeparam name="T">Type of the object the error messages are produced for.</typeparam>
    /// <typeparam name="TValue">Type of the member.</typeparam>
    public sealed class Member<T, TValue> : IMember
    {
        private readonly string _parentName;
        private readonly string _memberName;
        private readonly Func<T, TValue> _valueFactory;

        internal Member(Func<T, TValue> valueFactory, string memberName, string parentName = null)
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException("valueFactory");
            }
            if (memberName == null)
            {
                throw new ArgumentNullException("memberName");
            }
            _parentName = parentName;
            _memberName = memberName;
            _valueFactory = valueFactory;
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

        /// <summary>
        /// Returns the type of the value of this member.
        /// </summary>
        public Type Type
        {
            get { return typeof(TValue); }
        }

        /// <summary>
        /// Returns the value of this member.
        /// </summary>
        public TValue GetValue(T item)
        {
            return _valueFactory.Invoke(item);
        }

        internal Member<T, TValue> Rename(Func<string, string> nameSelector = null)
        {
            if (nameSelector == null)
            {
                return this;
            }
            return new Member<T, TValue>(_valueFactory, nameSelector.Invoke(_memberName), _parentName);            
        }
    }
}
