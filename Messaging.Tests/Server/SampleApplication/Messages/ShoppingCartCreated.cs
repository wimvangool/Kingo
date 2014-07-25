namespace System.ComponentModel.Messaging.Server.SampleApplication.Messages
{
    internal sealed class ShoppingCartCreated : IDomainEvent<Guid>
    {
        Guid IDomainEvent<Guid>.AggregateKey
        {
            get { return ShoppingCartId; }
        }

        int IDomainEvent<Guid>.AggregateVersion
        {
            get { return ShoppingCartVersion; }
        }

        public Guid ShoppingCartId;
        public int ShoppingCartVersion;              
    }
}
