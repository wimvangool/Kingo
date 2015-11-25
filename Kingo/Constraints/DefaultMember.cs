using System;
using System.Linq;

namespace Kingo.Constraints
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

        internal override MemberNameComponentStack NameComponentStack
        {
            get { return _nameComponentStack.Value; }
        }

        public override Type Type
        {
            get { return _type.Value; }
        }

        public object Value
        {
            get { return _value; }
        }

        private MemberNameComponentStack DefaultName()
        {
            return new IdentifierComponent(Type, _DefaultName, null);
        }

        private Type DetermineType()
        {
            if (_value == null)
            {
                Type interfaceType;

                if (TryGetImplementedConstraintInterface(_failedConstraint.GetType(), out interfaceType))
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
