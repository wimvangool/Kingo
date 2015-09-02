using System;

namespace Kingo.BuildingBlocks.Messaging.Domain
{    
    internal sealed class AggregateStub : AggregateRoot<Guid, int>
    {
        private readonly Guid _id;
        private int _version;

        internal AggregateStub(Guid id)
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
            get { return _version; }
            set { SetVersion(ref _version, value); }
        }

        /// <inheritdoc />
        protected override int NewVersion()
        {
            return _version + 1;
        }        

        #endregion

        internal void Update()
        {
            Version = NewVersion();
        }
    }
}