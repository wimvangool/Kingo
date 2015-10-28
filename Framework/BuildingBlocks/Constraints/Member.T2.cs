using System;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Constraints
{    
    internal sealed class Member<TMessage, TValue> : Member
    {
        private readonly string[] _parentNames;
        private readonly string _name;
        private readonly Func<TMessage, TValue> _valueFactory;

        internal Member(string[] parentNames, string name, Func<TMessage, TValue> valueFactory)            
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException("valueFactory");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
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
        
        internal TValue GetValue(TMessage message)
        {
            return _valueFactory.Invoke(message);
        }

        internal MemberByTransformation EnableTransformation()
        {
            return new MemberByTransformation(_parentNames, _name, Type);
        }

        internal Member<TMessage, TValueOut> CreateChildMember<TValueOut>(Func<TMessage, IConstraint<TValue, TValueOut>> constraintFactory)
        {
            Func<TMessage, TValueOut> valueFactory = message =>
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
            return new Member<TMessage, TValueOut>(_parentNames, _name, valueFactory);
        }

        private static Exception NewConstraintFailedException(IErrorMessage errorMessage)
        {
            var messageFormat = ExceptionMessages.Member_ConstraintFailed;
            var message = string.Format(messageFormat, errorMessage);
            return new InvalidOperationException(message);
        }
    }
}
