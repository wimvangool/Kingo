using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.BuildingBlocks.Constraints
{
    internal abstract class ErrorMessage : IErrorMessage
    {
        #region [====== DefaultMember ======]

        private sealed class DefaultMember : IMember
        {
            private const string _Name = "Value";

            private readonly ErrorMessage _errorMessage;
            private readonly Lazy<Type> _type;

            internal DefaultMember(ErrorMessage errorMessage)
            {
                _errorMessage = errorMessage;
                _type = new Lazy<Type>(DetermineType);
            }

            public string Key
            {
                get { return _Name; }
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

            private Type DetermineType()
            {
                Type interfaceType;

                if (TryGetImplementedConstraintInterface(_errorMessage.FailedConstraint.GetType(), out interfaceType))
                {
                    return interfaceType.GetGenericArguments()[0];
                }
                return typeof(object);
            }

            private static bool TryGetImplementedConstraintInterface(Type constraintType, out Type interfaceType)
            {
                var targetInterfaces = from targetInterface in constraintType.GetInterfaces()
                                       where targetInterface.IsGenericType && targetInterface.GetGenericTypeDefinition() == typeof(IConstraint<>)
                                       select targetInterface;

                return (interfaceType = targetInterfaces.FirstOrDefault()) != null;
            }
        }

        #endregion

        internal static readonly Identifier MemberIdentifier = Identifier.Parse("member");

        private readonly Dictionary<Identifier, object> _arguments;

        protected ErrorMessage()
        {
            _arguments = new Dictionary<Identifier, object>();
        }

        protected IDictionary<Identifier, object> Arguments
        {
            get { return _arguments; }
        }

        public abstract IConstraintWithErrorMessage FailedConstraint
        {
            get;
        }

        public abstract object FailedValue
        {
            get;
        }

        public void Add(string name, object argument)
        {
            Add(Identifier.ParseOrNull(name), argument);
        }

        public void Add(Identifier name, object argument)
        {
            _arguments.Add(name, argument);
        }

        public override string ToString()
        {
            return ToString(null);
        }

        public string ToString(IFormatProvider formatProvider)
        {
            return FormatErrorMessage(_arguments.Concat(MemberIfNotSpecified()), formatProvider).ToString();
        }

        private IEnumerable<KeyValuePair<Identifier, object>> MemberIfNotSpecified()
        {
            yield return new KeyValuePair<Identifier, object>(MemberIdentifier, new MemberWithValue(new DefaultMember(this), FailedValue));
        }

        protected abstract StringTemplate FormatErrorMessage(IEnumerable<KeyValuePair<Identifier, object>> arguments, IFormatProvider formatProvider);        
    }
}
