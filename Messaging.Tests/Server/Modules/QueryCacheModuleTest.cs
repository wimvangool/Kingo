using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Server.Modules
{
    [TestClass]
    public sealed class QueryCacheModuleTest
    {
        #region [====== Messages ======]

        private abstract class RequestMessage<TMessage> : Message<TMessage> where TMessage : RequestMessage<TMessage>
        {
            internal abstract long Value
            {
                get;
            }
        }

        private sealed class NonCachedMessage : RequestMessage<NonCachedMessage>
        {
            private readonly long _value;

            public NonCachedMessage()
            {
                _value = Clock.Current.UtcDateAndTime().Ticks;
            }

            public NonCachedMessage(long value)
            {
                _value = value;
            }

            internal override long Value
            {
                get { return _value; }
            }

            public override NonCachedMessage Copy()
            {
                return new NonCachedMessage(_value);
            }
        }

        [QueryCacheOptions(QueryCacheKind.Application)]
        private sealed class CachedMessage : RequestMessage<CachedMessage>
        {
            private readonly long _value;

            public CachedMessage()
            {
                _value = Clock.Current.UtcDateAndTime().Ticks;
            }

            public CachedMessage(long value)
            {
                _value = value;
            }

            internal override long Value
            {
                get { return _value; }
            }

            public override CachedMessage Copy()
            {
                return new CachedMessage(_value);
            }
        }

        #endregion

        #region [====== Query ======]

        private sealed class QuerySpy<TMessage> : Query<TMessage, string> where TMessage : RequestMessage<TMessage>
        {
            internal int InvocationCount
            {
                get;
                private set;
            }

            internal string LastResult
            {
                get;
                private set;
            }

            protected override string Execute(TMessage message)
            {
                LastResult = message.Value.ToString(CultureInfo.InvariantCulture);
                InvocationCount++;
                return LastResult;
            }
        }

        #endregion

        #region [====== DictionaryCacheManager ======]

        private sealed class DictionaryCacheManager : QueryCacheManager
        {
            private readonly Dictionary<object, object> _cache;

            public DictionaryCacheManager()
            {
                _cache = new Dictionary<object, object>();
            }

            protected override TMessageOut GetOrAddToApplicationCache<TMessageIn, TMessageOut>(TMessageIn message, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration, IQuery<TMessageIn, TMessageOut> query)
            {
                object cachedResult;

                if (_cache.TryGetValue(message, out cachedResult))
                {
                    return (TMessageOut) cachedResult;
                }
                var result = query.Execute(message);

                _cache.Add(message, result);

                return result;
            }

            protected override TMessageOut GetOrAddToSessionCache<TMessageIn, TMessageOut>(TMessageIn message, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration, IQuery<TMessageIn, TMessageOut> query)
            {
                throw new NotSupportedException();
            }

            protected override void InvalidateIfRequired<TMessageIn>(Func<TMessageIn, bool> mustInvalidate)
            {
                throw new NotSupportedException();
            }
        }

        #endregion     

        private DictionaryCacheManager _cacheManager;

        [TestInitialize]
        public void Setup()
        {
            _cacheManager = new DictionaryCacheManager();
        }
        
        [TestMethod]
        public void Module_ExecutesQuery_IfQueryCacheOptionsAttributeHasNotBeenSpecified()
        {            
            var message = new NonCachedMessage();
            var query = new QuerySpy<NonCachedMessage>();

            IQuery<NonCachedMessage, string> module = new QueryCacheModule<NonCachedMessage, string>(query, _cacheManager);

            var result = module.Execute(message);

            Assert.AreEqual(1, query.InvocationCount);
            Assert.AreEqual(query.LastResult, result);
        }

        [TestMethod]
        public void Module_ExecutesQuery_IfResultWasNotFoundInCache()
        {
            var message = new CachedMessage();
            var query = new QuerySpy<CachedMessage>();

            IQuery<CachedMessage, string> module = new QueryCacheModule<CachedMessage, string>(query, _cacheManager);

            var result = module.Execute(message);

            Assert.AreEqual(1, query.InvocationCount);
            Assert.AreEqual(query.LastResult, result);
        }

        [TestMethod]
        public void Module_RetrievesResultFromCache_IfResultHasBeenCached()
        {
            var message = new CachedMessage();
            var query = new QuerySpy<CachedMessage>();

            IQuery<CachedMessage, string> module = new QueryCacheModule<CachedMessage, string>(query, _cacheManager);

            var resultOne = module.Execute(message);
            var resultTwo = module.Execute(message);            

            Assert.AreEqual(1, query.InvocationCount);
            Assert.AreEqual(resultOne, resultTwo);
            Assert.AreEqual(query.LastResult, resultOne);
        }
    }
}
