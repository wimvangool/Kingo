using System;
using System.Linq;

namespace Kingo.Messaging.Validation
{    
    internal sealed class DefaultMember : Member
    {
        private static readonly Identifier _DefaultName = Identifier.Parse("Value");

        private readonly Lazy<MemberNameComponentStack> _nameComponentStack;
        private readonly Lazy<Type> _type;
        private readonly IConstraintWithErrorMessage _failedConstraint;
        private readonly object _value;        

        internal DefaultMember(IConstraintWithErrorMessage failedConstraint, object value)            
        {
            _nameComponentStack = new Lazy<MemberNameComponentStack>(DefaultName);
            _type = new Lazy<Type>(DetermineType);
            _failedConstraint = failedConstraint;
            _value = value;            
        }

        internal override MemberNameComponentStack NameComponentStack =>
             _nameComponentStack.Value;

        public override Type Type =>
             _type.Value;

        public object Value =>
             _value;

        private MemberNameComponentStack DefaultName() =>
             new IdentifierComponent(Type, _DefaultName, null);

        private Type DetermineType()
        {
            if (_value == null)
            {
                if (TryGetImplementedConstraintInterface(_failedConstraint.GetType(), out var interfaceType))
                {
                    return interfaceType.GetGenericArguments()[0];
                }
                return typeof(object);
            }
            return _value.GetType();
        }

        private static bool TryGetImplementedConstraintInterface(Type constraintType, out Type interfaceType)
        {
            var targetInterfaces = from targetInterface in constraintType.GetInterfaces()
                                   where targetInterface.IsGenericType && targetInterface.GetGenericTypeDefinition() == typeof(IConstraint<>)
                                   select targetInterface;

            return (interfaceType = targetInterfaces.FirstOrDefault()) != null;
        }        
    }
}
