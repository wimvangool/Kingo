using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Server.Caching
{
    [TestClass]
    public sealed class QueryCacheModuleTest
    {
        #region [====== Messages ======]

        private abstract class RequestMessage<TMessage> : Message<TMessage> where TMessage : RequestMessage<TMessage>
        {
            internal readonly long Value;

            protected RequestMessage()
            {
                Value = Clock.Current.UtcDateAndTime().Ticks;
            }

            protected RequestMessage(long value)
            {
                Value = value;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as RequestMessage<TMessage>);
            }

            public bool Equals(RequestMessage<TMessage> other)
            {
                if (ReferenceEquals(other, null))
                {
                    return false;
                }
                if (ReferenceEquals(other, this))
                {
                    return true;
                }
                return Value == other.Value;
            }

            public override int GetHashCode()
            {
                return Value.GetHashCode();
            }
        }

        private sealed class NonCachedMessage : RequestMessage<NonCachedMessage>
        {
            internal NonCachedMessage() { }

            private NonCachedMessage(long value) : base(value) { }

            public override NonCachedMessage Copy()
            {
                return new NonCachedMessage(Value);                
            }
        }

        [QueryCacheOptions(QueryCacheKind.Application)]
        private sealed class CachedMessage : RequestMessage<CachedMessage>
        {
            internal CachedMessage() { }

            private CachedMessage(long value) : base(value) { }

            public override CachedMessage Copy()
            {
                return new CachedMessage(Value);                
            }
        }

        private sealed class ResponseMessage : Message<ResponseMessage>
        {
            internal long Value
            {
                get;
                set;
            }

            public override ResponseMessage Copy()
            {
                return new ResponseMessage() { Value = Value };
            }
        }        

        #endregion

        #region [====== Query ======]

        private sealed class QuerySpy<TMessage> : Query<RequestMessage<TMessage>, ResponseMessage> where TMessage : RequestMessage<TMessage>
        {
            internal int InvocationCount
            {
                get;
                private set;
            }

            internal long LastResult
            {
                get;
                private set;
            }

            public override ResponseMessage Execute(RequestMessage<TMessage> message)
            {
                LastResult = message.Value;
                InvocationCount++;

                return new ResponseMessage()
                {
                    Value = message.Value
                };
            }
        }

        #endregion

        #region [====== DictionaryCacheModule ======]

        private sealed class DictionaryCacheModule : QueryCacheModule
        {            
            public DictionaryCacheModule() : base(LockRecursionPolicy.NoRecursion) { }

            protected override void Dispose(bool disposing)
            {
                if (InstanceLock.IsDisposed)
                {
                    return;
                }
                if (disposing)
                {
                    _ApplicationCache.Value.Clear();
                }
                base.Dispose(disposing);
            }

            protected override bool TryGetApplicationCacheFactory(out IQueryCacheManagerFactory applicationCacheFactory)
            {
                applicationCacheFactory = new DictionaryCacheManagerFactory(_ApplicationCache.Value, this);
                return true;
            }

            protected override bool TryGetSessionCacheFactory(out IQueryCacheManagerFactory sessionCacheFactory)
            {
                throw new NotSupportedException();
            }

            private static readonly ThreadLocal<Dictionary<object, object>> _ApplicationCache =
                new ThreadLocal<Dictionary<object, object>>(() => new Dictionary<object, object>());
        }

        private sealed class DictionaryCacheManager : QueryCacheManager
        {
            private readonly DictionaryCacheModule _cacheModule;
            private readonly Dictionary<object, object> _cache;

            internal DictionaryCacheManager(DictionaryCacheModule cacheModule, Dictionary<object, object> cache) : base(LockRecursionPolicy.NoRecursion)
            {
                _cacheModule = cacheModule;
                _cache = cache;
            }            

            protected override QueryCacheModule CacheProvider
            {
                get { return _cacheModule; }
            }

            protected override bool TryGetCacheEntry(object messageIn, out object messageOut)
            {
                return _cache.TryGetValue(messageIn, out messageOut);
            }

            protected override bool ContainsCacheEntry(object messageIn)
            {
                return _cache.ContainsKey(messageIn);
            }

            protected override void InsertCacheEntry(object messageIn, object messageOut, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration)
            {
                _cache.Add(messageIn, messageOut);
            }

            protected override bool UpdateCacheEntry(object messageIn, object messageOut, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration)
            {
                _cache[messageIn] = messageOut;
                return true;
            }

            protected override bool DeleteCacheEntry(object messageIn)
            {
                return _cache.Remove(messageIn);
            }

            protected override IEnumerable<CacheEntry> SelectCacheEntriesWithKeyOfType(Type keyType)
            {
                throw new NotSupportedException();
            }
        }

        private sealed class DictionaryCacheManagerFactory : QueryCacheManagerFactory<Dictionary<object, object>>
        {
            private readonly DictionaryCacheModule _cacheModule;

            internal DictionaryCacheManagerFactory(Dictionary<object, object> cache, DictionaryCacheModule cacheModule)
                : base(cache)
            {
                _cacheModule = cacheModule;
            }

            public override QueryCacheManager CreateCacheManager()
            {
                return new DictionaryCacheManager(_cacheModule, Cache);
            }
        }

        #endregion     

        private DictionaryCacheModule _cacheModule;

        [TestInitialize]
        public void Setup()
        {
            _cacheModule = new DictionaryCacheModule();
            _cacheModule.Start();
        }

        [TestCleanup]
        public void TearDown()
        {
            _cacheModule.Dispose();
        }
        
        [TestMethod]
        public void Module_ExecutesQuery_IfQueryCacheOptionsAttributeHasNotBeenSpecified()
        {                        
            var querySpy = new QuerySpy<NonCachedMessage>();
            var query = new QueryWrapper<NonCachedMessage, ResponseMessage>(new NonCachedMessage(), querySpy, QueryExecutionOptions.Default);

            var result = _cacheModule.GetOrAddToApplicationCache(query, null, null);

            Assert.AreEqual(1, querySpy.InvocationCount);
            Assert.AreEqual(querySpy.LastResult, result.Value);
        }

        [TestMethod]
        public void Module_ExecutesQuery_IfResultWasNotFoundInCache()
        {
            var querySpy = new QuerySpy<CachedMessage>();
            var query = new QueryWrapper<CachedMessage, ResponseMessage>(new CachedMessage(), querySpy, QueryExecutionOptions.Default);            

            var result = _cacheModule.GetOrAddToApplicationCache(query, null, null);

            Assert.AreEqual(1, querySpy.InvocationCount);
            Assert.AreEqual(querySpy.LastResult, result.Value);
        }

        [TestMethod]
        public void Module_RetrievesResultFromCache_IfResultHasBeenCached()
        {
            var querySpy = new QuerySpy<CachedMessage>();
            var query = new QueryWrapper<CachedMessage, ResponseMessage>(new CachedMessage(), querySpy, QueryExecutionOptions.Default);

            var resultOne = _cacheModule.GetOrAddToApplicationCache(query, null, null);
            var resultTwo = _cacheModule.GetOrAddToApplicationCache(query, null, null);           

            Assert.AreEqual(1, querySpy.InvocationCount);            
            Assert.AreEqual(querySpy.LastResult, resultOne.Value);
            Assert.AreEqual(querySpy.LastResult, resultTwo.Value);
        }
    }
}
