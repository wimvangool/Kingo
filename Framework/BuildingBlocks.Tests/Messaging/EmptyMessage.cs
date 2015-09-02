namespace Kingo.BuildingBlocks.Messaging
{
    public sealed class EmptyMessage : Message<EmptyMessage>
    {
        public override EmptyMessage Copy()
        {
            return new EmptyMessage();
        }
    }
}
