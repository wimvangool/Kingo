using System;
using System.Collections.Generic;
using System.Linq;

namespace Syztem.ComponentModel.FluentValidation
{
    internal sealed class OrConstraint<TValue> : Constraint
    {
        private readonly Member<TValue> _valueProvider;
        private readonly IErrorMessageProducer[] _constraints;
        private readonly FormattedString _errorMessage;

        internal OrConstraint(Member<TValue> valueProvider, IEnumerable<Action<IMemberConstraintSet, TValue>> constraints, FormattedString errorMessage)
        {
            _valueProvider = valueProvider;
            _constraints = CreatErrorMessageProducers(valueProvider, constraints).ToArray();
            _errorMessage = errorMessage;
        }

        private static IEnumerable<IErrorMessageProducer> CreatErrorMessageProducers(Member<TValue> valueProvider, IEnumerable<Action<IMemberConstraintSet, TValue>> constraints)
        {
            foreach (var constraint in constraints)
            {
                var constraintSet = valueProvider.CopyConstraintSet();
                
                constraint.Invoke(constraintSet, valueProvider.Value);

                yield return constraintSet;
            }
        }

        public override void AddErrorMessagesTo(IErrorMessageConsumer consumer)
        {
            if (consumer == null)
            {
                return;
            }
            var errorMessageMerger = new ErrorMessageMerger();
            var errorMessageCounter = new ErrorMessageCounter(errorMessageMerger);
            var errorCount = 0;

            foreach (var constraint in _constraints)
            {
                constraint.AddErrorMessagesTo(errorMessageCounter);

                // If the constraint did not produce an error, the whole constraint
                // passes and we can return immediately.
                if (errorCount == errorMessageCounter.ErrorCount)
                {
                    return;
                }
                errorCount = errorMessageCounter.ErrorCount;
            }

            // If all constraints produced (at least) one error, the or-constraints fails,
            // and a merged error is reported to the consumer provided.
            consumer.Add(_valueProvider.FullName, errorMessageMerger.MergeErrorMessages(_errorMessage));
        }
    }
}
