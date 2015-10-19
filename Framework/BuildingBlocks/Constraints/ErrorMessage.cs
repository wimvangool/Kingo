using System;

namespace Kingo.BuildingBlocks.Constraints
{
    internal abstract class ErrorMessage : IErrorMessage
    {
        public abstract IConstraintWithErrorMessage FailedConstraint
        {
            get;
        }

        public override string ToString()
        {
            return ToString(null);
        }

        public string ToString(IFormatProvider formatProvider)
        {
            return FormatErrorMessage(formatProvider).ToString();
        }

        protected abstract StringTemplate FormatErrorMessage(IFormatProvider formatProvider);
    }
}
