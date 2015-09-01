namespace Kingo.BuildingBlocks.ComponentModel
{
    internal sealed class MessageOne : Message<MessageOne>
    {
        public override MessageOne Copy()
        {
            return new MessageOne();
        }
    }
}
