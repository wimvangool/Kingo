using System;
using System.Threading.Tasks;
using Kingo.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    internal sealed class ChangeTrackerSpy : IChangeTracker
    {
        private readonly bool _hasChanges;
        private readonly Exception _exceptionToThrow;
        private readonly string _groupId;
        private int _hasChangesCount;
        private int _saveChangesCount;
        private int _undoChangesCount;

        public ChangeTrackerSpy(bool requiresFlush, Exception exceptionToThrow = null) :
            this(requiresFlush, Guid.NewGuid().ToString())
        {            
            _exceptionToThrow = exceptionToThrow;            
        }

        public ChangeTrackerSpy(bool requiresFlush, string flushGroupId)
        {
            _hasChanges = requiresFlush;
            _exceptionToThrow = null;
            _groupId = flushGroupId;
        }

        #region [====== IFlushable ======]        

        public string GroupId =>
            _groupId;

        public bool HasChanges(Guid unitOfWorkId)
        {
            _hasChangesCount++;

            return _hasChanges;
        }

        public void UndoChanges(Guid unitOfWorkId)
        {
            _undoChangesCount++;
        }

        public Task SaveChangesAsync(Guid unitOfWorkId)
        {            
            return AsyncMethod.Run(() =>
            {
                _saveChangesCount++;

                if (_exceptionToThrow != null)
                {
                    throw _exceptionToThrow;
                }
            });
        }

        #endregion

        public void AssertRequiresFlushCountIs(int count) =>
            Assert.AreEqual(count, _hasChangesCount);

        public void AssertFlushCountIs(int count) =>
            Assert.AreEqual(count, _saveChangesCount);

        public void AssertUndoCountIs(int count) =>
            Assert.AreEqual(count, _undoChangesCount);
    }
}
