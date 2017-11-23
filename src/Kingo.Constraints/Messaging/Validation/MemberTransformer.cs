namespace Kingo.Messaging.Validation
{
    internal class MemberTransformer
    {
        internal virtual Member<TValueOut> Transform<TValueIn, TValueOut>(Member<TValueIn> member, TValueOut valueOut) =>
             member.Transform(valueOut);
    }
}
