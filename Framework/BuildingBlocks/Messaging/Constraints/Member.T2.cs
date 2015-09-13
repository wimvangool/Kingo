﻿using System;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{    
    internal sealed class Member<TMessage, TValue> : IMember
    {
        private readonly string[] _parentNames;
        private readonly string _memberName;
        private readonly Func<TMessage, TValue> _valueFactory;

        internal Member(Func<TMessage, TValue> valueFactory, string memberName, string[] parentNames)
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

        internal MemberWithValue<TMessage, TValue> ToMember(TMessage message)
        {
            return new MemberWithValue<TMessage, TValue>(this, message);
        }
        
        internal TValue GetValue(TMessage message)
        {
            return _valueFactory.Invoke(message);
        }

        internal Member<TMessage, TValue> Rename(Func<string, string> nameSelector = null)
        {
            if (nameSelector == null)
            {
                return this;
            }
            return new Member<TMessage, TValue>(_valueFactory, nameSelector.Invoke(_memberName), _parentNames);            
        }

        internal Member<TMessage, TResult> Transform<TResult>(IConstraintWithErrorMessage<TMessage, TValue, TResult> constraint)
        {
            Func<TMessage, TResult> valueFactory = message =>
            {
                TResult result;
                IConstraintWithErrorMessage<TMessage> failedConstraint;

                if (constraint.IsSatisfiedBy(GetValue(message), message, out result, out failedConstraint))
                {
                    return result;
                }
                return default(TResult);
            };
            return new Member<TMessage, TResult>(valueFactory, _memberName, _parentNames);
        }
    }
}
