using System.Collections.Generic;
using System.Diagnostics;

namespace System.ComponentModel.Server
{
    internal sealed class UnitOfWorkItem : UnitOfWorkWrapper
    {
        private readonly IUnitOfWork _unitOfWork;        

        public UnitOfWorkItem(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException("unitOfWork");
            }
            _unitOfWork = unitOfWork;            
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override int ItemCount
        {
            get { return 1; }
        }

        public override Guid FlushGroupId
        {
            get { return _unitOfWork.FlushGroupId; }
        }

        public override bool CanBeFlushedAsynchronously
        {
            get { return _unitOfWork.CanBeFlushedAsynchronously; }
        }

        public override string ToString()
        {
            return _unitOfWork.GetType().Name;
        }

        public override bool WrapsSameUnitOfWorkAs(UnitOfWorkItem item)
        {            
            return ReferenceEquals(_unitOfWork, item._unitOfWork);
        }

        protected override UnitOfWorkGroup MergeWith(UnitOfWorkItem item)
        {
            return new UnitOfWorkGroup(this, item);
        }

        public override void CollectUnitsThatRequireFlush(ICollection<UnitOfWorkWrapper> units)
        {            
            if (_unitOfWork.RequiresFlush())
            {
                units.Add(this);
            }
        }

        public bool RequiresFlush()
        {
            return _unitOfWork.RequiresFlush();
        }

        public override void Flush()
        {
            _unitOfWork.Flush();
        }                      
    }
}
