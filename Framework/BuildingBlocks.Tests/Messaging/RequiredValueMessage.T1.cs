using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingo.BuildingBlocks.Messaging.Constraints;

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

        protected override IMessageValidator CreateValidator()
        {
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => Value).IsNotNull();

            return validator;
        }

        #endregion
    }
}
