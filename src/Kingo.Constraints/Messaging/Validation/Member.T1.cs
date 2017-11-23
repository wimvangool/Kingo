using System;

namespace Kingo.Messaging.Validation
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

        internal override MemberNameComponentStack NameComponentStack => _nameComponentStack;

        public override Type Type => ReferenceEquals(_value, null) ? typeof(TValue) : _value.GetType();

        public TValue Value => _value;

        internal Member<TOther> Transform<TOther>(TOther other) => new Member<TOther>(NameComponentStack, other);

        internal Member<TOther> Transform<TOther>(TOther other, Identifier identifier)
        {
            var name = NameComponentStack;

            if (identifier != null)
            {
                name = name.Push(identifier);
            }
            return new Member<TOther>(name, other);
        }

        internal Member<TOther> Transform<TOther>(TOther other, IndexList indexList) => new Member<TOther>(NameComponentStack.Push(indexList), other);

        internal void WriteErrorMessageTo(IErrorMessageCollection reader, IErrorMessageBuilder errorMessage)
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
