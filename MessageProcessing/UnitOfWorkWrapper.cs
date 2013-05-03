using System.Collections.Generic;

namespace YellowFlare.MessageProcessing
{
    internal abstract class UnitOfWorkWrapper
    {
        public abstract string Group
        {
            get;
        }

        public abstract bool ForceSynchronousFlush
        {
            get;
        }

        public abstract bool WrapsSameUnitOfWorkAs(UnitOfWorkWrapperItem item);

        public bool TryMergeWith(UnitOfWorkWrapperItem item, out UnitOfWorkWrapperGroup group)
        {
            if (WrapsSameUnitOfWorkAs(item) || item.Group == null || item.Group != Group)
            {
                group = null;
                return false;
            }
            group = MergeWith(item);
            return true;
        }

        protected abstract UnitOfWorkWrapperGroup MergeWith(UnitOfWorkWrapperItem wrapper);

        public abstract void CollectWrappersThatRequireFlush(ICollection<UnitOfWorkWrapper> wrappers);

        public abstract void Flush();        
    }
}
