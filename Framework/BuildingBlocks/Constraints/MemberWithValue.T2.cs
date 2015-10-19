using System;

namespace Kingo.BuildingBlocks.Constraints
{    
    internal sealed class MemberWithValue<TMessage, TValue> : IMember
    {
        private readonly IMember _member;
        private readonly TMessage _message;
        private readonly Lazy<TValue> _value;

        internal MemberWithValue(Member<TMessage, TValue> member, TMessage message)
        {
            _member = member;
            _message = message;
            _value = new Lazy<TValue>(() => member.GetValue(_message));
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
            get { return _member.Type; }
        }

        /// <summary>
        /// The value of this member.
        /// </summary>
        public TValue Value
        {
            get { return _value.Value; }
        }        
    }
}
