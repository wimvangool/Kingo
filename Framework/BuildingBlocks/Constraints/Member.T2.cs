using System;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Constraints
{    
    internal sealed class Member<T, TValue> : Member
    {
        private readonly string[] _parentNames;
        private readonly string _name;        
        private readonly Func<T, TValue> _valueFactory;

        internal Member(string[] parentNames, string name, Func<T, TValue> valueFactory)            
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException("valueFactory");
            }            
            _parentNames = parentNames;
            _name = name;            
            _valueFactory = valueFactory;
        }

        protected override string[] ParentNames
        {
            get { return _parentNames; }
        }

        protected override Identifier[] FieldsOrProperties
        {
            get { return Identifier.EmptyArray; }
        }

        public override string Name
        {
            get { return _name; }
        }        
        
        public override Type Type
        {
            get { return typeof(TValue); }
        }                
        
        internal TValue GetValue(T instance)
        {
            return _valueFactory.Invoke(instance);
        }

        internal MemberByTransformation EnableTransformation()
        {
            return new MemberByTransformation(_parentNames, _name, Type);
        }

        internal Member<T, TValueOut> CreateChildMember<TValueOut>(Func<T, IFilter<TValue, TValueOut>> constraintFactory)
        {
            Func<T, TValueOut> valueFactory = message =>
            {
                var value = GetValue(message);
                var constraint = constraintFactory.Invoke(message);
                IErrorMessage errorMessage;
                TValueOut valueOut;

                if (constraint.IsNotSatisfiedBy(value, out errorMessage, out valueOut))
                {
                    throw NewConstraintFailedException(errorMessage);
                }
                return valueOut;                
            };
            return new Member<T, TValueOut>(_parentNames, _name, valueFactory);
        }

        private static Exception NewConstraintFailedException(IErrorMessage errorMessage)
        {
            var messageFormat = ExceptionMessages.Member_ConstraintFailed;
            var message = string.Format(messageFormat, errorMessage);
            return new InvalidOperationException(message);
        }
    }
}
