namespace System.ComponentModel
{
    internal sealed class MessageTwo : Message<MessageTwo>
    {
        public override MessageTwo Copy()
        {
            return new MessageTwo();
        }
    }
}
