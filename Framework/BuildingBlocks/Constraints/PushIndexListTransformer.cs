namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class PushIndexListTransformer : MemberTransformer
    {
        private readonly IndexList _indexList;

        internal PushIndexListTransformer(IndexList indexList)
        {
            _indexList = indexList;
        }

        internal override Member<TValueOut> Transform<TValueIn, TValueOut>(Member<TValueIn> member, TValueOut valueOut)
        {
            return member.Transform(valueOut, _indexList);
        }
    }
}
