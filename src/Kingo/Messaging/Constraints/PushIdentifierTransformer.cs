using System;

namespace Kingo.Messaging.Constraints
{
    internal sealed class PushIdentifierTransformer : MemberTransformer
    {
        private readonly Identifier _fieldOrPropertyName;

        internal PushIdentifierTransformer(Identifier fieldOrPropertyName)
        {
            if (fieldOrPropertyName == null)
            {
                throw new ArgumentNullException(nameof(fieldOrPropertyName));
            }
            _fieldOrPropertyName = fieldOrPropertyName;
        }

        internal override Member<TValueOut> Transform<TValueIn, TValueOut>(Member<TValueIn> member, TValueOut valueOut)
        {
            return member.Transform(valueOut, _fieldOrPropertyName);
        }
    }
}
