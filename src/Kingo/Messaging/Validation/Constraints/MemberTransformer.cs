namespace Kingo.Messaging.Validation.Constraints
{
    internal class MemberTransformer
    {
        internal virtual Member<TValueOut> Transform<TValueIn, TValueOut>(Member<TValueIn> member, TValueOut valueOut)
        {
            return member.Transform(valueOut);
        }
    }
}
