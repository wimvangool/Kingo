using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices.Domain
{
    internal abstract class NumberSnapshotOrEvent : SnapshotOrEvent<Guid, int>
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
