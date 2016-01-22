using System;

namespace Kingo.Messaging.Domain
{
    internal sealed class AggregateStub : AggregateRoot
    {
        private sealed class CreatedEvent : DomainEvent
        {
            public new readonly Guid Key;
            public new readonly int Version;

            public CreatedEvent(Guid id, int version)
            {
                Key = id;
                Version = version;
            }
        }

        private readonly Guid _id;               

        internal AggregateStub(Guid id)
            : base(new CreatedEvent(id, 1))
        {
            _id = id;            
        }

        #region [====== Id & Version ======]

        /// <inheritdoc />
        public override Guid Id
        {
            get { return _id; }
        }      

        /// <inheritdoc />
        protected override int Version
        {
            get;
            set;
        }                

        #endregion        

        internal void Update()
        {
            Version = NextVersion();
        }
    }
}