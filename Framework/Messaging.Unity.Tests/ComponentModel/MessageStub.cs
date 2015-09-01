namespace ServiceComponents.ComponentModel
{
    internal sealed class MessageStub : Message<MessageStub>
    {
        public override MessageStub Copy()
        {
            return new MessageStub();
        }
    }
}
