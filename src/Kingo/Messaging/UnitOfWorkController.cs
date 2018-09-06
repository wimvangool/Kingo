using System;
using System.Threading.Tasks;
using Kingo.Resources;
using static Kingo.Threading.AsyncMethod;

namespace Kingo.Messaging
{
    internal sealed class UnitOfWorkController : Disposable, IUnitOfWorkController
    {
        #region [====== NullController ======]

        private sealed class NullController : IUnitOfWorkController
        {            
            public NullController(IUnitOfWorkCache cache)
            {
                Cache = cache;
            }

            public IUnitOfWorkCache Cache
            {
                get;
            }           

            public Task EnlistAsync(IUnitOfWork unitOfWork, object resourceId = null)
            {
                if (unitOfWork == null)
                {
                    throw new ArgumentNullException(nameof(unitOfWork));
                }
                if (unitOfWork.RequiresFlush())
                {
                    return unitOfWork.FlushAsync();
                }
                return NoValue;
            }

            public bool RequiresFlush() =>
                false;

            public Task FlushAsync() =>
                throw NewOperationNotSupportedException(nameof(FlushAsync));

            private static Exception NewOperationNotSupportedException(string methodName)
            {
                var messageFormat = ExceptionMessages.NullController_OperationNotSupported;
                var message = string.Format(messageFormat, methodName);
                return new NotSupportedException(message);
            }
        }

        private sealed class NullCache : IUnitOfWorkCache
        {
            public object this[string key]
            {
                get
                {
                    if (key == null)
                    {
                        throw new ArgumentNullException(nameof(key));
                    }
                    return null;
                }
                set => throw NewCacheNotSupportedException();
            }

            public void Remove(string key) =>
                throw NewCacheNotSupportedException();

            private static Exception NewCacheNotSupportedException() =>
                new InvalidOperationException(ExceptionMessages.NullCache_CacheNotSupported);
        }

        #endregion 
        
        public static readonly IUnitOfWorkController None = new NullController(new NullCache());       

        private readonly UnitOfWorkCache _cache;
        private IUnitOfWorkController _controller;

        internal UnitOfWorkController()
        {
            _cache = new UnitOfWorkCache();
            _controller = new UnitOfWorkControllerImplementation(_cache);
        }
        
        public IUnitOfWorkCache Cache =>
            _cache;        

        public Task EnlistAsync(IUnitOfWork unitOfWork, object resourceId = null) =>
            _controller.EnlistAsync(unitOfWork, resourceId);

        public bool RequiresFlush() =>
            _controller.RequiresFlush();
        
        public Task FlushAsync() =>
            _controller.FlushAsync();               

        protected override void DisposeManagedResources()
        {
            _cache.Dispose();

            base.DisposeManagedResources();
        }
    }
}
