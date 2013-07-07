using System.Collections.Generic;

namespace YellowFlare.MessageProcessing
{
    internal abstract class UnitOfWorkWrapper
    {
        public abstract string FlushGroup
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
            if (WrapsSameUnitOfWorkAs(item) || item.FlushGroup == null || item.FlushGroup != FlushGroup)
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
