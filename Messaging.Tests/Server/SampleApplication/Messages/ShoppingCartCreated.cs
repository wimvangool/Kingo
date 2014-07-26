namespace System.ComponentModel.Messaging.Server.SampleApplication.Messages
{
    internal sealed class ShoppingCartCreated : IAggregateEvent<Guid, Int32Version>
    {
        Guid IAggregateEvent<Guid, Int32Version>.AggregateKey
        {
            get { return ShoppingCartId; }
        }

        Int32Version IAggregateEvent<Guid, Int32Version>.AggregateVersion
        {
            get { return ShoppingCartVersion; }
        }

        public Guid ShoppingCartId;
        public Int32Version ShoppingCartVersion;              
    }
}
