using System.Collections.Generic;

namespace System.ComponentModel.Server
{
    internal abstract class UnitOfWorkWrapper
    {
        public abstract int ItemCount
        {
            get;
        }

        public abstract Guid FlushGroupId
        {
            get;
        }

        public abstract bool CanBeFlushedAsynchronously
        {
            get;
        }

        public abstract bool WrapsSameUnitOfWorkAs(UnitOfWorkItem item);

        public bool TryMergeWith(UnitOfWorkItem item, out UnitOfWorkGroup group)
        {
            if (WrapsSameUnitOfWorkAs(item) || item.FlushGroupId.Equals(Guid.Empty) || item.FlushGroupId != FlushGroupId)
            {
                group = null;
                return false;
            }
            group = MergeWith(item);
            return true;
        }

        protected abstract UnitOfWorkGroup MergeWith(UnitOfWorkItem item);

        public abstract void CollectUnitsThatRequireFlush(ICollection<UnitOfWorkWrapper> units);

        public abstract void Flush();        
    }
}
