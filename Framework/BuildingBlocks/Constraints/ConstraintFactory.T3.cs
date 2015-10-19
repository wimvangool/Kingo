using System;

namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class ConstraintFactory<TMessage, TValueIn, TValueOut>
    {
        private readonly Func<TMessage, IConstraint<TValueIn, TValueOut>> _factory;        

        internal ConstraintFactory(Func<TMessage, IConstraint<TValueIn, TValueOut>> factory)
        {
            _factory = factory;
        }

        internal ConstraintFactory<TMessage, TValueIn, TResult> And<TResult>(Func<TMessage, IConstraint<TValueOut, TResult>> constraintFactory)
        {
            if (constraintFactory == null)
            {
                throw new ArgumentNullException("constraintFactory");
            }
            return new ConstraintFactory<TMessage, TValueIn, TResult>(message => CreateConstraint(message).And(constraintFactory.Invoke(message)));
        }

        internal IConstraint<TValueIn, TValueOut> CreateConstraint(TMessage message)
        {
            return _factory.Invoke(message);
        }
    }
}
