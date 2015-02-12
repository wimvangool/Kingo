using System.Runtime.Caching;
using System.Threading;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Server.Modules
{
    [TestClass]
    public sealed class ObjectCacheControllerTest
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

        private sealed class Query : Query<RequestMessage, long>
        {
            private int _invocationCount;
            private long _lastResult;

            internal void VerifyThatInvocationCountIs(int invocationCount)
            {
                Assert.AreEqual(invocationCount, _invocationCount);
            }

            internal void VerifyLastResultIs(long result)
            {
                Assert.AreEqual(result, _lastResult);
            }

            protected override long Execute(RequestMessage message)
            {
                _invocationCount++;

                return _lastResult = message.Value;
            }            
        }

        private sealed class ObjectCacheControllerSpy : ObjectCacheController
        {
            private readonly ManualResetEventSlim _evictionHandle;
            private readonly MemoryCache _applicationCache;                        

            internal ObjectCacheControllerSpy(bool hasApplicationCache)
            {
                _evictionHandle = new ManualResetEventSlim();
                _applicationCache = hasApplicationCache ? new MemoryCache("ApplicationCache") : null;
            }

            protected override void Dispose(bool disposing)
            {
                if (IsDisposed)
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
                throw new NotImplementedException();
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

            protected internal override void OnCacheItemRemoved(object messageIn, object messageOut, ObjectCache cache, CacheEntryRemovedReason reason)
            {
                _evictionHandle.Set();
                
                base.OnCacheItemRemoved(messageIn, messageOut, cache, reason);
            }
        }

        #endregion

        private RequestMessage _message;
        private Query _query;
        
        [TestInitialize]
        public void Setup()
        {
            _message = new RequestMessage();
            _query = new Query();            
        }

        #region [====== ApplicationCache ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetOrAddToApplicationCache_Throws_IfMessageIsNull()
        {
            using (var cacheManager = CreateQueryCacheManager(false))
            {
                cacheManager.GetOrAddToApplicationCache(null, null, null, _query);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetOrAddToApplicationCache_Throws_IfQueryIsNull()
        {
            using (var cacheManager = CreateQueryCacheManager(false))
            {
                cacheManager.GetOrAddToApplicationCache(_message, null, null, null as Query);    
            }            
        }

        [TestMethod]
        public void GetOrAddToApplicationCache_ExecutesQuery_IfCacheWasNotFound()
        {
            long result;

            using (var cacheManager = CreateQueryCacheManager(false))
            {
                result = cacheManager.GetOrAddToApplicationCache(_message, null, null, _query);
            }  
            _query.VerifyThatInvocationCountIs(1);
            _query.VerifyLastResultIs(result);
        }

        [TestMethod]
        public void GetOrAddToApplicationCache_ExecutesQuery_IfResultHadNotBeenCached()
        {
            long result;

            using (var cacheManager = CreateQueryCacheManager(true))
            {
                result = cacheManager.GetOrAddToApplicationCache(_message, null, null, _query);
            }
            _query.VerifyThatInvocationCountIs(1);
            _query.VerifyLastResultIs(result);
        }

        [TestMethod]
        public void GetOrAddToApplicationCache_RetrievesCachedResult_IfResultHadBeenCached()
        {
            long resultOne;
            long resultTwo;

            using (var cacheManager = CreateQueryCacheManager(true))
            {
                resultOne = cacheManager.GetOrAddToApplicationCache(_message, null, null, _query);
                resultTwo = cacheManager.GetOrAddToApplicationCache(_message, null, null, _query);
            }
            _query.VerifyThatInvocationCountIs(1);
            _query.VerifyLastResultIs(resultOne);
            _query.VerifyLastResultIs(resultTwo);
        }

        [TestMethod]
        public void GetOrAddToApplicationCache_WritesToCache_WhenActiveTransactionCommits()
        {
            using (var cacheManager = CreateQueryCacheManager(true))
            {
                long resultOne;
                long resultTwo;                

                using (var transactionScope = new TransactionScope())
                {
                    // Since two consecutive calls are made inside the same transaction, the
                    // cache has not yet had a chance to store the result of the first into cache,
                    // and so the second call executes the query again.
                    resultOne = cacheManager.GetOrAddToApplicationCache(_message, null, null, _query);
                    resultTwo = cacheManager.GetOrAddToApplicationCache(_message, null, null, _query);

                    _query.VerifyThatInvocationCountIs(2);

                    transactionScope.Complete();  
                }
                // At this point, however, the transaction completed succesfully, and now the result is fetched
                // from cache.
                long resultThree = cacheManager.GetOrAddToApplicationCache(_message, null, null, _query);
  
                _query.VerifyThatInvocationCountIs(2);
                _query.VerifyLastResultIs(resultOne);
                _query.VerifyLastResultIs(resultTwo);
                _query.VerifyLastResultIs(resultThree);
            }                          
        }

        [TestMethod]
        public void GetOrAddToApplicationCache_DoesNotWriteToCache_IfActiveTransactionDoesNotCommit()
        {
            using (var cacheManager = CreateQueryCacheManager(true))
            {
                long resultOne;                

                using (new TransactionScope())
                {                    
                    resultOne = cacheManager.GetOrAddToApplicationCache(_message, null, null, _query);

                    _query.VerifyThatInvocationCountIs(1);                    
                }
                // At this point, the transaction has rolled back, so the result
                // of the previous execution was not added to the cache.
                long resultTwo = cacheManager.GetOrAddToApplicationCache(_message, null, null, _query);

                _query.VerifyThatInvocationCountIs(2);
                _query.VerifyLastResultIs(resultOne);
                _query.VerifyLastResultIs(resultTwo);                
            }
        }

        #endregion

        #region [====== CacheItem Invalidation and Removal ======]

        [TestMethod]
        public void CacheItem_IsRemovedAfterSpecifiedPeriod_IfAbsoluteExpirationIsSet()
        {            
            using (var cacheManagerSpy = new ObjectCacheControllerSpy(true))
            {
                IQueryCacheController cacheManager = cacheManagerSpy;
                long resultOne = cacheManager.GetOrAddToApplicationCache(_message, TimeSpan.FromSeconds(5), null, _query);

                cacheManagerSpy.WaitForCacheItemEviction(TimeSpan.FromSeconds(30));

                long resultTwo = cacheManager.GetOrAddToApplicationCache(_message, null, null, _query);

                _query.VerifyThatInvocationCountIs(2);
                _query.VerifyLastResultIs(resultOne);
                _query.VerifyLastResultIs(resultTwo);
            }
        }

        [TestMethod]
        public void CacheItem_IsRemovedAfterSpecifiedPeriod_IfSlidingExpirationIsSet()
        {
            using (var cacheManagerSpy = new ObjectCacheControllerSpy(true))
            {
                IQueryCacheController cacheManager = cacheManagerSpy;
                long resultOne = cacheManager.GetOrAddToApplicationCache(_message, null, TimeSpan.FromSeconds(5), _query);

                cacheManagerSpy.WaitForCacheItemEviction(TimeSpan.FromSeconds(30));

                long resultTwo = cacheManager.GetOrAddToApplicationCache(_message, null, null, _query);

                _query.VerifyThatInvocationCountIs(2);
                _query.VerifyLastResultIs(resultOne);
                _query.VerifyLastResultIs(resultTwo);
            }
        }

        [TestMethod]
        public void CacheItem_IsRemoved_IfItemIsInvalidatedManuallyAndNoTransactionIsActive()
        {
            using (var cacheManagerSpy = new ObjectCacheControllerSpy(true))
            {
                IQueryCacheController cacheManager = cacheManagerSpy;
                long resultOne = cacheManager.GetOrAddToApplicationCache(_message, null, null, _query);

                cacheManager.InvalidateIfRequired<RequestMessage>(message => true);
                cacheManagerSpy.WaitForCacheItemEviction(TimeSpan.FromSeconds(10));

                long resultTwo = cacheManager.GetOrAddToApplicationCache(_message, null, null, _query);

                _query.VerifyThatInvocationCountIs(2);
                _query.VerifyLastResultIs(resultOne);
                _query.VerifyLastResultIs(resultTwo);
            }
        }

        [TestMethod]
        public void CacheItem_IsRemoved_IfItemIsInvalidatedManuallyAndTransactionCommits()
        {
            using (var cacheManagerSpy = new ObjectCacheControllerSpy(true))
            {
                IQueryCacheController cacheManager = cacheManagerSpy;
                long resultOne = cacheManager.GetOrAddToApplicationCache(_message, null, null, _query);

                using (var transactionScope = new TransactionScope())
                {                                        
                    cacheManager.InvalidateIfRequired<RequestMessage>(message => true);

                    transactionScope.Complete();
                }
                cacheManagerSpy.WaitForCacheItemEviction(TimeSpan.FromSeconds(10));

                long resultTwo = cacheManager.GetOrAddToApplicationCache(_message, null, null, _query);

                _query.VerifyThatInvocationCountIs(2);
                _query.VerifyLastResultIs(resultOne);
                _query.VerifyLastResultIs(resultTwo);
            }
        }

        [TestMethod]
        public void CacheItem_IsNotRemoved_IfItemIsInvalidatedManuallyAndTransactionDoesNotCommit()
        {
            using (var cacheManagerSpy = new ObjectCacheControllerSpy(true))
            {
                IQueryCacheController cacheManager = cacheManagerSpy;
                long resultOne = cacheManager.GetOrAddToApplicationCache(_message, null, null, _query);

                using (new TransactionScope())
                {
                    cacheManager.InvalidateIfRequired<RequestMessage>(message => true);                    
                }
                cacheManagerSpy.WaitForCacheItemEviction(TimeSpan.FromSeconds(10), false);

                long resultTwo = cacheManager.GetOrAddToApplicationCache(_message, null, null, _query);

                _query.VerifyThatInvocationCountIs(1);
                _query.VerifyLastResultIs(resultOne);
                _query.VerifyLastResultIs(resultTwo);
            }
        }

        #endregion

        private static IQueryCacheController CreateQueryCacheManager(bool hasApplicationCache)
        {
            return new ObjectCacheControllerSpy(hasApplicationCache);
        }
    }
}
