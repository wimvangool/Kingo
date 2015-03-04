using System.Runtime.Caching;
using System.Threading;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Server.Caching
{
    [TestClass]
    public sealed class ObjectCacheModuleTest
    {
        #region [====== Nested Types ======]

        private sealed class RequestMessage : Message<RequestMessage>
        {
            public readonly long Value;

            public RequestMessage()
            {
                Value = Clock.Current.LocalDateAndTime().Ticks;
            }

            private RequestMessage(long value)
            {
                Value = value;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as RequestMessage);
            }

            public bool Equals(RequestMessage message)
            {
                if (ReferenceEquals(message, null))
                {
                    return false;
                }
                if (ReferenceEquals(message, this))
                {
                    return true;
                }
                return Value == message.Value;
            }

            public override int GetHashCode()
            {
                return Value.GetHashCode();
            }

            public override RequestMessage Copy()
            {
                return new RequestMessage(Value);
            }
        }

        private sealed class ResponseMessage : Message<ResponseMessage>
        {
            internal readonly long Value;

            internal ResponseMessage(long value)
            {
                Value = value;
            }

            public override ResponseMessage Copy()
            {
                return new ResponseMessage(Value);
            }
        }

        private sealed class Query : Query<RequestMessage, ResponseMessage>
        {
            private int _invocationCount;
            private long _lastResult;

            internal void VerifyThatInvocationCountIs(int invocationCount)
            {
                Assert.AreEqual(invocationCount, _invocationCount);
            }

            internal void VerifyLastResultIs(ResponseMessage result)
            {
                Assert.AreEqual(result.Value, _lastResult);
            }

            public override ResponseMessage Execute(RequestMessage message)
            {
                _invocationCount++;

                return new ResponseMessage(_lastResult = message.Value);
            }            
        }

        private sealed class ObjectCacheModuleSpy : ObjectCacheModule
        {
            private readonly ManualResetEventSlim _evictionHandle;
            private readonly MemoryCache _applicationCache;                        

            internal ObjectCacheModuleSpy(bool hasApplicationCache) : base(LockRecursionPolicy.NoRecursion)
            {
                _evictionHandle = new ManualResetEventSlim();
                _applicationCache = hasApplicationCache ? new MemoryCache("ApplicationCache") : null;
            }

            protected override void Dispose(bool disposing)
            {
                if (DisposeLock.IsDisposed)
                {
                    return;
                }
                if (disposing)
                {
                    _evictionHandle.Dispose();

                    if (_applicationCache != null)
                    {
                        _applicationCache.Dispose();
                    }
                }
                base.Dispose(disposing);
            }

            protected override bool TryGetApplicationCache(out ObjectCache cache)
            {
                return (cache = _applicationCache) != null;
            }

            protected override bool TryGetSessionCache(out ObjectCache cache)
            {
                throw new NotSupportedException();
            }

            internal void WaitForCacheItemEviction(TimeSpan timeout, bool expectEviction = true)
            {
                if (expectEviction)
                {
                    if (_evictionHandle.Wait(timeout))
                    {
                        return;
                    }
                    Assert.Fail("Cache Item was not removed within the expected time interval ({0}).", timeout);
                }
                else if (_evictionHandle.Wait(timeout))
                {
                    Assert.Fail("Cache Item was expected not to be removed.");
                }
            }

            protected internal override void OnCacheEntryDeleted(CacheEntryDeletedEventArgs e)
            {
                _evictionHandle.Set();
                
                base.OnCacheEntryDeleted(e);
            }
        }

        #endregion

        private RequestMessage _message;
        private Query _querySpy;       
        
        [TestInitialize]
        public void Setup()
        {
            _message = new RequestMessage();
            _querySpy = new Query();                    
        }

        private IQuery<ResponseMessage> CreateQuery(QueryExecutionOptions options = QueryExecutionOptions.Default)
        {
            return new QueryWrapper<RequestMessage, ResponseMessage>(_message, _querySpy, options);
        }

        #region [====== ApplicationCache ======]                

        [TestMethod]
        public void GetOrAddToApplicationCache_ExecutesQuery_IfCacheWasNotFound()
        {
            var query = CreateQuery();

            ResponseMessage result;

            using (var cacheModule = new ObjectCacheModuleSpy(false))
            {
                result = cacheModule.GetOrAddToApplicationCache(query, null, null);
            }  
            _querySpy.VerifyThatInvocationCountIs(1);
            _querySpy.VerifyLastResultIs(result);
        }

        [TestMethod]
        public void GetOrAddToApplicationCache_ExecutesQuery_IfResultHadNotBeenCached()
        {
            var query = CreateQuery();

            ResponseMessage result;

            using (var cacheModule = new ObjectCacheModuleSpy(true))
            {
                result = cacheModule.GetOrAddToApplicationCache(query, null, null);
            }
            _querySpy.VerifyThatInvocationCountIs(1);
            _querySpy.VerifyLastResultIs(result);
        }

        [TestMethod]
        public void GetOrAddToApplicationCache_RetrievesCachedResult_IfResultHadBeenCached()
        {
            var query = CreateQuery();

            ResponseMessage resultOne;
            ResponseMessage resultTwo;

            using (var cacheModule = new ObjectCacheModuleSpy(true))
            {
                resultOne = cacheModule.GetOrAddToApplicationCache(query, null, null);
                resultTwo = cacheModule.GetOrAddToApplicationCache(query, null, null);
            }
            _querySpy.VerifyThatInvocationCountIs(1);
            _querySpy.VerifyLastResultIs(resultOne);
            _querySpy.VerifyLastResultIs(resultTwo);
        }

        [TestMethod]
        public void GetOrAddToApplicationCache_WritesToCache_WhenActiveTransactionCommits()
        {
            var query = CreateQuery();

            using (var cacheModule = new ObjectCacheModuleSpy(true))
            {
                ResponseMessage resultOne;
                ResponseMessage resultTwo;                

                using (var transactionScope = new TransactionScope())
                {
                    // Since two consecutive calls are made inside the same transaction, the
                    // cache has not yet had a chance to store the result of the first into cache,
                    // and so the second call executes the query again.
                    resultOne = cacheModule.GetOrAddToApplicationCache(query, null, null);
                    resultTwo = cacheModule.GetOrAddToApplicationCache(query, null, null);

                    _querySpy.VerifyThatInvocationCountIs(2);

                    transactionScope.Complete();  
                }
                // At this point, however, the transaction completed succesfully, and now the result is fetched
                // from cache.
                ResponseMessage resultThree = cacheModule.GetOrAddToApplicationCache(query, null, null);
  
                _querySpy.VerifyThatInvocationCountIs(2);
                _querySpy.VerifyLastResultIs(resultOne);
                _querySpy.VerifyLastResultIs(resultTwo);
                _querySpy.VerifyLastResultIs(resultThree);
            }                          
        }

        [TestMethod]
        public void GetOrAddToApplicationCache_DoesNotWriteToCache_IfActiveTransactionDoesNotCommit()
        {
            var query = CreateQuery();

            using (var cacheModule = new ObjectCacheModuleSpy(true))
            {
                ResponseMessage resultOne;                

                using (new TransactionScope())
                {
                    resultOne = cacheModule.GetOrAddToApplicationCache(query, null, null);

                    _querySpy.VerifyThatInvocationCountIs(1);                    
                }
                // At this point, the transaction has rolled back, so the result
                // of the previous execution was not added to the cache.
                ResponseMessage resultTwo = cacheModule.GetOrAddToApplicationCache(query, null, null);

                _querySpy.VerifyThatInvocationCountIs(2);
                _querySpy.VerifyLastResultIs(resultOne);
                _querySpy.VerifyLastResultIs(resultTwo);                
            }
        }

        #endregion

        #region [====== ApplicationCache - CacheItem Invalidation and Removal ======]

        [TestMethod]
        public void CacheItem_IsRemovedAfterSpecifiedPeriod_IfAbsoluteExpirationIsSet()
        {
            var query = CreateQuery();

            using (var cacheModule = new ObjectCacheModuleSpy(true))
            {
                ResponseMessage resultOne = cacheModule.GetOrAddToApplicationCache(query, TimeSpan.FromSeconds(5), null);

                cacheModule.WaitForCacheItemEviction(TimeSpan.FromSeconds(30));

                ResponseMessage resultTwo = cacheModule.GetOrAddToApplicationCache(query, null, null);

                _querySpy.VerifyThatInvocationCountIs(2);
                _querySpy.VerifyLastResultIs(resultOne);
                _querySpy.VerifyLastResultIs(resultTwo);
            }
        }

        [TestMethod]
        public void CacheItem_IsRemovedAfterSpecifiedPeriod_IfSlidingExpirationIsSet()
        {
            var query = CreateQuery();

            using (var cacheModule = new ObjectCacheModuleSpy(true))
            {
                ResponseMessage resultOne = cacheModule.GetOrAddToApplicationCache(query, null, TimeSpan.FromSeconds(5));

                cacheModule.WaitForCacheItemEviction(TimeSpan.FromSeconds(30));

                ResponseMessage resultTwo = cacheModule.GetOrAddToApplicationCache(query, null, null);

                _querySpy.VerifyThatInvocationCountIs(2);
                _querySpy.VerifyLastResultIs(resultOne);
                _querySpy.VerifyLastResultIs(resultTwo);
            }
        }

        [TestMethod]
        public void CacheItem_IsRemoved_IfItemIsInvalidatedManuallyAndNoTransactionIsActive()
        {
            var query = CreateQuery();

            using (var cacheModule = new ObjectCacheModuleSpy(true))
            {                
                ResponseMessage resultOne = cacheModule.GetOrAddToApplicationCache(query, null, null);

                cacheModule.InvalidateIfRequired<RequestMessage>(message => true);
                cacheModule.WaitForCacheItemEviction(TimeSpan.FromSeconds(10));

                ResponseMessage resultTwo = cacheModule.GetOrAddToApplicationCache(query, null, null);

                _querySpy.VerifyThatInvocationCountIs(2);
                _querySpy.VerifyLastResultIs(resultOne);
                _querySpy.VerifyLastResultIs(resultTwo);
            }
        }

        [TestMethod]
        public void CacheItem_IsRemoved_IfItemIsInvalidatedManuallyAndTransactionCommits()
        {
            var query = CreateQuery();

            using (var cacheModule = new ObjectCacheModuleSpy(true))
            {
                ResponseMessage resultOne = cacheModule.GetOrAddToApplicationCache(query, null, null);

                using (var transactionScope = new TransactionScope())
                {                                        
                    cacheModule.InvalidateIfRequired<RequestMessage>(message => true);

                    transactionScope.Complete();
                }
                cacheModule.WaitForCacheItemEviction(TimeSpan.FromSeconds(10));

                ResponseMessage resultTwo = cacheModule.GetOrAddToApplicationCache(query, null, null);

                _querySpy.VerifyThatInvocationCountIs(2);
                _querySpy.VerifyLastResultIs(resultOne);
                _querySpy.VerifyLastResultIs(resultTwo);
            }
        }

        [TestMethod]
        public void CacheItem_IsNotRemoved_IfItemIsInvalidatedManuallyAndTransactionDoesNotCommit()
        {
            var query = CreateQuery();

            using (var cacheModule = new ObjectCacheModuleSpy(true))
            {
                ResponseMessage resultOne = cacheModule.GetOrAddToApplicationCache(query, null, null);

                using (new TransactionScope())
                {
                    cacheModule.InvalidateIfRequired<RequestMessage>(message => true);                    
                }
                cacheModule.WaitForCacheItemEviction(TimeSpan.FromSeconds(10), false);

                ResponseMessage resultTwo = cacheModule.GetOrAddToApplicationCache(query, null, null);

                _querySpy.VerifyThatInvocationCountIs(1);
                _querySpy.VerifyLastResultIs(resultOne);
                _querySpy.VerifyLastResultIs(resultTwo);
            }
        }

        #endregion

        #region [====== ApplicationCache - QueryExecutionOptions ======]

        [TestMethod]
        public void GetOrAddToCache_DoesNotWriteToCache_IfAllowCacheWriteIsFalse()
        {
            var queryAllowCacheRead = CreateQuery(QueryExecutionOptions.AllowCacheRead);
            var queryDefault = CreateQuery();            

            using (var cacheModule = new ObjectCacheModuleSpy(true))
            {
                // The first call executes the query, but does not store its result into the cache.
                var resultOne = cacheModule.GetOrAddToApplicationCache(queryAllowCacheRead, null, null);

                _querySpy.VerifyThatInvocationCountIs(1);
                _querySpy.VerifyLastResultIs(resultOne);

                // The second call executes the query again, because the result was not cached; but this time,
                // the result is stored into the cache.
                var resultTwo = cacheModule.GetOrAddToApplicationCache(queryDefault, null, null);                

                _querySpy.VerifyThatInvocationCountIs(2);
                _querySpy.VerifyLastResultIs(resultTwo);

                // The third call does not execute the query but simply returns the cached result.
                var resultThree = cacheModule.GetOrAddToApplicationCache(queryAllowCacheRead, null, null);

                _querySpy.VerifyThatInvocationCountIs(2);
                _querySpy.VerifyLastResultIs(resultThree);
            }
        }

        [TestMethod]
        public void GetOrAddToCache_DoesNotWriteOrReadFromCache_IfAllowCacheWriteAndAllowCacheReadAreFalse()
        {
            var queryNone = CreateQuery(QueryExecutionOptions.None);
            var queryDefault = CreateQuery();

            using (var cacheModule = new ObjectCacheModuleSpy(true))
            {
                // The first call executes the query, but does not store its result into the cache.
                var resultOne = cacheModule.GetOrAddToApplicationCache(queryNone, null, null);

                _querySpy.VerifyThatInvocationCountIs(1);
                _querySpy.VerifyLastResultIs(resultOne);

                // The second call executes the query again, because the result was not cached; but this time,
                // the result is stored into the cache.
                var resultTwo = cacheModule.GetOrAddToApplicationCache(queryDefault, null, null);

                _querySpy.VerifyThatInvocationCountIs(2);
                _querySpy.VerifyLastResultIs(resultTwo);

                // The third call executes the query yet again, because the cached result is ignored and then removed.
                var resultThree = cacheModule.GetOrAddToApplicationCache(queryNone, null, null);

                _querySpy.VerifyThatInvocationCountIs(3);
                _querySpy.VerifyLastResultIs(resultThree);

                // The fourth call executes the query again, because the previous call effectively cleared the cache.                
                var resultFour = cacheModule.GetOrAddToApplicationCache(queryDefault, null, null);

                _querySpy.VerifyThatInvocationCountIs(4);
                _querySpy.VerifyLastResultIs(resultFour);
            }
        }

        [TestMethod]
        public void GetOrAddToCache_DoesNotReadFromCache_IfAllowCacheReadIsFalse()
        {
            var queryAllowCacheWrite = CreateQuery(QueryExecutionOptions.AllowCacheWrite);
            var queryDefault = CreateQuery();

            using (var cacheModule = new ObjectCacheModuleSpy(true))
            {
                // The first call executes the query and stores its result into the cache.
                var resultOne = cacheModule.GetOrAddToApplicationCache(queryAllowCacheWrite, null, null);

                _querySpy.VerifyThatInvocationCountIs(1);
                _querySpy.VerifyLastResultIs(resultOne);

                // The second call simply retrieves the result from the cache.
                var resultTwo = cacheModule.GetOrAddToApplicationCache(queryDefault, null, null);

                _querySpy.VerifyThatInvocationCountIs(1);
                _querySpy.VerifyLastResultIs(resultTwo);

                // The third call executes the query again because it is not allowed to read the cached value.
                // However, because it is allowed to write to the cache, it updates the cache with the new value.
                var resultThree = cacheModule.GetOrAddToApplicationCache(queryAllowCacheWrite, null, null);

                _querySpy.VerifyThatInvocationCountIs(2);
                _querySpy.VerifyLastResultIs(resultThree);

                // The fourth call simply retrieves the result from the cache again.
                var resultFour = cacheModule.GetOrAddToApplicationCache(queryDefault, null, null);

                _querySpy.VerifyThatInvocationCountIs(2);
                _querySpy.VerifyLastResultIs(resultFour);
            }
        }

        #endregion                
    }
}
