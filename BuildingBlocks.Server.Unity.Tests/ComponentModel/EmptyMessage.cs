using Kingo.BuildingBlocks.Messaging;

namespace Kingo.BuildingBlocks.ComponentModel
{
    internal sealed class EmptyMessage : Message<EmptyMessage>
    {
        public override EmptyMessage Copy()
        {
            return new EmptyMessage();
        }
    }
}
