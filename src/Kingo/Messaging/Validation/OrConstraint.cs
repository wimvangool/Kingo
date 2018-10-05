using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Kingo.Messaging.Validation
{
    internal sealed class OrConstraint : CompositeConstraint, IOrConstraint
    {
        private readonly Func<IEnumerable<string>, object, string> _mergeErrorsFunction;

        public OrConstraint(params IConstraint[] constraints) :
            this(constraints, MergeErrors) { }

        public OrConstraint(IEnumerable<IConstraint> constraints) :
            this(constraints, MergeErrors) { }

        private OrConstraint(IEnumerable<IConstraint> constraints, Func<IEnumerable<string>, object, string> mergeErrorsFunction) :
            base(constraints)
        {
            _mergeErrorsFunction = mergeErrorsFunction ?? throw new ArgumentNullException(nameof(mergeErrorsFunction));
        }

        #region [====== Logical Operations ======]

        /// <inheritdoc />
        public override IOrConstraint Or(IConstraint constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException(nameof(constraint));
            }
            if (constraint == this)
            {
                return this;
            }
            return new OrConstraint(Constraints.Concat(new [] { constraint }), _mergeErrorsFunction);
        }

        #endregion

        #region [====== Validation ======]

        public override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var errorMessages = new List<ValidationResult>(Constraints.Length);

            foreach (var constraint in Constraints)
            {
                if (constraint.IsNotValid(value, validationContext, out var errorMessage))
                {
                    errorMessages.Add(errorMessage);
                    continue;
                }
                return ValidationResult.Success;
            }
            return new ValidationResult(_mergeErrorsFunction.Invoke(errorMessages.Select(error => error.ErrorMessage), value));
        }

        public IConstraint CombineErrors(Func<IEnumerable<string>, object, string> mergeErrorsFunction) =>
            new OrConstraint(Constraints, mergeErrorsFunction);

        private static string MergeErrors(IEnumerable<string> errorMessages, object value) =>
            string.Format(ErrorMessages.OrConstraint_ValueNotValid, value, string.Join(" | ", errorMessages));

        #endregion
    }
}
