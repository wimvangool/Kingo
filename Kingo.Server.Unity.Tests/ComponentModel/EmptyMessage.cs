using Kingo.Messaging;

namespace Kingo.ComponentModel
{
    internal sealed class EmptyMessage : Message<EmptyMessage>
    {
        public override EmptyMessage Copy()
        {
            return new EmptyMessage();
        }
    }
}
