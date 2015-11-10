using System;

namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class Member<TValue> : Member
    {        
        private readonly TValue _value;

        internal Member(MemberNameComponentStack nameComponentStack, TValue value)
            : base(nameComponentStack)
        {            
            _value = value;
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
            return new Member<TOther>(NameComponentStack.Push(identifier), other);
        }

        internal Member<TOther> Transform<TOther>(TOther other, IndexList indexList)
        {
            return new Member<TOther>(NameComponentStack.Push(indexList), other);
        }

        internal void WriteErrorMessageTo(IErrorMessageReader reader, IErrorMessage errorMessage)
        {
            var memberName = NameComponentStack;

            do
            {
                reader.Add(errorMessage, memberName.ToString());
            }
            while (memberName.Pop(out memberName));            
        }        
    }
}
