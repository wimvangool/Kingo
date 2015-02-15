using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Server.Modules
{
    [TestClass]
    public sealed class QueryCacheModuleTest
    {
        #region [====== Messages ======]

        private sealed class RequestMessage : Message<RequestMessage>
        {
            internal readonly long Value;

            internal RequestMessage()
            {
                Value = Clock.Current.UtcDateAndTime().Ticks;
            }

            private RequestMessage(long value)
            {
                Value = value;
            }

            public override RequestMessage Copy()
            {
                return new RequestMessage(Value);
            }
        }

        private abstract class ResponseMessage<TMessage> : Message<TMessage> where TMessage : ResponseMessage<TMessage>
        {
            internal long Value
            {
                get;
                set;
            }
        }

        private sealed class NonCachedMessage : ResponseMessage<NonCachedMessage>
        {            
            public override NonCachedMessage Copy()
            {
                return new NonCachedMessage()
                {
                    Value = Value
                };
            }
        }

        [QueryCacheOptions(QueryCacheKind.Application)]
        private sealed class CachedMessage : ResponseMessage<CachedMessage>
        {            
            public override CachedMessage Copy()
            {
                return new CachedMessage()
                {
                    Value = Value
                };
            }
        }

        #endregion

        #region [====== Query ======]

        private sealed class QuerySpy<TMessage> : Query<RequestMessage, TMessage> where TMessage : ResponseMessage<TMessage>, new()
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

            public override TMessage Execute(RequestMessage message)
            {
                LastResult = message.Value;
                InvocationCount++;

                return new TMessage()
                {
                    Value = message.Value
                };
            }
        }

        #endregion

        #region [====== DictionaryCacheManager ======]

        private sealed class DictionaryCacheManager : QueryCacheController
        {
            private readonly Dictionary<object, object> _cache;

            public DictionaryCacheManager()
            {
                _cache = new Dictionary<object, object>();
            }

            public override TMessageOut GetOrAddToApplicationCache<TMessageIn, TMessageOut>(QueryRequestMessage<TMessageIn> message, IQuery<TMessageIn, TMessageOut> query)
            {
                object cachedResult;

                if (_cache.TryGetValue(message, out cachedResult))
                {
                    return (TMessageOut) cachedResult;
                }
                var result = query.Execute(message.Parameters);

                _cache.Add(message, result);

                return result;
            }

            public override TMessageOut GetOrAddToSessionCache<TMessageIn, TMessageOut>(QueryRequestMessage<TMessageIn> message, IQuery<TMessageIn, TMessageOut> query)
            {
                throw new NotSupportedException();
            }

            public override void InvalidateIfRequired<TMessageIn>(Func<TMessageIn, bool> mustInvalidate)
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
            var message = new RequestMessage();
            var query = new QuerySpy<NonCachedMessage>();

            var module = new QueryCacheModule<RequestMessage, NonCachedMessage>(query, QueryExecutionOptions.Default, _cacheManager);
            var result = module.Execute(message);

            Assert.AreEqual(1, query.InvocationCount);
            Assert.AreEqual(query.LastResult, result.Value);
        }

        [TestMethod]
        public void Module_ExecutesQuery_IfResultWasNotFoundInCache()
        {
            var message = new RequestMessage();
            var query = new QuerySpy<CachedMessage>();

            var module = new QueryCacheModule<RequestMessage, CachedMessage>(query, QueryExecutionOptions.Default, _cacheManager);
            var result = module.Execute(message);

            Assert.AreEqual(1, query.InvocationCount);
            Assert.AreEqual(query.LastResult, result.Value);
        }

        [TestMethod]
        public void Module_RetrievesResultFromCache_IfResultHasBeenCached()
        {
            var message = new RequestMessage();
            var query = new QuerySpy<CachedMessage>();

            var module = new QueryCacheModule<RequestMessage, CachedMessage>(query, QueryExecutionOptions.Default, _cacheManager);
            var resultOne = module.Execute(message);
            var resultTwo = module.Execute(message);            

            Assert.AreEqual(1, query.InvocationCount);
            Assert.AreEqual(resultOne.Value, resultTwo.Value);
            Assert.AreEqual(query.LastResult, resultOne.Value);
        }
    }
}
