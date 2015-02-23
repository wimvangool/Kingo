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

        #region [====== DictionaryCacheManager ======]

        private sealed class DictionaryCacheProvider : QueryCacheProvider
        {            
            public DictionaryCacheProvider() : base(LockRecursionPolicy.NoRecursion) { }            

            protected override bool TryGetApplicationCacheFactory(out IQueryCacheManagerFactory applicationCacheFactory)
            {
                applicationCacheFactory = new DictionaryCacheManagerFactory(_ApplicationCache, this);
                return true;
            }

            protected override bool TryGetSessionCacheFactory(out IQueryCacheManagerFactory sessionCacheFactory)
            {
                throw new NotSupportedException();
            }

            private static readonly Dictionary<object, object> _ApplicationCache = new Dictionary<object, object>();
        }

        private sealed class DictionaryCacheManager : QueryCacheManager
        {
            private readonly DictionaryCacheProvider _cacheProvider;
            private readonly Dictionary<object, object> _cache;

            internal DictionaryCacheManager(DictionaryCacheProvider cacheProvider, Dictionary<object, object> cache) : base(LockRecursionPolicy.NoRecursion)
            {
                _cacheProvider = cacheProvider;
                _cache = cache;
            }            

            protected override QueryCacheProvider CacheProvider
            {
                get { return _cacheProvider; }
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
            private readonly DictionaryCacheProvider _cacheProvider;

            internal DictionaryCacheManagerFactory(Dictionary<object, object> cache, DictionaryCacheProvider cacheProvider)
                : base(cache)
            {
                _cacheProvider = cacheProvider;
            }

            public override QueryCacheManager CreateCacheManager()
            {
                return new DictionaryCacheManager(_cacheProvider, Cache);
            }
        }

        #endregion     

        private DictionaryCacheProvider _cacheProvider;

        [TestInitialize]
        public void Setup()
        {
            _cacheProvider = new DictionaryCacheProvider();
        }
        
        [TestMethod]
        public void Module_ExecutesQuery_IfQueryCacheOptionsAttributeHasNotBeenSpecified()
        {            
            var message = new NonCachedMessage();
            var query = new QuerySpy<NonCachedMessage>();

            var module = new QueryCacheModule<NonCachedMessage, ResponseMessage>(query, QueryExecutionOptions.Default, _cacheProvider);
            var result = module.Execute(message);

            Assert.AreEqual(1, query.InvocationCount);
            Assert.AreEqual(query.LastResult, result.Value);
        }

        [TestMethod]
        public void Module_ExecutesQuery_IfResultWasNotFoundInCache()
        {
            var message = new CachedMessage();
            var query = new QuerySpy<CachedMessage>();

            var module = new QueryCacheModule<CachedMessage, ResponseMessage>(query, QueryExecutionOptions.Default, _cacheProvider);
            var result = module.Execute(message);

            Assert.AreEqual(1, query.InvocationCount);
            Assert.AreEqual(query.LastResult, result.Value);
        }

        [TestMethod]
        public void Module_RetrievesResultFromCache_IfResultHasBeenCached()
        {
            var message = new CachedMessage();
            var query = new QuerySpy<CachedMessage>();

            var module = new QueryCacheModule<CachedMessage, ResponseMessage>(query, QueryExecutionOptions.Default, _cacheProvider);
            var resultOne = module.Execute(message);
            var resultTwo = module.Execute(message);            

            Assert.AreEqual(1, query.InvocationCount);
            Assert.AreEqual(resultOne.Value, resultTwo.Value);
            Assert.AreEqual(query.LastResult, resultOne.Value);
        }
    }
}
