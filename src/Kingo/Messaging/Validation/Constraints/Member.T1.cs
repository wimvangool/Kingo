using System;

namespace Kingo.Messaging.Validation.Constraints
{
    internal sealed class Member<TValue> : Member
    {
        private readonly MemberNameComponentStack _nameComponentStack;
        private readonly TValue _value;

        internal Member(MemberNameComponentStack nameComponentStack, TValue value)            
        {
            _nameComponentStack = nameComponentStack;
            _value = value;
        }

        internal override MemberNameComponentStack NameComponentStack
        {
            get { return _nameComponentStack; }
        }

        public override Type Type
        {
            get { return ReferenceEquals(_value, null) ? typeof(TValue) : _value.GetType(); }
        }

        public TValue Value
        {
            get { return _value; }
        }

        internal Member<TOther> Transform<TOther>(TOther other)
        {            
            return new Member<TOther>(NameComponentStack, other);
        }

        internal Member<TOther> Transform<TOther>(TOther other, Identifier identifier)
        {
            var name = NameComponentStack;

            if (identifier != null)
            {
                name = name.Push(identifier);
            }
            return new Member<TOther>(name, other);
        }

        internal Member<TOther> Transform<TOther>(TOther other, IndexList indexList)
        {
            return new Member<TOther>(NameComponentStack.Push(indexList), other);
        }

        internal void WriteErrorMessageTo(IErrorMessageReader reader, IErrorMessageBuilder errorMessage)
        {
            var memberName = NameComponentStack;
            var inheritanceLevel = ErrorInheritanceLevel.NotInherited;

            do
            {
                reader.Add(errorMessage, memberName.ToString(), inheritanceLevel);
                inheritanceLevel = inheritanceLevel.Increment();
            }
            while (memberName.Pop(out memberName));            
        }        
    }
}
