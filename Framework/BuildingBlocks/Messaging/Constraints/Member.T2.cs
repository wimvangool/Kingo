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
        private readonly string[] _parentNames;
        private readonly string _memberName;
        private readonly Func<T, TValue> _valueFactory;

        internal Member(Func<T, TValue> valueFactory, string memberName, string[] parentNames)
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException("valueFactory");
            }
            if (memberName == null)
            {
                throw new ArgumentNullException("memberName");
            }
            _parentNames = parentNames;
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
                const string separator = ".";

                return _parentNames.Length == 0
                    ? _memberName
                    : string.Join(separator, _parentNames) + separator + _memberName;                
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
            return new Member<T, TValue>(_valueFactory, nameSelector.Invoke(_memberName), _parentNames);            
        }

        internal Member<T, TResult> Transform<TResult>(IConstraintWithErrorMessage<TValue, TResult> constraint)
        {
            Func<T, TResult> valueFactory = value =>
            {
                TResult result;
                IConstraintWithErrorMessage failedConstraint;

                if (constraint.IsSatisfiedBy(GetValue(value), out result, out failedConstraint))
                {
                    return result;
                }
                return default(TResult);
            };
            return new Member<T, TResult>(valueFactory, _memberName, _parentNames);
        }
    }
}
