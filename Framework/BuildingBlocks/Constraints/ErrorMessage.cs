using System;
using System.Collections.Generic;

namespace Kingo.BuildingBlocks.Constraints
{
    internal abstract class ErrorMessage : IErrorMessage
    {
        private readonly Dictionary<Identifier, object> _arguments;

        protected ErrorMessage()
        {
            _arguments = new Dictionary<Identifier, object>();
        }

        public abstract IConstraintWithErrorMessage FailedConstraint
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
            return FormatErrorMessage(_arguments, formatProvider).ToString();
        }

        protected abstract StringTemplate FormatErrorMessage(IEnumerable<KeyValuePair<Identifier, object>> arguments, IFormatProvider formatProvider);        
    }
}
