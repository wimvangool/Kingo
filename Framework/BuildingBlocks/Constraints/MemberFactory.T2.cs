using System;

namespace Kingo.BuildingBlocks.Constraints
{    
    internal sealed class MemberFactory<T, TValue> : Member
    {
        private readonly MemberNameComponentStack _nameComponentStack;
        private readonly Func<T, TValue> _valueFactory;

        internal MemberFactory(MemberNameComponentStack nameComponentStack, Func<T, TValue> valueFactory)            
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException("valueFactory");
            }
            _nameComponentStack = nameComponentStack;
            _valueFactory = valueFactory;
        }

        internal override MemberNameComponentStack NameComponentStack
        {
            get { return _nameComponentStack; }
        }

        public override Type Type
        {
            get { return typeof(TValue); }
        }                
        
        internal Member<TValue> CreateMember(T instance)
        {
            return new Member<TValue>(NameComponentStack, _valueFactory.Invoke(instance));
        }                

        internal MemberFactory<T, TValueOut> CreateChildMember<TValueOut>(Func<T, IMemberConstraint<TValue, TValueOut>> memberConstraintFactory)
        {
            Func<T, TValueOut> valueFactory = message =>
            {
                var member = CreateMember(message);
                var memberConstraint = memberConstraintFactory.Invoke(message);
                var exceptionFactory = new MemberExceptionFactory();
                Member<TValueOut> transformedMember;

                if (memberConstraint.IsNotSatisfiedBy(member, exceptionFactory, out transformedMember))
                {
                    throw exceptionFactory.CreateException();
                }
                return transformedMember.Value;                
            };
            return new MemberFactory<T, TValueOut>(NameComponentStack, valueFactory);
        }        
    }
}
