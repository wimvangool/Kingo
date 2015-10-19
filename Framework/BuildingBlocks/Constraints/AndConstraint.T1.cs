using System;
using System.Linq;

namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class AndConstraint<TValue> : IConstraint<TValue>
    {
        private readonly ConstraintImplementation<TValue> _implementation;
        private readonly IConstraint<TValue>[] _constraints;

        internal AndConstraint(IConstraint<TValue> left, IConstraint<TValue> right)
        {
            _implementation = new ConstraintImplementation<TValue>(this);
            _constraints = new [] { left, right };
        }        

        private AndConstraint(AndConstraint<TValue> constraint, IConstraint<TValue> addedConstraint)            
        {
            _implementation = new ConstraintImplementation<TValue>(this);
            _constraints = constraint._constraints.Add(addedConstraint);
        }                

        #region [====== And, Or & Invert ======]

        public IConstraint<TValue> And(IConstraint<TValue> constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            return new AndConstraint<TValue>(this, constraint);
        }

        public IConstraintWithErrorMessage<TValue> Or(IConstraint<TValue> constraint)
        {
            return _implementation.Or(constraint);
        }

        public IConstraint<TValue> Invert()
        {
            return _implementation.Invert();
        }

        public IConstraint<TValue> Invert(string errorMessage, string name = null)
        {
            return _implementation.Invert(errorMessage, name);
        }

        public IConstraint<TValue> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return _implementation.Invert(errorMessage, name);
        }

        #endregion

        #region [====== MapInputToOutput ======]

        public IConstraint<TValue, TValue> MapInputToOutput()
        {
            return _implementation.MapInputToOutput();
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        public bool IsSatisfiedBy(TValue value)
        {
            return _constraints.All(constraint => constraint.IsSatisfiedBy(value));
        }

        public bool IsNotSatisfiedBy(TValue value, out IErrorMessage errorMessage)
        {
            foreach (var constraint in _constraints)
            {
                if (constraint.IsNotSatisfiedBy(value, out errorMessage))
                {
                    return true;
                }
            }
            errorMessage = null;
            return false;
        }        

        #endregion                                
    }
}
