using Kingo.BuildingBlocks.Constraints;

namespace Kingo.BuildingBlocks.Messaging
{
    public sealed class RequiredValueMessage<TValue> : Message<RequiredValueMessage<TValue>> where TValue : class
    {
        public TValue Value;

        public RequiredValueMessage(TValue value = null)
        {
            Value = value;
        }

        public override RequiredValueMessage<TValue> Copy()
        {
            return new RequiredValueMessage<TValue>(Value);
        }

        #region [====== Equals & GetHashCode ======]

        public override bool Equals(object obj)
        {
            return Equals(obj as RequiredValueMessage<TValue>);
        }

        public bool Equals(RequiredValueMessage<TValue> other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            if (ReferenceEquals(other, this))
            {
                return true;
            }
            return Equals(Value, other.Value);
        }

        public override int GetHashCode()
        {
            return typeof(TValue).GetHashCode();
        }

        #endregion

        #region [====== Validation ======]

        protected override IValidator<RequiredValueMessage<TValue>> CreateValidator()
        {
            var validator = new ConstraintValidator<RequiredValueMessage<TValue>>();

            validator.VerifyThat(message => message.Value).IsNotNull();

            return validator;
        }

        #endregion
    }
}
