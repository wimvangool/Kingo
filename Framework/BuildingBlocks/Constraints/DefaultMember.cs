using System;
using System.Linq;

namespace Kingo.BuildingBlocks.Constraints
{    
    internal sealed class DefaultMember : IMember
    {
        private const string _Name = "Value";

        private readonly IConstraintWithErrorMessage _failedConstraint;
        private readonly object _value;
        private readonly Lazy<Type> _type;

        internal DefaultMember(IConstraintWithErrorMessage failedConstraint, object value)
        {
            _failedConstraint = failedConstraint;
            _value = value;
            _type = new Lazy<Type>(DetermineType);
        }        

        public string FullName
        {
            get { return _Name; }
        }

        public string Name
        {
            get { return _Name; }
        }

        public Type Type
        {
            get { return _type.Value; }
        }

        public object Value
        {
            get { return _value; }
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
