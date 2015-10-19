using System;

namespace Kingo.BuildingBlocks.Constraints
{
    internal abstract class ConstraintImplementationBase<TValue>        
    {                
        protected abstract IConstraint<TValue> Constraint
        {
            get;
        }

        #region [====== And, Or & Invert =====]

        internal IConstraint<TValue> And(IConstraint<TValue> constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            return new AndConstraint<TValue>(Constraint, constraint);
        }

        internal IConstraintWithErrorMessage<TValue> Or(IConstraint<TValue> constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            return new OrConstraint<TValue>(Constraint, constraint);
        }
        
        internal IConstraint<TValue> Invert()
        {
            return Invert(null as StringTemplate);
        }
        
        internal IConstraint<TValue> Invert(string errorMessage, string name = null)
        {
            return Invert(StringTemplate.Parse(errorMessage), Identifier.Parse(name));
        }
        
        internal IConstraint<TValue> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new NotConstraint<TValue>(Constraint, errorMessage, name);
        }

        #endregion

        #region [====== MapInputToOutput ======]
        
        internal IConstraint<TValue, TValue> MapInputToOutput()
        {
            return new InputToOutputMapper<TValue>(Constraint);
        }

        #endregion
    }
}
