using System;
using System.Threading.Tasks;
using Kingo.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    internal sealed class UnitOfWorkResourceManagerSpy : IUnitOfWorkResourceManager
    {
        private readonly bool _requiresFlush;
        private readonly Exception _exceptionToThrow;
        private int _requiresFlushCount;
        private int _flushCount;

        public UnitOfWorkResourceManagerSpy(bool requiresFlush, Exception exceptionToThrow = null)
        {
            _requiresFlush = requiresFlush;
            _exceptionToThrow = exceptionToThrow;
        }

        #region [====== IUnitOfWorkResourceManager ======]

        public object ResourceId =>
            null;

        public bool RequiresFlush()
        {
            _requiresFlushCount++;

            return _requiresFlush;
        }

        public Task FlushAsync()
        {
            return AsyncMethod.Run(() =>
            {
                _flushCount++;

                if (_exceptionToThrow != null)
                {
                    throw _exceptionToThrow;
                }
            });
        }

        #endregion

        public void AssertRequiresFlushCountIs(int count)
        {
            Assert.AreEqual(count, _requiresFlushCount);            
        }

        public void AssertFlushCountIs(int count)
        {
            Assert.AreEqual(count, _flushCount);
        }
    }
}
