namespace Kingo.Messaging.Validation
{
    public sealed class RequiredValueMessage<TValue> : RequestMessageBase where TValue : class
    {
        public TValue Value;

        public RequiredValueMessage(TValue value = null)
        {
            Value = value;
        }        

        #region [====== Equals & GetHashCode ======]

        public override bool Equals(object obj) =>
             Equals(obj as RequiredValueMessage<TValue>);

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

        public override int GetHashCode() =>
             typeof(TValue).GetHashCode();

        #endregion

        #region [====== Validation ======]

        protected override IRequestMessageValidator CreateMessageValidator() =>
            base.CreateMessageValidator().Append(CreateConstraintValidator());

        private static IRequestMessageValidator<RequiredValueMessage<TValue>> CreateConstraintValidator()
        {
            var validator = new ConstraintValidator<RequiredValueMessage<TValue>>();

            validator.VerifyThat(message => message.Value).IsNotNull();

            return validator;
        }

        #endregion
    }
}
