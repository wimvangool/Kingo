using System;
using System.Linq;

namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class AndConstraint<TValue> : IConstraint<TValue>
    {        
        private readonly IConstraint<TValue>[] _constraints;

        internal AndConstraint(IConstraint<TValue> left, IConstraint<TValue> constraint)
        {            
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            _constraints = new [] { left, constraint };
        }        

        private AndConstraint(AndConstraint<TValue> left, IConstraint<TValue> constraint)            
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            _constraints = left._constraints.Add(constraint);
        }                

        #region [====== And, Or & Invert ======]

        public IConstraint<TValue> And(IConstraint<TValue> constraint)
        {            
            return new AndConstraint<TValue>(this, constraint);
        }

        public IConstraintWithErrorMessage<TValue> Or(IConstraint<TValue> constraint)
        {
            return new OrConstraint<TValue>(this, constraint);
        }

        public IConstraint<TValue> Invert()
        {
            throw new NotImplementedException();
        }        

        #endregion

        #region [====== MapInputToOutput ======]

        public IConstraint<TValue, TValue> MapInputToOutput()
        {
            return new InputToOutputMapper<TValue>(this);
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
