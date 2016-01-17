namespace Kingo.Messaging
{
    internal sealed class MessageOne : Message
    {
        public override Message Copy()
        {
            return new MessageOne();
        }
    }
}
