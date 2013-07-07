using System;
using System.Collections.Generic;

namespace YellowFlare.MessageProcessing
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

        public override string FlushGroup
        {
            get { return _unitOfWork.FlushGroup; }
        }

        public override bool CanBeFlushedAsynchronously
        {
            get { return _unitOfWork.CanBeFlushedAsynchronously; }
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
