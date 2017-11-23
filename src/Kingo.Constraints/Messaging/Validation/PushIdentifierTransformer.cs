using System;

namespace Kingo.Messaging.Validation
{
    internal sealed class PushIdentifierTransformer : MemberTransformer
    {
        private readonly Identifier _fieldOrPropertyName;

        internal PushIdentifierTransformer(Identifier fieldOrPropertyName)
        {            
            _fieldOrPropertyName = fieldOrPropertyName ?? throw new ArgumentNullException(nameof(fieldOrPropertyName));
        }

        internal override Member<TValueOut> Transform<TValueIn, TValueOut>(Member<TValueIn> member, TValueOut valueOut) =>
            member.Transform(valueOut, _fieldOrPropertyName);
    }
}
