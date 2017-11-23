using System;

namespace Kingo.Messaging.Validation
{    
    internal sealed class DelegateFilter<T, TValueIn, TValueOut> : Filter<TValueIn, TValueOut>
    {
        private readonly T _instance;
        private readonly Func<T, TValueIn, TValueOut> _fieldOrProperty;
   
        internal DelegateFilter(T instance, Func<T, TValueIn, TValueOut> fieldOrProperty)
        {           
            _instance = instance;
            _fieldOrProperty = fieldOrProperty ?? throw new ArgumentNullException(nameof(fieldOrProperty));
        }

        private DelegateFilter(DelegateFilter<T, TValueIn, TValueOut> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            _instance = constraint._instance;
            _fieldOrProperty = constraint._fieldOrProperty;
        }

        private DelegateFilter(DelegateFilter<T, TValueIn, TValueOut> constraint, Identifier name)
            : base(constraint, name)
        {
            _instance = constraint._instance;
            _fieldOrProperty = constraint._fieldOrProperty;
        }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        public override IFilterWithErrorMessage<TValueIn, TValueOut> WithName(Identifier name) => new DelegateFilter<T, TValueIn, TValueOut>(this, name);

        /// <inheritdoc />
        public override IFilterWithErrorMessage<TValueIn, TValueOut> WithErrorMessage(StringTemplate errorMessage) => new DelegateFilter<T, TValueIn, TValueOut>(this, errorMessage);

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValueIn> Invert(StringTemplate errorMessage, Identifier name = null) => new ConstraintInverter<TValueIn>(this)
            .WithErrorMessage(errorMessage)
            .WithName(name);

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValueIn value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            return true;
        }

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValueIn value, out TValueOut valueOut)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            valueOut = _fieldOrProperty.Invoke(_instance, value);
            return true;
        }

        #endregion
    }
}
