namespace Kingo.BuildingBlocks.Messaging
{
    internal sealed class MessageOne : Message<MessageOne>
    {
        public override MessageOne Copy()
        {
            return new MessageOne();
        }
    }
}
