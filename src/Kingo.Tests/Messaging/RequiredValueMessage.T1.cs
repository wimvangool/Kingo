using Kingo.Messaging.Constraints;

namespace Kingo.Messaging
{
    public sealed class RequiredValueMessage<TValue> : Message where TValue : class
    {
        public TValue Value;

        public RequiredValueMessage(TValue value = null)
        {
            Value = value;
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

        protected override IMessageValidator CreateValidator()
        {
            var validator = new ConstraintMessageValidator<RequiredValueMessage<TValue>>();

            validator.VerifyThat(message => message.Value).IsNotNull();

            return validator;
        }

        #endregion
    }
}
