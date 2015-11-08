using System;

namespace Kingo.BuildingBlocks.Constraints
{    
    internal sealed class DelegateFilter<TValueIn, TValueOut> : Filter<TValueIn, TValueOut>
    {
        private readonly Func<TValueIn, TValueOut> _fieldOrProperty;
   
        internal DelegateFilter(Func<TValueIn, TValueOut> fieldOrProperty)
        {
            if (fieldOrProperty == null)
            {
                throw new ArgumentNullException("fieldOrProperty");
            }
            _fieldOrProperty = fieldOrProperty;
        }

        private DelegateFilter(DelegateFilter<TValueIn, TValueOut> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            _fieldOrProperty = constraint._fieldOrProperty;
        }

        private DelegateFilter(DelegateFilter<TValueIn, TValueOut> constraint, Identifier name)
            : base(constraint, name)
        {
            _fieldOrProperty = constraint._fieldOrProperty;
        }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        public override IFilterWithErrorMessage<TValueIn, TValueOut> WithName(Identifier name)
        {
            return new DelegateFilter<TValueIn, TValueOut>(this, name);
        }

        /// <inheritdoc />
        public override IFilterWithErrorMessage<TValueIn, TValueOut> WithErrorMessage(StringTemplate errorMessage)
        {
            return new DelegateFilter<TValueIn, TValueOut>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValueIn> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new ConstraintInverter<TValueIn>(this)
                .WithErrorMessage(errorMessage)
                .WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValueIn value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return true;
        }

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValueIn value, out TValueOut valueOut)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            valueOut = _fieldOrProperty.Invoke(value);
            return true;
        }

        #endregion
    }
}
