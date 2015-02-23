using System.Runtime.Caching;
using System.Threading;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Server.Caching
{
    [TestClass]
    public sealed class ObjectCacheProviderTest
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

        private sealed class ObjectCacheControllerSpy : ObjectCacheProvider
        {
            private readonly ManualResetEventSlim _evictionHandle;
            private readonly MemoryCache _applicationCache;                        

            internal ObjectCacheControllerSpy(bool hasApplicationCache) : base(LockRecursionPolicy.NoRecursion)
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

            protected internal override void OnCacheEntryDeleted(CacheEntryDeletedEventArgs e)
            {
                _evictionHandle.Set();
                
                base.OnCacheEntryDeleted(e);
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
            using (var cacheProvider = CreateQueryCacheManager(false))
            {
                cacheProvider.GetOrAddToApplicationCache(null, _query, CreateCachePolicy(null, null));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetOrAddToApplicationCache_Throws_IfQueryIsNull()
        {
            using (var cacheProvider = CreateQueryCacheManager(false))
            {
                cacheProvider.GetOrAddToApplicationCache(_message, null as Query, CreateCachePolicy(null, null));    
            }            
        }

        [TestMethod]
        public void GetOrAddToApplicationCache_ExecutesQuery_IfCacheWasNotFound()
        {
            ResponseMessage result;

            using (var cacheProvider = CreateQueryCacheManager(false))
            {
                result = cacheProvider.GetOrAddToApplicationCache(_message, _query, CreateCachePolicy(null, null));
            }  
            _query.VerifyThatInvocationCountIs(1);
            _query.VerifyLastResultIs(result);
        }

        [TestMethod]
        public void GetOrAddToApplicationCache_ExecutesQuery_IfResultHadNotBeenCached()
        {
            ResponseMessage result;

            using (var cacheProvider = CreateQueryCacheManager(true))
            {
                result = cacheProvider.GetOrAddToApplicationCache(_message, _query, CreateCachePolicy(null, null));
            }
            _query.VerifyThatInvocationCountIs(1);
            _query.VerifyLastResultIs(result);
        }

        [TestMethod]
        public void GetOrAddToApplicationCache_RetrievesCachedResult_IfResultHadBeenCached()
        {
            ResponseMessage resultOne;
            ResponseMessage resultTwo;

            using (var cacheProvider = CreateQueryCacheManager(true))
            {
                resultOne = cacheProvider.GetOrAddToApplicationCache(_message, _query, CreateCachePolicy(null, null));
                resultTwo = cacheProvider.GetOrAddToApplicationCache(_message, _query, CreateCachePolicy(null, null));
            }
            _query.VerifyThatInvocationCountIs(1);
            _query.VerifyLastResultIs(resultOne);
            _query.VerifyLastResultIs(resultTwo);
        }

        [TestMethod]
        public void GetOrAddToApplicationCache_WritesToCache_WhenActiveTransactionCommits()
        {
            using (var cacheProvider = CreateQueryCacheManager(true))
            {
                ResponseMessage resultOne;
                ResponseMessage resultTwo;                

                using (var transactionScope = new TransactionScope())
                {
                    // Since two consecutive calls are made inside the same transaction, the
                    // cache has not yet had a chance to store the result of the first into cache,
                    // and so the second call executes the query again.
                    resultOne = cacheProvider.GetOrAddToApplicationCache(_message, _query, CreateCachePolicy(null, null));
                    resultTwo = cacheProvider.GetOrAddToApplicationCache(_message, _query, CreateCachePolicy(null, null));

                    _query.VerifyThatInvocationCountIs(2);

                    transactionScope.Complete();  
                }
                // At this point, however, the transaction completed succesfully, and now the result is fetched
                // from cache.
                ResponseMessage resultThree = cacheProvider.GetOrAddToApplicationCache(_message, _query, CreateCachePolicy(null, null));
  
                _query.VerifyThatInvocationCountIs(2);
                _query.VerifyLastResultIs(resultOne);
                _query.VerifyLastResultIs(resultTwo);
                _query.VerifyLastResultIs(resultThree);
            }                          
        }

        [TestMethod]
        public void GetOrAddToApplicationCache_DoesNotWriteToCache_IfActiveTransactionDoesNotCommit()
        {
            using (var cacheProvider = CreateQueryCacheManager(true))
            {
                ResponseMessage resultOne;                

                using (new TransactionScope())
                {
                    resultOne = cacheProvider.GetOrAddToApplicationCache(_message, _query, CreateCachePolicy(null, null));

                    _query.VerifyThatInvocationCountIs(1);                    
                }
                // At this point, the transaction has rolled back, so the result
                // of the previous execution was not added to the cache.
                ResponseMessage resultTwo = cacheProvider.GetOrAddToApplicationCache(_message, _query, CreateCachePolicy(null, null));

                _query.VerifyThatInvocationCountIs(2);
                _query.VerifyLastResultIs(resultOne);
                _query.VerifyLastResultIs(resultTwo);                
            }
        }

        #endregion

        #region [====== ApplicationCache - CacheItem Invalidation and Removal ======]

        [TestMethod]
        public void CacheItem_IsRemovedAfterSpecifiedPeriod_IfAbsoluteExpirationIsSet()
        {            
            using (var cacheProvider = new ObjectCacheControllerSpy(true))
            {
                ResponseMessage resultOne = cacheProvider.GetOrAddToApplicationCache(_message, _query, CreateCachePolicy(TimeSpan.FromSeconds(5), null));

                cacheProvider.WaitForCacheItemEviction(TimeSpan.FromSeconds(30));

                ResponseMessage resultTwo = cacheProvider.GetOrAddToApplicationCache(_message, _query, CreateCachePolicy(null, null));

                _query.VerifyThatInvocationCountIs(2);
                _query.VerifyLastResultIs(resultOne);
                _query.VerifyLastResultIs(resultTwo);
            }
        }

        [TestMethod]
        public void CacheItem_IsRemovedAfterSpecifiedPeriod_IfSlidingExpirationIsSet()
        {
            using (var cacheProvider = new ObjectCacheControllerSpy(true))
            {
                ResponseMessage resultOne = cacheProvider.GetOrAddToApplicationCache(_message, _query, CreateCachePolicy(null, TimeSpan.FromSeconds(5)));

                cacheProvider.WaitForCacheItemEviction(TimeSpan.FromSeconds(30));

                ResponseMessage resultTwo = cacheProvider.GetOrAddToApplicationCache(_message, _query, CreateCachePolicy(null, null));

                _query.VerifyThatInvocationCountIs(2);
                _query.VerifyLastResultIs(resultOne);
                _query.VerifyLastResultIs(resultTwo);
            }
        }

        [TestMethod]
        public void CacheItem_IsRemoved_IfItemIsInvalidatedManuallyAndNoTransactionIsActive()
        {
            using (var cacheProvider = new ObjectCacheControllerSpy(true))
            {                
                ResponseMessage resultOne = cacheProvider.GetOrAddToApplicationCache(_message, _query, CreateCachePolicy(null, null));

                cacheProvider.InvalidateIfRequired<RequestMessage>(message => true);
                cacheProvider.WaitForCacheItemEviction(TimeSpan.FromSeconds(10));

                ResponseMessage resultTwo = cacheProvider.GetOrAddToApplicationCache(_message, _query, CreateCachePolicy(null, null));

                _query.VerifyThatInvocationCountIs(2);
                _query.VerifyLastResultIs(resultOne);
                _query.VerifyLastResultIs(resultTwo);
            }
        }

        [TestMethod]
        public void CacheItem_IsRemoved_IfItemIsInvalidatedManuallyAndTransactionCommits()
        {
            using (var cacheProvider = new ObjectCacheControllerSpy(true))
            {
                ResponseMessage resultOne = cacheProvider.GetOrAddToApplicationCache(_message, _query, CreateCachePolicy(null, null));

                using (var transactionScope = new TransactionScope())
                {                                        
                    cacheProvider.InvalidateIfRequired<RequestMessage>(message => true);

                    transactionScope.Complete();
                }
                cacheProvider.WaitForCacheItemEviction(TimeSpan.FromSeconds(10));

                ResponseMessage resultTwo = cacheProvider.GetOrAddToApplicationCache(_message, _query, CreateCachePolicy(null, null));

                _query.VerifyThatInvocationCountIs(2);
                _query.VerifyLastResultIs(resultOne);
                _query.VerifyLastResultIs(resultTwo);
            }
        }

        [TestMethod]
        public void CacheItem_IsNotRemoved_IfItemIsInvalidatedManuallyAndTransactionDoesNotCommit()
        {
            using (var cacheProvider = new ObjectCacheControllerSpy(true))
            {
                ResponseMessage resultOne = cacheProvider.GetOrAddToApplicationCache(_message, _query, CreateCachePolicy(null, null));

                using (new TransactionScope())
                {
                    cacheProvider.InvalidateIfRequired<RequestMessage>(message => true);                    
                }
                cacheProvider.WaitForCacheItemEviction(TimeSpan.FromSeconds(10), false);

                ResponseMessage resultTwo = cacheProvider.GetOrAddToApplicationCache(_message, _query, CreateCachePolicy(null, null));

                _query.VerifyThatInvocationCountIs(1);
                _query.VerifyLastResultIs(resultOne);
                _query.VerifyLastResultIs(resultTwo);
            }
        }

        #endregion

        #region [====== ApplicationCache - QueryExecutionOptions ======]

        [TestMethod]
        public void GetOrAddToCache_DoesNotWriteToCache_IfAllowCacheWriteIsFalse()
        {
            var allowReadPolicy = CreateCachePolicy(null, null, QueryExecutionOptions.AllowCacheRead);
            var defaultPolicy = CreateCachePolicy(null, null);

            using (var cacheProvider = new ObjectCacheControllerSpy(true))
            {
                // The first call executes the query, but does not store its result into the cache.
                var resultOne = cacheProvider.GetOrAddToApplicationCache(_message, _query, allowReadPolicy);

                _query.VerifyThatInvocationCountIs(1);
                _query.VerifyLastResultIs(resultOne);

                // The second call executes the query again, because the result was not cached; but this time,
                // the result is stored into the cache.
                var resultTwo = cacheProvider.GetOrAddToApplicationCache(_message, _query, defaultPolicy);                

                _query.VerifyThatInvocationCountIs(2);
                _query.VerifyLastResultIs(resultTwo);

                // The third call does not execute the query but simply returns the cached result.
                var resultThree = cacheProvider.GetOrAddToApplicationCache(_message, _query, allowReadPolicy);

                _query.VerifyThatInvocationCountIs(2);
                _query.VerifyLastResultIs(resultThree);
            }
        }

        [TestMethod]
        public void GetOrAddToCache_DoesNotWriteOrReadFromCache_IfAllowCacheWriteAndAllowCacheReadAreFalse()
        {
            var nonePolicy = CreateCachePolicy(null, null, QueryExecutionOptions.None);
            var defaultPolicy = CreateCachePolicy(null, null);

            using (var cacheProvider = new ObjectCacheControllerSpy(true))
            {
                // The first call executes the query, but does not store its result into the cache.
                var resultOne = cacheProvider.GetOrAddToApplicationCache(_message, _query, nonePolicy);

                _query.VerifyThatInvocationCountIs(1);
                _query.VerifyLastResultIs(resultOne);

                // The second call executes the query again, because the result was not cached; but this time,
                // the result is stored into the cache.
                var resultTwo = cacheProvider.GetOrAddToApplicationCache(_message, _query, defaultPolicy);

                _query.VerifyThatInvocationCountIs(2);
                _query.VerifyLastResultIs(resultTwo);

                // The third call executes the query yet again, because the cached result is ignored and then removed.
                var resultThree = cacheProvider.GetOrAddToApplicationCache(_message, _query, nonePolicy);

                _query.VerifyThatInvocationCountIs(3);
                _query.VerifyLastResultIs(resultThree);

                // The fourth call executes the query again, because the previous call effectively cleared the cache.                
                var resultFour = cacheProvider.GetOrAddToApplicationCache(_message, _query, defaultPolicy);

                _query.VerifyThatInvocationCountIs(4);
                _query.VerifyLastResultIs(resultFour);
            }
        }

        [TestMethod]
        public void GetOrAddToCache_DoesNotReadFromCache_IfAllowCacheReadIsFalse()
        {
            var allowWritePolicy = CreateCachePolicy(null, null, QueryExecutionOptions.AllowCacheWrite);
            var defaultPolicy = CreateCachePolicy(null, null);

            using (var cacheProvider = new ObjectCacheControllerSpy(true))
            {
                // The first call executes the query and stores its result into the cache.
                var resultOne = cacheProvider.GetOrAddToApplicationCache(_message, _query, allowWritePolicy);

                _query.VerifyThatInvocationCountIs(1);
                _query.VerifyLastResultIs(resultOne);

                // The second call simply retrieves the result from the cache.
                var resultTwo = cacheProvider.GetOrAddToApplicationCache(_message, _query, defaultPolicy);

                _query.VerifyThatInvocationCountIs(1);
                _query.VerifyLastResultIs(resultTwo);

                // The third call executes the query again because it is not allowed to read the cached value.
                // However, because it is allowed to write to the cache, it updates the cache with the new value.
                var resultThree = cacheProvider.GetOrAddToApplicationCache(_message, _query, allowWritePolicy);

                _query.VerifyThatInvocationCountIs(2);
                _query.VerifyLastResultIs(resultThree);

                // The fourth call simply retrieves the result from the cache again.
                var resultFour = cacheProvider.GetOrAddToApplicationCache(_message, _query, defaultPolicy);

                _query.VerifyThatInvocationCountIs(2);
                _query.VerifyLastResultIs(resultFour);
            }
        }

        #endregion

        private static QueryCachePolicy CreateCachePolicy(TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration, QueryExecutionOptions options = QueryExecutionOptions.Default)
        {
            return new QueryCachePolicy(options, absoluteExpiration, slidingExpiration);
        }

        private static IQueryCacheProvider CreateQueryCacheManager(bool hasApplicationCache)
        {
            return new ObjectCacheControllerSpy(hasApplicationCache);
        }
    }
}
