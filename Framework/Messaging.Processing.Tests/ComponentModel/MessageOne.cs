namespace ServiceComponents.ComponentModel
{
    internal sealed class MessageOne : Message<MessageOne>
    {
        public override MessageOne Copy()
        {
            return new MessageOne();
        }
    }
}
