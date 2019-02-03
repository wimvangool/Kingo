using System;

namespace Kingo.MicroServices.Domain
{
    public abstract class NumberSnapshotOrEvent : SnapshotOrEvent<Guid, int>
    {
        protected NumberSnapshotOrEvent(Guid id, int version)
        {
            NumberId = id;
            NumberVersion = version;
        }

        protected override Guid Id =>
            NumberId;

        public Guid NumberId
        {
            get;
        }

        protected override int Version =>
            NumberVersion;

        public int NumberVersion
        {
            get;
        }
    }
}
