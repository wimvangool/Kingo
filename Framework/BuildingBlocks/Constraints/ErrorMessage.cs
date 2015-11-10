using System;
using System.Collections.Generic;

namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class ErrorMessage : IErrorMessage
    {
        #region [====== Builder ======]

        private sealed class Builder : ConstraintVisitor
        {
            private readonly ErrorMessage _errorMessage;

            private Builder(ErrorMessage errorMessage)
            {
                _errorMessage = errorMessage;
            }

            protected override void VisitAnd(IConstraint andConstraint) { }

            protected override void VisitOr(IConstraintWithErrorMessage orConstraint) { }

            protected override void VisitInverse(IConstraintWithErrorMessage inverseConstraint) { }

            protected override void Visit(IConstraintWithErrorMessage constraint)
            {
                _errorMessage.Put(constraint.Name, constraint);
            }

            internal static ErrorMessage BuildErrorMessage(IConstraintWithErrorMessage failedConstraint, object value)
            {
                var builder = new Builder(new ErrorMessage(failedConstraint, value));
                failedConstraint.AcceptVisitor(builder);
                return builder._errorMessage;
            }
        }

        #endregion        

        internal static readonly Identifier MemberIdentifier = Identifier.Parse("member");

        private readonly IConstraintWithErrorMessage _failedConstraint;
        private readonly object _value;
        private readonly Dictionary<Identifier, object> _arguments;

        private ErrorMessage(IConstraintWithErrorMessage failedConstraint, object value)
        {
            _failedConstraint = failedConstraint;
            _value = value;
            _arguments = new Dictionary<Identifier, object>()
            {
                { MemberIdentifier, new DefaultMember(_failedConstraint, value) }
            };
        }                        

        public void Put(string name, object argument)
        {
            Put(Identifier.ParseOrNull(name), argument);
        }

        public void Put(Identifier name, object argument)
        {
            _arguments[name] = argument;
        }

        public override string ToString()
        {
            return ToString(null);
        }

        public string ToString(IFormatProvider formatProvider)
        {
            return FormatErrorMessage(_arguments, formatProvider).ToString();
        }        

        private StringTemplate FormatErrorMessage(IEnumerable<KeyValuePair<Identifier, object>> arguments, IFormatProvider formatProvider)
        {
            return _failedConstraint.ErrorMessage.Format(arguments, formatProvider);
        }

        internal static ErrorMessage Build(IConstraintWithErrorMessage failedConstraint, object value)
        {
            return Builder.BuildErrorMessage(failedConstraint, value);
        }
    }
}
