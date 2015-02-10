using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Server.Modules
{
    [TestClass]
    public sealed class QueryCacheOptionsAttributeTest
    {
        #region [====== Nested Types ======]

        private sealed class RequestMessage : Message<RequestMessage>
        {            
            public override RequestMessage Copy()
            {
                return new RequestMessage();
            }
        }

        private sealed class Query : Query<RequestMessage, object>
        {
            protected override object Execute(RequestMessage message)
            {
                return message;
            }
        }

        private sealed class QueryCacheManagerSpy : QueryCacheManager
        {
            private QueryCacheKind? _kind;
            private object _message;
            private TimeSpan? _absoluteExpiration;
            private TimeSpan? _slidingExpiration;
            private object _query;            

            internal void VerifyThatCorrectKindWasInvoked(QueryCacheKind kind)
            {
                Assert.IsTrue(_kind.HasValue);
                Assert.AreEqual(kind, _kind.Value);
            }

            internal void VerifyThatMessageWas(object message)
            {
                Assert.AreSame(message, _message);               
            }

            internal void VerifyThatAbsoluteExpirationWas(TimeSpan? timeout)
            {
                Assert.AreEqual(timeout, _absoluteExpiration);
            }

            internal void VerifyThatSlidingExpirationWas(TimeSpan? timeout)
            {
                Assert.AreEqual(timeout, _slidingExpiration);
            }

            internal void VerifyThatQueryWas(object query)
            {
                Assert.AreSame(query, _query);
            }

            #region [====== QueryCacheManager ======]

            protected override TMessageOut GetOrAddToApplicationCache<TMessageIn, TMessageOut>(TMessageIn message, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration, IQuery<TMessageIn, TMessageOut> query)
            {
                _kind = QueryCacheKind.Application;
                _message = message;
                _absoluteExpiration = absoluteExpiration;
                _slidingExpiration = slidingExpiration;
                _query = query;

                return query.Execute(message);
            }

            protected override TMessageOut GetOrAddToSessionCache<TMessageIn, TMessageOut>(TMessageIn message, TimeSpan? absoluteExpiration, TimeSpan? slidingExpiration, IQuery<TMessageIn, TMessageOut> query)
            {
                _kind = QueryCacheKind.Session;
                _message = message;
                _absoluteExpiration = absoluteExpiration;
                _slidingExpiration = slidingExpiration;
                _query = query;

                return query.Execute(message);
            }

            protected override void InvalidateIfRequired<TMessageIn>(Func<TMessageIn, bool> mustInvalidate)
            {
                throw new NotSupportedException();
            }

            #endregion            
        }

        #endregion

        private RequestMessage _message;
        private Query _query;
        private QueryCacheManagerSpy _cacheManager;

        [TestInitialize]
        public void Setup()
        {
            _message = new RequestMessage();
            _query = new Query();
            _cacheManager = new QueryCacheManagerSpy();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetOrAddToCache_Throws_IfKindIsNotValid()
        {
            var attribute = new QueryCacheOptionsAttribute((QueryCacheKind) 10);

            attribute.GetOrAddToCache(_message, _query, _cacheManager);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetOrAddToCache_Throws_IfAbsoluteExpirationIsNotValid()
        {
            var attribute = new QueryCacheOptionsAttribute(QueryCacheKind.Application)
            {
                AbsoluteExpiration = "abc"
            };

            attribute.GetOrAddToCache(_message, _query, _cacheManager);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetOrAddToCache_Throws_IfSlidingExpirationIsNotValid()
        {
            var attribute = new QueryCacheOptionsAttribute(QueryCacheKind.Application)
            {
                SlidingExpiration = "abc"
            };

            attribute.GetOrAddToCache(_message, _query, _cacheManager);
        }        

        [TestMethod]
        public void GetOrAddToCache_InvokesCacheManagerCorrectly_IfKindIsApplicationAndNoTimeoutsAreSet()
        {
            var attribute = new QueryCacheOptionsAttribute(QueryCacheKind.Application);

            attribute.GetOrAddToCache(_message, _query, _cacheManager);

            _cacheManager.VerifyThatCorrectKindWasInvoked(QueryCacheKind.Application);
            _cacheManager.VerifyThatMessageWas(_message);
            _cacheManager.VerifyThatAbsoluteExpirationWas(null);
            _cacheManager.VerifyThatSlidingExpirationWas(null);     
            _cacheManager.VerifyThatQueryWas(_query);
        }

        [TestMethod]
        public void GetOrAddToCache_InvokesCacheManagerCorrectly_IfKindIsApplicationAndAbsoluteExpirationIsSet()
        {
            var attribute = new QueryCacheOptionsAttribute(QueryCacheKind.Application)
            {
                AbsoluteExpiration = "00:01:22"
            };

            attribute.GetOrAddToCache(_message, _query, _cacheManager);

            _cacheManager.VerifyThatCorrectKindWasInvoked(QueryCacheKind.Application);
            _cacheManager.VerifyThatMessageWas(_message);
            _cacheManager.VerifyThatAbsoluteExpirationWas(TimeSpan.FromMinutes(1).Add(TimeSpan.FromSeconds(22)));
            _cacheManager.VerifyThatSlidingExpirationWas(null);
            _cacheManager.VerifyThatQueryWas(_query);
        }

        [TestMethod]
        public void GetOrAddToCache_InvokesCacheManagerCorrectly_IfKindIsApplicationAndSlidingExpirationIsSet()
        {
            var attribute = new QueryCacheOptionsAttribute(QueryCacheKind.Application)
            {
                SlidingExpiration = "00:33:16"
            };

            attribute.GetOrAddToCache(_message, _query, _cacheManager);

            _cacheManager.VerifyThatCorrectKindWasInvoked(QueryCacheKind.Application);
            _cacheManager.VerifyThatMessageWas(_message);
            _cacheManager.VerifyThatAbsoluteExpirationWas(null);
            _cacheManager.VerifyThatSlidingExpirationWas(TimeSpan.FromMinutes(33).Add(TimeSpan.FromSeconds(16)));
            _cacheManager.VerifyThatQueryWas(_query);
        }

        [TestMethod]
        public void GetOrAddToCache_InvokesCacheManagerCorrectly_IfKindIsSessionAndNoTimeoutsAreSet()
        {
            var attribute = new QueryCacheOptionsAttribute(QueryCacheKind.Session);

            attribute.GetOrAddToCache(_message, _query, _cacheManager);

            _cacheManager.VerifyThatCorrectKindWasInvoked(QueryCacheKind.Session);
            _cacheManager.VerifyThatMessageWas(_message);
            _cacheManager.VerifyThatAbsoluteExpirationWas(null);
            _cacheManager.VerifyThatSlidingExpirationWas(null);
            _cacheManager.VerifyThatQueryWas(_query);
        }

        [TestMethod]
        public void GetOrAddToCache_InvokesCacheManagerCorrectly_IfKindIsSessionAndAbsoluteExpirationIsSet()
        {
            var attribute = new QueryCacheOptionsAttribute(QueryCacheKind.Session)
            {
                AbsoluteExpiration = "00:01:22"
            };

            attribute.GetOrAddToCache(_message, _query, _cacheManager);

            _cacheManager.VerifyThatCorrectKindWasInvoked(QueryCacheKind.Session);
            _cacheManager.VerifyThatMessageWas(_message);
            _cacheManager.VerifyThatAbsoluteExpirationWas(TimeSpan.FromMinutes(1).Add(TimeSpan.FromSeconds(22)));
            _cacheManager.VerifyThatSlidingExpirationWas(null);
            _cacheManager.VerifyThatQueryWas(_query);
        }

        [TestMethod]
        public void GetOrAddToCache_InvokesCacheManagerCorrectly_IfKindIsSessionAndSlidingExpirationIsSet()
        {
            var attribute = new QueryCacheOptionsAttribute(QueryCacheKind.Session)
            {
                SlidingExpiration = "00:33:16"
            };

            attribute.GetOrAddToCache(_message, _query, _cacheManager);

            _cacheManager.VerifyThatCorrectKindWasInvoked(QueryCacheKind.Session);
            _cacheManager.VerifyThatMessageWas(_message);
            _cacheManager.VerifyThatAbsoluteExpirationWas(null);
            _cacheManager.VerifyThatSlidingExpirationWas(TimeSpan.FromMinutes(33).Add(TimeSpan.FromSeconds(16)));
            _cacheManager.VerifyThatQueryWas(_query);
        }
    }
}
