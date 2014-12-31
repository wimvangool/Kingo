namespace System.ComponentModel.Server
{
    internal sealed class DomainEvent : Message<DomainEvent>
    {
        public override DomainEvent Copy()
        {
            return new DomainEvent();
        }
    }
}
