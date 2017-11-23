using System;

namespace Kingo.Messaging.Validation
{    
    internal sealed class MemberFactory<T, TValue>
    {        
        private readonly Func<T, Member<TValue>> _memberFactory;        

        internal MemberFactory(Func<T, TValue> fieldOrProperty, Identifier fieldOrPropertyName = null)            
        {            
            if (fieldOrProperty == null)
            {
                throw new ArgumentNullException(nameof(fieldOrProperty));
            }            
            _memberFactory = instance =>
            {
                MemberNameComponentStack name = new EmptyStack(typeof(T));

                if (fieldOrPropertyName != null)
                {
                    name = name.Push(fieldOrPropertyName);
                }
                var value = fieldOrProperty.Invoke(instance);

                return new Member<TValue>(name, value);
            };
        } 
        
        private MemberFactory(Func<T, Member<TValue>> memberFactory)
        {
            _memberFactory = memberFactory;
        }
        
        internal Member<TValue> CreateMember(T instance) =>
             _memberFactory.Invoke(instance);

        internal MemberFactory<T, TValueOut> CreateChildMember<TValueOut>(Func<T, IMemberConstraint<TValue, TValueOut>> memberConstraintFactory)
        {
            Func<T, Member<TValueOut>> memberFactory = instance =>
            {
                var member = CreateMember(instance);
                var memberConstraint = memberConstraintFactory.Invoke(instance);
                var exceptionFactory = new MemberExceptionFactory();
                Member<TValueOut> transformedMember;

                if (memberConstraint.IsNotSatisfiedBy(member, exceptionFactory, out transformedMember))
                {
                    throw exceptionFactory.CreateException();
                }
                return transformedMember;               
            };
            return new MemberFactory<T, TValueOut>(memberFactory);
        }

        internal MemberFactory<TValue, TResult> Transform<TResult>(Func<TValue, TResult> fieldOrProperty, Identifier fieldOrPropertyName, Func<T> valueProvider)
        {
            Func<TValue, Member<TResult>> memberFactory = instance =>
            {
                var value = valueProvider.Invoke();
                var member = _memberFactory.Invoke(value);
                var fieldOrPropertyValue = fieldOrProperty.Invoke(member.Value);

                return member.Transform(fieldOrPropertyValue, fieldOrPropertyName);
            };
            return new MemberFactory<TValue, TResult>(memberFactory);
        }
    }
}
