using System;

namespace Kingo.Messaging.Domain
{    
    internal sealed class AggregateStub : AggregateRoot<Guid, int>
    {
        private readonly Guid _id;
        private readonly int _alternateKey;
        private int _version;

        internal AggregateStub(Guid id, int alternateKey = 0)
        {
            _id = id;
            _alternateKey = alternateKey;
        }

        #region [====== Id & Version ======]

        /// <inheritdoc />
        public override Guid Id
        {
            get { return _id; }
        }

        public int AlternateKey
        {
            get { return _alternateKey; }
        }

        /// <inheritdoc />
        protected override int Version
        {
            get { return _version; }
            set { _version = value; }
        }

        /// <inheritdoc />
        protected override int NextVersion(int version)
        {
            return version + 1;
        }        

        #endregion

        internal void Update()
        {
            Version = NextVersion();
        }
    }
}